using System;

namespace Xms.Solution.Abstractions
{
    public class SolutionComponentItem
    {
        public Guid ObjectId { get; set; }
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public string ComponentTypeName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}