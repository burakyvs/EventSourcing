using EventStore.ClientAPI;
using System.Text.Json;
using System.Text;
using ES.Entity;
using ES.Entity.Events;

namespace ES.Business.EventAggregate
{
    public class AggregateRepository
    {
        readonly IEventStoreConnection _connection;
        public AggregateRepository(IEventStoreConnection connection)
            => _connection = connection;
        //Oluşturulan event'leri Event Store'a kaydeder.
        public async Task SaveAsync<T>(T aggregate) where T : Aggregate, new()
        {
            List<EventData> events = aggregate.GetEvents
                .Select(@event => new EventData(
                    eventId: Guid.NewGuid(),
                    type: @event.GetType().Name,//type : Event Store'a kaydedilecek olan event'in türünü sınıf olarak bildiriyoruz.
                    isJson: true,
                    data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize( //Event json türüne serialize ediliyor.
                        value: @event,
                        inputType: @event.GetType(),
                        options: new() { WriteIndented = true }
                        )),
                    metadata: Encoding.UTF8.GetBytes(@event.GetType().FullName))/*metadata : Metadata olarak binary formatta ilgili
event'in FullName bilgisini yani namespace ile birlikte full class adını tutmaktayız. Bu bilgiyi, event'leri 'Read Data Store'da
güncelleme yaparken hangi event'in gerçekleştiğini ayırt edebilmek için kullanacağız. */
                )
                .ToList();

            if (!events.Any())
                return;

            //Event'ler gönderiliyor...
            await _connection.AppendToStreamAsync(aggregate.StreamName, ExpectedVersion.Any, events);
            aggregate.GetEvents.Clear();
        }
        //Event Store'dan belirtilen Stream'de ki event'leri getirir.
        public async Task<dynamic> GetEvents(string streamName)
        {
            long nextSliceStart = 0L;
            List<ResolvedEvent> events = new();
            StreamEventsSlice readEvents = null;
            do
            {
                readEvents = await _connection.ReadStreamEventsForwardAsync(
                    stream: streamName,
                    start: nextSliceStart,
                    count: 4096,
                    resolveLinkTos: true
                    );

                if (readEvents.Events.Length > 0)
                    events.AddRange(readEvents.Events);

                nextSliceStart = readEvents.NextEventNumber;
            } while (!readEvents.IsEndOfStream);

            var a = events.Select(@event => new
            {
                @event.Event.EventNumber,
                @event.Event.EventType,
                @event.Event.Created,
                @event.Event.EventId,
                @event.Event.EventStreamId,
                Data = JsonSerializer.Deserialize(
                    json: Encoding.UTF8.GetString(@event.Event.Data),
                    returnType:  Type.GetType($"{Encoding.UTF8.GetString(@event.Event.Metadata)}, ES.Entity")/*returnType : Yukarıda 'SaveAsync'
metodunda metadata olarak tutulan event class'ının tam adı, burada ilgili event'in özgün sınıfına dönüştürülürken kullanılmaktadır.*/
                    ),
                Metadata = Encoding.UTF8.GetString(@event.Event.Metadata)
            });

            return a;
        }
        //Event'lerin uygulandığı User datasının son halini getirir.
        public async Task<User> GetData(string streamName)
        {
            dynamic events = await GetEvents(streamName);
            User user = new();
            foreach (var @event in events)
            {
                switch (@event.Data)
                {
                    case UserCreatedEvent o:
                        user.Id = o.UserId;
                        user.Name = o.Name;
                        user.UserName = o.UserName;
                        user.Email = o.Email;
                        user.EmailApprove = o.EmailApprove;
                        break;
                    case UserNameChangedEvent o:
                        user.Name = o.NewName;
                        break;
                    case UserEmailApprovedEvent o:
                        user.EmailApprove = true;
                        break;
                }
            }
            return user;
        }
    }
}
