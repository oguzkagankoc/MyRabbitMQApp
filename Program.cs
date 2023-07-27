using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MyRabbitMQApp
{
    class Program
    {
        static List<Product> ParseJsonData(string jsonData)
        {
            // JSON veriyi C# nesnesine çeviriyoruz
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonData);

            // products alanını alıyoruz
            var products = data.products;

            // Product sınıfına çeviriyoruz
            var productList = new List<Product>();
            foreach (var product in products)
            {
                productList.Add(new Product
                {
                    Id = product.id,
                    Title = product.title,
                    Description = product.description,
                    Price = product.price,
                    DiscountPercentage = product.discountPercentage,
                    Rating = product.rating,
                    Stock = product.stock,
                    Brand = product.brand,
                    Category = product.category,
                    Thumbnail = product.thumbnail.ToString(),
                    Images = product.images.ToObject<string[]>(),
                });
            }

            return productList;
        }

        static void Main(string[] args)
        {
            // Producer ve Consumer'ı aynı anda çalıştırmak için iki ayrı Thread oluşturuyoruz.
            Thread producerThread = new Thread(() => StartProducer());
            Thread consumerThread = new Thread(() => StartConsumer());

            producerThread.Start();
            consumerThread.Start();

            // Producer ve Consumer Thread'lerinin bitmesini bekliyoruz.
            producerThread.Join();
            consumerThread.Join();

            Console.WriteLine("Uygulama tamamlandı. Çıkış için bir tuşa basın.");
            Console.ReadKey();
        }

        static void StartProducer()
        {
            // Web sitesinden veriyi alıyoruz.
            string jsonData = GetJsonDataFromWebsite("https://dummyjson.com/products");

            // Veriyi JSON objelerine ayırıyoruz
            var products = ParseJsonData(jsonData);

            // Ayırdığımız JSON objelerini kuyruğa gönderiyoruz
            var producer = new Producer();
            int index = 1;
            foreach (var product in products)
            {
                string productJson = Newtonsoft.Json.JsonConvert.SerializeObject(product);
                producer.SendMessage(productJson, $"product_{index}");
                index++;
            }
        }


        static void StartConsumer()
        {
            var consumer = new Consumer();
            consumer.StartConsuming();
        }

        static string GetJsonDataFromWebsite(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();
                    return response.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Veri alınırken hata oluştu: " + ex.Message);
                    return null;
                }
            }
        }
    }
}
