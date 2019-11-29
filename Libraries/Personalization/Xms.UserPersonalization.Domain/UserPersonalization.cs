using System;

namespace Xms.UserPersonalization.Domain
{
    public class UserPersonalization
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}