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

        public static bool RepeatingTimeoutLock(object obj, Runnable runnable, int millisecondsTimeOut, int maxAttempts = 0, int interAttemptyDelay = 10)
        {
            maxAttempts = Math.Max(0, maxAttempts);
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
                for(int i = 1; i < maxAttempts; i++)
                {
                    if(TimeOutLockWithDelay(obj, runnable, millisecondsTimeOut, interAttemptyDelay))
                    {
                        return true;
                    }
                }
                if (TimeOutLockWithDelay(obj, runnable, millisecondsTimeOut, 0))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool TimeOutLockWithDelay(object obj, Runnable runnable, int millisecondsTimeOut, int delay)
        {
            bool isSuccessful = TimeoutLock(obj, runnable, millisecondsTimeOut);
            if(isSuccessful)
            {
                return true;
            }

            if (delay > 0)
            {
                Thread.Sleep(delay);
            }
            return false;
        }
    }
}
