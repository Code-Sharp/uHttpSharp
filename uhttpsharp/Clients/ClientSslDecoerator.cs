using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace uhttpsharp.Clients
{
    public class ClientSslDecorator : IClient
    {
        private readonly IClient _child;
        private readonly X509Certificate _certificate;
        private readonly SslStream _sslStream;

        public ClientSslDecorator(IClient child, X509Certificate certificate)
        {
            _child = child;
            _certificate = certificate;
            _sslStream = new SslStream(_child.Stream);
        }

        public async Task AuthenticateAsServer()
        {
            Task timeout = Task.Delay(TimeSpan.FromSeconds(10));
            if (timeout == await Task.WhenAny(_sslStream.AuthenticateAsServerAsync(_certificate, false, SslProtocols.Tls12, true), timeout).ConfigureAwait(false))
            {
                throw new TimeoutException("SSL Authentication Timeout");
            }
        }

        public Stream Stream
        {
            get { return _sslStream; }
        }

        public bool Connected
        {
            get { return _child.Connected; }
        }

        public void Close()
        {
            _child.Close();
        }

        public EndPoint RemoteEndPoint
        {
            get { return _child.RemoteEndPoint; }
        }
    }
}