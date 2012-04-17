using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace TaskWorkerTest
{
    public delegate void TaskWorkerEventHandler(TaskWorkerEventArgs args); //Self explainatory

    public class TaskWorkerEventArgs : EventArgs
    {
        #region Variables

        private object _returnObj = "Empty ReturnObj"; //Holds the ReturnObj, instantiated with placeholder string to avoid NullReferenceException

        private object _renderObj = "Empty Sender"; //Holds the SenderObj, instantiated with placeholder string to avoid NullReferenceException

        #endregion Variables

        #region Properties

        public object GetReturn //Returns the ReturnObj
        {
            get
            {
                return _returnObj;
            }
        }

        public object Sender
        {
            get
            {
                return _renderObj;
            }
        } //Returns the SenderObj

        #endregion Properties

        #region Constructors

        public TaskWorkerEventArgs(object returnObj) //Args with no sender
        {
            _returnObj = returnObj; //Get the object to be returned with the event args.
        }

        public TaskWorkerEventArgs(object returnObj, object sender) //Args with sender
        {
            _returnObj = returnObj; //Get the object to be returned with the event args.
            _renderObj = sender;    //Get the sender object to be returned with the event args.
        }

        #endregion Constructors
    }

    public class TaskWorkerExitToken { } //Empty class to serve as a token to close the queue thread (hackish, yes, but it works!)

    public class TaskWorker //The Worker Class
    {
        #region Variables

        private BlockingCollection<object> _taskQueue; //Used to block the infinite queue loop until there's a task object to work on
        private bool _killed = false;

        private List<Task> Tasks { get; set; }

        #endregion Variables

        #region Properties

        public int RunningTaskCount
        {
            get
            {
                return Tasks.Count;
            }
        }

        public int WaitingTaskCount
        {
            get
            {
                return _taskQueue.Count;
            }
        }

        public List<string> WaitingTaskNames
        {
            get
            {
                List<string> temp = new List<string>();
                foreach (object o in _taskQueue)
                {
                    temp.Add(o.ToString());
                }
                return temp;
            }
        }

        #endregion Properties

        #region Constructors

        public TaskWorker()
        {
            _taskQueue = new BlockingCollection<object>(); //Instantiates the collection
            Tasks = new List<Task>();
        }

        #endregion Constructors

        #region Functions

        public void QueueStart() //Method which will be thrown into a background thread
        {
            while (true) //start an infinite loop
            {
                object obj;
                obj = _taskQueue.Take(); //Fish us out the first object in the queue! If empty, will block the thread until not empty

                if (obj == null) //Check null, derp!
                {
                    continue; //if null, re-loop (the null is already removed from the queue by the .Take() method)
                }

                if (obj is TaskWorkerExitToken) //Check for exit token
                {
                    _killed = true;
                    RaiseReturn(obj, obj); //Report exit token to main thread
                    foreach (object o in _taskQueue)
                    {
                        object clearbuffer = _taskQueue.Take();
                    }
                    break; //Throw wrench in loop, KABOOOOOOOOOOOM
                }
                else if (obj is bool) //If bool, just toss it to the main thread,
                {
                    RaiseReturn(obj, obj);
                }
                else if (obj is Task) //If Derrived from Task
                {
                    Task task = (Task)obj; //Typecast the derived object to the base inherited type "Task"
                    //Return(task.Run(), task); //Call the Task.Run() method and return output to main thread! (All classes derived from Task will have this method)
                    Tasks.Add(task);

                    //TODO: figure out a way to do an async fireandforget event without a lot of complicated bullshit
                    // V---- not how to do it (correctly) ----V
                    Thread eventThread = new Thread(delegate() { RaiseReturn(task.Run(), task); }); //Lambda is awesome!
                    eventThread.IsBackground = true;
                    eventThread.Start();
                }
            }
        }

        public void End()
        {
            for (int i = (Tasks.Count - 1); i >= 0; i--)
            {
                object o = Tasks[i];
                if (o is Task)
                {
                    Task t = (Task)o;
                    t.Kill();
                }
            }
            _taskQueue.Add(new TaskWorkerExitToken());
        }

        public void Add(object addObj)
        {
            if (!_killed)
            {
                _taskQueue.Add(addObj);
            }
        }

        public Task GetTask(Guid g)
        {
            foreach (Task t in Tasks)
            {
                if (t.TaskId == g)
                {
                    return t;
                }
            }
            return null;
        }

        #endregion Functions

        #region Events

        public event TaskWorkerEventHandler OnReturn;

        private void RaiseReturn(object returnObj) //Implementation of RaiseReturn(returnObj) event
        {
            TaskWorkerEventArgs EventArgs = new TaskWorkerEventArgs(returnObj);

            OnReturn(EventArgs);
        }

        private void RaiseReturn(object returnObj, object sender) //Overload of RaiseReturn(returnObj), adds sender reference
        {
            TaskWorkerEventArgs EventArgs = new TaskWorkerEventArgs(returnObj, sender);
            if (sender is Task)
            {
                Tasks.Remove((Task)sender);
            }
            OnReturn(EventArgs);
        }

        #endregion Events
    }
}