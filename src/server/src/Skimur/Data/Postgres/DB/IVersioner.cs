using System.Threading.Tasks;

namespace Skimur.Data.Postgres.DB
{
    internal interface IVersioner
    {
        Task<int> CurrentVersion(MigrationType type);

        Task<bool> SetVersion(Migration migration);
    }
}
