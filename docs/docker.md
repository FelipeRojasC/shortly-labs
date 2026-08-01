# Docker — Guía de uso

Esta guía explica cómo construir, ejecutar y administrar el contenedor de Shortly usando
Docker y Docker Compose.

## Prerrequisitos

- **Docker Desktop** (incluye Docker Engine y Docker Compose v2+). Verifica tu instalación con:
  ```powershell
  docker --version
  docker compose version
  ```
- No necesitas tener instalado el SDK de .NET en tu máquina para correr la app en Docker — el
  `Dockerfile` usa un build multi-stage que descarga las imágenes de SDK/runtime que necesita.

## Configuración

1. Copia el archivo de variables de entorno de ejemplo:
   ```powershell
   Copy-Item .env.example .env
   ```
2. Ajusta `WEB_PORT` (puerto en tu máquina) o `ADMIN_PASSWORD` (contraseña del usuario
   administrador sembrado al iniciar) si lo necesitas. El valor por defecto de este proyecto es
   `8081` (no `8080`), porque el puerto 8080 puede estar ocupado por otro servicio local (por
   ejemplo, Apache/XAMPP) — si en tu máquina el 8080 está libre, puedes usarlo sin problema.

## Construir y levantar la aplicación

```powershell
docker compose up --build
```

- `--build` fuerza la reconstrucción de la imagen (necesario la primera vez, o después de
  cambiar código/dependencias). En corridas posteriores sin cambios, puedes omitir `--build`
  para que arranque más rápido:
  ```powershell
  docker compose up
  ```
- Para correrlo en segundo plano (sin bloquear la terminal):
  ```powershell
  docker compose up --build -d
  ```

Una vez arriba, la aplicación queda disponible en `http://localhost:8081` (o el puerto que
hayas definido en `WEB_PORT`). El endpoint de salud está en `http://localhost:8081/health`.

## Verificar el estado del contenedor

```powershell
docker ps
```

En la columna `STATUS` deberías ver `Up ... (healthy)` una vez que el `HEALTHCHECK` definido en
el `Dockerfile` pasa por primera vez (puede tardar unos segundos tras el arranque).

## Ver logs

```powershell
docker compose logs -f
```

`-f` (follow) deja el stream de logs abierto en tiempo real. Para ver solo el servicio web:
```powershell
docker compose logs -f shortly-web
```

## Detener y limpiar

Detener los contenedores (mantiene el volumen con la base de datos):
```powershell
docker compose down
```

Detener y **borrar también el volumen** (reinicia la base de datos desde cero, vuelve a
sembrar el usuario admin y los links de ejemplo la próxima vez que levantes):
```powershell
docker compose down -v
```

## Correr un servicio específico

Para ejecutar `shortly-web` de forma aislada (por ejemplo, para depurar sin levantar toda la
orquestación):
```powershell
docker compose run shortly-web
```

## Persistencia de datos

La base de datos SQLite (`database.db`) se guarda en el volumen nombrado `shortly-db-data`,
montado en `/app/data` dentro del contenedor. Esto significa que los datos **sobreviven** a que
detengas y vuelvas a levantar el contenedor con `docker compose down` + `docker compose up`
(sin la bandera `-v`), pero se pierden si usas `docker compose down -v`.

## Notas sobre la base de datos (`shortly-db`)

El proyecto usa **SQLite**, un motor de base de datos embebido (un archivo, no un proceso que
escucha en la red). Por eso no existe un contenedor `shortly-db` separado en
`docker-compose.yml`: su función la cumple el volumen `shortly-db-data`, que persiste el
archivo `database.db` de forma independiente al ciclo de vida del contenedor `shortly-web`.
Esto es coherente con el enunciado del laboratorio, que permite explícitamente continuar con
SQLite según el stack actual del proyecto.
