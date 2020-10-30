using Authorizer.Domain.Aggregates.Account;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authorizer.Repository.Mapping
{
    public class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            Table("AccountTransaction");
            Id(x => x.Id).GeneratedBy.Identity();

            Map(x => x.Time);

            Component(x => x.Amount, member =>
            {
                member.Map(m => m.Value).Column("Amount");
            });

            Component(x => x.Merchant, member =>
            {
                member.Map(m => m.Name).Column("Merchant");
            });

            References(x => x.Account).Column("AccountId");

        }

    }
}
