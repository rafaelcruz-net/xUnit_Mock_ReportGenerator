using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Authorizer.Infrastructure.Exception
{
    public class BusinessException : System.Exception
    {
        public List<BusinessValidationFailure> Errors { get; private set; } = new List<BusinessValidationFailure>();

        public BusinessException()
        {
            this.Errors = new List<BusinessValidationFailure>(); 
        }

        public BusinessException(string message, string errorName = "ValidationError") : base(message)
        {
            this.Errors.Add(new BusinessValidationFailure()
            {
                ErrorMessage = message,
                ErrorName = errorName
            });
        }

        public BusinessException(string message, System.Exception innerException) : base(message, innerException)
        {
            this.Errors.Add(new BusinessValidationFailure()
            {
                ErrorMessage = message,
                ErrorName = "InnerExceptionError"
            });
        }

        public void AddError(BusinessValidationFailure error)
        {
            this.Errors.Add(error);
        }

        public void ValidateAndThrow()
        {
            if (this.Errors.Any())
                throw this;
        }

        public string GetError()
        {
            var error = this.Errors.First();

            return JsonConvert.SerializeObject(new
            {
                Account = error.Account,
                Violations = new[]
                {
                    error.ErrorMessage
                }
            });
        }
    }

    public class BusinessValidationFailure
    {
        public string ErrorName { get; set; } = "violations";
        public string ErrorMessage { get; set; }

        public object Account { get; set; }

        public override string ToString()
        {
            return this.ErrorMessage;
        }
        
    }
}
