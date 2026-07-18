# Shortly Labs

Proyecto base **Shortly** (URL shortener en ASP.NET Core / .NET 10) provisto por el curso
de Arquitectura de Sistemas, DISC, UCN, sobre el cual se desarrollan los Laboratorios 3, 4 y 5.

> Código base original: [`godiecl/shortly`](https://github.com/godiecl/shortly)
> © 2026 Arquitectura de Sistemas, DISC, UCN, Chile.

## Estado del proyecto

| Laboratorio | Tema                                | Rama              | Estado |
|-------------|--------------------------------------|-------------------|--------|
| 3           | API REST + Content Negotiation       | `laboratory-3`    | ✅ Completo |
| 4           | Arquitectura de Microservicios (C4)  | `laboratory-4`    | ⬜ Pendiente |
| 5           | Containerización con Docker          | `laboratory-5`    | ⬜ Pendiente |

Cada laboratorio se desarrolla en su propia rama y se integra a `main` una vez validado,
de modo que el historial refleja el avance incremental del proyecto. El cierre de cada
laboratorio queda marcado con un tag (ver [Tags de entrega](#tags-de-entrega)).

## Stack técnico

- **.NET 10** / ASP.NET Core (Minimal APIs + Razor Pages)
- **Entity Framework Core** con SQLite
- **BCrypt.Net** para hash de contraseñas
- **Serilog** para logging estructurado
- **Scalar** + OpenAPI para documentación interactiva de la API
- **System.Xml.Serialization** para la serialización XML de las respuestas REST

## Estructura del proyecto

```
Application/
  DTOs/              → objetos de transferencia (CreateLinkRequest, LinkResponse, StatsResponse, ErrorResponse, UserResponse)
  Interfaces/        → contratos de servicios y repositorios (ILinkService, ILinkRepository, IUserService, IUserRepository)
  Services/          → lógica de negocio (LinkService, UserService)
Domain/
  Entities/          → entidades del dominio (Link, User)
Endpoints/
  UrlRedirectEndpoint.cs   → redirect GET /{shortUrl}
  LinksApiEndpoints.cs     → API REST: POST/GET/DELETE /api/urls, GET /api/stats
  ContentNegotiation.cs    → NegotiatedResult<T> y helpers de negociación JSON/XML
Infrastructure/
  Persistence/       → AppDbContext (EF Core)
  Repositories/       → implementaciones de acceso a datos
Pages/               → páginas Razor (login, registro, panel web)
docs/                → diagramas del proyecto (PlantUML)
IA.md                → registro de prompts de IA utilizados durante el desarrollo
```

## Cómo ejecutar

```bash
dotnet restore
dotnet run
```

La aplicación levanta por defecto en `http://localhost:5064` (revisa la consola al iniciar;
el puerto puede variar según `Properties/launchSettings.json`).

Al iniciar por primera vez, se crea la base de datos SQLite y se siembra:
- Un usuario administrador (`admin@shortly.disc.cl`, contraseña configurable vía
  `Seed:AdminPassword` o `admin123` por defecto).
- 3 links de ejemplo asociados a ese usuario.

La documentación interactiva de la API (Scalar) queda disponible en `/scalar/v1`.

## API REST (Laboratorio 3)

| Método | Endpoint            | Descripción                          | Códigos de estado |
|--------|----------------------|---------------------------------------|--------------------|
| POST   | `/api/urls`           | Crea una URL acortada                 | `201` creado · `400` datos inválidos |
| GET    | `/api/urls`           | Lista todas las URLs acortadas        | `200` |
| GET    | `/api/urls/{id}`      | Obtiene el detalle de una URL         | `200` · `404` no encontrada |
| DELETE | `/api/urls/{id}`      | Elimina una URL acortada              | `204` sin contenido · `404` no encontrada |
| GET    | `/api/stats`          | Estadísticas de uso (total de links, clicks, top 5) | `200` |

Cualquier `Accept` no soportado devuelve `406 Not Acceptable`. Errores no controlados
son capturados por un manejador global y también respetan el formato negociado.

### Content negotiation

La API responde en JSON o XML según el header `Accept`:

- `Accept: application/json` (o ausente / `*/*`) → JSON
- `Accept: application/xml` o `text/xml` → XML
- Cualquier otro valor → `406 Not Acceptable`

El `Content-Type` del request determina cómo se interpreta el body enviado (JSON o XML)
en el `POST /api/urls`.

### Ejemplos

```bash
# Crear una URL acortada
curl -X POST http://localhost:5064/api/urls \
  -H "Content-Type: application/json" -H "Accept: application/json" \
  -d '{"url":"https://example.com"}'

# Listar en XML
curl http://localhost:5064/api/urls -H "Accept: application/xml"

# Detalle de una URL
curl http://localhost:5064/api/urls/1

# Eliminar
curl -X DELETE http://localhost:5064/api/urls/1

# Estadísticas
curl http://localhost:5064/api/stats
```

En PowerShell, usa `Invoke-RestMethod` en vez de `curl`:

```powershell
Invoke-RestMethod -Uri "http://localhost:5064/api/urls" -Headers @{Accept="application/json"}
Invoke-RestMethod -Uri "http://localhost:5064/api/urls" -Method Post -ContentType "application/json" -Headers @{Accept="application/json"} -Body '{"url":"https://example.com"}'
Invoke-RestMethod -Uri "http://localhost:5064/api/urls/1" -Method Delete
```

## Tags de entrega

El cierre de cada laboratorio queda marcado con un tag anotado sobre el commit de entrega,
para tener un punto de referencia fijo independiente de si la rama sigue evolucionando:

```bash
git tag -a lab3-entrega -m "Entrega Laboratorio 3: API REST"
git push origin lab3-entrega
```

## Uso de IA

Todos los prompts utilizados durante el desarrollo están documentados en [`IA.md`](./IA.md),
según lo exigido por el enunciado de cada laboratorio.

## Autor

Felipe Rojas C. — Estudiante de Ingeniería Civil en Computación e Informática, UCN.