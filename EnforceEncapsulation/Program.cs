using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnforceEncapsulation
{
    /************************************
     * 2-31 - Using access modifiers
     * **********************************/
    public class Dog
    {
        public void Bark() { }
    }

    /***************************************
     * Using private access modifier
     * *************************************/
    public class Accessibility
    {
        //private string _myField; //private - only accessible within the class
        private string[] _myField; //user is oblivious to any inner implementation change of this field because of encapsulation
        public string MyProperty //this wraps access to _myField
        {
            //get { return _myField; }
            get { return _myField[0]; } //user will access MyProperty without knowing that implementation has changed - encapsulation!
            //set { _myField = value; }
            set { _myField[0] = value; }
        }
    }

    /****************************************************
     * Using protected access modifier with inheritance
     * *************************************************/
    public class Base
    {
        private int _privateField = 42;
        protected int _protectedField = 42;

        private void myPrivateMethod() { }
        protected void myProtectedMethod() { }
    }

    public class Derived :Base
    {
        public void DerivedMethod() 
        {
            _protectedField = 43; //this is accessible because protected gives access to derived class
            //_privateField = 43; //this generates error because field is private to base class only
            myProtectedMethod();//protected gives accesss
            //myPrivateMethod(); //error - private to base only
        }
    }

    /***********************************************
     * Using internal access modifier
     * ********************************************/
    internal class MyInternalClass  //class only available within this assembly
    {
        public void MyMethod() { } //although this is public, it is restricted by an internal class, so its only available withing this assembly
    }

    /********************************************
     * Creating a property
     * ****************************************/
    class Person
    {
        private string _firstname;

        public string FirstName //use accessors for private field
        {
            get { return _firstname; }

            set 
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException();
                _firstname = value;
            }
        }
    }

    /***********************************************
     * *Implementing an interface explicitly
     * ********************************************/
    interface IInterfaceA
    {
        void MyMethod();
    }

    class Implementation:IInterfaceA
    {
        void IInterfaceA.MyMethod() { }     //this implements IInterface explicityly    
    }

    interface ILeft
    {
        void Move();
    }

    interface IRight
    {
        void Move();
    }

    class MoveableObject: ILeft, IRight
    {
        public void Move() { Console.WriteLine("implicit"); } //implict interface implementation
        void IRight.Move() { Console.WriteLine("Explicit"); }
    }

    class Program
    {
        static void Main(string[] args)
        {
            /*Dog dog = new Dog(); //because it's a public class, everyone can call it
            dog.Bark();

            Implementation imp = new Implementation();
            //imp.MyMethod(); //no access to interface method!
                              //explicit interface implementation used to hide members of a class to outside users!
            ((IInterfaceA)imp).MyMethod(); //needed to cast imp to IInterfaceA before access to MyMethod is given
            */
            MoveableObject mo = new MoveableObject();
            //mo.Move(); // won't work if no implicit interface implementation present. need a cast to which one you want
            ((IRight)mo).Move(); //cast to interface works! Explicit 
            mo.Move(); //just made implicit call - this is the default
            
            IRight mo1 = new MoveableObject(); //this also works to call the IRight MOve
            mo1.Move();

            Console.ReadLine();
        }
    }
}
