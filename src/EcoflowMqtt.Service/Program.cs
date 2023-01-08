using EcoflowMqtt.Service;
using EcoflowMqtt.Service.Configuration;
using EcoflowMqtt.Service.Ecoflow;
using EcoflowMqtt.Service.Mqtt;
using EcoflowMqtt.Service.Prometheus;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Prometheus.Client;
using Prometheus.Client.Collectors;
using Prometheus.Client.MetricPusher;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<MqttWorker>();
        services.AddHostedService<PrometheusWorker>();

        services.AddScoped<IManagedMqttClient>(_ => new MqttFactory().CreateManagedMqttClient());
        services.AddScoped<IMqttClient>(_ => new MqttFactory().CreateMqttClient());

        services.AddScoped<IMqttAdapter, MqttAdapter>();
        services.AddScoped<IMqttTopicsRegistry, EcoflowMqttTopicsRegistry>();
        services.AddScoped<IMqttMessageHandler, EcoflowMqttMessageHandler>();

        services.AddHttpClient<IMqttClientOptionsFactory, EcoflowMqttClientOptionsFactory>()
            .ConfigureHttpClient(client =>
            {
                client.DefaultRequestHeaders.Add("lang", "de-de");
                client.DefaultRequestHeaders.Add("platform", "android");
                client.DefaultRequestHeaders.Add("sysversion", "11");
                client.DefaultRequestHeaders.Add("version", "4.1.2.02");
                client.DefaultRequestHeaders.Add("phonemodel", "SM-X200");
                client.DefaultRequestHeaders.UserAgent.Add(new("okhttp", "3.14.9"));

                client.BaseAddress = new Uri("https://api.ecoflow.com/");
            });

        services.AddOptions();

        services.Configure<EcoflowConfig>(context.Configuration.GetSection("Ecoflow"));

        services.AddSingleton<ICollectorRegistry>(new CollectorRegistry());
        services.AddSingleton<IMetricFactory>(c => new MetricFactory(c.GetRequiredService<ICollectorRegistry>()));
        services.AddSingleton<IMetricPusher>(c =>
        {
            var config = c.GetRequiredService<IConfiguration>().GetRequiredSection("Prometheus").Get<PrometheusConfig>()!;
            
            return new MetricPusher(new()
            {
                Endpoint = config.PushEndpoint,
                Job = "prom-push",
                CollectorRegistry = c.GetRequiredService<ICollectorRegistry>(),
                Instance = "prom-push"
            });
        });
        services.AddSingleton<IMetricPushServer>(c => new MetricPushServer(c.GetRequiredService<IMetricPusher>()));
        services.AddSingleton<IPrometheusExporter, PrometheusExporter>();
    })
    .Build();

await host.RunAsync();