using Microsoft.EntityFrameworkCore;
using todo_backend.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Dodaj DbContext z SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=mydb.db"));

// 🔹 Dodaj kontrolery
builder.Services.AddControllers();

// 🔹 Swagger (do testowania API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 CORS – zezwalamy tylko frontendowi z Vite
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // adres frontendu
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// 🔹 Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// 🔹 Użycie CORS
app.UseCors("AllowFrontend");

// 🔹 Routing dla kontrolerów
app.MapControllers();

app.Run();
