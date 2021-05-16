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


        class ProductModel
        {
            public string Name { get; set; }

            public decimal? Price { get; set; }
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
    }
}
