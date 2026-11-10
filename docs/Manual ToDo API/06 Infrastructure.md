# 06 Infrastructure

> Borrador de estructura. Redactar en la revisión por secciones con Omar o Julio.

## 6.1 Contexto de Entity Framework

- `IdentityMySqlContext` en `todoExample.MysqlDAL`.
- Entity Framework (EF) es un ORM: traduce entre clases de C# y tablas, y genera el SQL.
- Antes se usaba Dapper: se escribe el SQL a mano y Dapper convierte los resultados en objetos. La migración completa a EF se hizo el 2 de noviembre de 2026.

## 6.2 Mappings

- Un mapping son las instrucciones para EF sobre cómo convertir clases en tablas: llaves, relaciones y columnas obligatorias.
- Cada relación se declara una sola vez, en el mapping de la entidad hija (la que tiene la llave foránea).
- Todas las relaciones usan `Restrict`, no cascada.

## 6.3 Migraciones

- Una migración es un archivo que EF genera para crear o cambiar las tablas en la base de datos.
- Migración `SyncModel`: corrigió una columna sobrante `UserId` en `todotasks`.

## 6.4 Repositorios

- Un repositorio es la clase encargada de leer y guardar datos en la base.
- Repositorio genérico `EfRepository<T>` con condiciones en LINQ.
- Repositorio específico `TodoTaskTagRepository` para la tabla intermedia.
- Filtro global: `HasQueryFilter(e => e.Enabled)`. Lo deshabilitado no se devuelve en las consultas normales.

## 6.5 DapperDAL

- El proyecto `todoExample.DapperDAL` ya no se usa. Pendiente decidir si se borra. Ver [[13 Pendientes y mejoras]].
