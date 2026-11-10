using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using todoExample.Service.Security;

namespace todoExample.Service.Tests;

public class JwtSettingsRegistrationTests
{
    private static IStartupValidator BuildValidator(string? secretKey)
    {
        var values = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "todoExample.api",
            ["Jwt:Audience"] = "todoExample.clients"
        };
        if (secretKey is not null) values["Jwt:SecretKey"] = secretKey;

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        var services = new ServiceCollection();
        services.AddJwtSettings(configuration);

        return services.BuildServiceProvider().GetRequiredService<IStartupValidator>();
    }

    [Fact]
    public void StartupValidation_Fails_WhenSecretKeyMissing()
    {
        Assert.Throws<OptionsValidationException>(() => BuildValidator(null).Validate());
    }

    [Fact]
    public void StartupValidation_Fails_WhenSecretKeyShorterThan32Bytes()
    {
        var shortKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

        Assert.Throws<OptionsValidationException>(() => BuildValidator(shortKey).Validate());
    }

    [Fact]
    public void StartupValidation_Passes_WithValidKey()
    {
        BuildValidator(Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))).Validate();
    }
}
