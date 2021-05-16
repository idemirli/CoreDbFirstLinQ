using CoreDbFirstLinQ.Data;
using CoreDbFirstLinQ.Models;
using Microsoft.AspNetCore.Mvc;
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
            using (var db=new NorthwindContext())
            {
                DbSelect(db);  //Select sorguları
                DbWhere(db);   //Where Sorguları


                //Take-Skip kullanımı
                var products = db.Products.Take(5).ToList();  //İlk 5 kayıt alınır.
                var products2 = db.Products.Skip(5).Take(5).ToList();  //İkinci 5 kaydı alır.


                //Sıralama ve Jesaplama Sorguları
                var result = db.Products.Count(i => i.UnitPrice > 10 && i.UnitPrice < 30);  //Unit Price değeri 10 ve 30 arasında olan kaç kayıt var.
                var result2 = db.Products.Count(i => !i.Discontinued);
                var result3 = db.Products.Min(i => i.UnitPrice);  //Minimum fiyat
                var result4 = db.Products.Max(i => i.UnitPrice);  //Max fiyat
                var result5 = db.Products.Where(x=>x.CategoryId==1).Max(i => i.UnitPrice);  //1 numaralı kategorideki Max fiyat
                var result6 = db.Products.Where(x=> !x.Discontinued).Average(i => i.UnitPrice);  //Satışta olan ürünlerin ortalama fiyatı
                var result7 = db.Products.Where(x => !x.Discontinued).Sum(i => i.UnitPrice);  //Satışta olan ürünlerin toplam fiyatı
                var result8 = db.Products.OrderBy(i => i.UnitPrice).ToList(); //Artan fiyata göre sıralama yapar.
                var result9 = db.Products.OrderByDescending(i => i.UnitPrice).ToList(); //Azalan fiyata göre sıralama yapar.

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
    }
}
