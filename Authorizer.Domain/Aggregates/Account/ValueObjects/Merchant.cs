using System;
using System.Collections.Generic;
using System.Text;

namespace Authorizer.Domain.Aggregates.Account.ValueObjects
{
    public class Merchant
    {
        public virtual string Name { get; set; }

        public Merchant()
        {

        }

        public Merchant(string name)
        {
            this.Name = name;
        }
    }
}
