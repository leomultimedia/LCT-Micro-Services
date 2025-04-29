using Prometheus;

namespace OrderService.Services;

public interface IOrderMetricsService
{
    void RecordOrderCreated(string status);
    void RecordOrderStatusChange(string fromStatus, string toStatus);
    void RecordOrderProcessingTime(double seconds);
    void RecordOrderError(string errorType);
}

public class OrderMetricsService : IOrderMetricsService
{
    private readonly Counter _ordersCreatedCounter;
    private readonly Counter _ordersStatusChangedCounter;
    private readonly Histogram _orderProcessingTimeHistogram;
    private readonly Counter _orderErrorsCounter;

    public OrderMetricsService()
    {
        _ordersCreatedCounter = Metrics.CreateCounter(
            "order_service_orders_created_total",
            "Total number of orders created",
            new CounterConfiguration
            {
                LabelNames = new[] { "status" }
            });

        _ordersStatusChangedCounter = Metrics.CreateCounter(
            "order_service_orders_status_changed_total",
            "Total number of order status changes",
            new CounterConfiguration
            {
                LabelNames = new[] { "from_status", "to_status" }
            });

        _orderProcessingTimeHistogram = Metrics.CreateHistogram(
            "order_service_order_processing_seconds",
            "Time taken to process an order",
            new HistogramConfiguration
            {
                Buckets = Histogram.ExponentialBuckets(0.1, 2, 10)
            });

        _orderErrorsCounter = Metrics.CreateCounter(
            "order_service_errors_total",
            "Total number of errors encountered",
            new CounterConfiguration
            {
                LabelNames = new[] { "error_type" }
            });
    }

    public void RecordOrderCreated(string status)
    {
        _ordersCreatedCounter.WithLabels(status).Inc();
    }

    public void RecordOrderStatusChange(string fromStatus, string toStatus)
    {
        _ordersStatusChangedCounter.WithLabels(fromStatus, toStatus).Inc();
    }

    public void RecordOrderProcessingTime(double seconds)
    {
        _orderProcessingTimeHistogram.Observe(seconds);
    }

    public void RecordOrderError(string errorType)
    {
        _orderErrorsCounter.WithLabels(errorType).Inc();
    }
} 