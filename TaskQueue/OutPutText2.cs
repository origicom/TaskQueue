using System;
using System.ComponentModel;

namespace TaskWorkerTest
{
    public class OutPutText2 : Task
    {
        public override object Run()
        {
            Console.WriteLine("Output 2");
            return true;
        }
    }
}
