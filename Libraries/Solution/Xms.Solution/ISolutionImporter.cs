using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Xms.Solution
{
    public interface ISolutionImporter
    {
        Task<bool> ImportAsync(IFormFile file);
    }
}