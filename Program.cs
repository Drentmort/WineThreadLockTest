using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimerConsoleTest
{
    internal class Program
    {
        static ManualResetEvent @event = new ManualResetEvent(false);

        static double summ = 0;
        static double count = 0;
        static object _o = new object();

        static void Main(string[] args)
        {

            Console.WriteLine("Введите число таймеров");

            var timerCountStr = Console.ReadLine();
            if (!int.TryParse(timerCountStr, out var timerCount))
            {
                timerCount = 100;
            }

            Console.WriteLine($"Число таймеров {timerCount}");

            var tList =new List<PollingClass>();
            for(int i = 0; i < timerCount; i++)
            {
                var t = new PollingClass();
                t.TimeLockHandler += time =>
                {
                    lock (_o)
                    {
                        Console.WriteLine($"{DateTime.Now} очередной вызов занял {time.TotalMilliseconds} миллисекунд");
                        summ += time.TotalMilliseconds;
                        count++;
                        Console.WriteLine($"{DateTime.Now} в среденем {summ / count} миллисекунд");
                    }
                    
                };
                tList.Add(t);
                t.Start();
                
            }           
            @event.WaitOne();
        }
    }

    
}
