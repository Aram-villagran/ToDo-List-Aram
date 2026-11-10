# 02 Puesta en marcha

> Borrador de estructura,  revisión por secciones con Omar o Julio.

## 2.1 Requisitos del entorno

- .NET 10 SDK
- MySQL
- MySQL Workbench (recomendado)
- Visual Studio (backend) o VS Code
- Postman
- Git

## 2.2 Cadena de conexión

- Dónde se configura la conexión a MySQL.
- Qué valores cambian en local y cuáles no se suben al repositorio.
- No guardar contraseñas ni credenciales en este manual.

## 2.3 Migraciones

Desde la carpeta de la solución:

```bash
dotnet ef --project ./Infrastructure/todoExample.MysqlDAL/todoExample.MysqlDAL.csproj --startup-project ./UserInterface/todoExample.api/todoExample.api.csproj migrations add <Nombre>
```

```bash
dotnet ef --project ./Infrastructure/todoExample.MysqlDAL/todoExample.MysqlDAL.csproj --startup-project ./UserInterface/todoExample.api/todoExample.api.csproj database update
```

La API debe estar detenida antes de correr `dotnet ef` o `dotnet build`, porque Visual Studio bloquea los archivos.

## 2.4 Cómo arrancar

- API local: `http://localhost:5031` y `https://localhost:7258`
- Frontend local (pausado): `http://localhost:5173` (React con Vite)

## 2.5 Con qué usuario entrar

- Login: `POST /api/v1/Account/Login`
- El `Register` solo lo puede usar un admin.
- El token JWT dura 180 minutos. Si todo empieza a dar 401, hay que volver a iniciar sesión.
