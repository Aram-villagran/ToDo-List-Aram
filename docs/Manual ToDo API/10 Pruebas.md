# 10 Pruebas

> Borrador de estructura. Redactar en la revisión por secciones con Omar o Julio.

## 10.1 Casos probados

Pruebas de punta a punta en Postman, del 28 de octubre al 2 de noviembre de 2026:

- Autenticación (login, token, token caducado → 401)
- Listas (crear, editar, eliminar, nombre duplicado, lista con tareas pendientes)
- Tareas (crear, completar, completar dos veces, reabrir, eliminar completada)
- Tags (crear, nombre duplicado, deshabilitar)
- Asignación de tags a tareas
- Búsqueda: los 6 casos de filtros combinables (`completed`, `tag`, `text`)

## 10.2 Resultados

- Funciona y está probado: listas, tareas, tags, asignación de tags, filtros y autenticación, todo con Entity Framework.
