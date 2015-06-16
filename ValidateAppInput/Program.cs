using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ValidateAppInput
{
    /**********************Create Customer and Address classes**********************************/
    public class Customer
    {
        public int Id { get; set; }

        [Required, MaxLength(20)] //specified to database that this field is required and cant be longer than 20 characters. validation runs as I save to database
        public string FirstName { get; set; }

        [Required, MaxLength(20)]
        public string LastName { get; set; }

        [Required]
        public Address ShippingAddress { get; set; }

        [Required]
        public Address BillingAddress { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string AddressLine1 { get; set; }

        [Required, MaxLength(20)]
        public string AddressLine2 { get; set; }

        [Required, MaxLength(20)]
        public string City { get; set; }

        [RegularExpression(@"^[1-9][0-9]{3}\s?[a-zA-Z]{2}$")]//acceptable string format
        public string Zipcode { get; set; }
    }

    /********************Save new customer to database***************************************/
    public class ShopContext : DbContext  //DbContext is an instance used to query a database or group objects that are to be saved unto a database
    {
        public IDbSet<Customer> Customers { get; set; } //IDbSet represents a collection of entities. In this case, Customer objects

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Make sure the database knows hot to handle the duplicate address property
            //HasRequired means field is required in object?
            //WithMany means multiple customers can have same billing address?
            //WillCascadeOnDelete means if i delete the parent row, the children will be set to null
            modelBuilder.Entity<Customer>().HasRequired(bm => bm.BillingAddress).WithMany().WillCascadeOnDelete(false); 
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //Saving new customer to database
            using (ShopContext ctx = new ShopContext())
            {
                Address a = new Address
                {
                    AddressLine1 = "Somewhere 1",
                    AddressLine2 = "At some floor",
                    City = "SomeCity",
                    Zipcode = "1111AA"
                };

                Customer c = new Customer()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    BillingAddress = a,
                    ShippingAddress = a
                };

                ctx.Customers.Add(c); //SQL exception occured. Unable to create database
                ctx.SaveChanges();
            }
        }
    }
}
