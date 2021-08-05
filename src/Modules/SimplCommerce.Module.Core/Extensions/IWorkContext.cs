using System.Threading.Tasks;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.Module.Core.Extensions
{
    public interface IWorkContext
    {
        SystemApp GetCurrentSystemAppSync();
        Task<SystemApp> GetCurrentSystemApp();
        Task<User> GetCurrentUser();
    }
}
