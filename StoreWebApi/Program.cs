using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StoreDomain.Models;
using AutoMapper;
using Serilog;
using Microsoft.OpenApi.Models;
using Serilog.Sinks.MSSqlServer;
using StoreDataBase.AppContexts;
using StoreWebApi.ExceptionHandler;
using Microsoft.AspNetCore.Authorization;
using StoreWebApi.Policies;
using StoreService.Interfaces;
using StoreService.Services;
using StoreWebApi.Actions;
using StoreService.Helper;
using StoreDataBase.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.Services.AddControllers().AddNewtonsoftJson(x =>
    x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    //.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true
        })
.CreateLogger();

builder.Host.UseSerilog();


// CORS policy for your Angular app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

// Register services for dependency injection

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDbContext<WalletAppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("WalletConnectionString"));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("refreshTokenIsValid", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new ValidRefreshToken());
    });
});


builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IOrderService,OrderService>();
builder.Services.AddScoped<IEmailService,EmailService>();
builder.Services.AddScoped<IPaymentGateWayService, PaymentGateWayService>();
builder.Services.AddScoped(typeof(IGenericRepoService<>), typeof(GenericRepoServiceForStoreDb<>));
builder.Services.AddScoped(typeof(IGenericRepoService<>), typeof(GenericRepoServiceForWalletDb<>));
builder.Services.AddScoped<IUnitOfWorkServiceForStoreDb,UnitOfWorkServiceOfStoreDb>();
builder.Services.AddScoped<IUnitOfWorkForWalletDb,UnitOfWorkOfWalletDb>();
builder.Services.AddScoped<IExternalLogService,ExternalLogService>();
builder.Services.AddScoped<IAuthorizationHandler, CheckRefreshTokenIsValid>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ValidateRefreshTokenAttribute>();


// JWT Authentication setup
var JwtOptions = builder.Configuration.GetSection("Jwt").Get<Jwt>();
builder.Services.AddSingleton(JwtOptions);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = JwtOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = JwtOptions.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.Signingkey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});


// Add Swagger support
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter your token with this format: ''Bearer YOUR_TOKEN''",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});


var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
