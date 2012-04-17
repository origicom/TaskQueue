using System;
using System.Threading;

namespace TaskWorkerTest
{
    internal class TaskWorkerTest
    {
        public TaskWorker _tWorker = new TaskWorker();
        private Thread _taskThread;
        //static AutoResetEvent autoEvent = new AutoResetEvent(false);

        private static void Main(string[] args)
        {
            TaskWorkerTest TWT = new TaskWorkerTest();
            TWT.Go();
        }

        private void Go()
        {
            //Timer tmr = new Timer(tmr_Tick, 5, 0, 500);

            Console.CancelKeyPress += delegate
            {
                Console.WriteLine("Exit Detected, killing threads...");
                _tWorker.End();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                // The application terminates directly after executing this delegate.
            };
            StartThread();
            BuildQueue();

            Thread.Sleep(10000);
            //Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        //private void tmr_Tick(object state)
        //{
        //    Console.WriteLine(String.Format("Running Tasks: {0}", _tWorker.RunningTaskCount.ToString()));
        //    Console.WriteLine(String.Format("Waiting Tasks: {0}", _tWorker.WaitingTaskCount.ToString()));
        //    foreach (string s in _tWorker.WaitingTaskNames)
        //    {
        //        Console.WriteLine(s);
        //    }
        //}

        private void StartThread()
        {
            _tWorker = new TaskWorker();
            _tWorker.OnReturn += new TaskWorkerEventHandler(OnReturn);
            _taskThread = new Thread(new ThreadStart(_tWorker.QueueStart));
            _taskThread.IsBackground = true;
            _taskThread.Start();

            // V---- can't figure this out yet, sticking with Thread ----V
            //ThreadPool.QueueUserWorkItem(new WaitCallback(TaskWorker.QueueStart), autoEvent);
            //autoEvent.WaitOne();
        }

        private void BuildQueue()
        {
            Thread.Sleep(1000);
            _tWorker.Add(new Fib(1, 40));
            Thread.Sleep(1000);
            _tWorker.Add(new Fib(1, 45));
            Thread.Sleep(1000);
            _tWorker.Add(new Fib(1, 50));
            Thread.Sleep(1000);
            _tWorker.Add(new Fib(1, 55));
        }

        private void OnReturn(TaskWorkerEventArgs e)
        {
            Console.WriteLine("Task Completion Event Raised from {0}", e.Sender.GetType().ToString());
        }
    }
}