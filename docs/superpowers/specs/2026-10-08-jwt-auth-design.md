# Autenticación JWT, refresh token y seed de Identity

Fecha: 2026-10-08
Estado: pendiente de revisión

## Objetivo

Que la API `todoExample.api` permita registrar usuarios, iniciar sesión obteniendo un access token JWT (3 horas) y un refresh token, y renovar ambos con un endpoint `RefreshToken`. La base de datos debe crearse con los roles `user` y `admin` y un usuario administrador inicial.

## Decisiones confirmadas

- La "salt" pedida es la **llave secreta de firma del JWT** (`Jwt:SecretKey` en `appsettings`).
- Las cuentas se manejan con `UserManager<ApplicationUser>` (Identity), no con el repositorio Dapper.
- Un refresh token activo por usuario; cada login o refresh lo reemplaza (rotación).

## Supuestos (corregir si no aplican)

- El usuario semilla recibe el rol `admin`, se llama "John Doe", está habilitado y con correo confirmado.
- El refresh token dura 7 días.
- Los usuarios creados con `Register` reciben el rol `user`.

## Configuración (`appsettings.json` y `appsettings.Development.json`)

```json
"Jwt": {
  "SecretKey": "<64 bytes aleatorios en base64>",
  "Issuer": "todoExample.api",
  "Audience": "todoExample.clients",
  "AccessTokenMinutes": 180,
  "RefreshTokenDays": 7
}
```

Se mapea a `JwtSettings` (Options pattern). La aplicación falla al arrancar si `SecretKey` falta o mide menos de 32 bytes.

## Endpoints (`AccountController`, ruta `/api/v1/Account`)

| Método y ruta | Auth | Entrada | Respuesta |
|---|---|---|---|
| `POST /Register` | anónimo | `UserCreateDto` | `201 UserDto` · `400` con errores de Identity |
| `POST /Login` | anónimo | `LoginRequestDto` | `200 TokenResponseDto` · `401` |
| `POST /RefreshToken` | anónimo | `RefreshTokenRequestDto` | `200 TokenResponseDto` · `401` |
| `GET /` | JWT | `?email=` opcional | `200 List<UserDto>` |

Los `401` de `Login` y `RefreshToken` usan un mensaje genérico, sin indicar si el email existe.

## DTOs (`todoExample.domain/DTOs`)

- `UserCreateDto`: `FirstName`, `LastName`, `Email`, `Password`.
- `UserDto`: `Id` (`Guid`), `FirstName`, `LastName`, `Email`, `Roles`.
- `LoginRequestDto`: `Email`, `Password`.
- `RefreshTokenRequestDto`: `AccessToken`, `RefreshToken`.
- `TokenResponseDto`: `AccessToken`, `AccessTokenExpiresAt`, `RefreshToken`, `RefreshTokenExpiresAt`.

Los DTOs de entrada usan DataAnnotations (`[Required]`, `[EmailAddress]`) para que `[ApiController]` devuelva `400` automáticamente.

## Componentes

### `todoExample.domain`
- DTOs anteriores.
- `IAccountService` (async): `GetUsersAsync(string? email)`, `CreateUserAsync(UserCreateDto)`, `LoginAsync(LoginRequestDto)`, `RefreshTokenAsync(RefreshTokenRequestDto)`. Login y refresh devuelven `TokenResponseDto?` (`null` = no autorizado).
- `ITokenService`: `CreateAccessToken(user, roles)`, `GetPrincipalFromExpiredToken(token)`, `CreateRefreshToken()`, `HashRefreshToken(token)`.

### `todoExample.Service`
- `JwtSettings`.
- `TokenService`: JWT HS256 con claims `sub` (Id), `email`, `jti`, `given_name`, `family_name` y un claim de rol por cada rol. Refresh token de 64 bytes aleatorios (base64url); se persiste su hash SHA-256.
- `AccountService`: usa `UserManager` e `ITokenService`.
- `DependencyContainer.AddServices` registra ambos servicios y `JwtSettings`.

### `todoExample.api`
- `AddAuthentication(JwtBearer)` con los mismos parámetros de validación que `TokenService` (issuer, audience, firma, vida, `ClockSkew` = 0).
- `UseAuthentication()` y `UseAuthorization()`.
- `AccountController`.

## Flujos

**Login**
1. Buscar usuario por email. No existe → `401`.
2. `Enabled == false` o bloqueado → `401`.
3. `CheckPasswordAsync`; si falla, `AccessFailedAsync` (bloqueo tras intentos fallidos) y `401`.
4. `ResetAccessFailedCountAsync`, emitir access token y refresh token, guardar hash y expiración del refresh token.

**RefreshToken**
1. Validar firma, issuer y audience del access token ignorando la expiración. Inválido → `401`.
2. Obtener el usuario por el claim `sub`. No existe, deshabilitado o bloqueado → `401`.
3. Comparar el hash del refresh token recibido con el guardado (comparación en tiempo constante) y verificar que no haya expirado → si no, `401`.
4. Emitir tokens nuevos y reemplazar el guardado.

**Persistencia del refresh token**: `UserManager.SetAuthenticationTokenAsync(user, "todoExample", "RefreshToken", "<hashBase64>|<expiraciónUtcISO8601>")` sobre la tabla existente `AspNetUserTokens`. Sin migración.

## Seed (migración nueva con `HasData`)

- Roles `user` y `admin` con `Id`, `NormalizedName` y `ConcurrencyStamp` fijos.
- Usuario: `UserName` y `Email` = `doe@sacai.com.mx`, normalizados en mayúsculas, `EmailConfirmed = true`, `Enabled = true`, `FirstName = "John"`, `LastName = "Doe"`, `PasswordHash` precalculado con `PasswordHasher<ApplicationUser>` (la contraseña en texto plano no se guarda en el código), `SecurityStamp` y `ConcurrencyStamp` fijos, fechas UTC fijas.
- Relación usuario → rol `admin` en `AspNetUserRoles`.
- Se aplica con `dotnet ef database update`.

## Correcciones incluidas

- `UserDto.Id` → `Guid`; `UserCreateDto` con contraseña y nombre separado.
- `MysqlDAL/DependencyContainer`: quitar `RequireConfirmedAccount`, `AddSignInManager` y los dos `AddTokenProvider` redundantes.
- `ApplicationUser`: quitar la propiedad `Email` que oculta la de `IdentityUser` (CS0114) y marcarla `IsRequired()` con longitud 256 en `ApplicationUserMap`, para que el esquema no cambie.

## Pruebas

- Nuevo proyecto `Tests/todoExample.Service.Tests` (xUnit), agregado a la solución, con pruebas de `TokenService`: duración de 3 horas, claims y roles, rechazo de firma inválida, aceptación de token vencido sólo en `GetPrincipalFromExpiredToken`, refresh tokens únicos y hash determinista.
- Prueba de punta a punta con `todoExample.api.http` contra MySQL local (puerto 3307): login con el usuario semilla → endpoint protegido con el token → refresh → token viejo de refresh rechazado.

## Fuera de alcance

- Logout / revocación explícita, varios dispositivos por usuario, confirmación de correo, recuperación de contraseña.
- El proyecto `ToDoExample/` en la raíz, que no forma parte de la solución.
