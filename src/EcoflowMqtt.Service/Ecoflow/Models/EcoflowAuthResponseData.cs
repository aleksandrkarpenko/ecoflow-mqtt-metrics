namespace EcoflowMqtt.Service.Ecoflow.Models;

public record EcoflowAuthResponseData(
    EcoflowAuthResponseUserInfo User,
    string Token);