#define CONDITION1
#define CONDITION2
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics; //for Conditional attribute
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit; //used NuGet to add the assembly to this project


namespace Reflection
{
    /*******************apply an attribute*****************/
    [Serializable] //attribute of the class
    class Person
    {
        public string FirstName { get; set; }
        public string LastName  { get; set; }
        private int t1;
        public Person()
            {
                t1 = 5;
            }
    }


    class Program
    {
        /*****************Use multiple attributes****************/
        [Conditional("CONDITION1"), Conditional("CONDITION2")] //indicates to compiler that this method should be ignored unless a specific compiler option is
        static void MyMethod() { Console.WriteLine("MyMethod"); }                                                        //specified

        [Conditional("CONDITION1")] //can only use ConditionalAttribute on a method or attribute class
        class ConditionalClass : Attribute //inherit from Attribute base class to become an attribute class
        { }

        /***********Using a category attribute in xUnit************************************/
        [Fact]
        [Trait("Category", "Unit Test")]
        public void MyUnitTest()
        { }

        [Fact]
        [Trait("Category", "Unit Test")]
        public void MyIntegrationTest()
        { }

        /********************create a custom attribute***************************/
        public class CategoryAttribute : TraitAttribute  //derives from TraitAttribute
        {
            public CategoryAttribute(string value)
                : base("Category",value)  //call TraitAttribute constructor
            { }
        }

        public class UnitTestAttribute : CategoryAttribute
        {
            public UnitTestAttribute()
                :base("Unit Test")
            { }
        }

        /************************use a custom attribute**************************************/
        [Fact]
        [UnitTest] //this is a custom attribute - defined in UnitTestAttribute
        public static void MySecondUnitTest90()
        { }

        /*********defining the targets for a custom attribute****************************/
        [AttributeUsage(AttributeTargets.Method)]
        public class MyMethodAndParameterAttribute : Attribute
        {

        }

        //use above custom attribute
        [MyMethodAndParameter]
        public static void mymethod1 (){}

        //[MyMethodAndParameter]
        //class thiswontwork { }  //this is an error since attribute is only valid for Methods according to the Attribute USage
       
        //setting the AllowMultiple parameter for a custom attribute
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        class MyMultipleUsageCompleteAttribute : Attribute 
        {
            public string Description { get; set; } //add properties to custom attribute
            public MyMultipleUsageCompleteAttribute(string description)
            {
                Description = description;
            }
        }
        
        /**********************create an interface that can be found through reflection*********************************/
        public class MyApplication
        { }

        public interface IPlugin
        {
            string Name { get; }
            string Description { get; }
            bool Load(MyApplication application);
        }

        // create custom plug-in class
        public class MyPlugin : IPlugin
        {
            public string Name
            {
                get { return "MyPlugIn"; }
            }

            public string Description
            {
                get { return "My Sample Plug"; }
            }

            public bool Load(MyApplication application)
            {
                return true;
            }
        }

        public static void InspectAssembly()
        {
            //inspect assembly for types that implement a custom interface
            Assembly pluginAssembly = Assembly.Load("assemblyname");
            var plugins = from type1 in pluginAssembly.GetTypes() //type1 is a Type object. extract Type objects from pluginAssembly
                          where typeof(IPlugin).IsAssignableFrom(type1) && !type1.IsInterface //Check that extracted type can be an instance of IPlugin and that its not an interface
                          select type1; //if all checks well, select the objeect and put into plugins object;
            MyApplication app = new MyApplication();
            foreach (Type pluginType in plugins)    //for each Type in plugins, 
            {
                IPlugin plugin = Activator.CreateInstance(pluginType) as IPlugin; //create an instance as an IPlugin interface object
                Console.WriteLine(plugin.Description);
                plugin.Load(app); //invoke method
            }
        }

        /*******************************Getting the value of a field through reflection******************************/
        static void DumpObject(object obj)
        {
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic ); //BindingFlags control the search for members of the type
            foreach (FieldInfo field in fields)
            {
               // if (field.FieldType == typeof(int))
                //{
                    Console.WriteLine(field.GetValue(obj));
                //}
            }
        }

        
        
        /***********************************Main*********************************************************/
        static void Main(string[] args)
        {
            MyMethod(); // will only run if #define CONDITION1 #define CONDITION2 are defined at top of file

            //see if an attribute is defined
            if(Attribute.IsDefined(typeof(Person), typeof(SerializableAttribute)))
            {
                Console.WriteLine("Serializable attribute defined for Person class");
            }

            //Get a specific attibute instance
            ConditionalAttribute conditionalAttribute =
                (ConditionalAttribute)Attribute.GetCustomAttribute(
                typeof(ConditionalClass), //name of the class
                typeof(ConditionalAttribute));  //name of attribute
            Console.WriteLine(conditionalAttribute.ConditionString); //output : CONDITION1

            //Get instance of xUnit attributes
            Type clsType = typeof(Program);
            MethodInfo minfo = clsType.GetMethod("MyIntegrationTest");
            TraitAttribute traitAttribute =
                (TraitAttribute)Attribute.GetCustomAttribute(
                minfo,
                typeof(TraitAttribute));
            Console.WriteLine(traitAttribute.Value); //output : Unit Test :-)
            
            //get type using reflection
            int i = 42; bool a = i.Equals(44);
            System.Type type = i.GetType();
            Console.WriteLine( type.ToString()); //output : System.Int32
            i.ToString();
            Person nums = new Person { FirstName = "Toks", LastName = "Quaye" };
            DumpObject(nums);

            //execute a method through reflection
            MethodInfo compareToMethod = i.GetType().GetMethod("CompareTo", new Type[] { typeof(int) });
            int result = (int)compareToMethod.Invoke(i, new object[] { 42 });
            MethodInfo EqualsMeth = i.GetType().GetMethod("Equals", new Type[] { typeof(int) });
            bool result2 = (bool)EqualsMeth.Invoke(i, new object[] { 56 });

            /**********Generating "Hello World" with the CodeDom****************************************************/
            CodeCompileUnit compileUnit = new CodeCompileUnit(); //container for the CodeDom graph
            CodeNamespace myNamespace = new CodeNamespace("MyNamespace"); //namespace declaration
            myNamespace.Imports.Add(new CodeNamespaceImport("System")); //imports System namespace. equivalent to "using System"
            CodeTypeDeclaration myClass = new CodeTypeDeclaration("MyClass"); //declare a type - in this case, it a class
            CodeEntryPointMethod start = new CodeEntryPointMethod(); //entry point method
            CodeMethodInvokeExpression cs1 = new CodeMethodInvokeExpression( //invokes a method - in this case console.Writeline("Hello World");
                new CodeTypeReferenceExpression("Console"),
                "WriteLine", new CodePrimitiveExpression("Hello World!"));
            compileUnit.Namespaces.Add(myNamespace); //add the namespace to the CodeDom graph
            myNamespace.Types.Add(myClass); //add the class to the namespace
            myClass.Members.Add(start); //add entrypoint method to the class
            start.Statements.Add(cs1); //add the console writeline call to the entry method


            /*********************Generate a sourcefile from the CodeCompileUnit*****************************/
            CSharpCodeProvider provider = new CSharpCodeProvider();
            using (StreamWriter sw = new StreamWriter("HellowWorld.cs", false)) //file generated in the Reflection/bin/Debug directory
            {
                IndentedTextWriter tw = new IndentedTextWriter(sw, "   ");
                provider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());
                tw.Close();

            }

            /*********************************Creating a Func type with a Lambda***************************/
            Func<int, int, int> addFunc = (x, y) => x + y;
            Console.WriteLine(addFunc(7,5)); //output = 12

            /********************Creating "Hello Worlf" with an expression tree*******************************/
            BlockExpression blockExpr = Expression.Block(
                Expression.Call(
                null,
                typeof(Console).GetMethod("Write", new Type[] { typeof(String) }),
                Expression.Constant("Hello")
                ),
                Expression.Call(
                null,
                typeof(Console).GetMethod("WriteLine", new Type[] { typeof(String) }),
                Expression.Constant("World")
                )
                );

            Expression.Lambda<Action>(blockExpr).Compile()();
            Console.ReadLine();
        }
    }
}
