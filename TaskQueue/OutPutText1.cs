using System;
using System.ComponentModel;
namespace TaskWorkerTest
{
    public class OutPutText1 : Task
    {
        public override object Run()
        {
            Console.WriteLine("Output 1");
            return true;
        }
    }
}
