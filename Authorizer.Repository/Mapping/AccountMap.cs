using Authorizer.Domain.Aggregates.Account;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authorizer.Repository.Mapping
{
    public class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            Table("Account");
            Id(x => x.AccountId).GeneratedBy.Identity();
            Map(x => x.ActiveCard);

            Component(x => x.AvailableLimit, member =>
            {
                member.Map(m => m.Value).Column("AvailableLimit");
            });


            HasMany(x => x.Transactions).KeyColumn("AccountId").Not.LazyLoad().Cascade.AllDeleteOrphan().Inverse();
                
        }
    }
}
