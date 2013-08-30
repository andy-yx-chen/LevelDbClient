using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AndyTech.LevelDbClient
{
    public class AsyncSemaphore
    {
        private int resources;
        private SpinLock queueLock;
        private Queue<TaskCompletionSource<bool>> waitQueue;

        public AsyncSemaphore(int resources)
        {
            this.resources = resources;
            this.queueLock = new SpinLock();
            this.waitQueue = new Queue<TaskCompletionSource<bool>>();
        }

        public Task<bool> Acquire()
        {
            TaskCompletionSource<bool> waitItem = new TaskCompletionSource<bool>();
            bool lockTaken = false;
            try
            {
                this.queueLock.Enter(ref lockTaken);
                if (this.resources <= 0)
                {
                    this.waitQueue.Enqueue(waitItem);
                }
                else
                {
                    --this.resources;
                    waitItem.SetResult(true);
                }
            }
            finally
            {
                if (lockTaken)
                {
                    this.queueLock.Exit();
                }
            }

            return waitItem.Task;
        }

        public void Release()
        {
            bool lockTaken = false;
            try
            {
                this.queueLock.Enter(ref lockTaken);
                if (++this.resources > 0 && this.waitQueue.Count > 0)
                {
                    TaskCompletionSource<bool> waitItem = this.waitQueue.Dequeue();
                    --this.resources;
                    waitItem.SetResult(true);
                }
            }
            finally
            {
                if (lockTaken)
                {
                    this.queueLock.Exit();
                }
            }
        }
    }

}
