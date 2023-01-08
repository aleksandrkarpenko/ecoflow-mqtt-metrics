using System.Text;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace EcoflowMqtt.Service.Mqtt;

public sealed class MqttAdapter : IMqttAdapter
{
    private readonly IManagedMqttClient _mqttClient;
    private readonly IMqttClientOptionsFactory _clientOptionsFactory;
    private readonly IMqttTopicsRegistry _mqttTopicsRegistry;
    private readonly IMqttMessageHandler _messageHandler;

    private bool _isTopicsSubscribed;

    public MqttAdapter(
        IManagedMqttClient managedMqttClient,
        IMqttClientOptionsFactory clientOptionsFactory,
        IMqttTopicsRegistry mqttTopicsRegistry,
        IMqttMessageHandler messageHandler)
    {
        _mqttClient = managedMqttClient;
        _mqttClient.ConnectedAsync += HandleConnected;
        _mqttClient.ApplicationMessageReceivedAsync += HandlerReceived;
        _mqttClient.DisconnectedAsync += HandleDisconnected;
        
        _clientOptionsFactory = clientOptionsFactory;
        _mqttTopicsRegistry = mqttTopicsRegistry;
        _messageHandler = messageHandler;
    }

    public Task DisconnectAsync() =>
        _mqttClient.IsConnected ? _mqttClient.StopAsync() : Task.CompletedTask;

    public async Task ConnectAsync()
    {
        if (!_isTopicsSubscribed)
        {
            var topics = _mqttTopicsRegistry.GetTopics();
            
            foreach (var topic in topics)
            {
                await _mqttClient.SubscribeAsync(topic);
            }

            _isTopicsSubscribed = true;
        }

        var options = await _clientOptionsFactory.GetManagedClientOptionsAsync();
        
        await _mqttClient.StartAsync(options);
    }

    private Task HandleConnected(MqttClientConnectedEventArgs args)
    {
        return Task.CompletedTask;
    }

    private async Task HandlerReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);

        await _messageHandler.Handle(args.ApplicationMessage.Topic, payload);
    }

    private Task HandleDisconnected(MqttClientDisconnectedEventArgs args)
    {
        return Task.CompletedTask;
    }

    public void Dispose() =>
        _mqttClient.Dispose();
}