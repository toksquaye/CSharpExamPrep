using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageProgramFlow
{
    class Program
    {
        private static bool GetY()
        {
            Console.WriteLine("this method will never be called");
            return true;
        }
        static void ShortCircuitOrMain()
        {
            bool x = true;
            bool result = x || GetY(); //because of short circuiting, gety won't get called. x is true that makes the entire expression true, regardless of the
                                        //value of y
        }

        static void NullCoalescingMain()
        {
            int? x = null;
            
            int y = x ?? -1;  //if x is null, y will be given value -1
            Console.WriteLine(y);

            x = 4;
            y = x ?? -20;  
            Console.WriteLine(y);  //since x is not null, y = 4;
        }

        class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        static void CannotChangeForEachIterationVariable()
        {
            var people = new List<Person>
            {
                new Person() {FirstName="John", LastName="Doe"},
                new Person() {FirstName="Jane", LastName="Doe"}
            };

            foreach (Person p in people)
            {
                p.LastName = "Changed";
                //p = new Person(); //cannot assign new value cos p is the iteration variable
            }

        }
        static void Main(string[] args)
        {
            //ShortCircuitOrMain();
            NullCoalescingMain();
            Console.WriteLine("exit main");
            Console.ReadLine();

        }
    }
}
