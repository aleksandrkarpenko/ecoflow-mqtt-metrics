namespace EcoflowMqtt.Service.Ecoflow.Models;

public record EcoflowAuthRequest(
    string AppVersion,
    string Email,
    string Os,
    string OsVersion,
    string Scene,
    string UserType,
    string Password);