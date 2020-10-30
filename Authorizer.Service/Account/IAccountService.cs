using Authorizer.Service.Account.Dto;
using System.Threading.Tasks;

namespace Authorizer.Service.Account
{
    public interface IAccountService
    {
        Task<AccountDto> CreateAccount(AccountDto dto);
        Task<AccountDto> CreateTransaction(TransactionDto dto);
    }
}