using PetaPoco;
using System;

namespace Xms.Event.Domain
{
    [TableName("EventMessage")]
    [PrimaryKey("EventMessageId", AutoIncrement = false)]
    public class EventMessage
    {
        public Guid EventMessageId { get; set; }

        public string Publisher { get; set; }

        public string Data { get; set; }

        public DateTime PublishedOn { get; set; }

        public Guid TopicId { get; set; }
    }
}