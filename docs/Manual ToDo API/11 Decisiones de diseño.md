# 11 Decisiones de diseño

Cada decisión incluye el por qué.

## 11.1 Tecnológicas

- **.NET 10 / C#:** plataforma del equipo.
- **MySQL:** base de datos relacional del equipo.
- **Entity Framework Core:** ORM que traduce entre clases de C# y tablas. Se migró desde Dapper para no escribir el SQL a mano en cada consulta y para usar el filtro global de `Enabled`.
- **ASP.NET Identity + JWT:** usuarios y login ya resueltos por el framework. El token dura 180 minutos.

## 11.2 De diseño

- **Guid en lugar de long para los Id:** el PDF pedía long. Al final se decidió Guid, consistente con el Id de los usuarios de Identity.
- **`Enabled` como borrado lógico (sin columna `Removed`):** decisión del equipo. Lo deshabilitado se trata como inexistente (responde 404).
- **El `userId` sale del token, no de la URL:** el PDF sugería `/api/users/{userId}/tasks`. Tomarlo del token impide que un usuario consulte datos de otro.
- **Rutas:** `/api/v1/TodoList`, `/api/v1/TodoTask`, `/api/v1/Tag` y `/api/v1/TodoTask/{taskId}/tags/{tagId}`.
- **Restrict en todas las relaciones:** nada se borra en cascada. Las reglas de borrado las decide el service.
- **Cada relación se declara una sola vez,** en el mapping de la entidad hija.
- **Formato estándar `ApiResponseDto<T>`:** `{ error, message, data }` en todos los endpoints de negocio.
- **Try/catch en cada endpoint**, en lugar de un manejador global.
- **Sin métodos privados en los controllers:** `GetUserId()` pasó a ser una extensión reutilizable.
- **Los tags deshabilitados sí se muestran en "tags de una tarea"** (con `enabled: false`), pero no se pueden asignar a tareas nuevas ni se usan para buscar.
