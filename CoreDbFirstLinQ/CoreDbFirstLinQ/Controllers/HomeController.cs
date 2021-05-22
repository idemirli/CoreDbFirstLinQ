using CoreDbFirstLinQ.Data;
using CoreDbFirstLinQ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDbFirstLinQ.Controllers
{
    public class HomeController : Controller
    {
        //Giriş
        public class CustomerModel
        {
            public CustomerModel()
            {
                this.Orders = new List<OrderModel>();
            }
            public string CustomerId { get; set; }
            public string CustomerName { get; set; }
            public int OrderCount { get; set; }
            public List<OrderModel> Orders { get; set; }
        }

        public class OrderModel
        {
            public int OrderId { get; set; }
            public decimal Total { get; set; }
            public List<ProductModel> Products { get; set; }
        }

        public class ProductModel
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public decimal? Price { get; set; }
            public int Quantity { get; set; }
        }


        public IActionResult Index()
        {
            using (var db = new NorthwindContext())
            {
                #region [SELECT SORGULARI]
                //  DbSelect(db);             //Select sorguları
                #endregion

                #region [WHERE SORGULARI]
                //   DbWhere(db);              //Where Sorguları
                #endregion

                #region [TAKE-SKIP KULLANIMI]
                //  DbTakeSkip(db);           //Take-Skip Kullanımı
                #endregion

                #region [SIRALAMA HESAPLAMA İŞLEMLERİ]
                //  DbSiralamaHesaplama(db);  //Siralama hesaplama işlemleri
                #endregion


                //***********************


                #region [KAYIT İŞLEMLERİ]

                #region[KAYIT EKLEME]
                //  KayitEkle(db);
                #endregion


                #region [KAYIT GÜNCELLEME]
                //  KayitGuncelle(db);
                #endregion


                #region [CHANGE TRACKING]
                //  var product1 = db.Products.AsNoTracking().FirstOrDefault();  //AsNoTracking kullandığımda bu nesne üzerinde güncelleme yaparsam db ye yansımayacak.
                #endregion


                #region [KAYIT SİLME]

                //   KayitSilme(db);

                #endregion

                #endregion


                #region [BiRDEN FAZLA TABLO İLE ÇALIŞMA]

                //BirdenFazlaTabloIleCalisma_1(db);

                //BirdenFazlaTabloIleCalisma_2(db);
                #endregion


                //************************

                #region [KLASİK SQL SORGULARININ ENTİTY FRAMEWORK İLE KULLANILMASI]

                #region [DELETE - UPDATE]
                var sonuc = db.Database.ExecuteSqlRaw("delete from Products where productId=81");  //sonuc = 1 veya 0  gelir.
                #endregion

                #region [SELECT]
                var query = 4;
                var products = db.Products.FromSqlRaw("select * from Products where categortyId=4").ToList();
                var products2 = db.Products.FromSqlRaw($"select * from Products where categortyId={query}").ToList();   // * kullanmak zorunlu , spesifik olarak kolon seçiminde hata alınır.
                #endregion

                #region [CustomContext Oluşturulup Kullanılması]
                using (var customDb=new CustomNorthwindContext())
                {
                    var products3 = customDb.ProductModel.FromSqlRaw("select ProductId,ProductName ,UnitPrice  from Products").ToList();  // CustomNorthwindContext içinde OnModelCreating'te mapping yapmamız gerekiyor.
                }
                #endregion

                #endregion
            }

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static void DbSelect(NorthwindContext db)
        {
            var products = db.Products.ToList();
            var productsSelect = db.Products.Select(x => x.ProductName).ToList(); //Tek kolon almak için
            var productsSelect2 = db.Products.Select(x => new { x.ProductName, x.UnitPrice }).ToList();  //Birden fazla kolon almak için
            var productsSelect3 = db.Products.Select(x => new ProductModel
            {
                Name = x.ProductName,
                Price = x.UnitPrice
            }).ToList();                      //Birden fazla kolonu kendi modelimizin içine almak için

            var product = db.Products.First();  //Tek product döndürmek için, kayıt gelmezse exception alırız.
            var product2 = db.Products.FirstOrDefault();  //Tek product döndürmek için, kayıt gelmezse exception almaz.

            foreach (var p in products)
            {
                string productName = p.ProductName;
            }
        }

        public static void DbWhere(NorthwindContext db)
        {
            var products = db.Products.Where(x => x.UnitPrice > 18).ToList();  //18'den büyük fiyat bilgileri
            var productsWhere = db.Products.Select(x => new { x.ProductName, x.UnitPrice }).Where(x => x.UnitPrice > 18).ToList();  //Birden fazla kolon almak için
            var productsWhere2 = db.Products.Select(x => new { x.ProductName, x.UnitPrice }).Where(x => x.UnitPrice > 18 && x.UnitPrice < 30).ToList();  //Her iki koşulu sağlanmalı (AND)
            var productsWhere3 = db.Products.Select(x => new { x.ProductName, x.UnitPrice }).Where(x => x.UnitPrice > 18 || x.UnitPrice < 30).ToList();  //İki koşuldan biri sağlanıyor (OR)
            var productsWhere4 = db.Products.Where(x => x.ProductName.Contains("Ch")).ToList(); //Ch içeren kayıtları getirir.
        }

        public static void DbTakeSkip(NorthwindContext db)
        {
            //Take-Skip kullanımı
            var products = db.Products.Take(5).ToList();  //İlk 5 kayıt alınır.
            var products2 = db.Products.Skip(5).Take(5).ToList();  //İkinci 5 kaydı alır.
        }

        public static void DbSiralamaHesaplama(NorthwindContext db)
        {
            //Sıralama ve Jesaplama Sorguları
            var result = db.Products.Count(i => i.UnitPrice > 10 && i.UnitPrice < 30);  //Unit Price değeri 10 ve 30 arasında olan kaç kayıt var.
            var result2 = db.Products.Count(i => !i.Discontinued);
            var result3 = db.Products.Min(i => i.UnitPrice);  //Minimum fiyat
            var result4 = db.Products.Max(i => i.UnitPrice);  //Max fiyat
            var result5 = db.Products.Where(x => x.CategoryId == 1).Max(i => i.UnitPrice);  //1 numaralı kategorideki Max fiyat
            var result6 = db.Products.Where(x => !x.Discontinued).Average(i => i.UnitPrice);  //Satışta olan ürünlerin ortalama fiyatı
            var result7 = db.Products.Where(x => !x.Discontinued).Sum(i => i.UnitPrice);  //Satışta olan ürünlerin toplam fiyatı
            var result8 = db.Products.OrderBy(i => i.UnitPrice).ToList(); //Artan fiyata göre sıralama yapar.
            var result9 = db.Products.OrderByDescending(i => i.UnitPrice).ToList(); //Azalan fiyata göre sıralama yapar.
        }


        private static void KayitSilme(NorthwindContext db)
        {
            // 1. yol
            var product3 = db.Products.Find(88);
            if (product3 != null)
            {
                db.Products.Remove(product3);
                db.SaveChanges();
            }

            // 2.yol
            var product4 = new Product() { ProductId = 47 };
            db.Entry(product4).State = EntityState.Deleted;  //change tracking çalıştı , select atmadık.
            db.SaveChanges();


            // 3.yol
            var product6 = new Product() { ProductId = 75 };
            var product7 = new Product() { ProductId = 76 };
            var lstProducts = new List<Product>() { product6, product7 };
            db.Products.RemoveRange(lstProducts);
            db.SaveChanges();
        }

        private static void KayitGuncelle(NorthwindContext db)
        {
            // 1. yol
            var product = db.Products.Where(x => x.ProductId == 1).FirstOrDefault();
            if (product != null)
            {
                product.UnitsInStock += 10;
                db.SaveChanges();
            }
            //


            // 2.yol
            var product2 = new Product() { ProductId = 1 };
            db.Products.Attach(product2);   //change tracking çalışıyor ve Select atmak zorunda kalmıyoruz.
            product2.UnitsInStock = 50;
            db.SaveChanges();
        }

        private static void KayitEkle(NorthwindContext db)
        {
            Product p = new Product() { ProductName = "Deneme" };
            db.Products.Add(p);
            db.SaveChanges();

            //Liste olarak Ekleme
            Product p1 = new Product() { ProductName = "Deneme1" };
            Product p2 = new Product() { ProductName = "Deneme2" };
            List<Product> lstProduct = new List<Product>() { p1, p2 };
            db.Products.AddRange(lstProduct);
            db.SaveChanges();
        }


        private static void BirdenFazlaTabloIleCalisma_1(NorthwindContext db)
        {
            //var products = db.Products.Where(p => p.CategoryId == 1).ToList();
            //var products = db.Products.Include(p=>p.Category).Where(p => p.Category.CategoryName == "Beverages").ToList();
            //var products = db.Products
            //    .Where(p => p.Category.CategoryName == "Beverages")
            //    .Select(p=>new
            //    {
            //        name = p.ProductName,
            //        id = p.CategoryId,
            //        categoryname =p.Category.CategoryName
            //    })
            //    .ToList();

            //var categories = db.Categories.Where(c => c.Products.Count() == 0).ToList();
            //var categories = db.Categories.Where(c => c.Products.Any()).ToList();

            //var products = db.Products
            //    .Select(p =>                     
            //        new {
            //            companyName = p.Supplier.CompanyName,
            //            contactName = p.Supplier.ContactName,
            //            p.ProductName
            //        }).ToList();

            // extension methods
            // query expressions

            //var products = (from p in db.Products
            //                where p.UnitPrice>10
            //               select p).ToList();

            var products = (from p in db.Products
                            join s in db.Suppliers on p.SupplierId equals s.SupplierId
                            select new
                            {
                                p.ProductName,
                                contactName = s.ContactName,
                                companyName = s.CompanyName
                            }).ToList();


            foreach (var item in products)
            {
                Console.WriteLine(item.ProductName + " " + item.companyName + " " + item.contactName);
            }
        }

        private static void BirdenFazlaTabloIleCalisma_2(NorthwindContext db)
        {

            // Müşterilerin verdiği sipariş toplamı ?

            var customers = db.Customers
                .Where(cus => cus.CustomerId == "PERIC")
                .Select(cus => new CustomerModel
                {
                    CustomerId = cus.CustomerId,
                    CustomerName = cus.ContactName,
                    OrderCount = cus.Orders.Count,
                    Orders = cus.Orders.Select(order => new OrderModel
                    {
                        OrderId = order.OrderId,
                        Total = order.OrderDetails.Sum(od => od.Quantity * od.UnitPrice),
                        Products = order.OrderDetails.Select(od => new ProductModel
                        {
                            ProductId = od.ProductId,
                            Name = od.Product.ProductName,
                            Price = od.UnitPrice,
                            Quantity = od.Quantity
                        }).ToList()
                    }).ToList()
                })
                .OrderBy(i => i.OrderCount)
                .ToList();

            foreach (var customer in customers)
            {
                Console.WriteLine(customer.CustomerId + "=>" + customer.CustomerName + " => " + customer.OrderCount);
                Console.WriteLine("Siparişler");
                foreach (var order in customer.Orders)
                {
                    Console.WriteLine("*****************************");
                    Console.WriteLine(order.OrderId + "=>" + order.Total);
                    foreach (var product in order.Products)
                    {
                        Console.WriteLine(product.ProductId + "=>" + product.Name + "=>" + product.Price + "=>" + product.Quantity);
                    }
                }
            }
            Console.ReadLine();
        }

    }
}