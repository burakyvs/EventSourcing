using ES.Business.EventAggregate;
using EventStore.ClientAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IEventStoreConnection connection = EventStoreConnection.Create(
           connectionString: "ConnectTo=tcp://localhost:1115;DefaultUserCredentials=admin:changeit;UseSslConnection=true;TargetHost=eventstore.org;ValidateServer=false",
           connectionName: "API_Application",
           builder: ConnectionSettings.Create().KeepReconnecting()
       );

connection.Connected += static (sender, clientConnectionEventArgs) =>
{
    Console.WriteLine("Baðlantý saðlanmýþtýr.");
    Console.WriteLine($"Connection Name  : {clientConnectionEventArgs.Connection.ConnectionName}");
    Console.WriteLine($"Address Family : {clientConnectionEventArgs.RemoteEndPoint.AddressFamily}");
};
connection.Disconnected += (sender, clientConnectionEventArgs) =>
{
    Console.WriteLine("Baðlantý kesilmiþtir.");
    Console.WriteLine($"Connection Name  : {clientConnectionEventArgs.Connection.ConnectionName}");
    Console.WriteLine($"Address Family : {clientConnectionEventArgs.RemoteEndPoint.AddressFamily}");
};
connection.Reconnecting += (sender, clientReconnectingEventArgs) =>
{
    Console.WriteLine("Baðlantý yeniden deneniyor.");
    Console.WriteLine($"Connection Name  : {clientReconnectingEventArgs.Connection.ConnectionName}");
};
connection.ErrorOccurred += (sender, clientErrorEventArgs) =>
{
    Console.WriteLine("Hata oluþtu!.");
    Console.WriteLine($"Connection Name  : {clientErrorEventArgs.Connection.ConnectionName}");
    Console.WriteLine($"Exception Message : {clientErrorEventArgs.Exception.Message}");
};

connection.ConnectAsync().GetAwaiter().GetResult();




builder.Services.AddSingleton(connection);
builder.Services.AddSingleton<AggregateRepository>();
builder.Services.AddSingleton<UserAggregate>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
