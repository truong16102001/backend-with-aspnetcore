using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetCore.API;
using NetCore.DataAccess.DBContext;
using NetCore.DataAccess.IRepositories;
using NetCore.DataAccess.IServices;
using NetCore.DataAccess.Repositories;
using NetCore.DataAccess.Services;
using NetCore.DataAccess.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRoomServices, RoomServices>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddDbContext<MyDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("ConnStr")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
