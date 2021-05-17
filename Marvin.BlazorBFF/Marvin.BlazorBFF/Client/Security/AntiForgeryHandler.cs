using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Marvin.BlazorBFF.Client.Security
{
    public class AntiforgeryHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            request.Headers.Add("X-CSRF", "1");
            return base.SendAsync(request, cancellationToken);
        }
    }
}
