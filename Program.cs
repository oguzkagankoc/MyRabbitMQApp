using System;
using System.Threading;

namespace MyRabbitMQApp
{
    class Program
    {
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
            var producer = new Producer();
            producer.SendMessage();
        }

        static void StartConsumer()
        {
            var consumer = new Consumer();
            consumer.StartConsuming();
        }
    }
}
