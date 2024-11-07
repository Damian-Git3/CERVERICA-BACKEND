using CERVERICA.Controllers;
using CERVERICA.Data;
using CERVERICA.Middleware;
using CERVERICA.Models;
using CERVERICA.Providers;
using CERVERICA.Services;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.ResponseHeaders.Add("MyResponseHeader");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});
builder.Services.AddLogging(op => op.Services.AddSingleton<ILoggerProvider, CoolConsoleLoggerProvider>());

// Agregar servicios al contenedor
builder.Services.AddScoped<FirebaseNotificationService>();

// Obtenemos la configuraci�n del JWTSettings de appsettings
var JWTSettings = builder.Configuration.GetSection("JWTSetting");

// Configuraci�n para SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Env.GetString("cadenaSQL")));

// Agregamos la configuraci�n para ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.SaveToken = true;
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        ValidAudience = JWTSettings["ValidAudience"],
        ValidIssuer = JWTSettings["ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSettings.GetSection("securityKey").Value!))
    };
    opt.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            if (context.Principal == null)
            {
                context.Fail("Usuario no autorizado.");
                return;
            }

            var user = await userManager.GetUserAsync(context.Principal);
            if (user == null || !user.Activo)
            {
                context.Fail("Usuario no autorizado.");
            }
        }
    };
});

builder.Services.AddControllers();

builder.Services.AddScoped<RoutineController>();
/*
builder.Services.AddHostedService<HostedServiceRoutines>();
*/

builder.Services.AddEndpointsApiExplorer();

// Agregando la Definici�n de Seguridad
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization Example : 'Bearer secrettokenstringchain",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("NuevaPolitica",
        app =>
        {
            app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

var app = builder.Build();

Stripe.StripeConfiguration.ApiKey = "sk_test_51Q594yAtwLA1tBRcFFVO8alE9UI5I8NB2IlO3VtxoD44NceCxIti98zqaz2irsAJtrvLI9jhu2gTx8ifSJnV5Ak700CjRYcZb7";

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("NuevaPolitica");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserIdMiddleware>();
app.MapControllers();
app.UseHttpLogging();
app.UseMiddleware<RequestLoggingMiddleware>();

app.Run();

