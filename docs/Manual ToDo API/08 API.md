# 08 API

> Borrador de estructura. Redactar en la revisión por secciones con Omar o Julio.

## 8.1 Qué es una API

- Una API REST interactúa como un intermediario entre el frontend y el backend, funciona por el protocolo http: direcciones web con métodos (GET leer, POST crear, PUT editar, PATCH cambio parcial, DELETE borrar).
- Se comunica con JSON (JavaScript Object Notation): `{ "title": "Pagar la luz" }`.

## 8.2 Controllers

- Un controller recibe la petición HTTP, llama al service y devuelve la respuesta con su código (200, 404…).
- `TodoListController`, `TodoTaskController`, `TagController`, `TodoTaskTagController`, `AccountController`
- Sin métodos privados en los controllers. `GetUserId()` es una extensión reutilizable: `User.GetUserId()`.

## 8.3 ApiResponse y manejo de errores

- Formato estándar `ApiResponseDto<T>`: `{ error, message, data }`.
- Try/catch en cada endpoint: `InvalidOperationException` → 400, `UnauthorizedAccessException` → 401, cualquier otra → 500 genérico, con el detalle registrado con `ILogger`.

## 8.4 JWT

- JWT (JSON Web Token) es un token que se obtiene al iniciar sesión y se envía en cada petición para identificar al usuario. Dura 180 minutos.
- Login: `POST /api/v1/Account/Login`.

## 8.5 CORS

- CORS (Cross-Origin Resource Sharing) es una regla del navegador: un sitio web solo puede llamar a una API de otro origen si la API lo permite. No es autenticación.
- Habilitado en la API para el frontend en `http://localhost:5173`.

## 8.6 Autorización y propiedad de recursos

- El `userId` sale del token, no de la URL.
- Solo el dueño puede modificar su lista, sus tareas y sus tags.
