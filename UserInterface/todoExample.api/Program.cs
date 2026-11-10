using Microsoft.AspNetCore.Authentication.JwtBearer;
using todoExample.domain.Respository;
using todoExample.domain.Respository.Base;
using todoExample.domain.Service;
using todoExample.MysqlDAL.Repository;
using todoExample.Service;
using todoExample.Service.Security;
using todoExample.Service.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ITodoTaskTagService, TodoTaskTagService>();

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        // La validez de Jwt:SecretKey se verifica al arrancar (ValidateOnStart en AddJwtSettings)
        options.TokenValidationParameters = jwtSettings.CreateValidationParameters(validateLifetime: true);
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<ITodoTaskTagService, TodoTaskTagService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<ITodoTaskTagRepository, TodoTaskTagRepository>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
