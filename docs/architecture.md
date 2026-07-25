# Shortly — Documento de Arquitectura de Microservicios

Este documento acompaña a los diagramas del modelo C4 (`docs/c4-*.puml`) y explica el
razonamiento detrás de la descomposición propuesta del monolito Shortly en microservicios
independientes.

## 1. Justificación de la descomposición en servicios

La implementación actual de Shortly es un monolito ASP.NET Core donde la
creación de links, la redirección y las estadísticas comparten el mismo proceso, la misma base
de datos y la misma unidad de despliegue. La arquitectura propuesta la divide en cinco servicios,
cada uno con una única responsabilidad:

- **API Gateway** — el único punto de entrada expuesto a los clientes. Centraliza el ruteo, y es
  el lugar natural para agregar preocupaciones transversales (rate limiting, logging de
  requests, reenvío de contexto de autenticación) sin duplicar esa lógica en cada servicio.
- **URL Service** — es dueño del ciclo de vida de un link corto (crear, obtener, eliminar).
  Se construye sobre las responsabilidades que ya maneja `LinkService` en el monolito actual
  (`CreateLink`, `GetAllLinks`, `GetLinksByUserId`), extendidas con una superficie de API REST y
  soporte para eliminar. Tiene bastante escritura pero tráfico bajo comparado con los redirects.
- **Redirect Service** — es dueño de resolver un código corto hacia su URL original. Corresponde
  al `UrlRedirectEndpoint` actual, que ya aísla esta responsabilidad a nivel de endpoint incluso
  dentro del monolito. Se separa en su propio servicio porque tiene un perfil de tráfico
  fundamentalmente distinto: un link corto puede ser seguido miles de veces, mientras que se
  crea una sola vez. Aislarlo permite escalar de forma independiente y aplicar una estrategia
  cache-first sin afectar el camino de escritura.
- **Stats Service** — es dueño de la agregación y reporte de clicks. Desacoplarlo significa que
  el camino de redirección nunca se bloquea esperando escribir datos de analítica.
- **Identity Service** (planificado) — es dueño de la autenticación y las credenciales de
  usuario. Aún no está implementado en el código actual (que usa autenticación por cookies
  dentro del monolito), pero se modela aquí como el siguiente paso natural, para que cualquier
  servicio futuro pueda validar identidad de forma independiente.

Cada límite de servicio sigue la misma prueba: **¿esta responsabilidad cambia por una razón
distinta, a una velocidad distinta, o bajo una carga distinta que las demás?** La creación de
links, la redirección y las estadísticas cumplen esa prueba, por eso se convirtieron en
servicios separados en vez de quedarse juntos.

## 2. Patrones de comunicación

- **Síncrona (HTTP/REST)**: el API Gateway se comunica con URL Service, Redirect Service, Stats
  Service e Identity Service vía HTTP/REST, reflejando la naturaleza de solicitud/respuesta de
  sus operaciones (un cliente espera una respuesta inmediata a "crea este link" o "dame los
  datos de este link").
- **Asíncrona (AMQP vía message broker)**: Redirect Service publica un "evento de click" a un
  broker (RabbitMQ) en vez de llamar directamente a Stats Service. Esto es intencional — la
  respuesta del redirect no debe esperar a que se escriba un registro de estadística. Stats
  Service consume estos eventos a su propio ritmo. Esta es la única relación asíncrona del
  sistema, y existe justo donde un acoplamiento síncrono sería más dañino (el camino más
  sensible a la latencia: los redirects).
- **Caché (Redis, no es mensajería)**: Redirect Service lee desde una caché Redis antes de
  tocar la base de datos, y URL Service invalida/puebla proactivamente esa caché en cada
  escritura. Esto no es mensajería entre servicios en el sentido tradicional, pero sí es un
  canal de comunicación relevante para la consistencia (ver más abajo).

## 3. Propiedad de los datos

Cada servicio es dueño exclusivo de sus datos; ningún servicio lee la base de datos de otro
directamente.

| Servicio | Es dueño de | Accedido por otros vía |
|---|---|---|
| URL Service | `URL Database` (registros de links) | Solo llamadas API |
| Redirect Service | `Redirect Cache` (copia derivada/desnormalizada de los mapeos de links) | No se comparte — es una caché, no una fuente de verdad |
| Stats Service | `Stats Database` (agregados de clicks) | Solo llamadas API |
| Identity Service | `Identity Database` (usuarios, credenciales) | Solo llamadas API |

**Estrategia de consistencia**: la Redirect Cache es una copia optimizada para lectura,
eventualmente consistente, de datos que son dueños de URL Service. URL Service envía
invalidaciones en cada escritura, y Redirect Service recurre a la base de datos de URL Service
cuando hay un cache miss, así el sistema tolera una breve desactualización a cambio de menor
latencia en el redirect — un trade-off aceptable ya que el destino de un link corto rara vez
cambia después de creado. Los conteos de clicks en Stats Service son eventualmente consistentes
respecto a los eventos de click reales, ya que se procesan de forma asíncrona desde el broker;
esto es aceptable porque las estadísticas son informativas, no se usan para control de acceso ni
decisiones críticas del negocio.

## 4. Consideraciones de escalabilidad

- **Redirect Service** es el candidato más claro para escalar horizontalmente de forma
  independiente — se espera que reciba el mayor volumen de requests por lejos, es *stateless*, y
  su dependencia de caché (Redis) puede a su vez escalarse/particionarse de forma independiente.
- **API Gateway** escala horizontalmente detrás de un balanceador de carga; no mantiene estado
  propio.
- **URL Service** tiene tráfico comparativamente bajo y en ráfagas (crear un link es una acción
  deliberada del usuario, no un evento de alta frecuencia) y no necesita el mismo perfil de
  escalamiento que Redirect Service.
- **Stats Service** puede escalar sus consumidores de forma independiente a los productores del
  broker (Redirect Service), que es uno de los principales beneficios de desacoplarlos de forma
  asíncrona — un pico de clicks no obliga a Stats Service a mantenerse al día en tiempo real.
- **Identity Service**, una vez implementado, escalaría principalmente según el volumen de
  login/validación de tokens, que no está relacionado con el tráfico de links — otro argumento
  para mantenerlo separado.

## 5. Modos de falla y resiliencia

- **Si Redirect Service no responde**: es la falla más dañina, ya que es el camino de mayor
  valor para los usuarios finales que siguen un link. Se mitiga manteniendo a Redirect Service
  minimalista (menos dependencias que puedan fallar) y con la caché Redis absorbiendo la mayoría
  de las lecturas incluso si URL Service o su base de datos están momentáneamente degradados.
- **Si URL Service está caído**: los usuarios no pueden crear ni eliminar links, pero los links
  existentes se siguen resolviendo con normalidad a través de Redirect Service (que no depende
  de que URL Service esté arriba, solo de la caché/base de datos que ya pobló antes). Este
  aislamiento es un beneficio directo de la separación.
- **Si el Broker de Eventos de Click está caído o saturado**: Redirect Service no debe
  bloquearse ni fallar el redirect esperando la publicación — los eventos de click son
  "fire-and-forget" desde su perspectiva. Puede perderse o retrasarse algo de datos de click, lo
  cual es un trade-off aceptado (ver Propiedad de los datos). Las estadísticas son datos no
  críticos, así que este modo de falla tiene bajo impacto en el negocio.
- **Si Stats Service está caído**: no hay impacto para el usuario final; solo el endpoint
  `/api/stats` (vía el Gateway) queda no disponible, y los eventos de click se acumulan en el
  broker hasta que se recupere.
- **Si Identity Service está caído** (una vez implementado): fallarían el login/registro, pero
  las sesiones ya autenticadas y la funcionalidad de redirect no se verían afectadas, ya que ni
  URL Service ni Redirect Service deberían requerir validación de identidad en vivo para sus
  operaciones principales.
- **Estrategia general**: el API Gateway es el lugar indicado para agregar *circuit breakers* y
  timeouts por cada servicio downstream, para que un servicio degradado no agote los recursos
  del propio Gateway ni genere fallas en cascada hacia servicios no relacionados.

## 6. Propuesta de stack tecnológico

| Aspecto | Tecnología propuesta | Justificación |
|---|---|---|
| Framework de servicio | ASP.NET Core Minimal API (.NET 10) | Consistente con el código ya existente de Shortly; bajo overhead por servicio. |
| API Gateway | YARP (Yet Another Reverse Proxy) | Reverse proxy de primer nivel de .NET; se integra naturalmente con el resto del stack. |
| Almacenamiento relacional | PostgreSQL | Reemplazo de nivel productivo para el SQLite usado en el monolito; soporta esquemas/instancias por servicio. |
| Caché | Redis | Estándar de la industria para caché read-through; soporta la estrategia cache-first del redirect. |
| Message broker | RabbitMQ | Broker AMQP simple, suficiente para el único flujo asíncrono (eventos de click) de este sistema; menor complejidad operacional que un cluster de Kafka para esta escala. |
| Contenedorización | Docker (ver Laboratorio 5) | Cada servicio se convierte en un contenedor desplegable de forma independiente. |
| Descubrimiento de servicios / configuración | Configuración basada en variables de entorno por contenedor, tabla de ruteo a nivel de gateway | Mantiene la arquitectura inicial simple; evita introducir un service mesh antes de que se justifique por la escala. |

## Referencia a los diagramas

- Nivel 1 — `docs/c4-context.puml`
- Nivel 2 — `docs/c4-container.puml`
- Nivel 3 — `docs/c4-component-url-service.puml`, `docs/c4-component-redirect-service.puml`
- Nivel 4 (bonus) — `docs/c4-code-redirect.puml`