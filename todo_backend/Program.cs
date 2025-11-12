using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using todo_backend.Data;
using todo_backend.Services;
using todo_backend.Services.ActivityInstanceService;
using todo_backend.Services.ActivityMembersService;
using todo_backend.Services.ActivityRecurrenceRuleService;
using todo_backend.Services.ActivityService;

//using todo_backend.Services.ActivityStorage;
using todo_backend.Services.ActivitySuggestionService;
using todo_backend.Services.AuthService;
using todo_backend.Services.BlockedUsersService;
using todo_backend.Services.CategoryService;
using todo_backend.Services.FriendshipService;
using todo_backend.Services.NotificationService;
//using todo_backend.Services.RecurrenceService;
using todo_backend.Services.SecurityService;
using todo_backend.Services.StatisticsService;
using todo_backend.Services.TimelineActivityService;
using todo_backend.Services.UserAccountService;
using todo_backend.Services.UserFriendActions;
using todo_backend.Services.UserService;

var builder = WebApplication.CreateBuilder(args);

//Database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Controllers
builder.Services.AddControllers();

//Swagger - for endpoint testing
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Todo Backend API",
        Version = "v1"
    });

    // ⚡ Dodaj definicję autoryzacji
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Paste your JWT token here"
    });


    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//serwisy
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IUserFriendActionsService, UserFriendActionsService>();
//builder.Services.AddScoped<ITimelineActivityService, TimelineActivityService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
//builder.Services.AddScoped<IActivityMembersService, ActivityMembersService>();
//builder.Services.AddScoped<IActivityStorageService, ActivityStorageService>();
builder.Services.AddScoped<IBlockedUsersService, BlockedUsersService>();
//builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
//builder.Services.AddScoped<IRecurrenceService, RecurrenceService>();
//builder.Services.AddScoped<IActivitySuggestionService, ActivitySuggestionService>();

builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IActivityRecurrenceRuleService, ActivityRecurrenceRuleService>();
builder.Services.AddScoped<IActivityInstanceService, ActivityInstanceService>();

builder.Services.AddTransient<DataSeeder>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

//CORS - connection with frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") //frontend address
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Dane podstawowe do bazy
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seeder = services.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Seeder] Błąd inicjalizacji danych: {ex.Message}");
    }
}


app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();
