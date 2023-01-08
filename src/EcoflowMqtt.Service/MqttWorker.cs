using EcoflowMqtt.Service.Extensions;
using EcoflowMqtt.Service.Mqtt;

namespace EcoflowMqtt.Service;

public class MqttWorker : BackgroundService
{
    private readonly ILogger<MqttWorker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MqttWorker(ILogger<MqttWorker> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var adapter = scope.ServiceProvider.GetRequiredService<IMqttAdapter>();

        await RunAndHandleMqttEventsAsync(adapter, stoppingToken);
    }

    private async Task RunAndHandleMqttEventsAsync(IMqttAdapter mqttAdapter, CancellationToken cancellation)
    {
        await mqttAdapter.ConnectAsync();
        
        _logger.LogInformation("Mqtt adapter connected");

        await cancellation.WaitAsync();

        await mqttAdapter.DisconnectAsync();
        
        _logger.LogInformation("Mqtt adapter disconnected");
    }
}