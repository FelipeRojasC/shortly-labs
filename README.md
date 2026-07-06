# Shortly Labs

Proyecto base **Shortly** (URL shortener en ASP.NET Core / .NET 10) provisto por el curso
de Arquitectura de Sistemas, DISC, UCN, sobre el cual se desarrollan los Laboratorios 3, 4 y 5.

> Código base original: [`godiecl/shortly`](https://github.com/godiecl/shortly)
> © 2026 Arquitectura de Sistemas, DISC, UCN, Chile.

## Estado del proyecto

| Laboratorio | Tema                              | Rama              | Estado |
|-------------|------------------------------------|-------------------|--------|
| 3           | API REST + Content Negotiation     | `laboratory-3`    | 🚧 En progreso |
| 4           | Arquitectura de Microservicios (C4)| `laboratory-4`    | ⬜ Pendiente |
| 5           | Containerización con Docker        | `laboratory-5`    | ⬜ Pendiente |

Cada laboratorio se desarrolla en su propia rama y se integra a `main` una vez validado,
de modo que el historial refleja el avance incremental del proyecto.

## Stack técnico

- **.NET 10** / ASP.NET Core (Minimal APIs + Razor Pages)
- **Entity Framework Core** con SQLite
- **BCrypt.Net** para hash de contraseñas
- **Serilog** para logging estructurado
- **Scalar** + OpenAPI para documentación interactiva de la API

## Estructura del proyecto

```
Application/
  DTOs/            → objetos de transferencia (requests/responses)
  Interfaces/       → contratos de servicios y repositorios
  Services/         → lógica de negocio
Domain/
  Entities/         → entidades del dominio (Link, User)
Endpoints/          → endpoints de Minimal API (redirect, API REST)
Infrastructure/
  Persistence/       → DbContext de EF Core
  Repositories/       → implementaciones de acceso a datos
Pages/              → páginas Razor (login, registro, panel web)
docs/               → diagramas del proyecto (PlantUML)
```

## Cómo ejecutar

```bash
dotnet restore
dotnet run
```

La aplicación levanta por defecto en `https://localhost:5001` (revisa `Properties/launchSettings.json`).
Al iniciar por primera vez, se crea la base de datos SQLite y se siembra un usuario administrador
(`admin@shortly.disc.cl`, contraseña configurable vía `Seed:AdminPassword` o `admin123` por defecto).

La documentación interactiva de la API (Scalar) queda disponible en `/scalar/v1`.

## API REST (Laboratorio 3)

| Método | Endpoint            | Descripción                          |
|--------|----------------------|---------------------------------------|
| POST   | `/api/urls`           | Crea una URL acortada                 |
| GET    | `/api/urls`           | Lista todas las URLs acortadas        |
| GET    | `/api/urls/{id}`      | Obtiene el detalle de una URL         |
| DELETE | `/api/urls/{id}`      | Elimina una URL acortada              |
| GET    | `/api/stats`          | Estadísticas de uso                   |

### Content negotiation

La API responde en JSON o XML según el header `Accept`:

- `Accept: application/json` (o ausente / `*/*`) → JSON
- `Accept: application/xml` o `text/xml` → XML
- Cualquier otro valor → `406 Not Acceptable`

El `Content-Type` del request determina cómo se interpreta el body enviado (JSON o XML).

Ejemplos:

```bash
curl -X POST https://localhost:5001/api/urls \
  -H "Content-Type: application/json" -H "Accept: application/json" \
  -d '{"url":"https://example.com"}'

curl https://localhost:5001/api/urls -H "Accept: application/xml"
```

## Uso de IA

Todos los prompts utilizados durante el desarrollo están documentados en [`IA.md`](./IA.md),
según lo exigido por el enunciado de cada laboratorio.

## Autor
 
Felipe Rojas C. — Estudiante de Ingeniería Civil en Computación e Informática, UCN.