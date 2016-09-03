using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Skimur.Data.Mapper
{
    public interface IClassMapper
    {
        string TableName { get; }

        IList<PropertyMap> Properties { get; }

        Type EntityType { get; }
    }

    public class ClassMapper<T> : IClassMapper
    {
        public ClassMapper()
        {
            Properties = new List<PropertyMap>();
        }

        public string TableName { get; private set; }

        public IList<PropertyMap> Properties { get; }

        public Type EntityType => typeof(T);

        protected void Table(string tableName)
        {
            TableName = tableName;
        }

        protected PropertyMap Map(Expression<Func<T, object>> expression)
        {
            var propertyInfo = ReflectionUtils.GetProperty(expression) as PropertyInfo;
            return Map(propertyInfo);
        }
        
        protected PropertyMap Map(PropertyInfo propertyInfo)
        {
            var result = new PropertyMap(propertyInfo);
            GuardForDuplicatePropertyMap(result);
            Properties.Add(result);
            return result;
        }

        private void GuardForDuplicatePropertyMap(PropertyMap result)
        {
            if (Properties.Any(p => p.Name.Equals(result.Name)))
            {
                throw new ArgumentException(string.Format("Duplicate mapping for property {0} detected.", result.Name));
            }
        }
    }
}
