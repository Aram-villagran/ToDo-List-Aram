# ToDoExample — To-Do List API

Backend en C# con .NET 10 (ASP.NET Core Web API) para administrar tareas pendientes. Cada usuario tiene sus propias listas, cada lista tiene tareas, y las tareas se clasifican con tags.

- **Stack:** C# / .NET 10, ASP.NET Core Web API, MySQL, Entity Framework Core, ASP.NET Identity + JWT
- **Arquitectura:** cebolla — Core, Infrastructure, Business, API. Ver [docs/Manual ToDo API](docs/Manual%20ToDo%20API/Manual%20ToDo%20API.md)
- **API local:** `http://localhost:5031` y `https://localhost:7258`
- **Documentación completa:** [docs/Manual ToDo API](docs/Manual%20ToDo%20API/Manual%20ToDo%20API.md) (14 secciones, diagramas en Mermaid)

## Qué hace

- Usuarios con login por JWT (token de 180 minutos).
- Listas de tareas por usuario, con nombre único entre las activas del mismo usuario.
- Tareas con completar / reabrir, y búsqueda con filtros combinables: completadas, por tag y por texto.
- Tags por usuario, asignables a tareas.
- Borrado lógico con `Enabled`. Lo deshabilitado se trata como inexistente (404).

Las reglas completas están en [docs/Manual ToDo API/07 Business.md](docs/Manual%20ToDo%20API/07%20Business.md) y la referencia de endpoints en [docs/Manual ToDo API/09 Referencia de endpoints.md](docs/Manual%20ToDo%20API/09%20Referencia%20de%20endpoints.md).

## Requisitos

- .NET 10 SDK
- MySQL (local)
- Visual Studio o VS Code
- Postman (opcional, para probar la API)

## Cómo correrlo en local

### 1. Clonar y restaurar

```bash
git clone https://github.com/Aram-villagran/ToDo-List-Aram.git
cd ToDo-List-Aram
dotnet restore
```

### 2. Configurar los secretos en local (obligatorio)

`appsettings.json` **no contiene secretos**. Sin configuración local, la API no arranca: la validación de JWT exige una clave en base64 de al menos 32 bytes.

Genera una clave nueva (no reutilices ninguna clave que haya estado en el repositorio):

```powershell
# PowerShell
[Convert]::ToBase64String((1..64 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
```

```bash
# Bash / Git Bash
openssl rand -base64 64
```

Guarda esa clave y la contraseña de tu MySQL local con secretos de usuario (recomendado, nunca se sube a Git):

```bash
dotnet user-secrets init --project UserInterface/todoExample.api
dotnet user-secrets set "Jwt:SecretKey" "TU_CLAVE_NUEVA_EN_BASE64" --project UserInterface/todoExample.api
dotnet user-secrets set "ConnectionStrings:DefaultMySQLConnection" "Server=127.0.0.1;port=3306;database=todo;uid=root;pwd=TU_CONTRASEÑA_LOCAL;Allow User Variables=True;Connection Timeout=3600;Pooling=True" --project UserInterface/todoExample.api
```

Alternativa: copia `UserInterface/todoExample.api/appsettings.Development.example.json` a `UserInterface/todoExample.api/appsettings.Development.json` (ese archivo está en `.gitignore`, no se sube) y pon ahí tus valores reales.

> La API debe estar detenida antes de correr `dotnet ef` o `dotnet build`, porque Visual Studio bloquea los archivos.

### 3. Migraciones

Desde la raíz de la solución:

```bash
dotnet ef --project ./Infrastructure/todoExample.MysqlDAL/todoExample.MysqlDAL.csproj --startup-project ./UserInterface/todoExample.api/todoExample.api.csproj database update
```

Para crear una migración nueva, cambia `AddNewMigration` por un nombre descriptivo:

```bash
dotnet ef --project ./Infrastructure/todoExample.MysqlDAL/todoExample.MysqlDAL.csproj --startup-project ./UserInterface/todoExample.api/todoExample.api.csproj migrations add "AddNewMigration"
```

### 4. Arrancar

```bash
dotnet run --project UserInterface/todoExample.api
```

La API queda en `http://localhost:5031`.

### 5. Entrar

- Login: `POST /api/v1/Account/Login`
- El `Register` solo lo puede usar un admin.
- Si todo empieza a dar 401, el token caducó (180 minutos): vuelve a iniciar sesión.

En Postman: POST y PUT llevan body en raw → JSON; GET, PATCH y DELETE van sin body (excepto la asignación de tags, que es un POST sin body). Un 404 con body vacío significa que la URL no coincide con ninguna ruta; un 404 con mensaje significa que llegó al código.

## Estructura

```
ToDoExample.sln
├── Core/todoExample.domain        # Entidades, DTOs, interfaces
├── Infrastructure/
│   ├── todoExample.MysqlDAL       # EF Core: contexto, mappings, migraciones, repositorios
│   └── todoExample.DapperDAL      # Código anterior con Dapper, ya no se usa (pendiente decidir si se borra)
├── Business/todoExample.Service   # Services con las reglas de negocio
├── UserInterface/todoExample.api  # Controllers, Program.cs, appsettings.json
├── Tests/todoExample.Service.Tests
└── docs/
    ├── Manual ToDo API/           # Manual completo en Markdown (Obsidian)
    └── superpowers/
```

## Pruebas

Pruebas manuales en Postman: autenticación, listas, tareas, tags, asignación de tags y los 6 casos de filtros. Detalle en [docs/Manual ToDo API/10 Pruebas.md](docs/Manual%20ToDo%20API/10%20Pruebas.md).

## Pendientes

- Consultar un usuario por Id y deshabilitar un usuario (los pedía el ejercicio, no se implementaron).
- Confirmar que la rama `EfRepositories` se unió a `main`.
- Decidir si se elimina `todoExample.DapperDAL` y si se restaura `HomeController`.
- Detalle en [docs/Manual ToDo API/13 Pendientes y mejoras.md](docs/Manual%20ToDo%20API/13%20Pendientes%20y%20mejoras.md).
