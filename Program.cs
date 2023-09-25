using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Medals.Models;
using Medals.Hubs;

// Connection info stored in appsettings.json
IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register the DataContext service
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(configuration["ConnectionStrings:DefaultSQLiteConnection"]));
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Hubs",
        builder =>
        {
            builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                // Anonymous origins NOT allowed for web sockets
                .WithOrigins("http://localhost:3000","https://jgrissom.github.io")
                .AllowCredentials();
        });
});
builder.Services.AddSignalR();

builder.Services.AddControllers().AddNewtonsoftJson();;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Medals API", 
        Version = "v1",
        Description = "Olympic Medals API",
    });
    c.EnableAnnotations();
    c.TagActionsBy(api => new[] { api.HttpMethod });
});

var app = builder.Build();

app.UseCors("Hubs");

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("Hubs");

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<MedalsHub>("/medalsHub");
});

app.Run();
