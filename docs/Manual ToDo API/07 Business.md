# 07 Business

> Borrador de estructura. Redactar en la revisión por secciones con Omar o Julio.


## 7.1 Services

- Un service es la clase con las reglas de negocio. Por ejemplo: "no se puede completar una tarea ya completada".
- `TodoListService`, `TodoTaskService`, `TagService`, `TodoTaskTagService`, `AccountService`

## 7.2 Tabla regla del PDF → dónde se valida

| Regla del ejercicio | Dónde se valida |
|---|---|
| Usuario: nombre y correo obligatorios; correo único; un usuario deshabilitado no puede crear listas ni tareas | AccountService / Services |
| Lista: nombre obligatorio; un usuario no puede tener dos listas activas con el mismo nombre; solo el dueño la modifica; no se puede eliminar una lista con tareas pendientes | TodoListService |
| Tarea: título obligatorio; al crearla queda pendiente; no se puede completar dos veces; al completarla se registra `CompletedAt`; solo una completada se puede reabrir (y al reabrir, la fecha se borra); una completada no se puede eliminar; el cliente no puede modificar campos internos | TodoTaskService |
| Tag: nombre obligatorio y único entre los activos del mismo usuario; una tarea no puede tener el mismo tag dos veces; solo se pueden usar tags del mismo usuario; un tag deshabilitado no se puede asignar a tareas nuevas; quitar un tag no borra el tag; deshabilitar un tag no borra sus relaciones | TagService / TodoTaskTagService |
| Búsqueda: filtros combinables por completada (`completed`), por tag (`tag`) y por texto en título o descripción (`text`) | TodoTaskService — búsqueda |

## 7.3 Notas

- Los tags deshabilitados sí se muestran en "tags de una tarea" (con `enabled: false`), pero no se pueden asignar a tareas nuevas ni se usan para buscar.
- El `userId` sale del token, no de la URL. Esto impide que un usuario consulte datos de otro.
