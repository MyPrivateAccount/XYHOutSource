using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Distributed
{
    public static class DistributedCacheExtension
    {
        public static T Get<T>(this IDistributedCache cache, string key)
            where T : class, new()
        {
            byte[] data = cache.Get(key);
            if (data == null || data.Length == 0)
                return default(T);

            T obj = null;
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new MemoryStream(data))
            {
                obj = (T)formatter.Deserialize(stream);
            }


            return obj;
        }

        public async static Task<T> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken token = default(CancellationToken))
            where T : class, new()
        {
            byte[] data = await cache.GetAsync(key, token);
            if (data == null || data.Length == 0)
                return default(T);

            T obj = null;
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new MemoryStream(data))
            {
                obj = (T)formatter.Deserialize(stream);
            }


            return obj;
        }

        public static void Set<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
            where T : class, new ()
        {
            byte[] data = Serialize(value);

            cache.Set(key, data, options);

        }

        public async static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
            where T :class, new()
        {
            byte[] data = Serialize(value);

            await cache.SetAsync(key, data, options, token);
        }

        private static byte[] Serialize(object value)
        {
            byte[] data = new byte[0];
            if (value != null)
            {
                IFormatter formatter = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, value);
                    ms.Flush();
                    ms.Position = 0;
                    data = ms.ToArray();
                }
            }
            return data;
        }

    }
}
