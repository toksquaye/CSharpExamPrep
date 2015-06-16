using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsCallbacks
{
    class Program
    {
        public delegate int Calculate(int x, int y); // defines a delegate with method signature
        public int Add(int x, int y) { return x + y;  }
        public int Multiply(int x, int y) { return x * y; }
        public void UsingDelegates()
        {
            Calculate calc = Add;
            Console.WriteLine(calc(3,4)); //displays 7

            calc = Multiply;
            Console.WriteLine(calc(3,4)); //display 12
        }
        
        void MethodOne()
        {
            Console.WriteLine("MethodONe");
        }

        void MethodTwo()
        {
            Console.WriteLine("MethodTwo");
        }

        public delegate void Del();
        public void MulticastDelegate()
        {
            Del d = MethodOne;
            d += MethodTwo; //add another method to the delegate

            int invCount = d.GetInvocationList().GetLength(0); //get # of methods this delegate will invoke
            Console.WriteLine("invCount : {0}",invCount); //invCount = 2
            d();    //call delegate - which then calls Methodone & MethodTwo 
        }

        public delegate TextWriter CovarianceDel(); //delegate returninng TextWriter Object
        StreamWriter MethodStream() { return null; }
        StringWriter MethodString() { return null; }

        public void Covariance()
        {
            CovarianceDel del;
            del = MethodStream;
            del += MethodString;   //delegate assignment possible cos both StreamWriter & StringWriter inherit from TextWriter
        }

        void DoSomething(TextWriter tw) { }
        public delegate void ContravarianceDel(StreamWriter sw);

        public void Contravariance()
        {
            ContravarianceDel cd = DoSomething; //since Dosomething can work on TextWriter, it can work on StreamWriter
        }

        public void LambdaExpressions()
        {
            Calculate calc = (x, y) =>
                {
                    Console.WriteLine("Lambda expression delegate does addition"); 
                    return x + y;   //use lambda expression to create a delegate!
                    
                };
            Console.WriteLine(calc(3,4)); //displays 7

            calc = (x, y) => x * y;     //lambda expr to create a delegate
            Console.WriteLine(calc(3,4));//display 12
        }

        public void ActionDelegate()
        {
            Action<int, int> calc = (x, y) =>   //calc is name of action
                {
                    Console.WriteLine(x + y);
                };

            calc(5, 6);
        }

        //Events
        public class MyArgs : EventArgs //derive from EventArgs class
        {
            public MyArgs(int value)    //class constructore
            {
                Value = value;
            }
            public int Value { get; set; }

        }

        public class Pub
        {
            public event EventHandler<MyArgs> OnChange = delegate { }; //this assigment makes sure the event is never null
                                                                        //MyArgs is event argument

            public void Raise()
            {
                OnChange(this, new MyArgs(42));
            }
        }

        public void CreateAndRaiseEvent()
        {
            Pub p = new Pub();
            p.OnChange += (sender, e) =>  //subscribes to the event and passes in lambda expression anonymous method to execute once event is raised
                Console.WriteLine("Event Raised: {0}",e.Value);

            p.Raise();  //call method that raises the event
        }

        //Custom event accessor. 
        //Note : If you use regular event syntax, the compiler will generate accessor for you
        public class Pub2
        {
            private event EventHandler<MyArgs> onChange = delegate { };
            public event EventHandler<MyArgs> OnChange
            {
                add
                {
                    lock (onChange) //put a lock to make it thread safe
                    {
                        onChange += value;
                    }
                }

                remove
                {
                    lock (onChange)
                    {
                        onChange -= value;
                    }

                }
            }

            public void Raise()
            {
                onChange(this, new MyArgs(43));
            }
        }

        public void CreateAndRaiseEvent2()
        {
            Pub2 p = new Pub2();
            p.OnChange += (sender, e) =>  //subscribes to the event and passes in lambda expression anonymous method to execute once event is raised
                Console.WriteLine("Event Raised: {0}", e.Value);

            p.Raise();  //call method that raises the event
        }

        //Manually raise events with exception handling
        public class Pub3
        {
            public event EventHandler OnChange = delegate { }; //to avoid having event with no subscribers
            public void Raise()
            {
                var exceptions = new List<Exception>();  //a list of exceptions

                foreach (Delegate handler in OnChange.GetInvocationList()) //for each method to be invoked that have subscribed to the event
                {
                    try
                    {
                        handler.DynamicInvoke(this, EventArgs.Empty); //dynamically invoke method in the handler delegate
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex); // if an exception occurs, add to list of excpetions raised on this event
                    }
                }

                if(exceptions.Any())
                {
                    throw new AggregateException(exceptions);   //if exceptions occured, throw new exception to the caller passing on the list of exceptions
                                                                // encountered
                }
            }

        }

        public void EventswithExceptions()
        {
            Pub3 p = new Pub3();
            p.OnChange += (sender, e) => Console.WriteLine("Subscriber 1 called");

            p.OnChange += (sender, e) => { throw new Exception(); };

            p.OnChange += (sender, e) => Console.WriteLine("Subscriber 3 called");

            try
            {
                p.Raise();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.InnerExceptions.Count);
            }
        }


        static void Main(string[] args)
        {
            Program t = new Program();
            //t.UsingDelegates();
            //t.MulticastDelegate();
            //t.Covariance();
            //t.Contravariance();
            //t.LambdaExpressions();
            //t.ActionDelegate();
            t.CreateAndRaiseEvent();
            //t.CreateAndRaiseEvent2();
            //t.EventswithExceptions();
            Console.ReadLine();
        }
    }
}
