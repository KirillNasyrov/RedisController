using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisController;
using RedisController.Models;
using StackExchange.Redis;

namespace RedisController.Models;

public class RedisDataBase
{
    
    public IDatabase DataBase { get; set; }

    public RedisDataBase(IDatabase dataBase)
    {
        DataBase = dataBase;
    }

}

