using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Utils.Delegates;
using Utils.Net;

namespace GameServer.GameServerLogic.ConcurrentScheduling
{
    public class EventQueueEntry
    {
        public Runnable runnable;
        public AsyncUserToken userToken;
    }

    public class EventHandlingQueue
    {
        private LinkedList<EventQueueEntry> m_eventQueue;
        private List<EventHandlingAgent> m_agents;
        private ConcurrentDictionary<AsyncUserToken, object> m_locks;

        private bool m_isRunning = false;

        private object m_structLock = new object();

        private object m_agentNotifier = new object();

        public EventHandlingQueue(int numberOfAgents = 10)
        {
            if(numberOfAgents < 1)
            {
                throw new ArgumentException("numberOfAgents must be positive integer");
            }
            m_eventQueue = new LinkedList<EventQueueEntry>();
            m_locks = new ConcurrentDictionary<AsyncUserToken, object>();
            m_agents = new List<EventHandlingAgent>(numberOfAgents);
            for(int i = 0; i < numberOfAgents; i++)
            {
                var agent = new EventHandlingAgent(this, m_agentNotifier);
                m_agents.Add(agent);
            }
        }

        public void Start()
        {
            if(m_isRunning)
            {
                throw new InvalidOperationException("EventHandlingQueue is already running!");
            }
            m_isRunning = true;
            foreach(var agent in m_agents)
            {
                agent.Start();
            }
        }

        public void AddEvent(EventQueueEntry entry)
        {
            lock(m_structLock)
            {
                m_eventQueue.AddLast(entry);
                NotifyAgent();
            }
        }

        public EventQueueEntry GetEntryToProcess()
        {
            lock (m_structLock)
            {
                foreach (var entry in m_eventQueue)
                {
                    if(Monitor.TryEnter(GetUserTokenLock(entry.userToken)))
                    {
                        return entry;
                    }
                }
            }
            return null;
        }

        private object GetUserTokenLock(AsyncUserToken userToken)
        {
            return m_locks.GetOrAdd(userToken, _ => new object());
        }

        public void RemoveEntryAndReleaseLock(EventQueueEntry entry)
        {
            lock(m_structLock)
            {
                m_eventQueue.Remove(entry);
            }

            object tmpLock;
            if(m_locks.TryGetValue(entry.userToken, out tmpLock))
            {
                Monitor.Exit(tmpLock);
            }
            NotifyAgent();
        }

        private void NotifyAgent()
        {
            Console.WriteLine("Obavestavam, velicina niza: " + m_eventQueue.Count);
            lock (m_agentNotifier)
            {
                Monitor.Pulse(m_agentNotifier);
            }
        }
    }
}
