namespace EcoflowMqtt.Service.Ecoflow.Models;

public record EcoflowMqttMessage(
    long Id,
    string Version,
    long Timestamp,
    string ModuleType,
    Dictionary<string, double?> Params);