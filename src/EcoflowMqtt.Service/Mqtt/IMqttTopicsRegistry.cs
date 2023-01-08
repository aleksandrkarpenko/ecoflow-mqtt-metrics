namespace EcoflowMqtt.Service.Mqtt;

public interface IMqttTopicsRegistry
{
    string[] GetTopics();
}