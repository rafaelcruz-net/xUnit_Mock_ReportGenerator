using System;
using System.Collections.Generic;
using System.Text;

namespace Authorizer.Service.Account.Dto
{
    public class AccountDto
    {
        public bool ActiveCard { get; set; }
        public Decimal Limit { get; set; }

    }
}
