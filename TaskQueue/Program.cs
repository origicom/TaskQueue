using System;
using System.Threading;

namespace TaskWorkerTest
{
    class TaskWorkerTest
    {
        
        public TaskWorker TWorker = new TaskWorker();
        private Thread TaskThread;
        //static AutoResetEvent autoEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            TaskWorkerTest TWT = new TaskWorkerTest();
            TWT.Go();
        }

        private void Go()
        {
            StartThread();
            BuildQueue();
            Console.CancelKeyPress += delegate
            {
                Console.WriteLine("Exit Detected, killing threads...");
                TWorker.End();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                // The application terminates directly after executing this delegate.
            };
            //Console.ReadKey(true); //block main thread to not exit immediately (darned console apps! >.<)
            //Console.WriteLine("Joining Thread..."); //debug output
            //TaskThread.Join();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private void StartThread()
        {
            TWorker = new TaskWorker();
            TWorker.OnReturn += new TaskWorkerEventHandler(OnReturn);
            TaskThread = new Thread(new ThreadStart(TWorker.QueueStart));
            TaskThread.IsBackground = true;
            TaskThread.Start();

            // V---- can't figure this out yet, sticking with Thread ----V
            //ThreadPool.QueueUserWorkItem(new WaitCallback(TaskWorker.QueueStart), autoEvent);
            //autoEvent.WaitOne();
        }

        private void BuildQueue()
        {
            TWorker.Add(new Fib(1, 40));
            TWorker.Add(new Fib(1, 45));
            TWorker.Add(new Fib(1, 50));
            TWorker.Add(new Fib(1, 55));
            //for (int i = 0; i != 30; i++)
            //{
                  //Console.WriteLine(String.Format("Waiting Tasks: {0}", TWorker.WaitingTaskCount.ToString()));
                  //Console.WriteLine(String.Format("Running Tasks: {0}", TWorker.RunningTaskCount.ToString()));
            //    Thread.Sleep(2000);
            //}
            Thread.Sleep(3000);
            //TWorker.End();
        }

        private void OnReturn(TaskWorkerEventArgs e)
        {
            Console.WriteLine("Task Completion Event Raised from {0}", e.Sender.GetType().ToString());
        }
    }
}
