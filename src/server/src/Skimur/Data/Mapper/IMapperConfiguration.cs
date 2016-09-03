using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Skimur.Data.Mapper
{
    public interface IMapperConfiguration
    {
        IClassMapper GetMap(Type entityType);

        IClassMapper GetMap<T>() where T : class;
    }

    public class ClassMapperOptions
    {
        public ClassMapperOptions()
        {
            ClassMappers = new Dictionary<Type, IClassMapper>();
        }

        public IDictionary<Type, IClassMapper> ClassMappers { get; }

        public void Add<T>() where T:IClassMapper, new()
        {
            Add(new T());
        }

        public void Add(IClassMapper classMapper)
        {
            ClassMappers.Add(classMapper.EntityType, classMapper);
        }
    }

    public class MapperConfiguration : IMapperConfiguration
    {
        private readonly IDictionary<Type, IClassMapper> _classMaps = new Dictionary<Type, IClassMapper>();

        public MapperConfiguration(IOptions<ClassMapperOptions> classMappers)
        {
            foreach(var mapper in classMappers.Value.ClassMappers)
            {
                _classMaps.Add(mapper.Key, mapper.Value);
            }
        }

        public IClassMapper GetMap(Type entityType)
        {
            IClassMapper map;

            if (!_classMaps.TryGetValue(entityType, out map))
                throw new Exception($"No mapping for {entityType.Name}");

            return map;
        }

        public IClassMapper GetMap<T>() where T : class
        {
            return GetMap(typeof(T));
        }
    }
}
