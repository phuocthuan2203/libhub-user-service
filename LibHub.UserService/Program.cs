using LibHub.UserService.Data;
using LibHub.UserService.Data.Repositories;
using LibHub.UserService.Services;
using Microsoft.EntityFrameworkCore;
using Consul;
using LibHub.UserService.Infrastructure.Discovery;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, LibHub.UserService.Services.UserService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Consul Client configuration
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    var address = builder.Configuration["Consul:Address"];
    if (address != null)
    {
        consulConfig.Address = new Uri(address);
    }
}));

// Add our custom Consul registration service
builder.Services.AddHostedService<ConsulHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();

app.MapGet("/health", () => Results.Ok());

app.Run();
