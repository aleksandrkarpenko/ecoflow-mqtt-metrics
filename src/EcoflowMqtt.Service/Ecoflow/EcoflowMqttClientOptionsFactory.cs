using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Web;
using EcoflowMqtt.Service.Configuration;
using EcoflowMqtt.Service.Ecoflow.Models;
using EcoflowMqtt.Service.Mqtt;
using Microsoft.Extensions.Options;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace EcoflowMqtt.Service.Ecoflow;

public class EcoflowMqttClientOptionsFactory : IMqttClientOptionsFactory
{
    private readonly EcoflowConfig _ecoflowConfig;
    private readonly HttpClient _httpClient;

    public EcoflowMqttClientOptionsFactory(IOptions<EcoflowConfig> ecoflowConfig, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _ecoflowConfig = ecoflowConfig.Value;
    }
    
    public async Task<ManagedMqttClientOptions> GetManagedClientOptionsAsync()
    {
        var options = await GetClientOptionsAsync();
        return new ManagedMqttClientOptionsBuilder()
            .WithClientOptions(options)
            .Build();
    }

    public async Task<MqttClientOptions> GetClientOptionsAsync()
    {
        var ecoflowMqttCredentials = await GetEcoflowAccountCredentialsAsync();

        return new MqttClientOptionsBuilder()
            .WithTcpServer(ecoflowMqttCredentials.Data.Url, int.Parse(ecoflowMqttCredentials.Data.Port))
            .WithTls()
            .WithCredentials(
                ecoflowMqttCredentials.Data.CertificateAccount,
                ecoflowMqttCredentials.Data.CertificatePassword)
            .WithCleanSession()
            .WithTimeout(TimeSpan.FromSeconds(10))
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(90))
            .Build();
    }

    private async Task<EcoflowMqttCredentialsResponse> GetEcoflowAccountCredentialsAsync()
    {
        var accountInfo = await GetEcoflowAccountInfo();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            accountInfo.Data.Token);
        
        var queriStringBuilder = HttpUtility.ParseQueryString(string.Empty);
        queriStringBuilder["userId"] = accountInfo.Data.User.UserId;
        var queryString = queriStringBuilder.ToString();

        var response = await _httpClient.GetFromJsonAsync<EcoflowMqttCredentialsResponse>(
            $"iot-auth/app/certification?{queryString}");

        if (response == null)
        {
            throw new AuthenticationException();
        }

        return response;
    }

    private async Task<EcoflowAuthResponse> GetEcoflowAccountInfo()
    {
        var authRequestBody = new EcoflowAuthRequest(
            "4.1.2.02",
            _ecoflowConfig.Email,
            "android",
            "30",
            "IOT_APP",
            "ECOFLOW",
            Convert.ToBase64String(Encoding.UTF8.GetBytes(_ecoflowConfig.Password))
        );

        var authResponseMessage = await _httpClient.PostAsJsonAsync(
            "auth/login",
            authRequestBody,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        authResponseMessage.EnsureSuccessStatusCode();

        var authResponse = await authResponseMessage.Content.ReadFromJsonAsync<EcoflowAuthResponse>();

        if (authResponse == null)
        {
            throw new AuthenticationException();
        }

        return authResponse;
    }
}