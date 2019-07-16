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
        static void ProcessReceive(object sender, SocketAsyncEventArgs e)
        {
            byte[] buffer = e.Buffer;
            Console.WriteLine("Primanje u toku: !");
            Console.WriteLine(buffer.Length);
            AsyncUserToken token = (AsyncUserToken)e.UserToken;

            token.Process(e.Buffer);

            if (!token.Socket.ReceiveAsync(token.ReadEventArgs))
            {
                ProcessReceive(null, token.ReadEventArgs);
            }
        }

        static void Main(string[] args)
        {
            IPEndPoint endPoint = NetUtils.CreateEndPoint(
                SocketServerConfig.HOST,
                SocketServerConfig.PORT
            );

            SocketAsyncEventArgs readArgs = new SocketAsyncEventArgs();
            SocketAsyncEventArgs writeArgs = new SocketAsyncEventArgs();
            readArgs.Completed += ProcessReceive;

            byte[] buffer = new byte[10];
            readArgs.SetBuffer(buffer, 0, buffer.Length);

            AsyncUserToken userToken = new AsyncUserToken
            {
                Socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp),
                ReadEventArgs = readArgs,
                WriteEventArgs = writeArgs,
                OnMessageError = e => 
                {

                },
                OnMessageFullyReceived = e =>
                {
                    Console.WriteLine(Encoding.ASCII.GetString(e.Message));
                }
            };

            try
            {
                userToken.Socket.Connect(endPoint);

                Console.WriteLine("Slusam!");
                if(!userToken.Socket.ReceiveAsync(userToken.ReadEventArgs))
                {
                    ProcessReceive(null, userToken.ReadEventArgs);
                }

                Console.ReadKey();
            }
            catch(Exception e)
            {
                Console.WriteLine("Izuzetak: \n" + e);
            }
        }
    }
}
