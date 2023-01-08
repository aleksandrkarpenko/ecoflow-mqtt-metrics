namespace EcoflowMqtt.Service.Ecoflow.Models;

public record EcoflowMqttCredentialsResponseData(
    string Url,
    string Port,
    string Protocol,
    string CertificateAccount,
    string CertificatePassword);