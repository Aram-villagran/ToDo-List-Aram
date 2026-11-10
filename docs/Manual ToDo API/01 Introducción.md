# 01 Introducción

## 1.1 Propósito

Este manual documenta la To-Do List API Rest, el backend de una aplicación para administrar tareas pendientes.

La aplicación permite que cada usuario tenga sus propias listas de tareas. Cada lista agrupa tareas, y cada tarea se puede clasificar con etiquetas o tags. Por ejemplo, un usuario crea la lista "Trabajo", agrega la tarea "Pagar la luz" y le asigna el tag "urgente".

Este manual existe para que un desarrollador que no participe en el proyecto pueda entenderlo, ejecutarlo en su equipo y mantenerlo sin depender de quien lo escribió. Por eso se explica no solo qué hace cada parte, sino por qué se hizo así.

## 1.2 Audiencia

La audiencia principal son desarrolladores del equipo que necesiten entender, ejecutar o mantener el proyecto.

El manual asume que el lector conoce C# y bases de datos relacionales a nivel básico. Cada término propio del proyecto se define la primera vez que aparece. Los conceptos generales (API REST, JWT, Entity Framework) se explican en [[14 Glosario]] y en la sección donde se usan por primera vez.

> Nota: para la documentación oficial de una empresa, la audiencia probablemente será distinta. Antes de usar este manual fuera del equipo, hay que preguntar a quién va dirigido.

## 1.3 Alcance

La versión 1.0 cubre solo el backend y la base de datos:

- Las entidades Usuario, Lista de Tasks, Task y Tags, y la relación entre TaskTags.
- Las reglas de negocio de listas, tasks y tags.
- La API REST, la autenticación con JWT y el formato de respuesta.
- La base de datos MySQL y las migraciones de Entity Framework.

La versión 1.0 no cubre el frontend. El frontend en React se inició en noviembre de 2026 y se pausó para aprender primero los fundamentos. Se documentará cuando lo termine.

Tampoco cubre el despliegue en producción. El manual explica cómo ejecutar el proyecto en local.

## 1.4 Requisitos

Para ejecutar el proyecto se necesita:

- .NET 10 SDK
- MySQL (servidor local o accesible desde el equipo)
- MySQL Workbench (recomendado, para revisar las tablas)
- Visual Studio (backend) o VS Code
- Postman (para probar la API)
- Git

La API corre en local en `http://localhost:5031` y en `https://localhost:7258`. El detalle de instalación está en [[02 Puesta en marcha]].
