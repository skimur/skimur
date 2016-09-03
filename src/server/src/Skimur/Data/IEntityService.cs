using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Data
{
    public interface IEntityService
    {
        Task<int> Insert<T>(T entity, IDbConnection connection) where T : class;
    }
}
