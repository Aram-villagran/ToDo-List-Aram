using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace todoExample.Service.Security;

public static class JwtSettingsRegistration
{
    public static IServiceCollection AddJwtSettings(this IServiceCollection services, IConfiguration configuration)
    {
        // ValidateOnStart hace que la app no arranque con una llave ausente o corta
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .Validate(HasValidSigningKey, $"{JwtSettings.SectionName}:SecretKey debe ser base64 de al menos 32 bytes.")
            .ValidateOnStart();

        return services;
    }

    private static bool HasValidSigningKey(JwtSettings settings)
    {
        try
        {
            settings.GetSigningKey();
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}
