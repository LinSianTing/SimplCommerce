using System.Linq;
using System.Threading.Tasks;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.Core.Services
{
    public class ConnectionService : IConnectionService
    {
        private const string connectionEntityTypeId = "connection";

        private readonly IRepository<Connection> _connectionRepository;
        private readonly IEntityService _entityService;

        public ConnectionService(IRepository<Connection> connectionRepository, IEntityService entityService)
        {
            _connectionRepository = connectionRepository;
            _entityService = entityService;
        }

        public async Task Create(Connection connection)
        {
            using (var transaction = _connectionRepository.BeginTransaction())
            {
                //connection.Slug = _entityService.ToSafeSlug(connection.Slug, connection.Id, connectionEntityTypeId);
                _connectionRepository.Add(connection);
                await _connectionRepository.SaveChangesAsync();

                //_entityService.Add(connection.Name, connection.Slug, connection.Id, connectionEntityTypeId);
                //await _connectionRepository.SaveChangesAsync();

                transaction.Commit();
            }
        }

        public async Task Update(Connection connection)
        {
            //connection.Slug = _entityService.ToSafeSlug(connection.Slug, connection.Id, connectionEntityTypeId);
            //_entityService.Update(connection.Name, connection.Slug, connection.Id, connectionEntityTypeId);
            await _connectionRepository.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            var connection = _connectionRepository.Query().First(x => x.Id == id);
            await Delete(connection);
        }

        public async Task Delete(Connection connection)
        {
            //connection.IsDeleted = true;
            await _entityService.Remove(connection.Id, connectionEntityTypeId);
            _connectionRepository.SaveChanges();
        }
    }
}
