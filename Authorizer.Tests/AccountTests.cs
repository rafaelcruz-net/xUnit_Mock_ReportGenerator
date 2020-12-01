using Authorizer.Domain.Aggregates.Account;
using Authorizer.Domain.Aggregates.Account.Repository;
using Authorizer.Domain.Aggregates.Account.ValueObjects;
using Authorizer.Infrastructure.Exception;
using Authorizer.Infrastructure.Specification;
using Authorizer.Service.Account;
using Authorizer.Service.Account.Dto;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Authorizer.Tests
{
    public class AccountTests
    {
        private Mock<IAccountRepository> _repository;
        private IAccountService accountService;

        [Fact]
        public async Task Should_Setup_Account_Correctly()
        {
            // 3A -> ARRANGE, ACT, ASSERT

            _repository = new Mock<IAccountRepository>();
            accountService = new AccountService(_repository.Object);

            var account = new Account()
            {
                ActiveCard = true,
                AvailableLimit = new Domain.Aggregates.Account.ValueObjects.Limit(100),
                AccountId = 1
            };

            _repository.Setup(x => x.GetOneByCriteria(It.IsAny<ISpecification<Account>>())).Verifiable();
            _repository.Setup(x => x.Save(It.IsAny<Account>()));
            _repository.Setup(x => x.SaveChanges());

            var expected = await accountService.CreateAccount(new Service.Account.Dto.AccountDto()
            {
                ActiveCard = true,
                Limit = 100
            });

            expected.Should().NotBeNull();
            expected.ActiveCard.Should().BeTrue();
            expected.Limit.Should().Be(100);

            _repository.VerifyAll();

        }

        [Fact]
        public async Task Should_Setup_Account_Result_In_Account_Already_Initialized()
        {
            _repository = new Mock<IAccountRepository>();
            accountService = new AccountService(_repository.Object);

            var account = new Account()
            {
                ActiveCard = true,
                AvailableLimit = new Domain.Aggregates.Account.ValueObjects.Limit(100),
                AccountId = 1
            };

            _repository.Setup(x => x.GetOneByCriteria(It.IsAny<ISpecification<Account>>())).ReturnsAsync(account).Verifiable();

            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await accountService.CreateAccount(new Service.Account.Dto.AccountDto()
                {
                    ActiveCard = true,
                    Limit = 100
                });
            });
        }

        [Fact]
        public async Task Should_Accept_Transaction()
        {
            _repository = new Mock<IAccountRepository>();
            accountService = new AccountService(_repository.Object);

            var account = new Account()
            {
                ActiveCard = true,
                AvailableLimit = new Domain.Aggregates.Account.ValueObjects.Limit(100),
                AccountId = 1
            };

            _repository.Setup(x => x.GetOneByCriteria(It.IsAny<ISpecification<Account>>())).ReturnsAsync(account).Verifiable();

            TransactionDto transaction = new TransactionDto()
            {
                Amount = 50,
                Merchant = "Nubank",
                Time = DateTime.Now
            };

            _repository.Setup(x => x.Save(It.IsAny<Account>()));
            _repository.Setup(x => x.SaveChanges());

            var expected = await accountService.CreateTransaction(transaction);

            expected.Should().NotBeNull();
            expected.Limit.Should().Be(50);
            expected.ActiveCard.Should().BeTrue();
            account.Transactions.Should().HaveCount(1);

            _repository.VerifyAll();
        }

        [Fact]
        public async Task Should_Transaction_Result_In_Insufficent_Limit()
        {
            _repository = new Mock<IAccountRepository>();
            accountService = new AccountService(_repository.Object);

            var account = new Account()
            {
                ActiveCard = true,
                AvailableLimit = new Domain.Aggregates.Account.ValueObjects.Limit(100),
                AccountId = 1
            };

            _repository.Setup(x => x.GetOneByCriteria(It.IsAny<ISpecification<Account>>())).ReturnsAsync(account).Verifiable();

            TransactionDto transaction = new TransactionDto()
            {
                Amount = 150,
                Merchant = "Nubank",
                Time = DateTime.Now
            };

            var bex = await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await accountService.CreateTransaction(transaction);
            });

            bex.Errors.First().ErrorMessage.Should().Be("insufficient-limit");

        }

        [Fact]
        public async Task Should_Transaction_Result_Card_Not_Active()
        {
            _repository = new Mock<IAccountRepository>();
            accountService = new AccountService(_repository.Object);

            var account = Builder<Account>.CreateNew()
                                          .With(x => x.AvailableLimit = new Limit(100))
                                          .With(x => x.ActiveCard = false)
                                          .Build();

            _repository.Setup(x => x.GetOneByCriteria(It.IsAny<ISpecification<Account>>())).ReturnsAsync(account).Verifiable();

            TransactionDto transaction = new TransactionDto()
            {
                Amount = 80,
                Merchant = "Nubank",
                Time = DateTime.Now
            };

            var bex = await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await accountService.CreateTransaction(transaction);
            });

            bex.Errors.First().ErrorMessage.Should().Be("account-not-active");
        }

        [Fact]
        public async Task Should_Transaction_Result_High_Frequency_Small_Interval()
        {
            _repository = new Mock<IAccountRepository>();
            accountService = new AccountService(_repository.Object);


            var account = Builder<Account>.CreateNew()
                                          .With(x => x.AvailableLimit = new Limit(100))
                                          .With(x => x.ActiveCard = true)
                                          .Build();

            account.Transactions = Builder<Transaction>.CreateListOfSize(3)
                                                       .TheFirst(1)
                                                       .With(x => x.Amount = new Amount(100))
                                                       .With(x => x.Time = DateTime.Now.AddMinutes(-1))
                                                       .TheNext(1)
                                                       .With(x => x.Amount = new Amount(80))
                                                       .With(x => x.Time = DateTime.Now.AddSeconds(-30))
                                                       .TheLast(1)
                                                       .With(x => x.Amount = new Amount(60))
                                                       .With(x => x.Time = DateTime.Now)
                                                       .Build();



            _repository.Setup(x => x.GetOneByCriteria(It.IsAny<ISpecification<Account>>())).ReturnsAsync(account).Verifiable();

            TransactionDto transaction = new TransactionDto()
            {
                Amount = 60,
                Merchant = "Nubank",
                Time = DateTime.Now
            };

            var bex = await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await accountService.CreateTransaction(transaction);
            });

            bex.Errors.First().ErrorMessage.Should().Be("high-frequency-small-interval");
        }



        [Theory]
        [InlineData(100, 50, 50)]
        [InlineData(120, 60, 60)]
        [InlineData(200, 100, 100)]
        public async Task Should_Transaction_Result_Double_Transaction(decimal limit, decimal ammount, decimal ammount2)
        {
            _repository = new Mock<IAccountRepository>();
            accountService = new AccountService(_repository.Object);

            var account = new Account()
            {
                ActiveCard = true,
                AvailableLimit = new Domain.Aggregates.Account.ValueObjects.Limit(limit),
                AccountId = 1,
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Domain.Aggregates.Account.ValueObjects.Amount(ammount),
                        Merchant = new Domain.Aggregates.Account.ValueObjects.Merchant("McDonalds"),
                        Time = DateTime.Now.AddSeconds(-30)
                    },
                    new Transaction()
                    {
                        Amount = new Domain.Aggregates.Account.ValueObjects.Amount(ammount2),
                        Merchant = new Domain.Aggregates.Account.ValueObjects.Merchant("Burger King"),
                        Time = DateTime.Now.AddSeconds(-1)
                    }
                }
            };

            _repository.Setup(x => x.GetOneByCriteria(It.IsAny<ISpecification<Account>>())).ReturnsAsync(account).Verifiable();

            TransactionDto transaction = new TransactionDto()
            {
                Amount = ammount,
                Merchant = "Burger King",
                Time = DateTime.Now
            };

            var bex = await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await accountService.CreateTransaction(transaction);
            });

            bex.Errors.First().ErrorMessage.Should().Be("doubled-transaction");
        }


    }
}
