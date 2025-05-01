using Library_System.Application.Interfaces.IRepositories;
using Library_System.Application.Interfaces.IServices;
using Library_System.Application.Repositories;
using Library_System.Infrastructure.Cache.Interfaces;
using Library_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5174") 
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Register your services and repositories
builder.Services.AddScoped<IBookRequestService, BookRequestService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookRequestRepository, BookRequestRepository>();
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "BookApp_";
});

builder.Services.AddDbContext<LibraryDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library System API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();