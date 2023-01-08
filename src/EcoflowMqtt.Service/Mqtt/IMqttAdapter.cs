namespace EcoflowMqtt.Service.Mqtt;

public interface IMqttAdapter : IDisposable
{
    Task ConnectAsync();

    Task DisconnectAsync();
}