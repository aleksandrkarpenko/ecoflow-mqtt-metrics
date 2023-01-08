namespace EcoflowMqtt.Service.Ecoflow.Models;

public record EcoflowAuthResponse(
    string Code,
    string Message,
    EcoflowAuthResponseData Data);