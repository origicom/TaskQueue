using System;
using System.ComponentModel;

namespace TaskWorkerTest
{
    public class Fib : Task
    {
        private int _fibStart;
        private int _fibStop;

        public Fib(int start, int stop)
        {
            _fibStart = start;
            _fibStop = stop;
        }

        public override object Run()
        {
            for (int n = _fibStart; n < _fibStop && !(_killTask); n++)
            {
                Console.WriteLine(fib(n).ToString());
            }
            if (_killTask)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static int fib(int x)
        {
            if (x <= 1)
            {
                return x;
            }
            else
            {
                return fib(x - 1) + fib(x - 2);
            }
        }
    }
}