using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer.GameServerLogic.ConcurrentScheduling
{
    internal class EventHandlingAgent
    {
        private EventHandlingQueue m_queue;
        private object m_notifier;

        public EventHandlingAgent(EventHandlingQueue queue, object notifier)
        {
            m_queue = queue;
            m_notifier = notifier;
        }

        public void Start()
        {
            Thread thread = new Thread(Loop);
            thread.Start();
        }
        
        private void WaitForNotification()
        {
            lock(m_notifier)
            {
                Monitor.Wait(m_notifier);
            }
        }

        private void Loop()
        {
            while (true)
            {
                WaitForNotification();
                ProcessEntry();
            }
        }

        private void ProcessEntry()
        {
            EventQueueEntry entry = m_queue.GetEntryToProcess();
            if(entry != null)
            {
                Console.WriteLine("Uzeo lock!");
                try
                {
                    entry.runnable();
                }
                finally
                {
                    m_queue.RemoveEntryAndReleaseLock(entry);
                    Console.WriteLine("Pustam lock!");
                }
            }
        }
    }
}
