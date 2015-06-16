using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThread_Async
{
    
    class Program
    {

        //simple method that writes to the control 10 time. It will be called by a thread
        public static void ThreadMethod()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("ThreadProc: {0}", i);
                Thread.Sleep(0);    //this signals to Windows tbat thread is finished and control can be switched to another thread
                //instead of waiitng the entire time-slice that was given to the thread
            }

        }

        static void ThreadMain()
        {
            Thread t = new Thread(new ThreadStart(ThreadMethod)); //create thread object. pass in the method to be executed by the thread
            t.Start();  //start the thread execution

            for(int i=0; i<4; i++)
            {
                Console.WriteLine("Main:Do some work.");
                Thread.Sleep(0);
            }
            t.Join(); //waits for thread t to finish before continuation execution of main thread.
            Console.ReadKey();
        }

        public static void RunBackgroundMethod()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("ThreadProc: {0}", i);
                Thread.Sleep(1000);    //sleep for 1 second
            }

        }
        static void RunBackgroundMain()
        {
            Thread t = new Thread(new ThreadStart(RunBackgroundMethod)); //create thread object. pass in the method to be executed by the thread
            t.IsBackground = true; //this forces t thread to exit as soon as main thread finishes
            t.Start();  //start the thread execution
            Console.ReadKey();
        }

        //you can only pass use parameter type 'object' when passing value to a thread method using ParameterizedThreadStart
        static void ParamThreadMethod(object value)
        {
            for(int i=0; i<(int)value; i++)
            {
                Console.WriteLine("ParamThreadMethod Loop : {0}",i);
            }
        }
        public static void ParamThreadMain()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ParamThreadMethod));
            t.Start(5); //this passes the parameter into the method that the thread is execution
            Console.ReadKey();
        }

        static void StopThreadUsingSharedVariableMain()
        {
            bool stopped = false;
            Thread t = new Thread(() =>             //use lambda expression to create method that thread will run
            {
                while (!stopped)                    //can only stop this thread externally when stopped is set to true
                {
                    Console.WriteLine("Running......");
                    Thread.Sleep(1000); 
                }

            });
            t.Start();
            Console.WriteLine("Press any key to stop loop");
            Console.ReadKey();
            stopped = true;
            t.Join();//wait here till thread finishes
        }

        [ThreadStatic] //this attribute gives each thread its own private copy of _field
        //this also allows you to only access the field. the thread don't get to initiaize it.
        //if you need to initialize field for each thread, use ThreadLocal
        public static int _field;

        static void UsingThreadStaticMain()
        {
            new Thread(() =>
            {
                for(int i=0; i<10; i++)
                {
                    _field++;
                    Console.WriteLine("First thread uses _field: {0}",_field);
                }

            }).Start(); //using lambda expression, defines a thread method and starts it - all in 1 command!

            new Thread(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    _field++;
                    Console.WriteLine("Second thread uses _field: {0}", _field);
                }
            }).Start();
        }

        public static ThreadLocal<int> _field2 =        //creates ThreadLocal object that's initialized using a lambda expression function that return thread id
            new ThreadLocal<int>(() =>
            {
                return Thread.CurrentThread.ManagedThreadId;
            });

        static void UsingThreadLocalMain()
        {
            new Thread(() =>
            {
                for(int i=0; i <_field2.Value; i++)
                {
                    Console.WriteLine("Thread A : {0}",i);
                }
            }).Start();

            new Thread(() =>
            {
                for(int i=0; i< _field2.Value; i++)
                {
                    Console.WriteLine("Thread B:{0}",i);
                }
            }).Start();

        }

        //method used as WaitCallback must have parameter of object - which contains stateinfo
        //threadpool threads run as background threads
        public static void ThreadPoolMethod(object stateinfo)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("ThreadPoolMethod working....");
            }
        }

        static void ThreadPoolMain()
        {
            //s is WaitCallBack parameter
            ThreadPool.QueueUserWorkItem((s) =>
                {
                    Console.WriteLine("Working on a thread from threadpool");
                });

            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolMethod));
            Console.WriteLine("ThreadPoolMain ends");

        }

        static void UsingTasksMain()
        {
            Task t = Task.Run(() =>
            {
                for (int i=0; i<100; i++)
                {
                    Console.Write("X");
                }
            });
            t.Wait();   //waits till task is finished before exiting

            
        }

        static void TasksReturningValueMain()
        {
            Task<int> t = Task.Run(() => //task returs an int
             {
                 return 42;
             }).ContinueWith( (i) =>    //i is the output from the initial task. It's passed on as a parameter here
             {
                 return i.Result * 2;
             }); //only run continuation method if Task ran to completion
            Console.WriteLine(t.Result);
        }

        static void TaskwithContinuations()
        {
            Task<int> t = Task.Run(() =>
                {
                    return 32;
                });

            t.ContinueWith((i) => 
            {
                return i.Result + 10;
            },TaskContinuationOptions.OnlyOnCanceled);//only run this if t was canceled

            t.ContinueWith((i) =>
            {
                return i.Result * 2;
            },TaskContinuationOptions.OnlyOnFaulted);//only run this if t faulted

            var completedtask = t.ContinueWith((i) => 
            {
                Console.WriteLine("Completed task");
            },TaskContinuationOptions.OnlyOnRanToCompletion);

            completedtask.Wait(); //wait for completedtask to finish before exiting app

        }

        static void ParentChildTasksMain()
        {
            Task<Int32[]> parent = Task.Run(() =>
                {
                    var results = new Int32[3];

                    new Task(() => results[0] = 0, TaskCreationOptions.AttachedToParent).Start(); //initialize new task and starts it.
                    new Task(() => results[1] = 111, TaskCreationOptions.AttachedToParent).Start();//first parameter is value being stored in the array
                    new Task(() => results[2] = 2, TaskCreationOptions.AttachedToParent).Start();
                    return results;
                });

            //continuewith parameter contains the continuation action
            var finalTask = parent.ContinueWith(
                parentTask => {
                foreach(int i in parentTask.Result)
                    Console.WriteLine(i);
                });

            finalTask.Wait();
            
        }

        static void ParentChildTasksMain2()
        {
            Task<string[]> parent = Task.Run(() =>
            {
                var results = new string[3];

                new Task(() => results[0] = "Olubimpe", TaskCreationOptions.AttachedToParent).Start(); //initialize new task and starts it.
                new Task(() => results[1] = "Olatokunbo", TaskCreationOptions.AttachedToParent).Start();//first parameter is value being stored in the array
                new Task(() => results[2] = "Quaye", TaskCreationOptions.AttachedToParent).Start();
                return results;
            });

            //continuewith parameter contains the continuation action
            var finalTask = parent.ContinueWith(
                parentTask =>
                {
                    foreach (string i in parentTask.Result)
                        Console.WriteLine(i);
                });

            finalTask.Wait();

        }

        static void UsingTaskFactoryMain()
        {
            Task<Int32[]> parent = Task.Run(() => //this task returns an array of integers
                {
                    var results= new Int32[3];

                    TaskFactory tf = new TaskFactory(TaskCreationOptions.AttachedToParent, TaskContinuationOptions.ExecuteSynchronously);

                    tf.StartNew( () => results[0] = 0 ); //creates and starts a task. The task is to assign 0 to results[0]
                    tf.StartNew( () => results[1] = 1 );
                    tf.StartNew( () => results[2] = 2 );

                    return results;
                });

            var finalTask = parent.ContinueWith(parentTask =>
            {
                foreach (int i in parentTask.Result)
                    Console.WriteLine(i);
            });

            finalTask.Wait();
        }

        static void TaskWaitAllAny()
        {
            Task[] tasks = new Task[3];         //an array of 3 Tasks

            tasks[0] = Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("Task 1");
                    return 1;   
                });

            tasks[1] = Task.Run(() =>
                {
                    Thread.Sleep(2000);
                    Console.WriteLine("Task 2");
                    return 2;
                });

            tasks[2] = Task.Run(() =>
                {
                    Thread.Sleep(3000);
                    Console.WriteLine("Task 3");
                    return 3;
                });

            //Task.WaitAll(tasks); //wait for all tasks to finish before exiting method
            Task.WaitAny(tasks); //wait until any of the task is complete
        }

        static void TaskWaitAny()
        {
            Task<int>[] tasks = new Task<int>[3]; //an array of tasks that return an integer
            tasks[0] = Task.Run(() => { Thread.Sleep(5000); return 1; });
            tasks[1] = Task.Run(() => { Thread.Sleep(3000); return 2; });
            tasks[2] = Task.Run(() => { Thread.Sleep(2000); return 3; });

            while (tasks.Length > 0)
            {
                int i = Task.WaitAny(tasks);
                Console.WriteLine("i:{0}",i);
                Task<int> completedTask = tasks[i];
                Console.WriteLine("Completed Task : {0}", completedTask.Result);

                var temp = tasks.ToList();
                temp.RemoveAt(i);
                tasks = temp.ToArray();
            }
        }

        static void ParallelForForeach()
        {
            Parallel.For(1, 10, i =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("Thread For{0}", i);
                });

            var numbers = Enumerable.Range(1, 10);
            Parallel.ForEach(numbers, i => 
            {
                Thread.Sleep(1000);
                Console.WriteLine("Thread Foreach {0}",i);
            });
        }

        static void UsingParallelBreakMain()
        {
            ParallelLoopResult result = Parallel.
                For(0, 10, (int i, ParallelLoopState loopState) =>
                    {
                        if (i == 5)
                        {
                            Console.WriteLine("Breaking Loop");
                            loopState.Break();
                        }
                        Console.WriteLine("Sequence {0}",i);
                        return;
                    });
        }

        public static async Task<string> DownloadContent()
        {
            using (HttpClient client = new HttpClient())
            {
                //string result = await client.GetStringAsync("http://www.microsoft.com");
                //return result;

                //OR

                Task<string> getTaskgoing = client.GetStringAsync("http://www.microsoft.com");
                Console.WriteLine("Do some independent work which fetching the info.");
                string result = await getTaskgoing; //now wait here and yield control back to calling thread
                Console.WriteLine("task is back :*********************************************");
                return result;
            }
        }

        static void AsyncAwaitMain()
        {
            //string result = DownloadContent().Result;   //whenever a task is being returned, to access the data, use the .result field.
            Task<string> callAsynMethod = DownloadContent();//doing it this way allows calling thread to do other work while downloadcontent is working.
            Console.WriteLine("This is after the call to asyn method - but before using result of async method");
            string result = callAsynMethod.Result;
            Console.WriteLine(result);
        }

        public Task SleepAsyncA(int millisecondsTimeout)
        {
            return Task.Run( () => Thread.Sleep(millisecondsTimeout));
        }


        public Task SleepAsyncB(int millisecondTimeout)
        {
            TaskCompletionSource<bool> tcs = null; //this class allows a tak to run asynchronously only under certain conditions
            //uses anonymous function to define the callback function
            var t = new Timer(delegate { tcs.TrySetResult(true); },null,-1, -1);
            tcs = new TaskCompletionSource<bool>(t);
            t.Change(millisecondTimeout, -1);
            return tcs.Task;
        }
        
        static void UseAsParalledMain()
        {
            var nums = Enumerable.Range(1, 20);
            
            //this uses lambda expression to retrieve every integer divisible by 2
            var parallelResult = nums.AsParallel().Where(i => i % 2 == 0).ToArray(); //CLR determines if query should be made into a parallel one
            
            //this insists that query should always execute in parallel.
            var parallelResult2 = nums.AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism).Where(i => i % 2 == 0).ToArray();
            
            //in addition, this specify that maximum number of parallel tasks to run while executing query
            var parallelResult3 = nums.AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism).WithDegreeOfParallelism(10).Where(i => i % 2 == 0).ToArray();
            
            //since AsParalled doesn't guarantee output to be in order, this buffers the result, sorts it before putting in array
            var parallelResults4 = nums.AsParallel().AsOrdered().Where(i => i % 2 == 0).ToArray();
            
            foreach(var i in parallelResult) //this output sometime have output out of order
            {
                Console.WriteLine(i);
            }

            foreach(var i in parallelResults4) //this always has output in order because of .AsOrdered
            {
                Console.WriteLine(i);

            }

            
            //this ensures that processing is done in parallel, but also in sequential order
            //note that for AsSequential to give desired output, AsOrdered needs to be used as well.
            var parallelResults5 = nums.AsParallel().AsOrdered().Where(i => i % 2 == 0).AsSequential();
            foreach(int i in parallelResults5.Take(5)) //take 5 contiguous elements from the start of a sequence
                Console.WriteLine(i);

            //use of forall and foreach
            //foreach waits for all the results before it starts processing, but forall doesn't. - hence the little pause before the foreach output

            var nums2 = Enumerable.Range(1, 10);
            var parallelResults6 = nums2.AsParallel().Where(i => i % 2 == 0);
            parallelResults6.ForAll(e => Console.WriteLine("ForAll Paralled : {0}", e)); 
            Console.WriteLine("here");
            Parallel.ForEach(parallelResults6, i => Console.WriteLine("Foreach Parallel : {0}", i));
            
        }

        public static bool IsEven(int i)
        {
            if (i % 10 == 0) //if number divisible by 10, throw an exception
                throw new ArgumentException("i");
            return (i % 2 == 0);    //return true if divisible by 2, else false
        }

        static void catchAggregateException()
        {
            var numbers = Enumerable.Range(0, 20);
            try 
            {
                var parallelResult = numbers.AsParallel().Where(i => IsEven(i));
                parallelResult.ForAll(e => Console.WriteLine(e));
            }
            catch(AggregateException e) //aggregateexception exposes a list of all exceptions that happened during parallel execution
            {
                Console.WriteLine("There were {0} exceptions raised",e.InnerExceptions.Count);
            }
        }

        static void BlockingCollectionMain()//add example using complete adding
        {
            BlockingCollection<string> col = new BlockingCollection<string>();
            Task read = Task.Run(() =>
                {
                    while (true)
                    {
                        Console.WriteLine(col.Take());  //take an item from BlockingCollection col and write onto console
                    }
                });

            Task write = Task.Run(() =>
                {
                    while (true)
                    {
                        string s = Console.ReadLine(); //read input from console
                        if (string.IsNullOrWhiteSpace(s)) break; //if white space read, break out of infinite loop
                        col.Add(s); //add input from console onto blockingcollection col
                    }
                });

            write.Wait(); //wait for write to finish executing before proceeding in the method
        }

        static void GCEBlockingCollectionMain() //not working
        {
            BlockingCollection<string> col = new BlockingCollection<string>();
            Task read = Task.Run( () => 
                {
                    foreach(string v in col.GetConsumingEnumerable())
                        Console.WriteLine(v);
                });

            Task write = Task.Run(() =>
                {
                    foreach(string v in col.GetConsumingEnumerable())
                    {
                        string s = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(s)) break;
                        col.Add(s);
                    }
                });

            write.Wait();
        }

        static void ConcurrentBagMain()
        {
            ConcurrentBag<int> bag = new ConcurrentBag<int>();
            bag.Add(42);
            bag.Add(41);

            int result;
            if (bag.TryTake(out result)) //removes and returns the object
                Console.WriteLine(result);

            if(bag.TryPeek(out result)) //returns object without removing it
                Console.WriteLine("next item {0}. Items in bag {1}",result,bag.Count); //1 element left in bag

            Task.Run(() =>
                {
                    bag.Add(53);
                    Thread.Sleep(1000);
                    bag.Add(21);
                });

            Task.Run(() =>
                {
                    foreach (int i in bag)
                        Console.WriteLine(i); //didn't display 21 because it wasn't added yet at the start of the iteration
                }).Wait();
        }

        static void ConcurrentStackQueueMain()
        {
            ConcurrentStack<int> stack = new ConcurrentStack<int>();
            stack.Push(43);

            int result;
            if(stack.TryPop(out result))
                Console.WriteLine("trypop : {0} stack count:{1}",result,stack.Count);//pops if there is somthing to pop. if empty

            stack.PushRange(new int[] {1, 2, 3}); //push an integer array

            int[] values = new int[2];
            stack.TryPopRange(values);  //pop 2 values into the array

            foreach(int i in values)
                Console.WriteLine(i);//this prints last to values entered, since stack is last in first out


            ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
            queue.Enqueue(34);
            if(queue.TryDequeue( out result)) //must include word out to denote that value is an output of the TryDequeue function
                Console.WriteLine("Just dequeued {0}",result);
        }

        static void ConcurrentDictionaryMain()
        {
            var dict = new ConcurrentDictionary<string,int>(); //dictionary takes two parameters : key, value pair
            if (dict.TryAdd("k1", 42))
                Console.WriteLine("Added 42");

            if(dict.TryUpdate("k1",21,42))
                Console.WriteLine("42 updated to 21");

            dict["k1"] = 442;   //overwrite unconditionaly

            int r1 = dict.AddOrUpdate("k1", 3, (s, i) => i * 2);//if k1 is not there, add it with value 3. 
                                                                //if it's there, get value of lambda function : i is existing key value. new value = i * 2
            int r2 = dict.GetOrAdd("k2", 4);
        }
        static void Main(string[] args)
        {
            //ThreadMain();
            //RunBackgroundMain();
            //ParamThreadMain();
            //StopThreadUsingSharedVariableMain();
            //UsingThreadStaticMain();
            //UsingThreadLocalMain();
            //ThreadPoolMain();
            //UsingTasksMain();
            //TasksReturningValueMain();
            //TaskwithContinuations();
            //ParentChildTasksMain(); //when I run this, the values aren't always there in the console writeline
            //ParentChildTasksMain2();
            //UsingTaskFactoryMain();
            //TaskWaitAllAny();
            //TaskWaitAny();
            //ParallelForForeach();
            //UsingParallelBreakMain();
            //AsyncAwaitMain();
            //UseAsParalledMain();
            //catchAggregateException();
            //BlockingCollectionMain();
            //GCEBlockingCollectionMain();
            //ConcurrentBagMain();
            //ConcurrentStackQueueMain();
            ConcurrentDictionaryMain();
            Console.WriteLine("exited main program");
            Console.ReadKey();
        }


    }
}
