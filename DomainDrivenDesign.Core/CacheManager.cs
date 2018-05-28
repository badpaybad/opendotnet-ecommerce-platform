using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DomainDrivenDesign.Core
{
    public static class CacheManager
    {
        public class CacheItem
        {
            public string Key;
            public DateTime CreatedDate;
            public DateTime ExpiredDate;
        }

        public static readonly List<CacheItem> Keys = new List<CacheItem>();
        public static T Get<T>(string key)
        {
            var temp = HttpRuntime.Cache[key];
            if (temp == null) return default(T);

            return (T)temp;
        }

        public static void Set<T>(string key, T val, double expiredAfterSeconds = 86400)
        {
            var dn = DateTime.Now;
            var dex = DateTime.Now.AddSeconds(expiredAfterSeconds);
            HttpRuntime.Cache.Insert(key, val, null, dex, TimeSpan.Zero);

            Task.Run(() =>
            {
                lock (Keys)
                {
                    Keys.Add(new CacheItem()
                    {
                        Key = key,
                        CreatedDate = dn,
                        ExpiredDate = dex
                    });
                }
            });
        }


        public static void ClearKeys(List<string> keys)
        {
            lock (Keys)
            {
                Keys.RemoveAll(i => keys.Contains(i.Key));
            }
            foreach (var key in keys)
            {
                HttpRuntime.Cache.Remove(key);
            }
        }

        public static void ClearAll()
        {
            List<string> tempKeys;
            lock (Keys)
            {
                tempKeys = Keys.Select(i => i.Key).ToList();
            }
            foreach (var k in tempKeys)
            {
                HttpRuntime.Cache.Remove(k);
            }
            lock (Keys)
            {

                Keys.Clear();
            }
        }

        public static T GetOrSetIfNull<T>(string key, Func<T> builderFunc, double expiredAfterSeconds = 86400)
        {
            var temp = HttpRuntime.Cache[key];
            if (temp != null) return (T) temp;

            temp = builderFunc();
            if (temp == null) return default(T);

            Set(key,temp, expiredAfterSeconds);
            return (T)temp;
        }
    }
}
