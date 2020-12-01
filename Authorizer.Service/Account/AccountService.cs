using Authorizer.Domain.Aggregates.Account;
using Authorizer.Domain.Aggregates.Account.Repository;
using Authorizer.Infrastructure.Exception;
using Authorizer.Infrastructure.Specification;
using Authorizer.Service.Account.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DomainAccount = Authorizer.Domain.Aggregates.Account;

namespace Authorizer.Service.Account
{
    public class AccountService : IAccountService
    {
        private IAccountRepository AccountRepository { get; set; }

        public AccountService(IAccountRepository accountRepository)
        {
            this.AccountRepository = accountRepository;
        }

        public async Task<AccountDto> CreateAccount(AccountDto dto)
        {
            var spec = Specification<DomainAccount.Account>.CreateSpecification(x => x.ActiveCard);
            var businessException = new BusinessException();

            var accountInDatabase = await this.AccountRepository.GetOneByCriteria(spec);

            if (accountInDatabase != null)
            {
                businessException.AddError(new BusinessValidationFailure()
                {
                    ErrorMessage = "account-alreadyinitialized-for-account",
                    Account = new
                    {
                        activeCard = accountInDatabase.ActiveCard,
                        availableLimit = accountInDatabase.AvailableLimit.Value
                    }
                });
                businessException.ValidateAndThrow();
            }


            var account = new DomainAccount.Account();
            account.ActiveCard = true;
            account.AvailableLimit = new DomainAccount.ValueObjects.Limit(dto.Limit);

            await this.AccountRepository.Save(account);
            this.AccountRepository.SaveChanges();

            return new AccountDto()
            {
                ActiveCard = account.ActiveCard,
                Limit = account.AvailableLimit.Value
            };

        }

        public async Task<AccountDto> CreateTransaction(TransactionDto dto)
        {
            var spec = Specification<DomainAccount.Account>.CreateSpecification(x => x.ActiveCard);
            var businessException = new BusinessException();

            var account = await this.AccountRepository.GetOneByCriteria(spec);

            if (account == null)
            {
                businessException.AddError(new BusinessValidationFailure() { ErrorMessage = "account-not-initialized" });
                businessException.ValidateAndThrow();
            }

            var transaction = new Transaction()
            {
                Amount = new DomainAccount.ValueObjects.Amount(dto.Amount),
                Merchant = new DomainAccount.ValueObjects.Merchant(dto.Merchant),
                Time = dto.Time
            };

            account.ShouldPerformTransaction(transaction);

            account.ApplyTransaction(transaction);

            await this.AccountRepository.Save(account);

            this.AccountRepository.SaveChanges();

            return new AccountDto()
            {
                ActiveCard = account.ActiveCard,
                Limit = account.AvailableLimit.Value
            };
        }

    }
}
