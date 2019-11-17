using System.Threading.Tasks;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.Module.Core.Services
{
    public interface IConnectionService
    {
        Task Create(Connection connection);

        Task Update(Connection connection);

        Task Delete(long id);

        Task Delete(Connection connection);
    }
}
