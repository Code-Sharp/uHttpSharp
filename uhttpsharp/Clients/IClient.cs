using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace uhttpsharp.Clients
{
    public interface IClient
    {

        Stream Stream { get; }

        bool Connected { get; }

        void Close();

        EndPoint RemoteEndPoint { get; }

        Task InitializeStream();

    }
}
