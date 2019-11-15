using System.Dynamic;
using System.Net;
using uhttpsharp.Clients;
using uhttpsharp.Headers;

namespace uhttpsharp
{
    internal class HttpContext : IHttpContext
    {
        private readonly IHttpRequest _request;
        private readonly IClient _client;
        private readonly ICookiesStorage _cookies;
        private readonly ExpandoObject _state = new ExpandoObject();
        public HttpContext(IHttpRequest request, IClient client)
        {
            _request = request;
            _client = client;
            _cookies = new CookiesStorage(_request.Headers.GetByNameOrDefault("cookie", string.Empty));
        }

        public IHttpRequest Request
        {
            get { return _request; }
        }

        public IHttpResponse Response { get; set; }

        public ICookiesStorage Cookies
        {
            get { return _cookies; }
        }

        public dynamic State
        {
            get { return _state; }
        }

        public IClient Client {
            get { return _client; }
        }

        public EndPoint RemoteEndPoint
        {
            get { return _client.RemoteEndPoint; }
        }
    }
}