using Authorizer.Domain.Aggregates.Account.Repository;
using Authorizer.Repository.Repository;
using Hyperion.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Authorizer.Repository
{
    public static class Configuration
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            var sessionFactory = SessionFactory.CreateSessionFactory(Assembly.GetExecutingAssembly());

            services.AddSingleton<ISessionFactory>(sessionFactory);
            services.AddScoped<ISession>(s => sessionFactory.OpenSession());
            services.AddScoped(typeof(ISession<>), typeof(UnitOfWork<>));

            services.AddScoped<IAccountRepository, AccountRepository>();

            return services;
        }
    }
}
