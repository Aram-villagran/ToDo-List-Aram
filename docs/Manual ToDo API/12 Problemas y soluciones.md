# 12 Problemas y soluciones

## 12.1 Problemas resueltos

| Problema | Solución |
|---|---|
| Tipos long/Guid incompatibles en la primera migración | Corrección de entidades y nueva migración |
| La plantilla usaba EF para las tablas y Dapper para los datos | Adaptación del repositorio de Dapper a Guid y a `Enabled` |
| `UpdateAsync` no guardaba valores nulos | Corrección en el service / repositorio |
| Columna sobrante `UserId` en `todotasks` | Nueva migración `SyncModel` |
| Un service sin registrar en `Program.cs` | Registro en DI |
| Body vacío en Postman (error 415) | Enviar body en raw → JSON en POST y PUT |
| Tokens caducados (401) | Volver a iniciar sesión; el token dura 180 minutos |
| Conflicto de merge | Resolución manual y merge a `main` |

## 12.2 Lecciones

## 1. Llaves foráneas con tipos incompatibles (long vs Guid)

- **Síntoma:** al generar la primera migración, EF no pudo crear el contexto: _"The relationship from 'TodoTaskTagModel.Tag' to 'TagModel.TaskTags' with foreign key properties {'TagId' : Guid} cannot target the primary key {'Id' : long}"_.
- **Causa:** las entidades mezclaban tipos. Algunas llaves foráneas eran `Guid` y otras `long`, y `TodoTaskModel` y `TagModel` se relacionaban directamente entre sí (`List<TodoTaskModel>`) en lugar de hacerlo a través de la tabla intermedia `TodoTaskTagModel`.
- **Solución:** unificar todos los Id en `Guid` (decisión del equipo) y conectar ambas entidades únicamente a través de `TodoTaskTagModel`, cada una con su colección `TaskTags`.
- **Lección:** una llave foránea es una copia del Id de su padre, así que debe tener exactamente el mismo tipo. Una relación muchos a muchos se modela siempre con una entidad intermedia; si las navegaciones apuntan directo entre ambas entidades, EF intenta deducir una relación distinta a la diseñada.

## 2. Repositorio genérico diseñado para Id numéricos

- **Síntoma:** después de cambiar los Id a `Guid`, el repositorio de Dapper de la plantilla no podía insertar ni consultar correctamente.
- **Causa:** el repositorio estaba escrito para Id `long` autoincrementables. Obtenía el Id nuevo con `SELECT LAST_INSERT_ID()` (que con Guid regresa 0) y escribía el Id en el SQL sin comillas (un Guid sin comillas rompe la consulta). Además, filtraba por una columna `Removed` que ya no existía, porque el borrado lógico se cambió a `Enabled`.
- **Solución:** generar el Guid en C# antes del `INSERT` (`Guid.NewGuid()`), quitar `LAST_INSERT_ID`, poner el Id entre comillas en el SQL y reemplazar `Removed = 0` por `Enabled = 1`.
- **Lección:** cambiar el tipo de las llaves o el mecanismo de borrado lógico no es un cambio local: afecta todo el código de acceso a datos que dependa de ese supuesto. Antes de cambiar una decisión base, hay que buscar todos los lugares que la asumen.

## 3. `UpdateAsync` ignoraba los valores nulos

- **Síntoma (detectado al revisar el código):** al reabrir una tarea, `CompletedAt` debía quedar en `NULL`, pero la base de datos conservaba la fecha anterior. Lo mismo ocurría al quitar la fecha límite (`DueDate`).
- **Causa:** el método `UpdateAsync` del repositorio armaba el `UPDATE` solo con las propiedades que tenían valor; las nulas que no eran texto se omitían.
- **Solución:** incluir todas las columnas en el `UPDATE` (incluidos los nulos), excepto `Id`, `CreatedAt` y `DeletedAt`.
- **Lección:** con un `UPDATE` completo, nunca se debe enviar una entidad incompleta al repositorio, porque los campos faltantes se guardan como nulos. Regla derivada: los services siempre buscan la entidad completa antes de modificarla. (Con Entity Framework este riesgo desaparece, porque EF solo actualiza las columnas que cambiaron).

## 4. Columna `UserId` sobrante en `TodoTasks`

- **Síntoma:** error 500 al crear una tarea: _"Cannot add or update a child row: a foreign key constraint fails (FK_TodoTasks_AspNetUsers_UserId)"_.
- **Causa:** en el diseño inicial, la tarea pertenecía directamente al usuario. El modelo se corrigió (la tarea pertenece a una lista), pero la tabla en MySQL conservaba la columna `UserId` y su llave foránea. Al insertar, esa columna quedaba en `00000000-…`, un usuario inexistente. Además, una migración creada para quitarla se aplicó vacía, porque el snapshot de EF ya no tenía `UserId`.
- **Solución:** generar con EF la migración `SyncModel`, revisar que contuviera `DropForeignKey`, `DropIndex` y `DropColumn`, y aplicarla. Se confirmó con `DESCRIBE todotasks` (11 columnas, sin `UserId`).
- **Lección:** que el código compile no significa que la base de datos esté sincronizada con el modelo. Hay que revisar el contenido de cada migración antes de aplicarla, y confirmar el resultado en la base de datos.

## 5. Service sin registrar en la inyección de dependencias

- **Síntoma:** error 500 en todos los endpoints de asignación de tags: _"Unable to resolve service for type 'ITodoTaskTagService' while attempting to activate 'TodoTaskTagController'"_. La respuesta no tenía el formato `ApiResponse`.
- **Causa:** la línea `AddScoped<ITodoTaskTagService, TodoTaskTagService>()` no estaba en `Program.cs`: el cambio nunca se guardó ni se incluyó en el commit. El error ocurre al crear el controller, antes de entrar a cualquier método, por eso el try/catch no lo atrapa.
- **Solución:** registrar el service en `Program.cs`, antes de `builder.Build()`, guardar y reiniciar la API.
- **Lección:** cada service o repositorio nuevo debe registrarse en el contenedor de dependencias. Antes de cada commit, conviene guardar todo y revisar con `git status` que aparezcan los archivos esperados; un número de líneas sospechosamente bajo en el commit es una señal de archivos sin guardar.

## 6. Frontend bloqueado por CORS

- **Síntoma:** las peticiones desde el frontend (`http://localhost:5173`) a la API (`http://localhost:5031`) eran bloqueadas por el navegador, aunque en Postman funcionaban.
- **Causa:** al tener un puerto distinto, el navegador trata al frontend y a la API como orígenes diferentes, y bloquea la petición si la API no lo permite. Postman no es un navegador, así que no aplica esa regla.
- **Solución:** configurar una política de CORS en `Program.cs` (`AddCors` con `WithOrigins("http://localhost:5173")`, `AllowAnyHeader` y `AllowAnyMethod`) y aplicarla con `app.UseCors("Frontend")` antes de la autenticación y la autorización.
- **Lección:** CORS es una regla de seguridad del navegador, no de la API ni una forma de autenticación. El orden de los middlewares en `Program.cs` importa: las peticiones pasan por ellos de arriba hacia abajo.

## 7. Conflicto de merge con marcas en el código

- **Síntoma:** después de unir ramas, el proyecto no compilaba: _"Se encontró un marcador de conflicto de fusión"_ (CS8300) en `TodoTaskController.cs`.
- **Causa:** el mismo archivo se modificó en dos ramas distintas, y Git no pudo decidir qué versión conservar, así que dejó ambas dentro del archivo, separadas por `<<<<<<<`, `=======` y `>>>>>>>`.
- **Solución:** elegir la versión correcta en cada bloque, eliminar las marcas y confirmar con `git grep -n "<<<<<<<"` que no quedara ninguna antes del commit.
- **Lección:** `git add` marca un conflicto como resuelto aunque las marcas sigan en el archivo; Git no revisa el contenido. Antes de hacer commit después de un merge, hay que verificar que no queden marcas y que el proyecto compile. Trabajar en ramas cortas y unirlas seguido reduce los conflictos.
