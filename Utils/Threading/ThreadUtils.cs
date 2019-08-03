using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils.Delegates;

namespace Utils.Threading
{
    // Not Unity safe class
    public static class ThreadUtils
    {
        public static bool TimeoutLock(object obj, Runnable runnable, int millisecondsTimeOut)
        {
            bool isLockTaken = Monitor.TryEnter(obj, millisecondsTimeOut);
            if(isLockTaken)
            {
                try
                {
                    runnable?.Invoke();
                }
                finally {
                    Monitor.Exit(obj);
                }
            }
            return isLockTaken;
        }

        public static bool RepeatingTimeoutLock(object obj, Runnable runnable, int millisecondsTimeOut, uint maxAttempts = 0, int interAttemptyDelay = 10)
        {
            if(maxAttempts == 0)
            {
                while(true)
                {
                    TimeOutLockWithDelay(obj, runnable, millisecondsTimeOut, interAttemptyDelay);
                    return true;
                }
            }
            else
            {
                for(uint i = 1; i <= maxAttempts; i++)
                {
                    int delay = (i != maxAttempts) ? interAttemptyDelay : 0;
                    if(TimeOutLockWithDelay(obj, runnable, millisecondsTimeOut, delay))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool TimeOutLockWithDelay(object obj, Runnable runnable, int millisecondsTimeOut, int delay)
        {
            if(TimeoutLock(obj, runnable, millisecondsTimeOut))
            {
                return true;
            }

            Thread.Sleep(delay);
            return false;
        }
    }
}
