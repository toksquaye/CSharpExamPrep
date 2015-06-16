using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections_StoreRetrieve
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    
    /**************Inheriting from List<T> to form a custom collection*****************************/

    public class Person2
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public class PeopleCollection : List<Person2>
    {
        public void RemoveByAge(int age)
        {
            for(int index = this.Count -1; index >= 0; index--)
            {
                if (this[index].Age == age)
                    this.RemoveAt(index);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Person2  p in this)
            {
                sb.AppendFormat("{0} {1} is {2}", p.FirstName, p.LastName, p.Age);
            }
            return sb.ToString();
        }
    }
    class Program
    {

        /****************************************Using HashSet**************************************************/
        public void UseHashSet()
        {
            HashSet<int> oddSet = new HashSet<int>();
            HashSet<int> evenSet = new HashSet<int>();

            for (int x = 1; x <=10; x++)
            {
                if (x % 2 == 0)
                    evenSet.Add(x);
                else
                    oddSet.Add(x);
            }

            DisplaySet(oddSet);
            DisplaySet(evenSet);

            oddSet.UnionWith(evenSet);
            DisplaySet(oddSet);
        }


        private void DisplaySet(HashSet<int> set)
        {
            Console.Write("{");
            foreach (int i in set)
                Console.Write(" {0}", i);
            Console.WriteLine(" }");
        }

        static void Main(string[] args)
        {

            //using an array
            int[] arrayofInt = new int[10];

            for (int i = 0; i < arrayofInt.Length; i++)
                arrayofInt[i] = i;

            foreach (int i in arrayofInt)
                Console.Write(i);

            //arrays can be initialized directory
            int[] arrayofInt2 = { 0, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

            //using a two-dimensional array
            string[,] array2d = new string[3, 2] //array of 3 elements, where each element has 2 elements
            { {"one","two"} , {"three","four"}, {"five","six"} };
            Console.WriteLine(array2d[1,1]); //four


            //create a jagged array
            int[][] jaggedArray = 
            {
                new int[] {1,3,5,7,9},
                new int[] {2,4,6},
                new int[] {8, 0}

            };
            Console.WriteLine(jaggedArray[2][1]); //0

            //Using List<T>
            List<string> listOfString = new List<string> { "A", "B", "C", "D", "E" };

            foreach (string c in listOfString)
                Console.Write(c); //ABCDE
            listOfString.Remove("B");
            foreach (string c in listOfString)
                Console.Write(c); //ACDE
            listOfString.Add("F");
            foreach (string c in listOfString)
                Console.Write(c); //ACDEF
            Console.WriteLine(listOfString.Count); //5
            bool hasC = listOfString.Contains("C");
            Console.WriteLine(hasC); //True

            //Using Dictionary<Tkey><TValue>
            Person p1 = new Person { Id = 1, Name = "Name1" };
            Person p2 = new Person { Id = 2, Name = "Name2" };
            Person p3 = new Person { Id = 3, Name = "Name3" };

            var dict = new Dictionary<int, Person> { {p1.Id, p1}, {p2.Id,p2}};
            dict.Add(p3.Id, p3);

            dict[0] = new Person { Id = 0, Name = "TOks" }; // add this to the bottome of the dictionary
            foreach(KeyValuePair<int, Person> v in dict)
            {
                Console.WriteLine("key:{0}, value: {1}",v.Key, v.Value.Name);
            }

            Person result;
            if(!dict.TryGetValue(5,out result))
                Console.WriteLine("No person with a key of 5 can be found");

            //Using HashSet<T>
            Program p = new Program();
            p.UseHashSet();
        
            //Using Queue<T>
            Queue<string> myQueue = new Queue<string>();
            myQueue.Enqueue("Hello");
            myQueue.Enqueue("Out");
            myQueue.Enqueue("there!");

            foreach (string s in myQueue)
                Console.Write("{0} ",s); //Hello Out there!

            //Using Stack<T>
            Stack<string> myStack = new Stack<string>();
            myStack.Push("Hello");
            myStack.Push("Out");
            myStack.Push("there!");

            foreach(string s in myStack)
                Console.Write("{0} ",s);// there! Out Hello

            //Using custom collection
            Person2 p2_1 = new Person2
            {
                FirstName = "Sade",
                LastName = "Baderinwa",
                Age = 42
            };

            Person2 p2_2 = new Person2
            {
                FirstName = "Lola",
                LastName = "Falana",
                Age = 54
            };

            PeopleCollection people = new PeopleCollection { p2_1, p2_2 };
            people.RemoveByAge(54);
            Console.WriteLine(people.Count); // 1

            Console.ReadLine();
        }
    }
}
