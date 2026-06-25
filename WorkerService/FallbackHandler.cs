using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Extensions.Http;

namespace WorkerService.TryName;
public class HttpStatusCodeFallbackHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FALLBACK ENGAGED] Downstream API is dead/circuit broken: {ex.Message}");

            var fallbackResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent("{\"status\":\"Unavailable\",\"message\":\"Fallback triggered. Service is offline.\"}", System.Text.Encoding.UTF8, "application/json")
            };
            return fallbackResponse;
        }
    }
}