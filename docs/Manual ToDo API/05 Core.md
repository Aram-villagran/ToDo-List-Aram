# 05 Core

> Borrador de estructura. Redactar en la revisión por secciones con Omar o Julio.

## 5.1 Entidades

- `TodoListModel`, `TodoTaskModel`, `TagModel`, `TodoTaskTagModel`, `ApplicationUser`, `BaseEntity`
- Todas las entidades de negocio heredan de `BaseEntity`: `Id` (Guid), `Enabled`, `CreatedAt`, `UpdatedAt` y `DeletedAt`.
- `TodoTaskTagModel` no hereda de `BaseEntity`. Es la tabla intermedia y su llave es compuesta: `TodoTaskId` + `TagId`.

## 5.2 DTOs

- Un DTO es el "formulario" de entrada o salida: lo que el cliente envía o recibe.
- Protege los campos internos. El cliente no puede modificar directamente `Id`, lista, fechas ni estado.
- `ApiResponseDto<T>`: formato estándar (JSON)`{ error, message, data }` en todos los endpoints de negocio.

## 5.3 Interfaces

- Interfaces de services: `ITodoListService`, `ITodoTaskService`, `ITagService`, `ITodoTaskTagService`
- Interfaces de repositorios: `IRepository<T>`, `ITodoTaskTagRepository`
- El Core define los contratos. Las capas externas los implementan.
