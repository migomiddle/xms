using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xms.Web.Customize.Models
{
    public class HomePageModel
    {
        public List<HomePageSolutionComponentModel> SolutionComponents{ get;set;}
        public Guid? SolutionId { get;  set; }
    }
    public class HomePageSolutionComponentModel
    {
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        
        public int TotalCount { get; set; }
    }
}
