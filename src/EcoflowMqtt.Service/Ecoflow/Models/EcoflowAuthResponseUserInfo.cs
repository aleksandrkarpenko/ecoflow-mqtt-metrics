namespace EcoflowMqtt.Service.Ecoflow.Models;

public record EcoflowAuthResponseUserInfo(
    string UserId,
    string Email,
    string Name,
    string Icon,
    int State,
    string RegType,
    string Destroyed,
    string RegisterLang,
    bool Administrator);