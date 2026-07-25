# IA.md — Uso de Inteligencia Artificial (Laboratorio 4)

Durante el desarrollo del Laboratorio 4 se utilizó **Claude (Anthropic, claude.ai)** como apoyo
para diseñar los diagramas C4 (Contexto, Contenedores, Componentes y Código), redactar el
documento de arquitectura, y resolver dudas de Git/PlantUML. A continuación se listan los
prompts utilizados, en orden cronológico.

| # | Prompt (resumen fiel de la consulta realizada) | Uso dado a la respuesta |
|---|---------------------------------------------------|---------------------------|
| 1 | Se preguntó si había que crear una nueva rama para el Lab 4, aclarando que según lo indicado por el profesor cada laboratorio debe partir de la base del proyecto, no del avance de labs anteriores. | Se confirmó que la rama `laboratory-3` nunca se había mezclado con `main`, por lo que `main` seguía siendo la base limpia; se definió la estrategia de ramas hermanas (`laboratory-3`, `laboratory-4`, `laboratory-5`, todas partiendo de `main`). |
| 2 | Se subió un `.zip` con el estado de la rama `main` del repositorio, pidiendo analizarlo para comenzar el laboratorio. | Se leyó el enunciado real (`LAB_4_MICROSERVICE.md`) y el código base desde el archivo, en vez de asumir su contenido. |
| 3 | Se preguntó dónde se puede probar/renderizar un diagrama PlantUML. | Se explicaron 3 alternativas (servidor online, extensión de VS Code, plugin de JetBrains). |
| 4 | Se pidió continuar con "el siguiente punto" del plan (diagrama de Contenedores — Nivel 2). | Se generó el archivo `docs/c4-container.puml`, definiendo los microservicios propuestos (API Gateway, URL Service, Redirect Service, Stats Service, Identity Service) y sus relaciones. |
| 5 | Se pidió continuar con "el siguiente" (diagramas de Componentes — Nivel 3). | Se generaron `docs/c4-component-url-service.puml` y `docs/c4-component-redirect-service.puml`. |
| 6 | Se preguntó si había que elegir solo uno de los dos diagramas de componentes o incluir ambos. | Se aclaró que el rubric exige un mínimo de 2 servicios descompuestos, por lo que ambos archivos son obligatorios, no alternativos. |
| 7 | Se pidió continuar con el siguiente punto (diagrama de Código — Nivel 4, bonus). | Se generó un diagrama de clases (`docs/c4-code-linkservice.puml`, posteriormente corregido — ver prompt 9). |
| 8 | Se pidió una versión en español del documento de arquitectura para facilitar su lectura. | Se generó `architecture_es.md` como traducción completa del documento. |
| 9 | Se señaló que el documento de arquitectura y el diagrama de código citaban clases del Laboratorio 3 (`LinksApiEndpoints`, `NegotiatedResult<T>`) que no existen en la rama `laboratory-4`, ya que esta parte del monolito base sin la API REST del Lab 3. | Se verificó el código real de la rama base (`ILinkService`, `ILinkRepository`, `UrlRedirectEndpoint`) y se corrigieron los 3 archivos afectados: se reemplazó el diagrama de código por uno basado en el flujo real de redirect (`docs/c4-code-redirect.puml`), se generizaron las etiquetas de tecnología en el diagrama de componentes de URL Service, y se removieron las referencias incorrectas al Laboratorio 3 en `architecture.md`. |
| 10 | Se subió un `.zip` con el estado real del proyecto tras los pasos anteriores, preguntando cuál era el archivo "code redirect", ya que no se habían aplicado todavía las correcciones del prompt 9. | Se detectó que el archivo viejo (`c4-code-linkservice.puml`, con las referencias incorrectas) seguía presente y que `docs/architecture.md` no se había guardado nunca en el proyecto; se reentregaron los 3 archivos corregidos con instrucciones exactas de qué borrar/reemplazar. |
| 11 | Se solicitó generar este `IA.md` con el registro de prompts del Laboratorio 4. | Este documento. |

## Nota sobre el uso de IA en este proyecto

Al igual que en el Laboratorio 3, la IA se usó como **par de diseño**, generando directamente el
contenido técnico de los diagramas C4 (en formato PlantUML) y del documento de arquitectura, en
base a la descripción del enunciado y al código real del proyecto (verificado archivo por archivo
antes de proponer contenido). Un error real de la IA —referenciar clases del Laboratorio 3 que no
existen en esta rama— fue detectado por el estudiante y corregido en conjunto, quedando reflejado
en los prompts 9 y 10 de esta tabla. El estudiante gestionó el flujo de ramas de Git, revisó y
aprobó cada archivo antes de subirlo.