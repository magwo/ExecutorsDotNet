using System;
using System.Collections.Generic;
//using UnityEngine;
using System.Threading;

namespace Executors {

    /// <summary>
    /// Simple class for (basic) unit testing of executors.
    /// </summary>
    /// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
    /// <todo>Replace with NUnit tests</todo>
    public class ExecutorTester {

        class MultiplyTask : ICallable<double> {
            int a;
            int b;
            public MultiplyTask(int a, int b) {
                this.a = a;
                this.b = b;
            }

            public double Call() {
                Thread.Sleep(10);
                return (double)a * b;
            }
        }


        class ExceptionThrowingTask : ICallable<int> {
            public int Call() {
                throw new ExecutionException(new Exception("Task thrown exception."));
            }
        }

        bool doLogging;


        public ExecutorTester(bool doLogging)
        {
            this.doLogging = doLogging;
        }

        private void Log(string msg)
        {
            if (doLogging)
            {
                //Debug.Log("ExecutorTester: " + msg);
            }
        }


        public void TestAllExecutors() {
            List<IExecutor> toBeTested = new List<IExecutor>();
            toBeTested.Add(new ImmediateExecutor());
            toBeTested.Add(new SingleThreadExecutor());

            foreach(IExecutor executor in toBeTested) {
                DoBasicTest(executor);
            }

            foreach(IExecutor executor in toBeTested) {
                DoExceptionTest(executor);
            }

            foreach(IExecutor executor in toBeTested) {
                DoShutdownTest(executor);
            }

            DoShutdownWithPendingTasksTest(new SingleThreadExecutor());
            DoShutdownWithPendingTasksTest(new SingleThreadExecutor(ShutdownMode.CancelQueuedTasks));
        }

        



        void DoBasicTest(IExecutor executor) {

            List<Future<double>> futures = new List<Future<double>>();
            List<double> expectedAnswers = new List<double>();

            for(int i = 1; i < 10; i++) {
                futures.Add(executor.Submit(new MultiplyTask(i, i)));
                //futures.Add(executor.Submit<double>(delegate () { return i*i; } ));
                expectedAnswers.Add(i * i);
            }

            for(int i = 0; i < futures.Count; i++) {
                AssertAlmostEqual(futures[i].GetResult(), expectedAnswers[i]);
                AssertTrue(futures[i].IsDone);

                Log("Basic test " + i + " with executor type " + executor.GetType().Name + " passed.");
            }
        }



        void DoExceptionTest(IExecutor executor) {
            List<Future<int>> futures = new List<Future<int>>();

            for(int i = 0; i < 10; i++) {
                futures.Add(executor.Submit(new ExceptionThrowingTask()));
                //futures.Add(executor.Submit<int>(
                //  delegate () { throw new ExecutionException(new Exception("Task thrown exception.")); }));
            }

            for(int i = 0; i < futures.Count; i++) {
                try {
                    futures[i].GetResult();
                    // Not good
                    throw new Exception("Shouldn't be here...");
                } catch(ExecutionException) {
                    // All good
                }

                AssertTrue(futures[i].IsDone);

                Log("Exception test " + i + " with executor type " + executor.GetType().Name + " passed.");
            }
        }

        void DoShutdownTest(IExecutor executor) {
            executor.Shutdown();

            for(int i = 0; i < 20; i++) {
                if(executor.IsShutdown()) {
                    Log("Shutdown test with executor type " + executor.GetType().Name + " passed.");
                    return;
                } else {
                    Thread.Sleep(100);
                }
            }
            throw new Exception("Executor " + executor.GetType().Name + " failed to shutdown in a timely manner.");
        }


        void DoShutdownWithPendingTasksTest(IExecutor executor) {

            List<Future<double>> futures = new List<Future<double>>();
            for(int i = 0; i < 20; i++) {
                futures.Add(executor.Submit(new MultiplyTask(i, i)));
            }

            Thread.Sleep(100);
            DoShutdownTest(executor);
            int queueSize = executor.GetQueueSize();
            Log("Items in queue after shutdown: " + queueSize);
            int successCount = 0;
            int cancelledCount = 0;
            for(int i = 0; i < futures.Count; i++) {
                try {
                    if(!futures[i].IsDone) {
                        throw new Exception("All queued tasks should have been set to done during shutdown.");
                    }
                    double result = futures[i].GetResult();
                    successCount++;
                } catch(ExecutionException) {
                    cancelledCount++;
                }
            }
            AssertEqual(queueSize, cancelledCount);
            Log("Shutdown with pending tasks: " + successCount + " completed, " + cancelledCount + " cancelled.");
        }


        void AssertTrue(bool condition) {
            if(!condition) {
                throw new Exception("Condition not true.");
            }
        }

        void AssertEqual(int i1, int i2) {
            if(i1 != i2) {
                throw new Exception("Numbers are not equal: " + i1 + " , " + i2);
            }
        }

        void AssertAlmostEqual(double d1, double d2) {
            if(System.Math.Abs(d1 - d2) > 0.0000001f) {
                throw new Exception("Numbers are not equal: " + d1 + " , " + d2);
            }
        }

    }
}

