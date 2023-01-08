namespace EcoflowMqtt.Service.Prometheus;

public interface IPrometheusExporter
{
    void GaugeSet(string name, string description, double value, string[] labelNames, string[] labelValues);
}