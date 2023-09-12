using System.Data;
using System.Diagnostics;
using System.Net.Http.Json;
using Dapper;
using MySqlConnector;

public class Program
{
    //private static IDbConnection divisorCache = new MySqlConnection("Server=cache-db;Database=cache-database;Uid=div-cache;Pwd=C@ch3d1v;");
    
    public static void Main()
    {
        var first = 1_000_000_000;
        var last = 1_000_000_020;

        var numberWithMostDivisors = first;
        var result = 0;

    
        
        var watch = Stopwatch.StartNew();

        var client = new HttpClient();
        client.BaseAddress = new Uri("http://cache-service/Cache");
       
        //int divNumbers = int.Parse(stringResponse.Result);



        for (var i = first; i <= last; i++)
        {
            var innerWatch = Stopwatch.StartNew();
            //my get
            var internalResponse = client.Send(new HttpRequestMessage(HttpMethod.Get, client.BaseAddress + "?number=1"));
            var internalStringResponse = internalResponse.Content.ReadAsStringAsync();
            internalStringResponse.Wait();
            Console.WriteLine("RESP: " + internalStringResponse.Result);

            var divisorCounter = int.Parse(internalStringResponse.Result);
            //var divisorCounter = divisorCache.QueryFirstOrDefault<int>("SELECT divisors FROM counters WHERE number = @number", new { number = i });


            if (divisorCounter == 0)
            {
                for (var divisor = 1; divisor <= i; divisor++)
                {
                    if (i % divisor == 0)
                    {
                        divisorCounter++;
                    }
                }

                var postResponse = client.PostAsync(client.BaseAddress + "?number=1&divisorCounter=1", null);
                //POST reference i stedet
                //divisorCache.Execute("INSERT INTO counters (number, divisors) VALUES (@number, @divisors)", new { number = i, divisors = divisorCounter });
            }   //divisorCache.Post
            
            innerWatch.Stop();
            Console.WriteLine("Counted " + divisorCounter + " divisors for " + i + " in " + innerWatch.ElapsedMilliseconds + "ms");

            if (divisorCounter > result)
            {
                numberWithMostDivisors = i;
                result = divisorCounter;
            }
        }
        watch.Stop();
        
        Console.WriteLine("The number with most divisors inside range is: " + numberWithMostDivisors + " with " + result + " divisors.");
        Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds + "ms");
        Console.ReadLine();
    }
}