using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using Tea.Api.Middlewares;
using Tea.Application.Interfaces;
using Tea.Application.Services.Implements;
using Tea.Application.Services.Interfaces;
using Tea.Application.Validators.Categories;
using Tea.Domain.Entities;
using Tea.Domain.Repositories;
using Tea.Infrastructure.Configurations;
using Tea.Infrastructure.DataAccess;
using Tea.Infrastructure.DataAccess.Seeds;
using Tea.Infrastructure.Interfaces;
using Tea.Infrastructure.Repositories;
using Tea.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Config SeriLog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

            var result = new
            {
                Status = StatusCodes.Status400BadRequest,
                Errors = errors
            };

            return new BadRequestObjectResult(result);
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Assembly Validator
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CategoryCreateParentRequestValidator>();


builder.Services.AddDbContext<TeaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<AppUser>(opt =>
{
    opt.Password.RequiredLength = 6;
    opt.Password.RequireDigit = false; // không yêu cầu có số
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false; // không yêu cầu phải có chữ và số

    opt.SignIn.RequireConfirmedEmail = false;
})
    .AddRoles<AppRole>()
    .AddRoleManager<RoleManager<AppRole>>()
    .AddEntityFrameworkStores<TeaContext>()
    .AddSignInManager<SignInManager<AppUser>>()
    .AddUserManager<UserManager<AppUser>>()
    .AddDefaultTokenProviders(); // be able to create tokens for email confirmation


// be able to authenticate users using JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.SaveToken = true;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSetting:Key"])),
            ValidIssuer = builder.Configuration["JWTSetting:Issuer"], // domain name project api
            ValidateIssuer = true,
            ValidAudience = builder.Configuration["JWTSetting:Audience"], // người phát hành
            ValidateAudience = true,
            ValidateLifetime = true, // auto validate expired token
            ClockSkew = TimeSpan.Zero, // xóa bỏ lệch múi giờ
        };
        opt.Events = new JwtBearerEvents()
        {
            // 2 nếu có throw exception chạy xuống 3 rồi 4
            // vào đây khi controller có authorize
            OnTokenValidated = context =>
            {
                var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                return tokenService.ValidToken(context);
            },
            // 3
            OnAuthenticationFailed = context =>
            {
                return Task.CompletedTask;
            },
            // 1
            // luôn vào đầu tiên
            OnMessageReceived = context =>
            {
                return Task.CompletedTask;
            },
            //4 
            OnChallenge = context =>
            {
                return Task.CompletedTask;
            }
        };
    });

// register and config redis
builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
    var connectionString = builder.Configuration.GetConnectionString("Redis");
    if (connectionString == null)
        throw new Exception("Can not gett redis connection string");
    var configuration = ConfigurationOptions.Parse(connectionString, true);
    return ConnectionMultiplexer.Connect(configuration);
});


// register DI
builder.Services.Configure<CloudinaryConfig>(builder.Configuration.GetSection(CloudinaryConfig.ConfigName));
builder.Services.Configure<TokenConfig>(builder.Configuration.GetSection(TokenConfig.ConfigName));
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection(EmailConfig.ConfigName));
builder.Services.Configure<VNPayConfig>(builder.Configuration.GetSection(VNPayConfig.ConfigName));

builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();


builder.Services.AddSingleton<ICartService, CartService>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
.WithOrigins("https://localhost:4200", "http://localhost:4200"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// seed data
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<TeaContext>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    await context.Database.MigrateAsync();
    await RoleSeed.SeedAsync(roleManager);
    await RoleClaimSeed.SeedAsync(context, roleManager);
    await CategorySeed.SeedAsync(context);
    await ItemSeed.SeedAsync(context);
    await ItemCategorySeed.SeedAsync(context);
    await UserSeed.SeedAsync(userManager);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
