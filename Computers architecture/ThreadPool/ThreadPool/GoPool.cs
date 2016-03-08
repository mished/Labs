using System;
using System.Collections.Generic;
using System.Threading;

namespace Mished.ThreadPool {

    public class GoPool {
        private int threadsCount;
        private SemaphoreSlim throttler;
        private Object thisLock = new Object();
        private Queue<Action> tasks = new Queue<Action>();

        private GoPool() { }

        public GoPool(int threads) {
            threadsCount = threads;
            throttler = new SemaphoreSlim(threadsCount);
        }

        public void Go(Action action) {
            tasks.Enqueue(action);
            ProcessTask();
        }

        private async void ProcessTask() {
            await throttler.WaitAsync();
            new Thread(() => {
                Action task;
                lock (thisLock) {
                    task = tasks.Dequeue();
                }
                task();
                throttler.Release();
            }).Start();
        }

    }
}
