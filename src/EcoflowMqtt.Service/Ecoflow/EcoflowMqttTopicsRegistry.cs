using EcoflowMqtt.Service.Configuration;
using Microsoft.Extensions.Options;

namespace EcoflowMqtt.Service.Mqtt;

public class EcoflowMqttTopicsRegistry : IMqttTopicsRegistry
{
    private readonly EcoflowConfig _ecoflowConfig;

    public EcoflowMqttTopicsRegistry(IOptions<EcoflowConfig> ecoflowConfig)
    {
        _ecoflowConfig = ecoflowConfig.Value;
    }

    public string[] GetTopics() =>
        _ecoflowConfig.SerialIds.Select(x => $"/app/device/property/{x}").ToArray();
}