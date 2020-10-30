using System;
using System.Collections.Generic;
using System.Text;

namespace Authorizer.Service.Account.Dto
{
    public class TransactionDto
    {
        public String Merchant { get; set; }
        public Decimal Amount { get; set; }
        public DateTime Time { get; set; }
    }
}
