# IA.md — Uso de Inteligencia Artificial

Durante el desarrollo del Laboratorio 3 se utilizó Claude (Anthropic, claude.ai) como herramienta de apoyo para definir el plan de desarrollo, analizar la arquitectura del proyecto base *Shortly*, resolver dudas puntuales de código y Git, y maquetar la documentación. A continuación se listan las interacciones, en orden cronológico.

| # | Prompt (resumen fiel de la consulta realizada) | Uso dado a la respuesta |
|---|---|---|
| 1 | Se entregó el enunciado del Laboratorio 3 para solicitar apoyo en la creación de un plan de desarrollo estructurado a alto nivel. | Se definió la ruta de trabajo inicial y los hitos del proyecto. |
| 2 | Se compartió el repositorio base pidiendo directrices paso a paso, priorizando la explicación conceptual de la arquitectura por sobre la entrega de código. | Se estableció a la IA como guía metodológico para asegurar la comprensión antes de programar. |
| 3 | Se proporcionó la estructura de directorios del proyecto (Application, Domain, Endpoints, Infrastructure) para alinear el plan de desarrollo con la arquitectura limpia existente. | Se ajustó el plan técnico al stack real de ASP.NET Core (.NET 10). |
| 4 | Se consultó sobre la mejor manera de revisar avances de código de forma iterativa y validar el progreso. | Se definió la dinámica de compartir extractos de archivos clave directamente en el chat. |
| 5 | Se compartió el código de las entidades y servicios clave para analizar la convención de nombres, interfaces y estilos del proyecto base. | Se utilizó para diseñar las firmas de los nuevos métodos manteniendo la coherencia y los estándares del proyecto original. |
| 6 | Se pidieron recomendaciones sobre convenciones de nombres para crear el repositorio personal del laboratorio. | Se definió el nombre del repositorio y la estrategia inicial de subida. |
| 7 | Se consultó sobre la continuidad de los laboratorios (3, 4 y 5) basándose en los enunciados, para prever una estrategia de control de versiones adecuada. | Se analizó el alcance a futuro para estructurar mejor las ramas de Git. |
| 8 | Se solicitó ayuda para diseñar una estrategia de ramas en Git (`laboratory-3`, etc.) que permitiera encapsular el trabajo sin afectar futuros laboratorios. | Se definieron los comandos para manejo de branches y tags para un control de versiones limpio. |
| 9 | Se pidió una estructura base para el `README.md` y recomendaciones para un `.gitignore` adecuado en un proyecto .NET. | Se obtuvieron las plantillas iniciales para la documentación y control de archivos. |
| 10 | Se solicitó ajustar el formato de la sección de "Autor" en el `README` para incluir correctamente los datos académicos. | Mejora y estructuración de la documentación del proyecto. |
| 11 | Se pidió integrar un `.gitignore` propio (plantilla JetBrains) con las recomendaciones de la IA, sin perder configuraciones. | Consolidación del archivo `.gitignore` final. |
| 12 | Se consultó por un error de sintaxis en PowerShell al intentar ejecutar un comando Bash (`rm -rf .git`). | Resolución de problemas de terminal (uso de `Remove-Item`). |
| 13 | Se realizaron consultas específicas sobre cómo estructurar la inyección de dependencias, los DTOs y el "content negotiation" (JSON/XML) para los nuevos endpoints REST. | Sirvió como guía conceptual para que el estudiante programara e implementara las interfaces y métodos requeridos. |
| 14 | Se preguntó por el funcionamiento específico del operador `=>` en C# (visto en la capa de repositorios). | Aclaración conceptual sobre sintaxis (*expression-bodied members*). |
| 15 | Se consultó si unos *warnings* de vulnerabilidades NuGet aparecidos tras compilar afectaban el desarrollo del laboratorio. | Confirmación de que no eran bloqueantes para los cambios actuales. |
| 16 | Se solicitó una revisión de la estructura del archivo `Program.cs` para confirmar que el registro de servicios y endpoints estuviera conceptualmente correcto. | Validación de la lógica de inyección de dependencias implementada. |
| 17 | Se compartieron los logs de ejecución (`dotnet run`) para verificar el correcto levantamiento del servidor y la inyección de datos semilla. | Confirmación del estado exitoso del entorno local para iniciar pruebas. |
| 18 | Se consultó por un error de conexión al hacer pruebas manuales con `Invoke-RestMethod`. | Se diagnosticó el flujo de pruebas (faltaba mantener el servidor en ejecución en una terminal paralela). |
| 19 | Se compartieron los resultados de las pruebas de los endpoints (casos 404/406) para verificar si el comportamiento se alineaba con lo exigido en la rúbrica. | Validación funcional del código desarrollado. |
| 20 | Se consultó sobre el estado de `git status` para confirmar si los archivos detectados correspondían al flujo esperado de *commits*. | Apoyo en la validación y organización de *commits* limpios. |
| 21 | Se pidió orientación conceptual para estructurar un manejador de errores global y se solicitó un *checklist* contra la rúbrica del laboratorio. | Ajuste final de *middlewares* y verificación de requerimientos faltantes. |
| 22 | Se solicitó ayuda para estructurar y maquetar este documento `IA.md` resumiendo las interacciones de diseño. | Creación del documento de registro final. |

### Nota sobre el uso de IA en este proyecto

La Inteligencia Artificial fue utilizada bajo un rol de "arquitecto consultor" y par de revisión. Se le delegó la tarea de analizar la estructura del código base, proponer planes de desarrollo ordenados y explicar conceptos técnicos de ASP.NET Core (como Minimal APIs y Content Negotiation). 

La escritura del código, su adaptación a la lógica del negocio, la ejecución de pruebas escalonadas y la creación de los *commits* correspondientes fue realizada íntegramente por el estudiante, verificando en cada iteración que el resultado compilara y funcionara según los requerimientos académicos antes de integrar nuevos cambios.