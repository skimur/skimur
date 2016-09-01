using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Skimur.Data.Postgres
{
    public class MigrationResourceFinder : IMigrationResourceFinder
    {
        public MigrationResources Find()
        {
            var resources = new MigrationResources();
            
            var assemblies = new List<Assembly>();

            foreach (var runtime in DependencyContext.Default.RuntimeLibraries)
            {
                if(runtime.Name.StartsWith("Skimur"))
                    assemblies.Add(Assembly.Load(new AssemblyName(runtime.Name)));
            }
            
            foreach(var assembly in assemblies)
            {
                foreach(var type in assembly.GetTypes())
                {
                    if(typeof(Migration).IsAssignableFrom(type))
                    {
                        if(!type.GetTypeInfo().IsAbstract)
                            resources.Migrations.Add(Activator.CreateInstance(type) as Migration);
                    }
                }
            }

            return resources;
        }
    }
}
