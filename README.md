# ToDoExample

## Migraciones (Entity Framework Core)

Ejecutar los comandos desde la raíz de la solución.

### Crear una migración

Cambia `"AddNewMigration"` por un nombre descriptivo del cambio.

```bash
dotnet ef --project ./Infrastructure/todoExample.MysqlDAL/todoExample.MysqlDAL.csproj --startup-project ./UserInterface/todoExample.api/todoExample.api.csproj migrations add "AddNewMigration"
```

### Aplicar las migraciones a la base de datos

```bash
dotnet ef --project ./Infrastructure/todoExample.MysqlDAL/todoExample.MysqlDAL.csproj --startup-project ./UserInterface/todoExample.api/todoExample.api.csproj database update
```
