using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hyperion.Infrastructure.Database
{
    public static class SessionFactory
    {
        public static ISessionFactory CreateSessionFactory(Assembly assembly)
        {
            var configuration = Fluently.Configure()
                               .Database(SQLiteConfiguration.Standard.UsingFile($"{Guid.NewGuid().ToString()}.db"))
                               .Mappings(m => m.FluentMappings.AddFromAssembly(assembly))
                               .BuildConfiguration();

            var sessionFactory = configuration.BuildSessionFactory();

            var session = sessionFactory.OpenSession();

            var schema = new SchemaExport(configuration);
            schema.Drop(false, true, session.Connection);
            schema.Create(false, true, session.Connection);
            session.Flush();
            session.Close();

            return sessionFactory;
        }
        
    }
}
