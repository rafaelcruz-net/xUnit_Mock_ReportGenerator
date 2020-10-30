using System;
using System.Collections.Generic;
using System.Text;

namespace Authorizer.Domain.Aggregates.Account.ValueObjects
{
    public class Limit
    {
        public virtual decimal Value { get; set; }

        internal Limit() { }

        public Limit(decimal limit)
        {
            this.Value = limit;
        }

        internal void Subtract(Amount amount)
        {
            this.Value -= amount.Value;
        }
    }
}
