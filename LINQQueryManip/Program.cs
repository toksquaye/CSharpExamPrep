using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LINQQueryManip
{
    //Extension Methods
    public static class IntExtensions
    {
        public static int Multiply(this int num, int otherNum)
        {
            return num * otherNum;
        }
    }
    class Program
    {
        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        /**************************Sampel Order class for LINQ queries******************************/
        public class Product
        {
            public string Description { get; set; }
            public decimal Price { get; set; }
        }

        public class OrderLine
        {
            public int Amount { get; set; }
            public Product Product { get; set; }
        }

        public class Order
        {
            public List<OrderLine> OrderLines { get; set; }
        }
        static void Main(string[] args)
        {
            //Use object initializer 
            Person p = new Person { FirstName = "Toks", LastName = "Quaye" };

            //use collection initializer
            var people = new List<Person>
            {
                new Person{FirstName="Mac", LastName="Daddy"},
                new Person{FirstName="Joe", LastName="Shmo"}
            };

            //anonymous method
            Func<int, int> myDelegate =  //Func returns a value. Action doesn't
                delegate(int x)
                {
                    return x * 2;
                };

            Console.WriteLine(myDelegate(4)); //outputs 8

            //using lambda
            Func<int, int> myDelegate2 =
            x => x * 2; // => signifies becomes
            Console.WriteLine(myDelegate2(9)); // outputs 18


            //use int extension method multipy
            int j = 9;
            Console.WriteLine(j.Multiply(5)); // outputs 45

            //Create anonymous types
            var person2 = new 
            {
                FirstName = "Bose",  //utilizes object initializers
                LastName = "Femi"
            };
            Console.WriteLine(person2.GetType().Name); // gets current instance type name ; "<>f_AnonymousType()'2"

            //A LINQ Select Query - Query Syntax
            int[] data = { 1, 2, 5, 8, 11 };
            var result = from d in data // obtain the data
                         where d % 2 == 0 // create a query : query numbers divisible by 2
                         select d; //execute the query
            foreach (int i in result)
                Console.Write("{0}, ", i);
            Console.WriteLine();

            // LINQ Query - Method Syntax
            var result2 = data.Where(d => d % 2 == 0); //Where parameter is a Func<int,bool> predicate

            //LINQ Select Operator
            var result3 = from d in data
                          where d > 5
                          select d;
            Console.WriteLine(string.Join("; ",result3)); //Join concatenates elemets of the array using specified separator
                                                            //display 8; 11\

            //Use orderby
            var result4 = from d in data
                          where d >= 5
                          orderby d descending
                          select d;
            Console.WriteLine(string.Join(", ",result4));

            //LINQ multiple From statements
            int[] data1 = { 1, 2, 5 };
            int[] data2 = { 2, 4, 6 };
            var result5 = from d1 in data1
                          from d2 in data2
                          select d1 * d2;
            Console.WriteLine(string.Join(", ",result5));

            //order class instance
            Product p1 = new Product { Description = "green marker", Price = 34.45M };
            Product p2 = new Product { Description = "bag", Price = 12.33M };
            Product p3 = new Product { Description = "bracelet", Price = 8.23M };

            List<Product> products = new List<Product> {p1, p2, p3};
          
            Order o1 = new Order { 
                OrderLines =  new List<OrderLine>
                {
                    new OrderLine { Amount = 10, Product = p1 },
                    new OrderLine { Amount = 8, Product = p2 },
                    new OrderLine { Amount = 15, Product = p3 }
                } 
            };

            Order o3 = new Order
            {
                OrderLines = new List<OrderLine>
                {
                    new OrderLine { Amount = 18, Product = p1 },
                    new OrderLine { Amount = 68, Product = p2 },
                    new OrderLine { Amount = 150, Product = p3 }
                }
            };
            Order o2 = new Order
            {
                OrderLines = new List<OrderLine>
                {
                    new OrderLine { Amount = 9, Product = p1 },
                    new OrderLine { Amount = 5, Product = p3 }
                }
            };

            List<Order> orders = new List<Order> { o1, o2, o3 };
            var averageNumberOfOrderLines = orders.Average(o => o.OrderLines.Count);
            Console.WriteLine(averageNumberOfOrderLines);    //output : 2.5

            //Using group by and projection
            var result6 = from o in orders  //from the list of orders
                         from l in o.OrderLines //from the list of Orderlines in those orders
                         group l by l.Product into p4 // focus on the product in each orderline
                         select new //create output contains for each product, the total number of products there are
                         {
                             Product = p4.Key.Description, //  p4.Key , //product.  this value isnt distinct. hmmm
                             Amount = p4.Sum(x => x.Amount)//sume of the amount of this product
                         };
            foreach(var item in result6)
                Console.WriteLine("{0}, {1}",item.Product, item.Amount);

            //Using Join
            string[] popularProductNames = { "bag", "green marker" };
            var popularProducts = from p7 in products    // from list of producs 
                                  join n in popularProductNames on p7.Description equals n //join items whose product description matches values in popularProduct names
                                  select p7;
            Console.WriteLine();
            foreach (var pp in popularProducts)
                Console.WriteLine("{0}, {1}", pp.Description, pp.Price);

            //Using Skip and take to implement paging
            var pagedOrders = orders
                .Skip(1)
                .Take(1);
            foreach(var pO in pagedOrders)
                Console.WriteLine(pO.OrderLines.Count);

            //Sample xml
            string xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                                <people>
                                <person firstname=""john"" lastname=""doe"">
                                    <contactdetails>
                                        <emailaddress>john@unknown.com</emailaddress>
                                        <phonenumber>0987654321</phonenumber>
                                    </contactdetails>
                                </person>
                                <person firstname=""jane"" lastname=""doe"">
                                    <contactdetails>
                                        <emailaddress>jane@unknown.com</emailaddress>
                                        <phonenumber>001122334455</phonenumber>
                                    </contactdetails>
                                </person>
                                </people>";

            //Query some XML using LINQ to XML
            XDocument doc = XDocument.Parse(xml);
            IEnumerable<string> personNames = from px in doc.Descendants("person")
                                              select (string)px.Attribute("firstname") + " " + (string)px.Attribute("lastname");//Attribute returns type XAttribute
                                                                                                 //which can be explicitly cast - in this case to string
            foreach(string s1 in personNames)
                Console.WriteLine(s1); //displays : john doe
                                        //          jane doe

            //Using Where and OrderBy in a LINQ to XML query
            IEnumerable<string> personNames2 = from pn2 in doc.Descendants("person") //for each record that is a descendant of person
                                               where pn2.Descendants("phonenumber").Any() //for any record that as a phonenumber
                                               let name_pn2 = (string)pn2.Attribute("firstname") + " " + (string)pn2.Attribute("lastname")//grab the first & lastnames
                                               orderby name_pn2 //put names in alphabetical order
                                               select name_pn2;

            foreach(string s in personNames2)
                Console.WriteLine(s); //displays : jane doe
                                        //          john doe

            //Creating XML with the XElement class - using object initializer and collection initializers
            XElement root = new XElement("Root", 
                        new List<XElement>
                        {
                            new XElement("Child1", 
                                           new XAttribute("Child1FName","Owolabi"),
                                           new List<XElement>
                                           {                                
                                            new XElement("contantinfo")
                                           }
                                            ),
                            new XElement("Child2"),
                            new XElement("Child3")
                        },
                        new XAttribute("MyAttribute",42));

            root.Save("test.xml"); //serialize content of root object into file
            //outputs the following in test.xml
            /* 
             * <?xml version="1.0" encoding="utf-8"?>
                <Root MyAttribute="42">
                <Child1 Child1FName="Owolabi">
                <contantinfo />
                </Child1>
                <Child2 />
                <Child3 />
                </Root> */

            //Updating XML in a procedural way
            //when I used doc, which is an XDocument object, it worked. Hwoever, I shouldve used an XElement object
            foreach(XElement pp in doc.Descendants("person"))
            {
                string name = (string)pp.Attribute("firstname") + (string)pp.Attribute("lastname");
                pp.Add(new XAttribute("IsMale", name.Contains("john"))); //.Contains returns true if the name contains the text "john"
                XElement contactDetails = pp.Element("contactdetails");
                if(!contactDetails.Descendants("cellphone").Any())
                {
                    contactDetails.Add(new XElement("cellphone", "1234512345"));
                }
            }
            doc.Save("test2.xml");
            //output to text2.xml
            /*
             * 
<?xml version="1.0" encoding="utf-8"?>
  <people>
   <person firstname="john" lastname="doe" IsMale="true">
    <contactdetails>
      <emailaddress>john@unknown.com</emailaddress>
      <phonenumber>0987654321</phonenumber>
      <cellphone>1234512345</cellphone>
    </contactdetails>
  </person>
  <person firstname="jane" lastname="doe" IsMale="false">
    <contactdetails>
      <emailaddress>jane@unknown.com</emailaddress>
      <phonenumber>001122334455</phonenumber>
      <cellphone>1234512345</cellphone>
    </contactdetails>
  </person>
</people>
             * 
             */

            //Transforming XML with functional creation
            XElement root2 = XElement.Parse(xml);
            XElement newTree = new XElement("People",
                from ppf in root2.Descendants("person")
                let name = (string)ppf.Attribute("firstname") + (string)ppf.Attribute("lastname") //grab name
                let contactDetails = ppf.Element("contactdetails") //grab contactdetails element
                select new XElement("person",                       //create new element
                    new XAttribute("IsMale", name.Contains("john")),  //with attribute ismale
                    ppf.Attributes(), //add these attributes & values as well. returns the attributes of person : firstname, lastname
                    new XElement("contactdetails", //creates contactdetails element
                        contactDetails.Element("emailaddress"), //extract this element from contractDetails that was previously grabbed 
                        contactDetails.Element("mobilenumber")  //if this element is in contractDetails, take it. if not, create new mobilenumber element & value
                        ?? new XElement("mobilenumber","1122334455")
                        )));
            newTree.Save("test3.xml");
          
            //output of test3.xml
            /*
<?xml version="1.0" encoding="utf-8"?>
<People>
  <person IsMale="true" firstname="john" lastname="doe">
    <contactdetails>
      <emailaddress>john@unknown.com</emailaddress>
      <mobilenumber>1122334455</mobilenumber>
    </contactdetails>
  </person>
  <person IsMale="false" firstname="jane" lastname="doe">
    <contactdetails>
      <emailaddress>jane@unknown.com</emailaddress>
      <mobilenumber>1122334455</mobilenumber>
    </contactdetails>
  </person>
</People>
             * */
            Console.ReadLine();
        }
    }
}
