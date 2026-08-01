# IA.md — Uso de Inteligencia Artificial (Laboratorio 5)

Durante el desarrollo del Laboratorio 5 se utilizó **Claude (Anthropic, claude.ai)** como apoyo
para diseñar el `Dockerfile`, el `docker-compose.yml`, el `.dockerignore`, la documentación de
uso de Docker, y para resolver dudas de configuración, Git y del propio entorno de Docker. A
continuación se listan los prompts utilizados, en orden cronológico.

| # | Prompt (resumen fiel de la consulta realizada) | Uso dado a la respuesta |
|---|---------------------------------------------------|---------------------------|
| 1 | Se solicitó comenzar el Laboratorio 5. | Se creó la rama `laboratory-5` desde `main` (proyecto base limpio) y se leyó el enunciado real (`LAB_5_DOCKER.md`) para definir el plan de trabajo. |
| 2 | Ante la pregunta de si migrar a PostgreSQL o mantener SQLite para la base de datos del contenedor, se indicó seguir literalmente lo que dice el enunciado del laboratorio, que permite continuar con SQLite. | Se descartó la migración de base de datos y se diseñó el `docker-compose.yml` en torno a SQLite, documentando explícitamente por qué no existe un contenedor `shortly-db` separado (SQLite es un motor embebido, no cliente-servidor). |
| 3 | Se pidió agregar el endpoint `/health` necesario para el `HEALTHCHECK` del Dockerfile. | Se agregaron `AddHealthChecks()` y `MapHealthChecks("/health")` a `Program.cs`. |
| 4 | Se compartió el contenido completo de `Program.cs` pidiendo la versión completa con los cambios aplicados. | Se entregó el archivo completo con las dos líneas nuevas integradas, para copiar y reemplazar directamente. |
| 5 | Se confirmó que la prueba local de `/health` respondía `Healthy`. | Se validó el paso y se continuó con el `Dockerfile`. |
| 6 | Se preguntó dónde ubicar físicamente el archivo `Dockerfile` dentro del proyecto. | Se aclaró que debe ir en la raíz del repositorio, junto a `Shortly.csproj`, sin extensión. |
| 7 | Se compartió el contenido de `appsettings.json`, incluyendo la cadena de conexión SQLite. | Se usó para configurar correctamente la variable de entorno `ConnectionStrings__AppDbContext` en `docker-compose.yml`, apuntando al volumen persistente. |
| 8 | Se pidió continuar tras confirmar la ubicación de los archivos generados. | Se generaron `docker-compose.yml` y `.env` (con variables de configuración). |
| 9 | Se preguntó "¿ahora qué vendría?" para continuar con el siguiente paso. | Se generó el archivo `.dockerignore` y se indicó probar el contenedor con `docker compose up --build`. |
| 10 | Se informó no tener Docker instalado. | Se dieron instrucciones de instalación de Docker Desktop en Windows (incluyendo el requisito de WSL 2) y cómo verificar la instalación. |
| 11 | Se compartió la salida de `docker --version` y `docker compose version` tras la instalación, preguntando si ya se podía continuar. | Se confirmó la instalación correcta y se sugirió una prueba adicional (`docker run hello-world`) antes de construir la imagen del proyecto. |
| 12 | Se pidió continuar sin ejecutar pruebas de Docker por el momento. | Se advirtió sobre el riesgo de no probar antes, pero se respetó la decisión y se continuó con la documentación (`docs/docker.md`). |
| 13 | Se subió un `.zip` con el estado real del proyecto para verificar qué archivos existían efectivamente. | Se revisó el repositorio real, confirmando que `Dockerfile`, `docker-compose.yml`, `.dockerignore` y `.env.example` ya estaban commiteados correctamente, y que `docker.md` se había guardado en la raíz en vez de dentro de `docs/`, sin commitear. Se dieron instrucciones para moverlo y commitearlo. |
| 15 | Se solicitó continuar con el `IA.md` de este laboratorio. | Este documento. |

## Nota sobre el uso de IA en este proyecto

Al igual que en los Laboratorios 3 y 4, la IA se uso como guia, ayudando a generar directamente el contenido técnico del
`Dockerfile`, `docker-compose.yml`, `.dockerignore` y la documentación, en base a los requisitos
del enunciado y a la configuración real del proyecto (verificada a partir de `appsettings.json`
y el código existente antes de proponer cada archivo). El estudiante gestionó la instalación de
Docker, la ubicación y el versionado de los archivos en su repositorio local, corrigió una
confusión propia sobre la elección de nombre de archivo de configuración, y decidió postergar la
ejecución de pruebas del contenedor, siendo advertido explícitamente del riesgo de hacerlo.