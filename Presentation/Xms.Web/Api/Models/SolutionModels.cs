using System;

namespace Xms.Web.Api.Models
{
    public class GetSolutionComponentsModel
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public Guid SolutionId { get; set; }

        public bool InSolution { get; set; }
    }
}