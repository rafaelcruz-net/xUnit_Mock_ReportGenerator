using Authorizer.Domain.Aggregates.Account.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authorizer.Domain.Aggregates.Account
{
    public class Transaction
    {
        public virtual int Id { get; set; }
        public virtual Merchant Merchant { get; set; }
        public virtual Amount Amount { get; set; }
        public virtual DateTime Time { get; set; }
        public virtual Account Account { get; set; }
    }
}
