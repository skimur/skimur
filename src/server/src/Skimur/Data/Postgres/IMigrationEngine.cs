using Skimur.Data;
using System.Threading.Tasks;

namespace Skimur.Data.Postgres
{
    public interface IMigrationEngine
    {
        Task<bool> Execute(IDbConnectionProvider conn, MigrationResources resources);
    }
}
