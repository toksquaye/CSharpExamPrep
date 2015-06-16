using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncResourcesMultiThread
{
    class Program
    {
        static void UseLockMain()
        {
            int n = 0;
            object _lock = new object(); //lock should always be reference type
            var up = Task.Run(() =>
                {
                    for (int i = 0; i < 1000; i++)
                        lock (_lock) //lock this variable while it's been updated. any other thread will need to wait until this operation is complete to access it.
                            n++; //not atomic. first grabs value of n, then increments, then puts it back. hence the need for a lock
                });

            for (int i = 0; i < 1000; i++)
                lock (_lock)
                    n--;
            up.Wait();
            Console.WriteLine(n); //with the lock, n will always be 0. without the lock, no guarantee of always getting same answer
            
        }

        private static volatile int _flag = 0; //volatile keyword forces correct order of the code. without it,compilter can switch order of code to optimizate
                                                //causing inconsistent output
        private static int _value = 0;

        public static void Thread1()
        {
            _value = 5;
            _flag = 1;
        }

        public static void Thread2()
        {
            if (_flag == 1)
                Console.WriteLine(_value);
        }
    
        static void UseVolatileMain()
        {
            Task t = Task.Run(() => Thread1());
            Task t1 = Task.Run( () => Thread2());
            
            Task.WaitAll(t, t1);

        }
    
        static void UseInterlocked()
        {
            int n = 0;
            var up = Task.Run(() =>
                {
                    for (int i = 0; i < 1000; i++)
                        Interlocked.Increment(ref n);    //guarantees atomic increment                
                });

            for (int i = 0; i < 1000; i++)
                Interlocked.Decrement(ref n);

            up.Wait();
            Console.WriteLine(n);

            int isinUse = 5;
            if(Interlocked.Exchange(ref isinUse, 1) == 5) //extract original value of isinUse, return it, change its value to 1 - all in atomic fashion
            {
                Console.WriteLine("The result should be 6. Result : {0}",isinUse + 5);
            }


            Interlocked.CompareExchange(ref isinUse, 11, 1); //isinUse is compared to 1. If equal, change to 11
            Console.WriteLine("isinUse : {0}",isinUse);  //value is 11
            Interlocked.CompareExchange(ref isinUse, 111, 1); //isinUse is compared to 1. If not equal, nothing changes
            Console.WriteLine("isinUse : {0}", isinUse);   //value is 11
            
        }

        static void UseCancellationTokenMain()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();    //this signals to a token that it should be cancelled
            CancellationToken token = cancellationTokenSource.Token;    //this is the token from the previously defined tokensource object

            Task task = Task.Run(() => //this task runs a loop waiting for cancel token to set
                {
                    while(!token.IsCancellationRequested)
                    {
                        Console.WriteLine("*");
                        Thread.Sleep(1000);
                    }
                },token); //cancellation token passed into the Task

            Console.WriteLine("Press enter to stop the task");
            Task.Run(() =>      //this task sets the cancel token once user presses enter. making this into a task allows the main thread to continue 
                                //while this waits on user input.
                {
                    Console.ReadLine();
                    cancellationTokenSource.Cancel(); //this sets token.iscancellationRequested to true - signalling to the task to cancel itself
                });
        }
        

        static void OperationCanceledExceptionMain()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;


            Task task = Task.Run(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        Console.Write("*");
                        Thread.Sleep(1000);
                    }
                    token.ThrowIfCancellationRequested();  //throw an exception if this token has had a cancellation requested
                });

            try
            {
                Console.WriteLine("Press enter to end task");
                Console.ReadLine();
                cancellationTokenSource.Cancel();
                task.Wait(); //wait here for task to complete so you can catch the exception.  without this line, mainthread continues and doesn't catch exception
            }
            catch(AggregateException e)
            {
                Console.WriteLine("AggregateException: {0}",e.InnerExceptions[0].Message); //InnerExceptions is an array of exceptions that occured during parallel execution
            }
            
        }

        static void CanceledExceptionContinuationMain() //not working
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;


            Task task = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Console.Write("^");
                    Thread.Sleep(1000);
                }
                throw new OperationCanceledException();

            }, token).ContinueWith((t) => //t is the task.
            {
                t.Exception.Handle((e) =>
                    {
                        Console.WriteLine(e.InnerException.Message);
                        return true;
                    }); //handle the exception from previous task. takes it from aggregateexception
                Console.WriteLine("You have cancelled the task");
            },TaskContinuationOptions.OnlyOnCanceled);

            Console.WriteLine("Press enter to end task");
            Console.ReadLine();
            cancellationTokenSource.Cancel();
            task.Wait(); //wait here for task to complete so you can catch the exception.  without this line, mainthread continues and doesn't catch exception

        }

        static void WaitAnyMain()
        {
            Task longrunning = Task.Run( () => Thread.Sleep(100000));

            int index = Task.WaitAny(new[] { longrunning }, 1000); //1st param is an array of tasks. only 1 here.

            if (index == -1) //index is -1 if after 1000millisecond, no task has completed yet.
                Console.WriteLine("Task timed out");
        }
        static void Main(string[] args)
        {
           // UseLockMain();
           // UseVolatileMain();
            //UseInterlocked();
           // UseCancellationTokenMain();
            //OperationCanceledExceptionMain();
            //CanceledExceptionContinuationMain();
            WaitAnyMain();
            Console.WriteLine("Exiting main thread");
            Console.ReadLine();
        }
    }
}
