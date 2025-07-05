using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RBS.Data;
using RBS.Models;
using RBS.Services.Implementation;
using RBS.Services.Implenetation;
using RBS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext with connection string from appsettings
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
    {
        // Configure password requirements
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    
        // Configure email requirements
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false; // Set to true if you want email confirmation
    })
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();



builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ITestingService, TestingService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IConflictSpaceService, ConflictSpaceService>();
builder.Services.AddScoped<IConflictTableService, ConflictTableService>();
builder.Services.AddScoped<IConflictChairService, ConflictChairService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ILayoutService, LayoutService>();
builder.Services.AddScoped<ISpaceReservationService, SpaceReservationService>();
builder.Services.AddScoped<ITableReservationService, TableReservationService>();
builder.Services.AddScoped<IChairReservationService, ChairReservationService>();
builder.Services.AddScoped<IHostService, HostService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IFoodCategoryService, FoodCategoryService>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IIngridientService, IngridientService>();
builder.Services.AddScoped<IOrderFoodService, OrderFoodService>();

// for Apple payment service 
builder.Services.AddScoped<IApplePaymentService, ApplePaymentService>();
builder.Services.AddHttpClient<IApplePaymentService, ApplePaymentService>();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// JWT Service
builder.Services.AddScoped<IJWTService, JWTService>();

// Authentication configuration - Now using configuration values
var jwtKey = builder.Configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT:Key not found in configuration");
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? throw new InvalidOperationException("JWT:Issuer not found in configuration");
var jwtAudience = builder.Configuration["JWT:Audience"] ?? throw new InvalidOperationException("JWT:Audience not found in configuration");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie().AddGoogle(options =>
    {
        var clientId = builder.Configuration["Authentication:Google:ClientId"];

        if (clientId == null)
        {
            throw new ArgumentNullException(nameof(clientId));
        }
        
        var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        
        if (clientSecret == null)
        {
            throw new ArgumentNullException(nameof(clientSecret));
        }
        
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero,
        };
    });
    

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("Universal", policy => policy.RequireRole("Owner, Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/api/NewGoogle/login/google", ([FromQuery] string returnUrl, LinkGenerator linkGenerator,
    SignInManager<User> signInManager, HttpContext httpContext) =>
{
    var properties = signInManager.ConfigureExternalAuthenticationProperties("Google",
        linkGenerator.GetPathByName(httpContext, "GoogleLoginCallback")
        + $"?returnUrl={returnUrl}");
    
    return Results.Challenge(properties, ["Google"]);
});

app.MapGet("/api/NewGoogle/login/google/callback", async ([FromQuery] string returnUrl,
    HttpContext httpContext, IAccountService accountService) =>
{
    var result = await httpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
    
    if (!result.Succeeded)
    {
        return Results.Unauthorized();
    }
        
    await accountService.LoginWithGoogleAsync(result.Principal);
        
    return Results.Redirect(returnUrl);
    
}).WithName("GoogleLoginCallback");

app.Run();