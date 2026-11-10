# 09 Referencia de endpoints

> Borrador de estructura. Una tabla por controller: método, ruta, body y respuestas.

## 9.1 TodoList — `/api/v1/TodoList`

| Método | Ruta | Body | Respuestas |
|---|---|---|---|
| GET | `/api/v1/TodoList` | — | 200 lista de listas del usuario |
| POST | `/api/v1/TodoList` | JSON con nombre | 200 creada / 400 regla no cumplida |
| PUT | `/api/v1/TodoList/{id}` | JSON | 200 actualizada / 404 no existe o no es del usuario |
| DELETE | `/api/v1/TodoList/{id}` | — | 200 eliminada / 400 tiene tareas pendientes / 404 no existe |

## 9.2 TodoTask — `/api/v1/TodoTask`

| Método | Ruta | Body | Respuestas |
|---|---|---|---|
| GET | `/api/v1/TodoTask?listId=` | — | 200 tareas de la lista |
| GET | `/api/v1/TodoTask/pending` | — | 200 tareas pendientes del usuario |
| GET | `/api/v1/TodoTask/search?completed=&tag=&text=` | — | 200 tareas con filtros combinables |
| POST | `/api/v1/TodoTask` | JSON con título | 200 creada, queda pendiente |
| PUT | `/api/v1/TodoTask/{id}` | JSON | 200 actualizada / 404 no existe |
| PATCH | `/api/v1/TodoTask/{id}/complete` | — | 200 completada, registra `CompletedAt` / 400 ya estaba completada |
| PATCH | `/api/v1/TodoTask/{id}/reopen` | — | 200 reabierta, borra `CompletedAt` / 400 no estaba completada |
| DELETE | `/api/v1/TodoTask/{id}` | — | 200 eliminada / 400 estaba completada / 404 no existe |

## 9.3 Tag — `/api/v1/Tag`

| Método | Ruta | Body | Respuestas |
|---|---|---|---|
| GET | `/api/v1/Tag` | — | 200 tags del usuario |
| POST | `/api/v1/Tag` | JSON con nombre | 200 creado / 400 nombre duplicado |
| PUT | `/api/v1/Tag/{id}` | JSON | 200 actualizado / 404 no existe |
| PATCH | `/api/v1/Tag/{id}/disable` | — | 200 deshabilitado |
| DELETE | `/api/v1/Tag/{id}` | — | 200 eliminado |

## 9.4 TodoTaskTag — `/api/v1/TodoTask/{taskId}/tags/{tagId}`

| Método | Ruta | Body | Respuestas |
|---|---|---|---|
| POST | `/api/v1/TodoTask/{taskId}/tags/{tagId}` | — | 200 tag asignado / 400 ya lo tenía o tag deshabilitado |
| DELETE | `/api/v1/TodoTask/{taskId}/tags/{tagId}` | — | 200 tag quitado (no borra el tag) |

## 9.5 Account — `/api/v1/Account`

| Método | Ruta | Body | Respuestas |
|---|---|---|---|
| POST | `/api/v1/Account/Login` | JSON con correo y contraseña | 200 token JWT (180 min) / 401 credenciales |
| POST | `/api/v1/Account/Register` | JSON | Solo admin |

En Postman: POST y PUT llevan body en raw → JSON; GET, PATCH y DELETE van sin body (excepto la asignación de tags, que es un POST sin body). Un 404 con body vacío significa que la URL no coincide con ninguna ruta; un 404 con mensaje significa que llegó al código.
