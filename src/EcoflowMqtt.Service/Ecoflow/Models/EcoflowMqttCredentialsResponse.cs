namespace EcoflowMqtt.Service.Ecoflow.Models;

public record EcoflowMqttCredentialsResponse(
    string Code,
    string Message,
    EcoflowMqttCredentialsResponseData Data);