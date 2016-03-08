using System;
using System.Collections.Generic;
using System.Threading;

namespace Mished.ThreadPool {

    public class GoPool : IDisposable {
        private int threadsCount;
        private SemaphoreSlim pendingTasks;
        private Object thisLock = new Object();
        private Queue<Action> tasks = new Queue<Action>();
        private List<Thread> workers = new List<Thread>();

        private GoPool() { }

        public GoPool(int threads) {
            threadsCount = threads;
            pendingTasks = new SemaphoreSlim(0);
            for (var i = 0; i < threadsCount; ++i) {
                var worker = new Thread(ConsumeTasks);
                workers.Add(worker);
                worker.Start();
            }
        }

        public void Go(Action action) {
            lock (thisLock) {
                tasks.Enqueue(action);
            }
            pendingTasks.Release();
        }

        private void ConsumeTasks() {
            while (true) {
                pendingTasks.Wait();
                Action task;
                lock (thisLock) {
                    task = tasks.Dequeue();
                }
                if (task == null) return;
                task();
            }
        }

        public void Dispose() {
            workers.ForEach(x => Go(null));
        }

    }
}
