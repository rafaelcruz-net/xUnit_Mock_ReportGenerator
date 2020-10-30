using System;
using System.Collections.Generic;
using System.Text;

namespace Authorizer.Domain.Aggregates.Account.ValueObjects
{
    public class Amount
    {
        public virtual Decimal Value { get; set; }

        internal Amount() { }

        public Amount(decimal value)
        {
            this.Value = value;
        }
    }
}
