using ProjectLevelConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utils.Net;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint endPoint = NetUtils.CreateEndPoint(
                SocketServerConfig.HOST,
                SocketServerConfig.PORT
            );

            SocketAsyncEventArgs readArgs = new SocketAsyncEventArgs();
            SocketAsyncEventArgs writeArgs = new SocketAsyncEventArgs();

            byte[] buffer = new byte[1024];
            readArgs.SetBuffer(buffer, 0, buffer.Length);

            AsyncUserToken userToken = new AsyncUserToken
            {
                Socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp),
                ReadEventArgs = readArgs,
                WriteEventArgs = writeArgs,
                OnMessageError = e => {

                },
                OnMessageFullyReceived = e =>
                {

                }
            };

            try
            {
                userToken.Socket.Connect(endPoint);
            }
            catch(Exception e)
            {
                Console.WriteLine("Izuzetak: \n" + e);
            }
            Console.WriteLine("Hej");

            Console.ReadKey();
        }
    }
}
