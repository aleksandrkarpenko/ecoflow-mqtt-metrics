using System.Text;
using System.Text.Json;
using EcoflowMqtt.Service.Ecoflow.Models;
using EcoflowMqtt.Service.Mqtt;
using EcoflowMqtt.Service.Prometheus;

namespace EcoflowMqtt.Service.Ecoflow;

public class EcoflowMqttMessageHandler : IMqttMessageHandler
{
    private static JsonSerializerOptions DeSerializerOptions { get; } =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };

    private static string[] LabelNames { get; } = { "device_sn" };
    
    private readonly IPrometheusExporter _prometheusExporter;

    public EcoflowMqttMessageHandler(IPrometheusExporter prometheusExporter)
    {
        _prometheusExporter = prometheusExporter;
    }

    public ValueTask Handle(string topic, string payload)
    {
        var mqttMessage = JsonSerializer.Deserialize<EcoflowMqttMessage>(payload, DeSerializerOptions)!;
        var deviceSerialNumber = topic.Substring(topic.LastIndexOf('/') + 1);

        foreach (var param in mqttMessage.Params)
        {
            var metric = GetMetric(param);

            if (metric != null)
            {
                _prometheusExporter.GaugeSet(
                    metric.Value.Name,
                    metric.Value.Description,
                    metric.Value.Value,
                    LabelNames,
                    new[] { deviceSerialNumber });
            }
        }
        
        return ValueTask.CompletedTask;
    }

    private static (string Name, string Description, double Value)? GetMetric(
        KeyValuePair<string, double?> messageParam)
    {
        var promName = ConvertToPrometheusName(messageParam.Key);

        return (promName, messageParam.Key, messageParam.Value ?? 0);
    }

    private static string ConvertToPrometheusName(string metricName)
    {
        var stringBuilder = new StringBuilder("ecoflow_", 8 + metricName.Length * 2);

        foreach (var character in metricName)
        {
            if (character == '.')
            {
                stringBuilder.Append('_');
            }
            else if (Char.IsUpper(character))
            {
                stringBuilder.Append('_').Append(Char.ToLower(character));
            }
            else
            {
                stringBuilder.Append(character);   
            }
        }

        return stringBuilder.ToString();
    }
}