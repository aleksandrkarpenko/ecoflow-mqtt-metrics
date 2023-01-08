namespace EcoflowMqtt.Service.Mqtt;

public interface IMqttMessageHandler
{
    ValueTask Handle(string topic, string payload);
}