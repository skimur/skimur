using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure
{
    public class Mapper : IMapper
    {
        private readonly object _lock = new object();
        private readonly Dictionary<Tuple<Type, Type>, Delegate> _delegates = new Dictionary<Tuple<Type, Type>, Delegate>();

        public void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            Map(source, typeof(TSource), destination, typeof(TDestination));
        }

        public TTarget Map<TSource, TTarget>(TSource source) where TTarget : class, new()
        {
            if (source == null) return null;

            var target = new TTarget();
            Map(source, target);

            return target;
        }

        private void Map(object source, Type sourceType, object destination, Type destinationType)
        {
            var key = new Tuple<Type, Type>(sourceType, destinationType);
            
            // ReSharper disable InconsistentlySynchronizedField
            if (_delegates.ContainsKey(key))
            {
                ((Action<object, object>)_delegates[key])(source, destination);
                return;
            }
            // ReSharper restore InconsistentlySynchronizedField

            lock (_lock)
            {
                if (_delegates.ContainsKey(key))
                {
                    ((Action<object, object>)_delegates[key])(source, destination);
                    return;
                }

                var sourceParameter = Expression.Parameter(typeof(object), "source");
                var targetParameter = Expression.Parameter(typeof(object), "target");

                var sourceVariable = Expression.Variable(sourceType, "castedSource");
                var targetVariable = Expression.Variable(destinationType, "castedTarget");

                var expressions = new List<Expression>();

                expressions.Add(Expression.Assign(sourceVariable, Expression.Convert(sourceParameter, sourceType)));
                expressions.Add(Expression.Assign(targetVariable, Expression.Convert(targetParameter, destinationType)));

                foreach (var property in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!property.CanRead)
                        continue;

                    var targetProperty = destinationType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
                    if (targetProperty != null
                        && targetProperty.CanWrite
                        && targetProperty.PropertyType.IsAssignableFrom(property.PropertyType))
                    {
                        expressions.Add(
                            Expression.Assign(
                                Expression.Property(targetVariable, targetProperty),
                                Expression.Convert(
                                    Expression.Property(sourceVariable, property), targetProperty.PropertyType)));
                    }
                }

                var lambda =
                    Expression.Lambda<Action<object, object>>(
                        Expression.Block(new[] { sourceVariable, targetVariable }, expressions), sourceParameter, targetParameter);

                var del = lambda.Compile();

                _delegates[key] = del;

                del(source, destination);
            }
        }
    }
}
