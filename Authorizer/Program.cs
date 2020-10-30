using Authorizer.Infrastructure.Exception;
using Authorizer.Repository;
using Authorizer.Service;
using Authorizer.Service.Account;
using Authorizer.Service.Account.Dto;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorizer
{
    class Program
    {
        /// <param name="operation">Operation could be: account or transaction</param>
        /// <param name="input">the json input</param>
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                                 .AddRepository()
                                 .AddServices()
                                 .BuildServiceProvider();

            var accountService = serviceProvider.GetRequiredService<IAccountService>();

            while (true)
            {
                AccountDto result = null;

                Console.Write("> ");

                var param = Console.ReadLine().Split("--", StringSplitOptions.RemoveEmptyEntries);

                if (param.Contains("quit"))
                    Environment.Exit(0);

                var operation = param.FirstOrDefault(x => x.Contains("operation"));
                
                var input = param.FirstOrDefault(x => x.Contains("input"));

                if (String.IsNullOrEmpty(operation))
                {
                    Console.WriteLine("Not support operation");
                    continue;
                }

                if (String.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Not support operation");
                    continue;
                }

                var json = input.Replace("input", "");

                try
                {

                    if (operation.Contains("account"))
                        result = await accountService.CreateAccount(JsonConvert.DeserializeObject<AccountDto>(json));
                    else if (operation.Contains("transaction"))
                        result = await accountService.CreateTransaction(JsonConvert.DeserializeObject<TransactionDto>(json));
                    else
                    {
                        Console.WriteLine("Not support operation");
                        continue;
                    }

                    Console.WriteLine(JsonConvert.SerializeObject(result));
                }
                catch (BusinessException bex)
                {
                    Console.WriteLine(bex.GetError());
                    continue;
                }
            }
        }
    }
}
