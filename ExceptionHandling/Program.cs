using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;

namespace ExceptionHandling
{
    [Serializable]  // indicates this class can be serialized
    public class OrderProcessingException : Exception, ISerializable
    {
        public int OrderId { get; private set; }

        public OrderProcessingException(int orderId)
            :base("This input is unacceptable")
        {
            OrderId = orderId;
            //this.InnerException = "The order number is unacceptable";
            this.HelpLink = "http://www.mydomain.com/infoaboutexception";
        }

        public OrderProcessingException(int orderId, string message, Exception innerException)
        : base(message,innerException) //this calls the exception class construction that takes msg and exception as parammeters
        {
            OrderId = orderId;
            this.HelpLink = "http://www.mydomain.com/infoaboutexception";
        }

        protected OrderProcessingException(SerializationInfo info, StreamingContext context)
        {
            OrderId = (int)info.GetValue("OrderId", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("OrderId", OrderId, typeof(int));
        }

    }
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                //Console.Write("Enter a number :");
                string s = /*null; */ Console.ReadLine();
                ExceptionDispatchInfo possibleException = null;
                Task tsk = Task.Run(() =>
                    {
                        //Use ExceptionDispatchInfo.Throw                        
                        try
                        {
                            Console.WriteLine("Enter value in tsk");
                            string t = Console.ReadLine();
                            int.Parse(t);
                        }
                        catch (FormatException ex)
                        {
                            possibleException = ExceptionDispatchInfo.Capture(ex);
                        }                        
                    });

                tsk.Wait();
                if (possibleException != null) // this exception was caught in the tsk thread, but ExceptionDispathInfo allows me to save it and then throw it on 
                                                // the main thread. cool!
                {
                    possibleException.Throw();  //now throw the exception that was captured
                }

                //Throw new exception object
                try
                {
                    if (string.IsNullOrWhiteSpace(s))
                        throw new ArgumentNullException("Input", "Valid Input is Required");

                }
                catch (ArgumentNullException ex)
                {
                    //throw; //this retains original stack info of the exception 
                    throw new FormatException("Invalid format of input", ex); //this puts the initial exception ex into the InnerException field of this new 
                                                    //exception, thereby retaining all info needed for debug
                }

                //throw custom exception
                try
                {
                    int i = int.Parse(s);
                    //if (i == 42) Environment.FailFast("Special number entered");
                    if (i == 43) throw new OrderProcessingException(43);
                }
                catch(ArgumentNullException ex)
                {
                    Console.WriteLine("{0} is not valid. {1}",s,ex.Message);
                    Environment.FailFast(ex.Message, ex);
                    break;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("{0} not a valid number format : {1}", s, ex.Message);
                    break;
                }
                finally
                {
                    Console.WriteLine("this is finally");
                }

            }

            
            Console.WriteLine
                ("End of program");
            Console.ReadLine();
        }
    }
}
