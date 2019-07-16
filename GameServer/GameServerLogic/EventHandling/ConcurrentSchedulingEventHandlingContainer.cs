using GameServer.GameServerLogic.ConcurrentScheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Interface;
using Utils.Net;

namespace GameServer.GameServerLogic.EventHandling
{
    public class ConcurrentSchedulingEventHandlingContainer : IEventHandlingContainer
    {
        private IEventHandlingContainer m_innerEventHandlingContainer;
        private EventHandlingQueue m_queue;

        public ConcurrentSchedulingEventHandlingContainer(
            IEventHandlingContainer innerEventHandlingContainer, int numberOfAgents = 10)
        {
            m_innerEventHandlingContainer = innerEventHandlingContainer ??
                throw new ArgumentNullException(nameof(innerEventHandlingContainer));
            m_queue = new EventHandlingQueue(numberOfAgents);
        }

        public void StartQueue()
        {
            m_queue.Start();
            Console.WriteLine("Serving queue started");
        }

        public void OnMessageError(AsyncUserToken userToken)
        {
            m_queue.AddEvent(new EventQueueEntry {
                userToken = userToken,
                runnable = () => m_innerEventHandlingContainer.OnMessageError(userToken)
            });
        }

        public void OnMessageReceived(MessageWrapper message)
        {
            m_queue.AddEvent(new EventQueueEntry
            {
                userToken = message.UserToken,
                runnable = () => m_innerEventHandlingContainer.OnMessageReceived(message)
            });
        }

        public void OnUserConnect(AsyncUserToken userToken)
        {
            m_queue.AddEvent(new EventQueueEntry
            {
                userToken = userToken,
                runnable = () => m_innerEventHandlingContainer.OnUserConnect(userToken)
            });
        }

        public void OnUserDisconnect(AsyncUserToken userToken)
        {
            m_queue.AddEvent(new EventQueueEntry
            {
                userToken = userToken,
                runnable = () => m_innerEventHandlingContainer.OnUserDisconnect(userToken)
            });
        }
    }
}
