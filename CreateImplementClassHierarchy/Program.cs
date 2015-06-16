using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateImplementClassHierarchy
{
    class Program
    {
        /************************************
         * Creating and Implementing an interface
         * ****************************************/

        interface IExample
        {
            string GetResult();
            int Value { get; set; }
            event EventHandler ResultRetrieved;
            int this[string index] { get; set; }
        }

        class ExampleImplementation : IExample
        {
            public string GetResult()
            {
                return "result";
            }

            public int Value { get; set; }

            public event EventHandler CalculationPerformed;
            public event EventHandler ResultRetrieved;

            public int this[string index]
            {
                get { return 42; }
                set { }
            }
        }

        /****************Adding a set accessor to an implemented interface property********************************/
        interface IReadOnlyInterface
        {
            int Value { get; }
        }

        struct ReadAndWriteImplementation : IReadOnlyInterface
        {
            public int Value { get; set; }
        }

        /*******Creating an interface with a generic type parameter**********************************/
        interface IRepository<T>
        {
            T FindbyID(int id);
            IEnumerable<T> All();
        }

        public class Product
        {
            public void sayIt()
            {
                Console.WriteLine("This is a Product");
            }
        }
        class ImplementIRepositoryProduct : IRepository<Product> //Implement IRepository with type product
        {
            List<Product> allProducts;

            public ImplementIRepositoryProduct()
            {
                allProducts = new List<Product>();
                Product j;
                for (int i = 0; i < 10; i++)
                {
                    j = new Product();
                    allProducts.Add(j);
                }
            }
            public Product FindbyID(int id)
            {
                return allProducts.ElementAt(id);
            }

            public IEnumerable<Product> All()
            {

                return allProducts;
            }
        }

        /***************Instantiating a concrete type that implements an interface *********/
        interface IAnimal
        {
            void Move();
        }

        class Dog : IAnimal
        {
            public void Move() { Console.WriteLine("Move"); }
            public void Bark() { Console.WriteLine("Bark"); }
            public void MoveAnimal(IAnimal animal)
            {
                animal.Move();
            }
        }

        /*********************Creating a base class ********************************/
        interface IEntity
        {
            int Id { get; }
        }

        class ImplementIEntity : IEntity
        {
            public int Id { get; set; }
            public string ValString { get; set; }
        }
        class Repository<T> where T : IEntity  //the type has to implement IEntity interface
        {
            protected IEnumerable<T> _elements;

            public Repository(IEnumerable<T> elements)
            {
                _elements = elements;
            }


            public T FindByID(int id)
            {
                return _elements.SingleOrDefault(e => e.Id == id);
            }
        }

        /*******************Inheriting from a base class **********************************************/
        class Order : IEntity
        {
            public int Id { get; private set; }
            public decimal amount { get; set; }
            public Order(int id, decimal Amount)
            {
                Id = id;
                this.amount = Amount;
            }
        }

        class OrderRepository : Repository<Order>
        {
            public OrderRepository(IEnumerable<Order> orders)
                : base(orders) { }  //calls base class constructor

            public IEnumerable<Order> FilterOrdersOnAmount(decimal amount)
            {
                List<Order> result = new List<Order>();
                foreach (Order order in _elements)
                {
                    if (order.amount.Equals(amount))
                        result.Add(order);
                }
                return result;
            }
        }

        /******************Overriding a virtual method*******************************/
        class Base
        {
            public virtual void Execute()
            { Console.WriteLine("Execute"); }
        }

        class Derived : Base
        {
            //internal override void Execute() //wont work - cant change access modifier when overriding!
            public override void Execute()
            {
                /*Log("Before executing");
                base.Execute();
                Log("After executing");*/
                Console.WriteLine("Derived Execute");

            }

            private void Log(string message)
            {
                Console.WriteLine(message);
            }
        }

        /*******************************Hiding a method with the new keyword*************************/
        class Base1
        {
            public void Execute() { Console.WriteLine("Base.Execute"); }
        }

        class Derived1 : Base1
        {
            public new void Execute() { Console.WriteLine("Derived.Execute"); }
        }

        /******************************Creating an abstract class********************************/
        abstract class Base2
        {
            public virtual void MethodWithImplementation()
            {
                Console.WriteLine("MethodWithImplementation");
            }

            public abstract void AbstractMethod();
        }

        class Derived2 : Base2
        {
            public override void AbstractMethod()
            {
                Console.WriteLine("Implemented Abstract Method");
            }
        }

        /********************Rectangle class with area calculation**********************/
        class Rectangle
        {
            public virtual int Height { get; set; }
            public virtual int Width { get; set; }

            public Rectangle(int width, int height)
            {
                Width = width;
                Height = height;
            }

            public int Area // this is not a method. Its a field! or property!
            {
                get { return Height * Width; }
            }

        }

        /*********************A Square that inherits from Rectangle*****************************/
        private class Square : Rectangle
        {
            public Square(int size)
                : base(size, size)
            { }

            public override int Width
            {
                get
                {
                    return base.Width;
                }
                set
                {
                    base.Width = value;
                    base.Height = value;
                }
            }

            public override int Height
            {
                get
                {
                    return base.Height;
                }
                set
                {
                    base.Height = value;
                    base.Width = value;
                }
            }
        }

        /*******************************Implementing the IComparable interface*******************/
        class Order2 : IComparable
        {
            public DateTime Created { get; set; }
            public int CompareTo(object obj)
            {
                if (obj == null) return 1;

                Order2 o = obj as Order2; //this is a cast

                if (o == null)
                {
                    throw new ArgumentException("Object is not an Order2");
                }

                return this.Created.CompareTo(o.Created);
            }
        }
        /***********Implementing IEnumerable<T> on a custom type**********************************************/
        class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Person(string firstname, string lastname)
            {
                FirstName = firstname;
                LastName = lastname;
            }

            public override string ToString()
            {
                return FirstName + " " + LastName;
            }
        }

        /*class People : IEnumerable<Person>
        {
            Person[] people;

            public People(Person[] people)
            {
                this.people = people;
            }

            public IEnumerator<Person> GetEnumerator()
            {
                for (int index = 0; index < people.Length; index++)
                {
                    yield return people[index]; //yield lets you keep track of your current location  in the collection
                }
            }

            IEnumerator<Person> IEnumerable<Person>.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /*****************************MAIN********************************************/
        static void Main(string[] args)
        {
            IReadOnlyInterface t1 = new ReadAndWriteImplementation();
            //t1.Value = 24; // this won't work since value is readonly for the interface
            var t2 = t1.Value; // this works since its a read

            ReadAndWriteImplementation t3 = new ReadAndWriteImplementation();
            t3.Value = 24; //this works since the class implements both get & set.

            //ReadAndWriteImplementation t4 = new IReadOnlyInterface(); //impossible - cannot create an instance of an interface.

            //create object with class that implements generic interface IRepository
            ImplementIRepositoryProduct t5 = new ImplementIRepositoryProduct();
            foreach (Product j in t5.All())
            {
                j.sayIt();
            }

            //Instatiating concrete type that implements an interface
            IAnimal animal = new Dog();
            animal.Move();
            //animal.Bark(); //not possible cos Bark isn't part of the interface.

            ((Dog)animal).Bark();   //now it's possible with a cast!

            Dog dog = new Dog();
            Dog anotherdog = new Dog();
            dog.MoveAnimal(anotherdog);

            //creating a base class
            ImplementIEntity ii1 = new ImplementIEntity();
            ii1.Id = 0;
            ii1.ValString = "Olubimpe";
            ImplementIEntity ii2 = new ImplementIEntity();
            ii2.Id = 1;
            ii2.ValString = "Olatokunbo";
            List<ImplementIEntity> ls1 = new List<ImplementIEntity>();
            ls1.Add(ii1);
            ls1.Add(ii2);

            Repository<ImplementIEntity> rs1 = new Repository<ImplementIEntity>(ls1);
            var answer = rs1.FindByID(1);

            //inheriting from a base class
            Order o1 = new Order(0, 23.45M);
            Order o2 = new Order(1, 34.3M);
            Order o3 = new Order(2, 23.45M);
            List<Order> lo1 = new List<Order>();
            lo1.Add(o1);
            lo1.Add(o2);
            lo1.Add(o3);
            OrderRepository or1 = new OrderRepository(lo1);
            var ans2 = or1.FilterOrdersOnAmount(23.45M);
            var ans3 = or1.FindByID(1);

            //overriding virtual method
            Base b = new Base();
            b.Execute(); //output - Execute
            b = new Derived();
            b.Execute();  //output - Derived Execute
            Derived d1 = new Derived();
            d1.Execute();  // output - Derived Execute

            //overriding new method
            Base1 b1 = new Base1();
            b1.Execute();  //output Base.Execute
            b1 = new Derived1();
            b1.Execute(); //output Base.Execute
            Derived1 d2 = new Derived1();
            d2.Execute(); // Derived execute

            //Base2 b2 = new Base2(); //cannot create an instance of an abstract class
            Derived2 d3 = new Derived2();
            d3.MethodWithImplementation();
            d3.AbstractMethod();

            //rectangle classs with area calculation
            Rectangle rect = new Rectangle(4, 9);
            Console.WriteLine(rect.Area);

            //using square class - this class violates the is-a-kind-of relationship
            rect = new Square(10) { Width = 5, Height = 9 };
            Console.WriteLine(rect.Area); //answer = 81. should be 45. in this case, a rectangle is not a type of square - thus violating the Liskov substitution
            //principle
            //implementing IComparable INterface
            List<Order2> orders = new List<Order2>
            {
                new Order2 {Created = new DateTime(2014, 12,1)},
                new Order2 {Created = new DateTime(2014, 1,6)},
                new Order2 {Created = new DateTime(2014, 7,8)},
                new Order2 {Created = new DateTime(2012, 2, 20)}
            };
            orders.Sort(); //sort command uses the implemented CompareTo function of the interface to sort.

            // Syntactic sugar of the foreach statement
            List<int> numbers = new List<int> { 1, 2, 3, 5, 7, 9 };
            using (List<int>.Enumerator enumerator = numbers.GetEnumerator())
            {
                while (enumerator.MoveNext()) Console.WriteLine(enumerator.Current);
            }

            Console.ReadLine();
        }
    }
}
