using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Medals.Models;
using Medals.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Connection info stored in appsettings.json
IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register the DataContext service
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(configuration["ConnectionStrings:DefaultSQLiteConnection"]));
builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(configuration["ConnectionStrings:DefaultSQLServerConnection"]));

// configure identity framework defaults
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

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
    // To Enable authorization using Swagger (JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
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
                }
            },
            new string[] {}
        }
    });
    c.TagActionsBy(api => new[] { api.HttpMethod });
});

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Add Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

var app = builder.Build();

app.UseCors("Hubs");

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medals API v1");
        c.RoutePrefix = "";
    });
// }

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("Hubs");

app.UseAuthentication();
app.UseAuthorization();

app.UseFileServer();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<MedalsHub>("/medalsHub");
});

app.Run();
