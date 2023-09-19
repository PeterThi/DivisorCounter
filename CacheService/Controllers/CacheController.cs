using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using Dapper;
using MySqlConnector;
namespace CacheService.Controllers;

[ApiController]
[Route("[controller]")]
public class CacheController : ControllerBase
{
    private IDbConnection divisorCache = new MySqlConnection("Server=cache-db;Database=cache-database;Uid=div-cache;Pwd=C@ch3d1v;");

    public CacheController()
    {
        divisorCache.Open();
        var tables = divisorCache.Query<string>("SHOW TABLES LIKE 'counters'");
       if (!tables.Any())
        {
            divisorCache.Execute("CREATE TABLE counters (number BIGINT NOT NULL PRIMARY KEY, divisors INT NOT NULL)");
        }
    }

    [HttpGet]
    public int Get(long number) //få hvor mange divisors et enkelt nummer (param) har
    {
        
        var divisorCounter = divisorCache.QueryFirstOrDefault<int>("SELECT divisors FROM counters WHERE number = @number", new { number = number });
        return divisorCounter;
    }

    [HttpPost]
    public void Post([FromQuery] long number, [FromQuery] int divisorCounter) //opdater tabel?
    {

        divisorCache.Execute("INSERT INTO counters (number, divisors) VALUES (@number, @divisors)", new { number = number, divisors = divisorCounter });

    }
}