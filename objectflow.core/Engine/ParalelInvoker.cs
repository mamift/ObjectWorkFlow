﻿using System.Collections.Generic;
using System.Threading;

namespace Rainbow.ObjectFlow.Engine
{
    internal class ParallelInvoker<T> : MethodInvoker<T> where T : class
    {
        public IList<OperationDuplex<T>> RegisteredOperations = new List<OperationDuplex<T>>();
        private Dispatcher<T> _engine;

        public ParallelInvoker(Dispatcher<T> engine)
        {
            _engine = engine;
        }

        public ParallelInvoker()
        {
            _engine = new Dispatcher<T>();
        }

        public override T Execute(T data)
        {
            ManualResetEvent finishedEvent;
            ThreadProxy.ThreadCount = RegisteredOperations.Count;

            using (finishedEvent = new ManualResetEvent(false))
            {
                finishedEvent = ExecuteParallelSequence(data, finishedEvent);

                finishedEvent.WaitOne();
            }

            return data;
        }

        private ManualResetEvent ExecuteParallelSequence(T data, ManualResetEvent finishedEvent)
        {
            for (int i = 0; i < RegisteredOperations.Count; i++)
            {
                finishedEvent = QueueOperation(RegisteredOperations[i], data, finishedEvent);
            }
            return finishedEvent;
        }

        private ManualResetEvent QueueOperation(OperationDuplex<T> function, T data, ManualResetEvent finishedEvent)
        {
            var threadContainer = new ThreadProxy(ref _engine, function, data, ref finishedEvent);
            ThreadPool.QueueUserWorkItem(a => threadContainer.Start());

            return finishedEvent;
        }

        public void Add(OperationDuplex<T> operation)
        {
            RegisteredOperations.Add(operation);
        }

        private class ThreadProxy
        {
            public static int ThreadCount;

            private readonly OperationDuplex<T> _function;
            private readonly T _data;
            private readonly Dispatcher<T> _engine;
            private readonly ManualResetEvent _finishedEvent;

            public ThreadProxy(ref Dispatcher<T> engine, OperationDuplex<T> threadStartFunction, T parameter, ref ManualResetEvent finishedEvent)
            {
                _finishedEvent = finishedEvent;
                _function = threadStartFunction;
                _data = parameter;
                _engine = engine;
            }

            public void Start()
            {
                _engine.Execute(_function, _data);
                if (IsFinalThread())
                {
                    _finishedEvent.Set();
                }
            }

            private static bool IsFinalThread()
            {
                return 0 == Interlocked.Decrement(ref ThreadCount);
            }
        }
    }
}