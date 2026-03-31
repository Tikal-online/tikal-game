namespace TikalBackend.IntegrationTests.Extensions;

internal static class HttpClientExtensions
{
    extension(HttpClient client)
    {
        public Task<HttpResponseMessage> GetAsyncWithUser(string url, TestUser user)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url).WithUser(user);

            return client.SendAsync(request);
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