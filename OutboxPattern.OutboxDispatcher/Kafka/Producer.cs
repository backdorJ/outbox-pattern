using Confluent.Kafka;

namespace OutboxPattern.OutboxDispatcher.Kafka;

public class Producer
{
    private readonly IProducer<Null, string> _producer;
    
    public Producer(IConfigurationRoot configuration)
    {
        var config = new ProducerConfig()
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
        };
        
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string message)
    {
        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
    }
}