using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetCore.DataAccess.DBContext;
using NetCore.DataAccess.IRepositories;
using NetCore.DataAccess.IServices;
using NetCore.DataAccess.Repositories;
using NetCore.DataAccess.Services;
using NetCore.DataAccess.UnitOfWork;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IRoomServices, RoomServices>();
builder.Services.AddScoped<ITokenServices, TokenServices>();
builder.Services.AddScoped<ICookieServices, CookieServices>();

builder.Services.AddDbContext<MyDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("ConnStr")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = false,
        ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
        ValidAudience = builder.Configuration["Jwt:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(
        configuration["RedisCacheUrl"]!));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        configuration["RedisCacheUrl"];
});

builder.Services.AddScoped<
    IRedisServices,
    RedisServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
