using System.Net.Http.Json;

namespace TikalBackend.IntegrationTests.Extensions;

internal static class HttpClientExtensions
{
    extension(HttpClient client)
    {
        public Task<HttpResponseMessage> GetAsyncWithUser(string url, TestUser user, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url).WithUser(user);

            return client.SendAsync(request, cancellationToken);
        }

        public Task<HttpResponseMessage> DeleteAsyncWithUser(string url, TestUser user, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url).WithUser(user);

            return client.SendAsync(request, cancellationToken);
        }

        public Task<HttpResponseMessage> PutAsyncWithUser(string url, TestUser user, object? body, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = JsonContent.Create(body)
            }.WithUser(user);

            return client.SendAsync(request, cancellationToken);
        }

        public Task<HttpResponseMessage> PostAsyncWithUser(string url, TestUser user, object? body, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(body)
            }.WithUser(user);

            return client.SendAsync(request, cancellationToken);
        }
    }

    extension(HttpRequestMessage request)
    {
        private HttpRequestMessage WithUser(TestUser user)
        {
            request.Headers.Add("X-Test-UserId", user.UserId);

            return request;
        }
    }
}