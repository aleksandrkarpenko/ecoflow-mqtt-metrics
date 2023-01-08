using EcoflowMqtt.Service.Extensions;
using Prometheus.Client.MetricPusher;

namespace EcoflowMqtt.Service;

public class PrometheusWorker : BackgroundService
{
    private readonly ILogger<PrometheusWorker> _logger;
    private readonly IMetricPushServer _metricPushServer;

    public PrometheusWorker(ILogger<PrometheusWorker> logger, IMetricPushServer metricPushServer)
    {
        _logger = logger;
        _metricPushServer = metricPushServer;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _metricPushServer.Start();
        
        _logger.LogInformation("Metric pusher service started");

        await stoppingToken.WaitAsync();
        
        _metricPushServer.Stop();
        
        _logger.LogInformation("Metric pusher service stopped");
    }
}