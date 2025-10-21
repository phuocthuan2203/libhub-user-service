using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibHub.UserService.Infrastructure.Discovery;

public class ConsulHostedService : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly ILogger<ConsulHostedService> _logger;
    private readonly IConfiguration _configuration;
    private string _registrationId = "";

    public ConsulHostedService(IConsulClient consulClient, ILogger<ConsulHostedService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _consulClient = consulClient;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var serviceName = _configuration["ServiceConfig:Name"] ?? throw new InvalidOperationException("ServiceConfig:Name is not configured");
        var servicePortString = _configuration["ServiceConfig:Port"] ?? throw new InvalidOperationException("ServiceConfig:Port is not configured");
        var servicePort = int.Parse(servicePortString);
        var serviceAddress = _configuration["ServiceConfig:Address"] ?? throw new InvalidOperationException("ServiceConfig:Address is not configured");
        
        _registrationId = $"{serviceName}-{Guid.NewGuid()}";

        var registration = new AgentServiceRegistration
        {
            ID = _registrationId,
            Name = serviceName,
            Address = serviceAddress,
            Port = servicePort,
            Tags = new[] { "LibHub", serviceName },
            Check = new AgentServiceCheck
            {
                HTTP = $"http://{serviceAddress}:{servicePort}/health",
                Interval = TimeSpan.FromSeconds(10),
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60)
            }
        };

        _logger.LogInformation("Registering service [{ServiceId}] with Consul...", _registrationId);
        await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("De-registering service [{ServiceId}] from Consul...", _registrationId);
        try
        {
            await _consulClient.Agent.ServiceDeregister(_registrationId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error de-registering from Consul.");
        }
    }
}
