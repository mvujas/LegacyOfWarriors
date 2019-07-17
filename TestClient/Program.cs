using ProjectLevelConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils.Net;

namespace TestClient
{
    class Program
    {
        static string bytesToStr(byte[] bajtovi)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(var bajt in bajtovi)
            {
                stringBuilder.Append(bajt);
            }
            return stringBuilder.ToString();
        }

        static void ProcessReceive(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                byte[] buffer = e.Buffer;
                AsyncUserToken token = (AsyncUserToken)e.UserToken;

                token.Process(buffer, e.BytesTransferred);

                if (!token.Socket.ReceiveAsync(token.ReadEventArgs))
                {
                    ProcessReceive(null, token.ReadEventArgs);
                }
            }
            else
            {
                Console.WriteLine("Greska!");
            }
        }

        static int count = 0;

        static void ProcessSend(object sender, SocketAsyncEventArgs e)
        {
            if(e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                if(count < 100000)
                {
                    SendNext((AsyncUserToken)e.UserToken);
                }
            }
            else
            {
                Console.WriteLine("Greska!");
            }
        }

        static void SendNext(AsyncUserToken userToken)
        {
            int i = count++;

            Console.WriteLine("Saljem poruku broj: " + i);

            byte[] message = MessageTransformer.PrepareMessageForSending(
                        Encoding.ASCII.GetBytes("Neka poruka numero: " + i));

            Console.WriteLine("Duzina: " + message.Length);

            Console.WriteLine("Poruka u bajtovima: " + bytesToStr(message));

            userToken.WriteEventArgs.SetBuffer(message, 0, message.Length);
            if (!userToken.Socket.SendAsync(userToken.WriteEventArgs))
            {
                ProcessSend(null, userToken.WriteEventArgs);
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
                    Console.WriteLine("Stigla poruka: " + Encoding.ASCII.GetString(e.Message));
                }
            };

            userToken.WriteEventArgs.Completed += ProcessSend;

            try
            {
                userToken.Socket.Connect(endPoint);

                SendNext(userToken);

                if (!userToken.Socket.ReceiveAsync(userToken.ReadEventArgs))
                {
                    ProcessReceive(null, userToken.ReadEventArgs);
                }

                Console.ReadKey();
            }
            catch(Exception e)
            {
                Console.WriteLine("Izuzetak: \n" + e);
            }

            try
            {
                userToken.Socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }

            userToken.Socket.Close();
            Console.ReadKey();
        }
    }
}
