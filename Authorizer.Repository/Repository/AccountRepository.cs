using Authorizer.Domain.Aggregates.Account;
using Authorizer.Domain.Aggregates.Account.Repository;
using Hyperion.Infrastructure.Database;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authorizer.Repository.Repository
{
    public class AccountRepository : UnitOfWork<Account>, IAccountRepository, IRepository<Account>
    {

        public AccountRepository(ISession session) : base(session)
        {

        }

    }
}
