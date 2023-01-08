using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace EcoflowMqtt.Service.Mqtt;

public interface IMqttClientOptionsFactory
{
    Task<ManagedMqttClientOptions> GetManagedClientOptionsAsync();
    
    Task<MqttClientOptions> GetClientOptionsAsync();
}