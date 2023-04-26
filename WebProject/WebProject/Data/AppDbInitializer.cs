using Microsoft.AspNetCore.Builder;
using WebProject.Models;

namespace WebProject.Data
{
    public class AppDbInitializer
    {
        public static void seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                context.Database.EnsureCreated();
            /*    //Users
                if (!context.users.Any())
                {
                    context.users.AddRange(new List<user>()
                    {
                        new user()
                        {
                          //  username ="test",
                         //   password ="test",
                            //email ="test@gmail.com",
                            first_name ="test",
                            last_name ="test",
                            Address="test",
                            phone_number="22123123",
                            payment_method="cash"
                        },
                        new user()
                        {
                         //   username ="admin",
                         //   password ="admin",
                          //  email ="admin@gmail.com",
                            first_name ="admin",
                            last_name ="admin",
                            Address="admin",
                            phone_number="99123123",
                            payment_method="cash"
                        },
                        new user()
                        {
                         //   username ="user",
                        //    password ="user",
                       //     email ="user@gmail.com",
                            first_name ="user",
                            last_name ="user",
                            Address="user",
                            phone_number="55123123",
                            payment_method="cash"
                        }
                    }
                        );
                    context.SaveChanges();
                }*/
                //products
                if (!context.products.Any())
                {
                    context.products.AddRange(new List<product>()
                    {
                        new product()
                        {
                            name="PC Desktop Lenovo",
                            description="none",
                            quantity=55,
                            price= 1500.5 ,
                            image= "https://www.gstatic.com/webp/gallery/1.webp"

                        },
                        new product()
                        {
                            name="Laptop Asus",
                            description="none",
                            quantity=20,
                            price= 1100.0 ,
                            image= "http://dotnethow.net/images/actors/actor-2.jpeg"
						},
                        new product()
                        {
                            name="Red Dragon Mouse",
                            description="none",
                            quantity=32,
                            price= 120.0 ,
                            image= "http://dotnethow.net/images/actors/actor-3.jpeg"
						}
                    }
                        );
                    context.SaveChanges();
                }
               
                /*orders
                if (!context.orders.Any())
                {
                    context.orders.AddRange(new List<order>()
                    {
                        new order()
                        {
                            total=1220.0,
                            date_order=DateTime.Now.AddDays(7),
                            status="processing",
                            userid=3
                            
                        },
                        new order()
                        {
                            total=120.0,
                            date_order=DateTime.Now.AddDays(7),
                            status="in delievry",
                            userid=3
                        },
                        new order()
                        {
                            total=1500.5,
                            date_order=DateTime.Now.AddDays(7),
                            status="Approved",
                            userid=1
                        }
                    }
                        );
                    context.SaveChanges();
                }
				//order_products
				if (!context.order_products.Any())
				{
			    context.order_products.AddRange(new List<order_product>()
	            {
		                new order_product()
		                {
			                orderid = context.orders.First().orderid,
			                productid = 5,
			                quantity = 2

		                },
		                new order_product()
		                {
			                orderid = context.orders.Skip(1).First().orderid,
			                productid = 6,
			                quantity = 5
		                },
		                new order_product()
		                {
			                orderid = context.orders.Skip(2).First().orderid,
			                productid = 7,
			                quantity = 4
		                }
	            });
			    context.SaveChanges();
				}*/
			}
            
            
        }


    }
}
