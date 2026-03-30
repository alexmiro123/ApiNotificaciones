using ApiNotificaciones.Hubs;
using ApiNotificaciones.Interfaces;
using ApiNotificaciones.Messaging;
using ApiNotificaciones.Services;
using ApiTalentoHumano.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });


builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});


builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationDbContext>(options => { });


// Add services to the container.

//builder.Services.AddHostedService<RabbitMQConsumer>();
builder.Services.AddScoped<INotificacionRabbitService, NotificacionRabbitService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:4200", // Angular DEV
                    "https://10.10.10.26:4442",
                    "https://wserp.piggis.com:7041",
                    "https://wserp.piggis.com:4442"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAngular");
app.MapControllers();

app.MapHub<NotificacionHub>("/notificacionHub");

app.Run();
