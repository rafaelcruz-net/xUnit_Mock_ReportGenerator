using Authorizer.Domain.Aggregates.Account.ValueObjects;
using Authorizer.Infrastructure.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Authorizer.Domain.Aggregates.Account
{
    public class Account 
    {
        public Account()
        {
            this.Transactions = new List<Transaction>();
        }


        public virtual int AccountId { get; set; }

        public virtual bool ActiveCard { get; set; }

        public virtual Limit AvailableLimit { get; set; }

        public virtual IList<Transaction> Transactions { get; set; }

        public virtual void ShouldPerformTransaction(Transaction transaction)
        {
            var businessException = new BusinessException();

            VerifyActiveCard(businessException);

            VerifyAvailableLimit(transaction, businessException);

            VerifyTransactions(transaction, businessException);

        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            this.AvailableLimit.Subtract(transaction.Amount);

            transaction.Account = this;
            
            this.Transactions.Add(transaction);

        }


        #region Private Transaction

        private void VerifyTransactions(Transaction transaction, BusinessException businessException)
        {
            var transactions = this.Transactions.Where(x => x.Time.ToLocalTime() >= transaction.Time.AddMinutes(-2).ToLocalTime() && x.Time.ToLocalTime() <= transaction.Time.ToLocalTime());

            if (transactions.Count() >= 3)
                AddBusinessException("high-frequency-small-interval", businessException);

            if (transactions.Any(x => x.Merchant.Name == transaction.Merchant.Name && x.Amount.Value == transaction.Amount.Value)) 
                AddBusinessException("doubled-transaction", businessException);
        }

        private void VerifyActiveCard(BusinessException businessException)
        {
            if (this.ActiveCard == false)
            {
                AddBusinessException("account-not-active", businessException);
            }
        }

        private void AddBusinessException(string violationName, BusinessException businessException)
        {
            businessException.AddError(new BusinessValidationFailure()
            {
                ErrorMessage = violationName,
                Account = new
                {
                    activeCard = this.ActiveCard,
                    availableLimit = this.AvailableLimit.Value
                }
            });

            businessException.ValidateAndThrow();
        }

        private void VerifyAvailableLimit(Transaction transaction, BusinessException businessException)
        {
            if (transaction.Amount.Value > this.AvailableLimit.Value)
                AddBusinessException("insufficient-limit", businessException);

        }
        #endregion
    }
}
