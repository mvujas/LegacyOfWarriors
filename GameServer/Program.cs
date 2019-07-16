using GameServer.Logic;
using GameServer.Repositories;
using System;
using GameServer.Net;
using Utils.Net;
using System.Net;
using ProjectLevelConfig;
using GameServer.GameServerLogic.ConcurrentScheduling;
using System.Collections.Generic;

namespace GameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*Config.Prepare();

            Initializer.Initialize();

            var abc = new CustomEventHandlingContainer();

            SocketServer socketServer = new SocketServer(
                100,
                100,
                abc
            );

            abc.server = socketServer;



            IPEndPoint endPoint = NetUtils.CreateEndPoint(
                SocketServerConfig.HOST,
                SocketServerConfig.PORT
            );

            socketServer.Start(endPoint);*/

            List<AsyncUserToken> tokens = new List<AsyncUserToken>();
            for(int i = 0; i < 5; i++)
            {
                tokens.Add(new AsyncUserToken());
            }

            List<int> entries = new List<int>();

            EventHandlingQueue queue = new EventHandlingQueue(2);
            for(int i = 0; i < 1000000; i++)
            {
                int k = i;
                queue.AddEvent(new EventQueueEntry
                {
                    runnable = () => {
                        if(k % tokens.Count == 1)
                        {
                            entries.Add(k);
                        }
                    },
                    userToken = tokens[i % 5]
                });
            }


            Console.WriteLine("Dodati");
            Console.ReadKey();


            Console.WriteLine("Provera:");
            Console.WriteLine("Velicina: " + entries.Count);
            int referent = int.MinValue;
            foreach (var entry in entries)
            {
                if (entry < referent)
                {
                    Console.WriteLine("Greska!");
                }
            }

            Console.WriteLine("Kraj!");

            Console.ReadKey();


        }
    }
}
