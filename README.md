# Laboratorio 5 — Containerización de Shortly con Docker

**Shortly** es un servicio de acortamiento de URLs construido en ASP.NET Core (.NET 10). Este
laboratorio containeriza la aplicación con Docker: build multi-stage, usuario no-root,
healthcheck, orquestación con Docker Compose, y un servicio de caché (Redis) como bonus.

> Código base original: [`godiecl/shortly`](https://github.com/godiecl/shortly)
> © 2026 Arquitectura de Sistemas, DISC, UCN, Chile.

## Stack técnico

- **.NET 10** / ASP.NET Core (Minimal APIs + Razor Pages)
- **Entity Framework Core** con SQLite
- **Redis** — caché de links para el flujo de redirect (bonus)
- **Docker** / Docker Compose

## Prerrequisitos

- **Docker Desktop** (incluye Docker Engine y Docker Compose v2+):
  ```powershell
  docker --version
  docker compose version
  ```
- No necesitas el SDK de .NET instalado para correr la app en Docker — el `Dockerfile` se
  encarga de eso en su etapa de build.

## Cómo ejecutar

```powershell
Copy-Item .env.example .env
docker compose up --build
```

La aplicación queda disponible en `http://localhost:8081` (puerto configurable vía `WEB_PORT`
en `.env`; el valor por defecto es 8081 en vez de 8080 porque este último suele estar ocupado
por otros servicios locales, como Apache/XAMPP).

Al levantar por primera vez, se crea la base de datos SQLite y se siembra un usuario
administrador (`admin@shortly.disc.cl`, contraseña configurable vía `ADMIN_PASSWORD` en `.env`,
`admin123` por defecto) junto con 3 links de ejemplo.

Guía completa de uso (build, run, stop, logs, ejecutar un servicio específico) en
[`docs/docker.md`](./docs/docker.md).

## Qué incluye la containerización

| Requisito | Archivo |
|-----------|---------|
| Build multi-stage, usuario no-root, layer caching, healthcheck | [`Dockerfile`](./Dockerfile) |
| Orquestación de servicios, red bridge, volúmenes nombrados, límites de recursos | [`docker-compose.yml`](./docker-compose.yml) |
| Exclusión de archivos innecesarios del build context | [`.dockerignore`](./.dockerignore) |
| Variables de configuración (puerto, password admin) | [`.env.example`](./.env.example) |
| Guía de uso | [`docs/docker.md`](./docs/docker.md) |

### Dockerfile

- Multi-stage: `mcr.microsoft.com/dotnet/sdk:10.0` para build/publish, `mcr.microsoft.com/dotnet/aspnet:10.0` para runtime — la imagen final no contiene SDK ni herramientas de build.
- Corre como usuario no-root (`appuser`).
- `HEALTHCHECK` que consulta `GET /health` cada 30s.
- Orden de capas optimizado: `Shortly.csproj` + `dotnet restore` antes de copiar el resto del código fuente, para aprovechar el caché de Docker.

### docker-compose.yml

- **`shortly-web`**: build desde el `Dockerfile`, puerto configurable, restart policy, límites de CPU/memoria.
- **`shortly-cache`** (bonus): Redis 7, sin puerto expuesto al host — solo accesible dentro de la red interna.
- Red bridge personalizada (`shortly-network`).
- Volumen nombrado `shortly-db-data` para persistir el archivo SQLite (`database.db`) independientemente del ciclo de vida del contenedor.

> **Nota sobre `shortly-db`:** el proyecto usa SQLite, un motor embebido sin proceso de
> servidor, por lo que no existe un contenedor de base de datos separado — su rol lo cumple el
> volumen nombrado `shortly-db-data`. Detalle en [`docs/docker.md`](./docs/docker.md).

### Bonus — Cache service (Redis)

`shortly-cache` está integrado de verdad en el código, no solo declarado en el compose:
`LinkService.GetLink` (el método detrás del redirect `GET /{shortUrl}`) usa una estrategia
cache-first — primera consulta a un link corto va a la base de datos y se cachea; consultas
siguientes se resuelven desde Redis. Redis también respalda el almacenamiento de sesiones de
autenticación (`MemoryCacheTicketStore`), por lo que tanto el login como el redirect dependen de
que el servicio de caché esté disponible.

## Endpoint de salud

```
GET /health
```

Usado por el `HEALTHCHECK` del `Dockerfile` para reportar el estado del contenedor
(`docker ps` debería mostrar `(healthy)` una vez que la app arranca).

## Uso de IA

Todos los prompts utilizados durante el desarrollo de este laboratorio están documentados en
[`IA.md`](./IA.md), según lo exigido por el enunciado.

## Autor

Felipe Rojas C. — Estudiante de Ingeniería Civil en Computación e Informática, UCN.