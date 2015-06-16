using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SerializeDeserialize
{
    /***********************************************Serializing an object with the xmlSerializer********************************************/
    [Serializable]
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
  
    /*************************************Using XML attributes to configure serialization****************************************************/
    [Serializable]
    public class Order
    {
        [XmlAttribute]
        public int ID { get; set; }

        [XmlIgnore]
        public bool IsDirty { get; set; }

        [XmlArray("Lines")]
        [XmlArrayItem("OrderLine")]
        public List<OrderLine> OrderLines { get; set; }
    }

    [Serializable]
    public class VIPOrder : Order
    {
        public string Description { get; set; }
    }

    [Serializable]
    public class OrderLine
    {
        [XmlAttribute]
        public int ID { get; set; }

        [XmlAttribute]
        public int Amount { get; set; }

        [XmlElement("OrderedProduct")]
        public Product Product { get; set; }
    }
    
    [Serializable]
    public class Product
    {
        [XmlAttribute]
        public int ID { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }

    /*********************************************Binary Serialization*******************************************************/
    [Serializable]
    public class Person2
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [NonSerialized]
        private bool isDirty = true;

        //Influencing serialization and deserialization
        [OnSerializing()]
        internal void OnSerializingMethod(StreamingContext context)
        {
            Console.WriteLine("OnSerializing");
        }

        [OnSerialized()]
        internal void OnSerializedMethod(StreamingContext context)
        {
            Console.WriteLine("OnSerialized");
        }

        [OnDeserializing()]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            Console.WriteLine("OnDeserializing");
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Console.WriteLine("OnDeserialized");
        }
    }

   /**********************************************Implementing ISerializable*********************************************************/
    [Serializable]
    public class PersonComplex : ISerializable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        private bool isDirty = false;

        public PersonComplex() {} //constructor
        protected PersonComplex(SerializationInfo info, StreamingContext context) //params - info needed to serialize/deserialize
                                                                                  //context - source/destination of stream to serialize/deserialize to
        {//this method is called during deserialization
            ID = info.GetInt32("Value1");
            Name = info.GetString("Value2");
            isDirty = info.GetBoolean("Value3");
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand,SerializationFormatter = true)]
        //all callers higher on the call stack are required to have permission before calling this method
        public void GetObjectData(SerializationInfo info, StreamingContext context)
            //this is called when object is serialized
        {
            info.AddValue("Value1", ID);
            info.AddValue("Value2", Name);
            info.AddValue("Value3", isDirty);
        }

    }

    /******************************************Using DataContract***********************************************/

    [DataContract]
    public class PersonDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        private bool isDirty = false;
    }

    class Program
    {

        //Serializing a derived, complex class to XML
        private static Order CreateOrder()
        {
            Product p1 = new Product { ID = 1, Description = "p2", Price = 9 };
            Product p2 = new Product { ID = 2, Description = "p3", Price = 6 };

            Order order = new VIPOrder
            {
                ID = 4,
                Description = "Order for John Doe. Use the nice giftwrap",
                OrderLines = new List<OrderLine>
                {
                    new OrderLine{ ID = 5, Amount = 1, Product = p1},
                    new OrderLine { ID = 6, Amount = 10, Product = p2}
                }
            };
            return order;
        }
      static void Main(string[] args)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Person));
            string xml;
            using (StringWriter stringwriter = new StringWriter())  
            {
                Person p = new Person
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Age = 24
                };
                serializer.Serialize(stringwriter, p); //serialize object p. write it into stringwriter
                xml = stringwriter.ToString(); //copy serialized data to a string
                
            }
            Console.WriteLine(xml);
          /* Output of xml:
<?xml version="1.0" encoding="utf-16"?>
<Person xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://
www.w3.org/2001/XMLSchema">
  <FirstName>John</FirstName>
  <LastName>Doe</LastName>
  <Age>24</Age>
</Person>
*/
          using (StringReader stringreader = new StringReader(xml))
          {
              {
                  Person p = (Person)serializer.Deserialize(stringreader); //Deserialize method returns an object that get cast into Person
                  Console.WriteLine("{0} {1} is {2} years old",p.FirstName, p.LastName, p.Age); //John Doe is 24 years old

              }
          }

          //Serializing a derived, complex class to XML
          XmlSerializer serializer1 = new XmlSerializer(typeof(Order), new Type[] { typeof(VIPOrder) });
          string xml1;
          using (StringWriter stringwriter1 = new StringWriter())
          {
              Order order = CreateOrder();
              serializer1.Serialize(stringwriter1, order); 
              xml1 = stringwriter1.ToString();

          }
          Console.WriteLine(xml1);

/*
 * <?xml version="1.0" encoding="utf-16"?>
<Order xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://w
ww.w3.org/2001/XMLSchema" xsi:type="VIPOrder" ID="4">
  <Lines>
    <OrderLine ID="5" Amount="1">
      <OrderedProduct ID="1">
        <Price>9</Price>
        <Description>p2</Description>
      </OrderedProduct>
    </OrderLine>
    <OrderLine ID="6" Amount="10">
      <OrderedProduct ID="2">
        <Price>6</Price>
        <Description>p3</Description>
      </OrderedProduct>
    </OrderLine>
  </Lines>
  <Description>Order for John Doe. Use the nice giftwrap</Description>
</Order>
*/
          using(StringReader stringreader1 = new StringReader(xml1))
          {
              Order o1 = (Order)serializer1.Deserialize(stringreader1);
              Console.WriteLine("Order ID: {0}",o1.ID); //Order ID: 4
          }

          //Using binary serialiation
          Person2 p1 = new Person2
          {
              Id = 1,
              Name = "John Doe"
          };

          IFormatter formatter = new BinaryFormatter();
          using (Stream stream = new FileStream("data.bin", FileMode.Create))
          {
              formatter.Serialize(stream, p1);
          }

          //data.bin now contains binary serialized data
          using (Stream stream = new FileStream("data.bin", FileMode.Open))
          {
              Person2 dp = (Person2)formatter.Deserialize(stream); //opens the file to deserialize. puts object in dp
              Console.WriteLine("id: {0}, name: {1}", dp.Id, dp.Name );//dp.isDirty is not accessible since its not serializable
          }
          
          //Serialize object that implements ISerializable
          PersonComplex pc1 = new PersonComplex()
          {
              ID = 5,
              Name = "Toks"
          };
          XmlSerializer xmC = new XmlSerializer(typeof(PersonComplex));
          string xmlPc;
          using (StringWriter swC = new StringWriter())
          {
              xmC.Serialize(swC, pc1);
              xmlPc = swC.ToString();
          }
          Console.WriteLine(xmlPc);

          //Using DataContractSerializer
          PersonDataContract pdc = new PersonDataContract()
          {
              Id = 1,
              Name = "John Doe"
          };

          using (Stream stream = new FileStream("data.xml", FileMode.Create))
          {
              DataContractSerializer ser = new DataContractSerializer(typeof(PersonDataContract));
              ser.WriteObject(stream, pdc);
          }
          /*
           * 
          <PersonDataContract xmlns="http://schemas.datacontract.org/2004/07/SerializeDeserialize" 
           * xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><Id>1</Id><Name>John Doe</Name></PersonDataContract>
           * */
          using (Stream stream = new FileStream("data.xml", FileMode.Open))
          {
              DataContractSerializer ser = new DataContractSerializer(typeof(PersonDataContract));
              PersonDataContract result = (PersonDataContract)ser.ReadObject(stream);
          }

          //Using the DataContractJsonSerializer - unable to resolve namespace

          /*using(MemoryStream stream = new MemoryStream())
          {
              DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(PersonDataContract));
              
          }*/
            Console.ReadLine();
        }
    }
}
