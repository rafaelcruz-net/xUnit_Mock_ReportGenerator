using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Authorizer.Filter
{
    public class HttpResponseException
    {
        public HttpStatusCode StatusCode { get; set; }

        public String Title { get; set; } = "One or more validation errors occurred";

        public List<HttpResponseMessage> Violations { get; set; } = new List<HttpResponseMessage>();
    }
}
