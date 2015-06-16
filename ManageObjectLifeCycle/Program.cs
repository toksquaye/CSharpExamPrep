using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ManageObjectLifeCycle
{
    /************************Adding a finalizer*******/
    public class SomeType
    {
        ~SomeType() //finalizer
        {
            //code to run when finalizer executes
            //garbage collector determines when this executes
        }
    }
    //Don't depend on GC to run finalizer. Free resources yourself by implementing IDisposable interface
    
    /*************************IMplementing IDispose & FInalizer******************************/
    class UnmanagedWrapper : IDisposable
    {
        private IntPtr unmanagedBuffer;  //represents a pointer or handle to an object
        public FileStream Stream { get; private set; }

        public UnmanagedWrapper()
        {
            CreateBuffer();
            this.Stream = File.Open("temp.dat", FileMode.Create); //this creates a new file
        }

        private void CreateBuffer()
        {
            byte[] data = new byte[1024];
            new Random().NextBytes(data); //fills the next specifield array of bytes with random numbers
            unmanagedBuffer = Marshal.AllocHGlobal(data.Length); //allocates unmanaged memory of 1024 bytes
            Marshal.Copy(data, 0, unmanagedBuffer, data.Length); //copy contents of data into unmanagedbuffer
        }

        ~UnmanagedWrapper() //finalizer called by gc if code doesn't directly call the Dispose method
        {
            Dispose(false); //call Dispose. free unmanaged resource but don't close the stream - another finalizer will do that
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);//request that system not call finalizer of this object.
                                             //since this class has a finalizer, the GC will automatically queue it to be executed.
                                            //however, since we've called Dispose to release all unmanaged resources, we don't need the GC to call it.
        }
        protected virtual void Dispose(bool disposing)
        {
            Marshal.FreeHGlobal(unmanagedBuffer); //free memory that was allocated in CreateBuffer()
            if (disposing)  //this is only set to true when Dispose is being explictly called by code. otherwise, its false if finalize is being executed by GC.
                            //the GC will take care of closing the Stream when it calls the Stream Finalizer
            {
                if (Stream != null) //if stream is still open : Defensive program. U never know if this is being called multiple times.
                {
                    Stream.Close(); //close stream and release resources
                }
            }
        }
    }
 
   
   /********************************************Main*************************************************/
    class Program
    {

        /******************************Using Weak Reference***********************************************/
        static WeakReference data;
        public static void RunWRObject()
        {
            object result = GetData();
            GC.Collect(); //Uncommenting this like will make data.Target null
            result = GetData();
        }
        
        private static object LoadLargeList()
        {
            byte[] temp = new byte[1024];
            new Random().NextBytes(temp);
            return temp;

        }
        private static object GetData()
        {
            if (data == null)
            {
                data = new WeakReference(LoadLargeList());
            }
            if (data.Target == null)  //check if the WeakRefenced object is still there
            {
                Console.WriteLine("Weak Referenced object has been cleaned up by GC");
                data.Target = LoadLargeList(); //if it's not, load it into memory again
            }
            else
                Console.WriteLine("Weak Referenced object hasn't been cleaned up by GC");
            return data.Target;
        }

        static void Main(string[] args)
        {
            //Not closing a file will throw an error
            /*StreamWriter stream = File.CreateText("temp.dat");
            stream.Write("Write this");
            File.Delete("temp.dat"); //Throws an IOException because file is open */

            //Force garbage collection - which should close the file. 
            //Still cause IO Exception
            /*StreamWriter stream = File.CreateText("temp1.dat");
            stream.Write("Write this");
            GC.Collect(); //forces garbage collection (Not recommended)
            GC.WaitForPendingFinalizers(); //makes sure that all finalizers have run before the code continues to execute
            File.Delete("temp1.dat");*/

            //using executes try/finally code block. it automatically calls Dispose  on the object being used, thus releasing the file handle and allowing us
            //to close it using the File.Delete statement
            using (StreamWriter sw = File.CreateText("temp2.dat"))
            {
                sw.Write("Utilizing the using statement");
            }
            File.Delete("temp2.dat");

            RunWRObject();
            Console.ReadLine();
        }
    }
}
