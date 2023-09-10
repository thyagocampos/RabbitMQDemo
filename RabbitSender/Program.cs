using RabbitMQ.Client;
using System.Text;

//Criando a conexão para o servidor
ConnectionFactory factory = new();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Rabbit Sender App";
IConnection cnn = factory.CreateConnection();
IModel channel = cnn.CreateModel();

//Criando o canal
string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);

//Criando e enviando a mensagem
for (int i = 0; i < 60; i++) {        
    byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Message number {i}");

    channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

    Console.WriteLine($"Sending message {i}");

    Thread.Sleep(1000);
}

channel.Close();
cnn.Close();