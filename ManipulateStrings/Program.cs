using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ManipulateStrings
{
    class Program
    {
        /************************override ToString()*************************************/
        class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public Person(string firstname, string lastName)
            {
                this.FirstName = firstname;
                this.LastName = lastName;
            }

            public override string ToString()
            {
                return FirstName + " " + LastName;
            }

            public string ToString(string format)
            {
                if (string.IsNullOrWhiteSpace(format) || format == "G") format = "FL";
                format = format.Trim().ToUpperInvariant();//trim - remove all leading and trailing spaces. return to uppercase using casing rule of culture

                switch (format)
                {
                    case "FL" :
                        return FirstName + " " + LastName;
                    case "LF":
                        return LastName + " " + FirstName;
                    case "FSL":
                        return FirstName + ", " + LastName;
                    case "LSF":
                        return LastName + ", " + FirstName;
                    default:
                        throw new FormatException(String.Format("The '{0}' format string is not supported.", format));
                }
            }
        }

        /***********************************Main******************************************/

        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder("Initial Value");
            sb[0] = 'B';

            String s1 = new String('X', 10);

            // Using a StringWriter as the ouput fo an XML Writer
            var stringWriter = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(stringWriter)) //create a new instance of XMLwriter
            {
                writer.WriteStartElement("book"); //write a start tag named "book"
                writer.WriteElementString("price", "19.95"); //write element with specified name and value
                writer.WriteEndElement(); //close a previously started element : "book"
                writer.Flush(); //flush whatever is in the xml buffer onto the listening stream
            }
            string xml = stringWriter.ToString();
            Console.WriteLine(xml);

            //Using a StringReader as the input for an XmlReader
            var stringReader = new StringReader(xml); //read from the xml string
            using (XmlReader reader = XmlReader.Create(stringReader)) //create XmlReader instance
            {
                reader.ReadToFollowing("price"); //read until an element named "price" is found
                decimal price = decimal.Parse(reader.ReadInnerXml(),//read entire content of price element. parse  and save value
                    new CultureInfo("en-US")); //make sure that you read the decimal part correctly
            }

            //Using IndexOf and LastIndexOf
            string value = "My Sample Value";
            int indexOfp = value.IndexOf("p"); //returns 6
            int lastIndexOfe = value.LastIndexOf("e"); //returns 14

            //Using Startswith and EndsWith
            string value2 = "<mycustominput>";
            if (value2.StartsWith("<")) { Console.WriteLine("Starts with <"); }
            if (value2.EndsWith(">")) { Console.WriteLine("Ends with >"); }

            //Read a substring
            string substring = value.Substring(3, 6); //(start index, length). returns Sample

            //Changing a string with a regular expression
            string pattern = "(Mr\\.? |Mrs\\.? |Miss\\? |Ms\\.?)";
            string[] names = {"Mr. Henry Hunt", "Mrs. Fola Ashi", "Ms. Debra Hack",
                             "Tosin Tinunbu", "Ms. Abike Sanni"};

            foreach(string name in names)
            {
                Console.WriteLine(Regex.Replace(name,pattern,string.Empty)); //when the pattern matches something in name, replace it with an empty string.
            }

            //Iterating over a string
            foreach (char c in value2)
                Console.WriteLine(c);

            foreach(string s2 in "My sentence is separated by spaces".Split(' '))
                Console.WriteLine(s2);

            //use overridden ToString
            Person p = new Person("Toks", "Quaye");
            Console.WriteLine(p.ToString()); //output = "Toks Quaye"
            Console.WriteLine(p.ToString("lsf"));

            //Displaying a number with a currency format string
            double cost = 1234.56;
            Console.WriteLine(cost.ToString("C", //string format
                new System.Globalization.CultureInfo("en-US"))); //culture - US ENglish //output = $1,234.56
            //Displaying a DateTime with different format strings
            DateTime d = new DateTime(2013, 4, 22);
            CultureInfo provider = new CultureInfo("en-US"); //specify English-US culture - determines how its displayed
            Console.WriteLine(d.ToString("d",provider)); //  4/22/2013
            Console.WriteLine(d.ToString("d",new CultureInfo("en-GB"))); //specify English-British culture 22/04/2013
            Console.WriteLine(d.ToString("M", provider)); // April 22

            //Creating a Composite string formatting
            int a = 1;
            int b = 2;
            string result = string.Format("a:{0}, b: {1}", a, b);
            Console.WriteLine(result); //output : 'a:1, b: 2'
            Console.ReadLine();
        }
    }
}
