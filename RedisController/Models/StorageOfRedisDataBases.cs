using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisController;
using RedisController.Models;
using StackExchange.Redis;

namespace RedisController.Models
{
    internal class StorageOfRedisDataBases : IEnumerable
    {
        private List<RedisDataBase> redisDatabases;
        public StorageOfRedisDataBases()
        {
            redisDatabases = new List<RedisDataBase>();
        }

        public void AddDatabase(RedisDataBase database)
        {
            redisDatabases.Add(database);
        }

        public IEnumerator GetEnumerator()
        {
            return redisDatabases.GetEnumerator();
        }
    }
}
