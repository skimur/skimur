using Dapper;
using Skimur.Data.Mapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skimur.Data
{
    public class EntityService : IEntityService
    {
        IMapperConfiguration _mapperConfiguration;

        public EntityService(IMapperConfiguration mapperConfiguration)
        {
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<int> Insert<T>(T entity, IDbConnection connection) where T:class
        {
            var mapper = _mapperConfiguration.GetMap<T>();

            var key = mapper.Properties.SingleOrDefault(x => x.KeyType != KeyType.NotAKey);
            if (key == null) throw new Exception($"There was no key mapped for {typeof(T).Name}");

            var columnNames = new StringBuilder();

            foreach(var property in mapper.Properties)
            {
                columnNames.Append($"{property.ColumnName}");
                if (mapper.Properties.IndexOf(property) < mapper.Properties.Count - 1)
                    columnNames.Append(",");
            }

            var parameterNames = new StringBuilder();

            foreach (var property in mapper.Properties)
            {
                parameterNames.Append($"@{property.Name}");
                if (mapper.Properties.IndexOf(property) < mapper.Properties.Count - 1)
                    parameterNames.Append(",");
            }

            var sql = new StringBuilder();
            sql.AppendFormat($"insert into {mapper.TableName} ({columnNames.ToString()}) values ({parameterNames.ToString()})");

            var result = await connection.ExecuteAsync(sql.ToString(), entity);

            return result;
        }
    }
}
