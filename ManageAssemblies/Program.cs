using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ManageAssemblies
{
    class Program
    {
        class Person
        {

        }

        class Employee : Person { }
        class Customer : Person { }
        class Manager : Employee { }
        static void Main(string[] args)
        {

            int i = 163;
            Console.WriteLine(string.Format("{0} = {1,4} or 0x{2:X}",(char)i,i,i));

            decimal amount = decimal.Parse("$123,456.78",NumberStyles.Currency);
            Console.WriteLine("{0}",amount);

            double value1 = 10;
            float value2 = (float)value1; //narrowing conversion

            int value3 = 10;
            long value4 = value3; //widening conversion

            Employee employee1 = new Employee();
            Person person1 = employee1;
            Person p1 = new Employee(); //an employee object can be represented as a person
            //Manager m1 = (Manager) new Person();
            Person p2 = new Person();
            Employee emp = p2 as Employee;
            string.
            Console.ReadLine();

        }
    }
}
