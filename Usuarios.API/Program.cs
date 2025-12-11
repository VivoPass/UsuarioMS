using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using log4net;
using log4net.Config;
using MassTransit;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Usuarios.API.Controllers;
using Usuarios.Application.Commands.CommandHandlers;
using Usuarios.Application.Events;
using Usuarios.Application.Events.EventHandlers;
using Usuarios.Application.Validations;
using Usuarios.Domain.Factories;
using Usuarios.Domain.Interfaces;
using Usuarios.Infrastructure.Configurations;
using Usuarios.Infrastructure.Consumers;
using Usuarios.Infrastructure.Interfaces;
using Usuarios.Infrastructure.Persistences.Repositories.MongoDB;
using Usuarios.Infrastructure.Queries.QueryHandlers;
using Usuarios.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar log4net
XmlConfigurator.Configure(new FileInfo("log4net.config"));
builder.Services.AddSingleton<ILog>(provider => LogManager.GetLogger(typeof(UsuariosController)));

Env.Load();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Usuarios API",
        Version = "v1",
        Description = "API del Microservicio de Usuarios que gestiona la información de usuarios, roles y actividades.",
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});

// Registrar configuración de MongoDB
builder.Services.AddSingleton<MongoDBConfig>();
builder.Services.AddSingleton<AuditoriaDbConfig>();

// REGISTRA EL REPOSITORIO ANTES DE MediatR
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IUsuarioHistorialActividad, UsuarioHistorialActividadRepository>();
builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
builder.Services.AddScoped<IUsuarioFactory, UsuarioFactory>();
builder.Services.AddScoped<IRolFactory, RolFactory>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// REGISTRA MediatR PARA TODOS LOS HANDLERS
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CrearUsuarioCommandHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ModificarUsuarioCommandHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ModificarPreferenciasUsuarioCommandHandler).Assembly));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetUsuarioByCorreoQueryHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetUsuarioByIdQueryHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetTodosUsuariosQueryHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(HistActUsuarioQueryHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetRolByIdQueryHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetRolByNombreQueryHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetTodosRolesQueryHandler).Assembly));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(HistorialActividadEventHandler).Assembly));

builder.Services.AddValidatorsFromAssemblyContaining<CrearUsuarioDTOValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<ModificarUsuarioDTOValidation>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<HistActConsumer>();

    busConfigurator.SetKebabCaseEndpointNameFormatter();
    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(Environment.GetEnvironmentVariable("RABBIT_URL")), h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RABBIT_USERNAME"));
            h.Password(Environment.GetEnvironmentVariable("RABBIT_PASSWORD"));
        });
        configurator.ReceiveEndpoint(Environment.GetEnvironmentVariable("RABBIT_QUEUE_ACTIVITY"), e => {
            e.ConfigureConsumer<HistActConsumer>(context);
        });
        configurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        configurator.ConfigureEndpoints(context);
    });
});
EndpointConvention.Map<HistorialActividadEvent>(new Uri("queue:" + Environment.GetEnvironmentVariable("RABBIT_QUEUE_ACTIVITY")));

// Configuración CORS permisiva (¡Solo para desarrollo!)
builder.Services.AddCors(options =>
{
    // Define la política "AllowAll" para desarrollo
    options.AddPolicy("AllowAll",
        builder =>
        {
            // Permite cualquier origen (dominio), método (GET, POST, etc.) y encabezado.
            // Esto es crucial para que el entorno de Canvas (iframe) pueda conectarse a localhost.
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar CORS
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Usuarios API v1");
    //c.RoutePrefix = string.Empty;
});

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
