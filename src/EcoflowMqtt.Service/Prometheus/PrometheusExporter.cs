using System.Collections.Concurrent;
using Prometheus.Client;

namespace EcoflowMqtt.Service.Prometheus;

public class PrometheusExporter : IPrometheusExporter
{
    private readonly ConcurrentDictionary<string, IMetricFamily<IGauge>> _gauges = new();
    private readonly IMetricFactory _factory;

    public PrometheusExporter(IMetricFactory factory)
    {
        _factory = factory;
    }

    public void GaugeSet(string name, string description, double value, string[] labelNames, string[] labelValues)
    {
        var gauge = _gauges.GetOrAdd(name, n => _factory.CreateGauge(name, description, labelNames: labelNames));

        gauge.WithLabels(labelValues).Set(value);
    }
}