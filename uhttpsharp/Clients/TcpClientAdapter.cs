using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace uhttpsharp.Clients
{
    public class TcpClientAdapter : IClient
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;

        public TcpClientAdapter(TcpClient client)
        {
            _client = client;
            _stream = _client.GetStream();

            // The next lines are commented out because they caused exceptions, 
            // They have been added because .net doesn't allow me to wait for data (ReadAsyncBlock).
            // Instead, I've added Task.Delay in MyStreamReader.ReadBuffer when
            // Read returns without data.
            
            // See https://github.com/Code-Sharp/uHttpSharp/issues/14
            
            // Read Timeout of one second.
            // _stream.ReadTimeout = 1000;
        }

        public Stream Stream
        {
            get { return _stream; }
        }

        public bool Connected
        {
            get { return _client.Connected; }
        }

        public void Close()
        {
            _client.Close();
        }


        public EndPoint RemoteEndPoint
        {
            get { return _client.Client.RemoteEndPoint; }
        }

        public Task InitializeStream() {
            return Task.Factory.GetCompleted();
        }
    }
}