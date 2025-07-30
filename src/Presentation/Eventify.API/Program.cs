using Asp.Versioning;
using Eventify.API.Extensions;
using Eventify.API.Filters;
using Eventify.API.Middleware;
using Eventify.Persistence;
using Eventify.Persistence.Seeds;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using Eventify.Application;
using Eventify.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Eventify.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
});

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("version"),
        new HeaderApiVersionReader("X-Version"),
        new UrlSegmentApiVersionReader()
    );
})
.AddMvc()
.AddApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// Add layer services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = jwtSettings.GetValue<bool>("ValidateIssuer"),
        ValidateAudience = jwtSettings.GetValue<bool>("ValidateAudience"),
        ValidateLifetime = jwtSettings.GetValue<bool>("ValidateLifetime"),
        ValidateIssuerSigningKey = jwtSettings.GetValue<bool>("ValidateIssuerSigningKey"),
        RequireExpirationTime = jwtSettings.GetValue<bool>("RequireExpirationTime"),
        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
        ValidAudience = jwtSettings.GetValue<string>("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.GetValue<string>("Key") ?? throw new InvalidOperationException("JWT Key is required"))),
        ClockSkew = TimeSpan.FromMinutes(jwtSettings.GetValue<int>("ClockSkewInMinutes"))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning("JWT Authentication failed: {Exception}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Log.Information("JWT Token validated for user: {UserId}",
                context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        }
    };
});

// Add custom authorization policies
builder.Services.AddCustomAuthorization();

// Add HTTP Context Accessor
builder.Services.AddHttpContextAccessor();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Event Management API",
        Version = "v1",
        Description = "A comprehensive Event Management System API with authentication and payment processing",
        Contact = new OpenApiContact
        {
            Name = "Event Management Team",
            Email = "support@eventmanagement.com"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
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
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddCheck("jwt-configuration", () =>
    {
        var jwtKey = builder.Configuration["JwtSettings:Key"];
        return string.IsNullOrEmpty(jwtKey) ?
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("JWT Key not configured") :
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
    })
    .AddCheck("stripe-configuration", () =>
    {
        var stripeKey = builder.Configuration["Stripe:SecretKey"];
        var webhookSecret = builder.Configuration["Stripe:WebhookSecret"];

        if (string.IsNullOrEmpty(stripeKey) || string.IsNullOrEmpty(webhookSecret))
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Stripe not configured");
        }

        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
    });

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add Response Compression
builder.Services.AddResponseCompression();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Management API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
    });
}

// Add custom middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseCors("AllowAll");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Add resource ownership validation
app.UseResourceOwnershipValidation();

app.MapControllers();

// Add Health Check endpoint
app.MapHealthChecks("/health");

// Seed database and roles
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var serviceProvider = scope.ServiceProvider;

        await context.Database.MigrateAsync();
        await ApplicationDbContextSeed.SeedDefaultDataAsync(context);
        await IdentitySeedData.SeedDefaultRolesAndUsersAsync(serviceProvider);
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred while migrating or seeding the database");
    throw;
}

// Start the application
try
{
    Log.Information("Starting Event Management API with Authentication and Payment Processing");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}