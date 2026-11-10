# 14 Glosario

| Término                        | Definición                                                                                                                    |
| ------------------------------ | ----------------------------------------------------------------------------------------------------------------------------- |
| Backend / Frontend             | El backend guarda datos y aplica reglas (no se ve). El frontend es la pantalla con la que interactúa el usuario.              |
| API REST                       | El mesero del backend: direcciones web con métodos (GET leer, POST crear, PUT editar, PATCH cambio parcial, DELETE borrar).   |
| JSON                           | Formato de texto para intercambiar datos: `{ "title": "Pagar la luz" }`.                                                      |
| Base de datos (MySQL)          | Donde se guardan los datos, en tablas con filas y columnas.                                                                   |
| Arquitectura de cebolla        | Forma de organizar el código en capas. El centro (Core) no depende de nadie; las capas externas dependen de las internas.     |
| Entidad (Model)                | Clase de C# que representa una tabla (por ejemplo, `TodoTaskModel`).                                                          |
| DTO (Data Transfer Object)     | "Formulario" de entrada o salida: lo que el cliente envía o recibe. Protege los campos internos.                              |
| Repositorio                    | Clase encargada de leer y guardar datos en la base. No tiene nada que ver con un repositorio de Git.                          |
| Service                        | Clase con las reglas de negocio (Ej.: "no se puede completar una tarea ya completada").                                       |
| Controller                     | Recibe la petición HTTP, llama al service y devuelve la respuesta con su código (200, 404…).                                  |
| Entity Framework (EF)          | Herramienta que traduce entre clases de C# y tablas, y genera el SQL sola.                                                    |
| Dapper                         | Herramienta más ligera: se escribe el SQL a mano y Dapper convierte los resultados en objetos.                                |
| Mapping                        | Instrucciones para EF sobre cómo convertir clases en tablas: llaves, relaciones, columnas obligatorias.                       |
| Migración                      | Archivo que EF genera para crear o cambiar las tablas en la base de datos.                                                    |
| Llave primaria / foránea       | La primaria identifica cada fila (Id). La foránea apunta a otra tabla (por ejemplo, `TodoListId` en una tarea).               |
| Relación 1:n / n:n             | Uno a muchos (una lista, muchas tareas). Muchos a muchos (tareas y tags) se resuelve con una tabla intermedia.                |
| Borrado lógico (soft delete)   | En lugar de borrar la fila, se marca como deshabilitada (`Enabled = false`).                                                  |
| Borrado en cascada / Restrict  | Cascada: al borrar el padre se borran sus hijos. Restrict: no deja borrar un padre con hijos. Aquí se usa Restrict.           |
| JWT (token)                    | "Pulsera" digital que se obtiene al iniciar sesión y se envía en cada petición para identificar al usuario. Dura 180 minutos. |
| CORS                           | Regla del navegador: un sitio web solo puede llamar a una API de otro origen si la API lo permite. No es autenticación.       |
| Inyección de dependencias (DI) | .NET crea y entrega automáticamente las clases que cada componente necesita, si están registradas en `Program.cs`.            |
| Lambda / función flecha        | Función corta sin nombre: `t => t.UserId == userId` (C#) o `(e) => setEmail(e.target.value)` (JavaScript).                    |
| Inyección SQL                  | Ataque en el que un texto malicioso altera una consulta. Se evita enviando los valores como parámetros.                       |
