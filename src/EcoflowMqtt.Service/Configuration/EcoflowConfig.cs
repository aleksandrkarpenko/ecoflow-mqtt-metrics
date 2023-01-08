namespace EcoflowMqtt.Service.Configuration;

public class EcoflowConfig
{
    public string Email { get; init; } = string.Empty;
    
    public string Password { get; init; } = string.Empty;

    public string[] SerialIds { get; init; } = Array.Empty<string>();
}