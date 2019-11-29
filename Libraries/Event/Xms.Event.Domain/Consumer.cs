using PetaPoco;
using System;

namespace Xms.Event.Domain
{
    [TableName("Consumer")]
    [PrimaryKey("ConsumerId", AutoIncrement = false)]
    public class Consumer
    {
        public Guid ConsumerId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid TopicId { get; set; }

        public Guid EventMessageId { get; set; }

        public bool Received { get; set; }

        public DateTime ReceivedOn { get; set; }
    }
}