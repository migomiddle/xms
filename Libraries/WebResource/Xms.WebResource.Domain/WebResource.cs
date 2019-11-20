using PetaPoco;
using System;
using Xms.WebResource.Abstractions;

namespace Xms.WebResource.Domain
{
    [TableName("WebResource")]
    [PrimaryKey("WebResourceId", AutoIncrement = false)]
    public class WebResource
    {
        public Guid WebResourceId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public WebResourceType WebResourceType { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid SolutionId { get; set; }
        public int ComponentState { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}