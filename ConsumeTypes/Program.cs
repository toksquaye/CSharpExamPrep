using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace ConsumeTypes
{
    /*Implement an implicit and explicit conversion operator*/
    class Money /*: IFormattable*/
    {
        public Money(decimal amount)
        {
            Amount = amount;
        }

        public decimal Amount { get; set; }

        public static implicit operator decimal(Money money) //casting from Money to decimal (implicit)
        {
            return money.Amount;
        }

        public static explicit operator int(Money money)   //casting from Money to int  (explicit)
        {
            return (int)money.Amount;
        }

        public static explicit operator string(Money money) //casting from Money to string (explicit)
        {
            return money.Amount.ToString();
        }
        public override string ToString()      //this overrrides ToString function of every object
        {
            return this.Amount.ToString();
        }
        
        public string ToString(string format, IFormatProvider provider)
        {
            return ToString();

        }
        public static Money Parse(string input)
        {
            decimal temp = Convert.ToDecimal(input);
            return new Money(temp);
        }
    
    }

    
    
    class Program
    {

        /**********************************************
             * Using is and as operators
             * ********************************************/
        void OpenConnection(DbConnection connection)
        {
            if(connection is SqlConnection)     //
            {

            }

        }

        void LogStream(Stream stream)
        {
            MemoryStream memoryStream = stream as MemoryStream;
            if( memoryStream != null)
            {

            }
        }

        class Animal
        {
            public string voice { get; set; }
            public Animal()
            {
                voice = "I am an animal";
            }
        }

        class Dog : Animal
        {
            public Dog()
            {
                voice = "I am a Dog";
            }
        }

        class TestConversions
        {
            public void testMethod( Dog inputDog)
            {
                if (inputDog is Animal)  //this is true = Dog is an Animal. Conversion allowed
                {
                    Console.WriteLine("inputDog is Animal");
                }

                Animal anAnimal = new Animal();
                if (anAnimal is Dog)        //this is not true. Conversion is not allowed!
                {
                    Console.WriteLine("anAnimal is Dog");
                }

            }
        }

        /**************************************************
         * Using dynamic - Exporting some data to excel
         * ************************************************/
        static void DisplayInExcel(IEnumerable<dynamic> entities)
        {
            var excelApp = new Excel.Application();
            excelApp.Visible = true;
            excelApp.Workbooks.Add();

            dynamic worksheet = excelApp.ActiveSheet;

            worksheet.Cells[1, "A"] = "Header A";
            worksheet.Cells[1, "B"] = "Header B";

            var row = 1;
            foreach(var entity in entities)
            {
                row++;
                worksheet.Cells[row, "A"] = entity.ColumnA;
                worksheet.Cells[row, "B"] = entity.ColumnB;
            }

            worksheet.Columns[1].AutoFit();
            worksheet.Columns[2].AutoFit();

        }

        public class SampleObject : DynamicObject
        {
            public string someProperty = "hun";
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = binder.Name;
                return true;
            }
        }
        static void Main(string[] args)
        {
            //*******************
            //boxing and unboxing
            //*******************
            string.Concat("To box or not to box", 42, true); // all theree params required are obj types. VS boxed the value types to objects to use them in func

            int i = 42;
            object o = i;   //box to put into an object
            int x = (int)o; //cast to unbox

            IFormattable t = 3; // box 3 to use it as an interface object

            /**********************
            /*Implicit Conversions
            / *********************/

            int i1 = 42;
            double d = i1;  //implicit conversion of int to double. this is ok cos there's no precision loss in converstaion

            HttpClient client = new HttpClient();
            object o1 = client;  //implicit conversion to base type. ok since all types inherit from object
            IDisposable d1 = client; //HttpClient implements IDisposable, so since IDisposable is a base class, this is an implicit conversion

            /***********************************
             * Explicit Conversions / Casting
             * *********************************/

            double x1 = 1234.7;
            int a = (int)x1;  //cast double into int caousing loss of precision. a = 1234


            /***********************************************************************
            /* Using User defined Implicit and Explicit conversion operator 
             * *********************************************************************/
            Money m = new Money(42.42M); //putting M represent the number as a decimal, instead of a float
            decimal amount = m;
            int truncatedamount = (int)m;
            string s = (string)m; //explicit cast defined in class
            string s1 = m.ToString(); // use of ToString overload in class

            /******************************************************************************
             * Using the built-in Convert and Parse methods
             * ***************************************************************************/
            int value = Convert.ToInt32("42");  //convert to an equivalent 32-bit signed integer
            value = int.Parse("42"); //converts string to integer equivalent
            bool success = int.TryParse("42", out value); //attempts to convert. returns a boolean indicating if attempt was successful

            /***********************************
             * Custom Parse method
             * ***********************************/
            Money test = Money.Parse("34.23");



            /*******************************************
             *  Using is and as operators 
             *  ***************************************/

            TestConversions t1 = new TestConversions();
            t1.testMethod(new Dog());

            /************************************************
             * Export some data to Excel
             * *********************************************/

            var entities = new List<dynamic>
            {
                new
                {
                    ColumnA = 1,
                    ColumnB = "foo"
                },
                new
                {
                    ColumnA = 2,
                    ColumnB = "Bar"
                }
            };
            DisplayInExcel(entities);

            /***********************************************
             * custom dynamicobject
             * ******************************************/
            dynamic obj = new SampleObject();
            Console.WriteLine(obj.someProperty); //since dymamic objects are resolved at runtime, intellisense doesn't kick in
        }

    }
}
