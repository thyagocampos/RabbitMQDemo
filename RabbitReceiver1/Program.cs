using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Criando a conexão para o servidor
ConnectionFactory factory = new();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Rabbit Receiver1 App";
IConnection cnn = factory.CreateConnection();
IModel channel = cnn.CreateModel();

//Criando o canal
string exchangeName = "DemoExchange";
string routingKey = "demo-routing-key";
string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);

//prefetchSize  -> não importa o tamanho da mensagem
//prefetchCount -> quantas mensagens queremos que seja enviada a você a cada vez
//o último parãmetro -> indica se iremos aplicar essas configs apenas para essa instância ou todo sistema
channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (sender, args) =>
{
    Task.Delay(TimeSpan.FromSeconds(5)).Wait();

    var body = args.Body.ToArray();
    
    string message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Message Received: {message}");

    //Setamos a mensagem como entregue
    channel.BasicAck(args.DeliveryTag, false);
    
    //Se ouver lagum problema podemos setar a mensagem com a taga de delivery false 
};

string consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
cnn.Close();