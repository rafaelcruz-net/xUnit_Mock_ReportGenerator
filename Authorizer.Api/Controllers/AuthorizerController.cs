using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authorizer.Service.Account;
using Authorizer.Service.Account.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authorizer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizerController : ControllerBase
    {
        private IAccountService AccountService { get; set; }

        public AuthorizerController(IAccountService service)
        {
            this.AccountService = service;
        }

        [HttpPost]
        [Route("Account")]
        public async Task<IActionResult> CreateAccount(AccountDto dto)
        {
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var result = await this.AccountService.CreateAccount(dto);

            return Created(String.Empty, result);

        }

        [HttpPost]
        [Route("Transaction")]
        public async Task<IActionResult> CreateTransaction(TransactionDto dto)
        {
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var result = await this.AccountService.CreateTransaction(dto);

            return Ok(result);

        }
    }
}
