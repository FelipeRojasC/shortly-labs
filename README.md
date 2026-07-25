# Shortly Labs

Proyecto base **Shortly** (URL shortener en ASP.NET Core / .NET 10) provisto por el curso
de Arquitectura de Sistemas, DISC, UCN, sobre el cual se desarrollan los Laboratorios 3, 4 y 5.

> Código base original: [`godiecl/shortly`](https://github.com/godiecl/shortly)
> © 2026 Arquitectura de Sistemas, DISC, UCN, Chile.

## Estado del proyecto

| Laboratorio | Tema                                | Rama              | Estado |
|-------------|--------------------------------------|-------------------|--------|
| 3           | API REST + Content Negotiation       | `laboratory-3`    | ✅ Completo |
| 4           | Arquitectura de Microservicios (C4)  | `laboratory-4`    | ✅ Completo |
| 5           | Containerización con Docker          | `laboratory-5`    | ⬜ Pendiente |

Cada laboratorio se desarrolla en su **propia rama, partiendo siempre de `main` (proyecto base
sin modificar)**, no uno sobre el otro. Esto significa que el código de esta rama
(`laboratory-4`) corresponde al monolito base original: no incluye la API REST del Laboratorio 3
(esa vive únicamente en la rama `laboratory-3`). El trabajo de este laboratorio son los diagramas
C4 y el documento de arquitectura en `docs/`, que proponen cómo evolucionar este mismo monolito
hacia una arquitectura de microservicios.

El cierre de cada laboratorio queda marcado con un tag (ver [Tags de entrega](#tags-de-entrega)).

## Stack técnico

- **.NET 10** / ASP.NET Core (Minimal APIs + Razor Pages)
- **Entity Framework Core** con SQLite
- **BCrypt.Net** para hash de contraseñas
- **Serilog** para logging estructurado
- **Scalar** + OpenAPI para documentación interactiva de la API

## Estructura del proyecto

```
Application/
  DTOs/              → objetos de transferencia (LinkResponse, UserResponse)
  Interfaces/        → contratos de servicios y repositorios (ILinkService, ILinkRepository, IUserService, IUserRepository)
  Services/          → lógica de negocio (LinkService, UserService)
Domain/
  Entities/          → entidades del dominio (Link, User)
Endpoints/
  UrlRedirectEndpoint.cs   → redirect GET /{shortUrl}
Infrastructure/
  Persistence/       → AppDbContext (EF Core)
  Repositories/       → implementaciones de acceso a datos
Pages/               → páginas Razor (login, registro, panel web)
docs/                → diagramas C4 (PlantUML) y documento de arquitectura del Laboratorio 4
IA.md                → registro de prompts de IA utilizados durante el desarrollo de este laboratorio
```

## Cómo ejecutar

```bash
dotnet restore
dotnet run
```

La aplicación levanta por defecto en `http://localhost:5064` (revisa la consola al iniciar;
el puerto puede variar según `Properties/launchSettings.json`).

Al iniciar por primera vez, se crea la base de datos SQLite y se siembra un usuario administrador
(`admin@shortly.disc.cl`, contraseña configurable vía `Seed:AdminPassword` o `admin123` por
defecto) junto con 3 links de ejemplo.

La documentación interactiva de la API (Scalar) queda disponible en `/scalar/v1`.

## Arquitectura de Microservicios (Laboratorio 4)

En `docs/` se encuentra el modelo C4 completo que propone la descomposición de este monolito en
microservicios independientes (API Gateway, URL Service, Redirect Service, Stats Service,
Identity Service), junto con el documento de arquitectura que justifica cada decisión de diseño.

| Nivel | Archivo |
|-------|---------|
| 1 — Contexto | [`docs/c4-context.puml`](./docs/c4-context.puml) |
| 2 — Contenedores | [`docs/c4-container.puml`](./docs/c4-container.puml) |
| 3 — Componentes | [`docs/c4-component-url-service.puml`](./docs/c4-component-url-service.puml), [`docs/c4-component-redirect-service.puml`](./docs/c4-component-redirect-service.puml) |
| 4 — Código (bonus) | [`docs/c4-code-redirect.puml`](./docs/c4-code-redirect.puml) |
| — Documento de arquitectura | [`docs/architecture.md`](./docs/architecture.md) |

Los diagramas se pueden renderizar con la extensión "PlantUML" de VS Code, o pegando su contenido
en [plantuml.com/plantuml](https://www.plantuml.com/plantuml/uml/).

## Uso de IA

Todos los prompts utilizados durante el desarrollo de este laboratorio están documentados en
[`IA.md`](./IA.md), según lo exigido por el enunciado.

## Autor

Felipe Rojas C. — Estudiante de Ingeniería Civil en Computación e Informática, UCN.