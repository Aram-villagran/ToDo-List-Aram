using Microsoft.IdentityModel.Tokens;

namespace todoExample.Service.Security;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    private const int MinimumKeyBytes = 32;

    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 180;
    public int RefreshTokenDays { get; set; } = 7;

    public SymmetricSecurityKey GetSigningKey()
    {
        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(SecretKey);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException($"{SectionName}:SecretKey debe estar en base64.", ex);
        }

        if (keyBytes.Length < MinimumKeyBytes)
        {
            throw new InvalidOperationException(
                $"{SectionName}:SecretKey debe medir al menos {MinimumKeyBytes} bytes.");
        }

        return new SymmetricSecurityKey(keyBytes);
    }

    public TokenValidationParameters CreateValidationParameters(bool validateLifetime) => new()
    {
        ValidateIssuer = true,
        ValidIssuer = Issuer,
        ValidateAudience = true,
        ValidAudience = Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = GetSigningKey(),
        ValidateLifetime = validateLifetime,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = "sub",
        RoleClaimType = "role"
    };
}
