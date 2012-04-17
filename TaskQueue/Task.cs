using System;
using System.Collections;
using System.ComponentModel;

namespace TaskWorkerTest
{
    public abstract class Task
    {
        private Guid _taskID = Guid.NewGuid();
        protected bool _killTask = false;

        public virtual Guid TaskId
        {
            get
            {
                return _taskID;
            }
        }

        public abstract object Run();

        public virtual void Kill()
        {
            _killTask = true;
        }
    }
}