using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
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

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar configuración de MongoDB
builder.Services.AddSingleton<MongoDBConfig>();

// REGISTRA EL REPOSITORIO ANTES DE MediatR
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IUsuarioHistorialActividad, UsuarioHistorialActividadRepository>();
builder.Services.AddScoped<IUsuarioFactory, UsuarioFactory>();
builder.Services.AddScoped<IRolFactory, RolFactory>();

// REGISTRA MediatR PARA TODOS LOS HANDLERS
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CrearUsuarioCommandHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ModificarUsuarioCommandHandler).Assembly));

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
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Permite cualquier dominio
            .AllowAnyMethod()  // GET, POST, PUT, DELETE, etc.
            .AllowAnyHeader(); // Cualquier cabecera
    });
});

var app = builder.Build();

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
