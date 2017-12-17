using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace Jqpress.Framework.Cache
{
    public class CacheHelper
    {
        private CacheHelper() { }

        /// <summary>
        /// CacheDependency 说明
        /// 如果您向 Cache 中添加某个具有依赖项的项，当依赖项更改时，
        /// 该项将自动从 Cache 中删除。例如，假设您向 Cache 中添加某项，
        /// 并使其依赖于文件名数组。当该数组中的某个文件更改时，
        /// 与该数组关联的项将从缓存中删除。
        /// [C#] 
        /// Insert the cache item.
        /// CacheDependency dep = new CacheDependency(fileName, dt);
        /// cache.Insert("key", "value", dep);
        /// </summary>
        //>> Based on Factor = 5 default value
        /// <summary>
        /// 天
        /// </summary>
        public static readonly int DayFactor = 17280;
        /// <summary>
        /// 小时
        /// </summary>
        public static readonly int HourFactor = 720;
        /// <summary>
        /// 分钟
        /// </summary>
        public static readonly int MinuteFactor = 12;
        /// <summary>
        /// 秒
        /// </summary>
        public static readonly double SecondFactor = 0.2;
        /// <summary>
        /// 调节
        /// </summary>
        //  private static int Factor = 0;
        private static int Factor = 5;
        private static readonly System.Web.Caching.Cache _cache;

       // private static readonly string CachePrefix = ConfigHelper.SitePrefix;
        private static readonly string CachePrefix = "";

        public static void ReSetFactor(int cacheFactor)
        {
            Factor = cacheFactor;
        }

        /// <summary>
        /// Static initializer should ensure we only have to look up the current cache
        /// instance once.
        /// </summary>
        static CacheHelper()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                _cache = context.Cache;
            }
            else
            {
                _cache = HttpRuntime.Cache;
            }

        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public static void Clear()
        {
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            ArrayList al = new ArrayList();
            while (CacheEnum.MoveNext())
            {
                al.Add(CacheEnum.Key);
            }

            foreach (string key in al)
            {
                _cache.Remove(key);
            }
        }

        public static void RemoveByPattern(string pattern)
        {
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            // Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline  );
            while (CacheEnum.MoveNext())
            {
                if (regex.IsMatch(CachePrefix + CacheEnum.Key.ToString()))
                    _cache.Remove(CachePrefix + CacheEnum.Key.ToString());
            }
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            _cache.Remove(CachePrefix + key);
        }

        /// <summary>
        /// 缓存OBJECT.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Insert(string key, object obj)
        {
            Insert(key, obj, null, 1);
        }
        /// <summary>
        /// 缓存obj 并建立依赖项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="dep"></param>
        public static void Insert(string key, object obj, CacheDependency dep)
        {
            Insert(key, obj, dep, MinuteFactor * 3);
        }
        /// <summary>
        /// 按秒缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="seconds"></param>
        public static void Insert(string key, object obj, int seconds)
        {
            Insert(key, obj, null, seconds);
        }
        /// <summary>
        /// 按秒缓存对象 并存储优先级
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="seconds"></param>
        /// <param name="priority"></param>
        public static void Insert(string key, object obj, int seconds, CacheItemPriority priority)
        {
            Insert(key, obj, null, seconds, priority);
        }
        /// <summary>
        /// 按秒缓存对象 并建立依赖项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="dep"></param>
        /// <param name="seconds"></param>
        public static void Insert(string key, object obj, CacheDependency dep, int seconds)
        {
            Insert(key, obj, dep, seconds, CacheItemPriority.Normal);
        }
        /// <summary>
        /// 按秒缓存对象 并建立具有优先级的依赖项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="dep"></param>
        /// <param name="seconds"></param>
        /// <param name="priority"></param>
        public static void Insert(string key, object obj, CacheDependency dep, int seconds, CacheItemPriority priority)
        {
            if (obj != null)
            {
                _cache.Insert(CachePrefix + key, obj, dep, DateTime.Now.AddSeconds(Factor * seconds), TimeSpan.Zero, priority, null);
            }

        }

        ///// <summary>
        ///// 插入生存周期短的缓存项目
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="obj"></param>
        ///// <param name="secondFactor"></param>
        //public static void MicroInsert(string key, object obj, int secondFactor)
        //{
        //    if (obj != null)
        //    {
        //        _cache.Insert(key, obj, null, DateTime.Now.AddSeconds(Factor * secondFactor), TimeSpan.Zero);
        //    }
        //}


        ///// <summary>
        ///// 插入生存周期很长的缓存项目，在系统运行期间永久缓存
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="obj"></param>
        //public static void MaxInsert(string key, object obj)
        //{
        //    MaxInsert(key, obj, null);
        //}

        ///// <summary>
        ///// 具有依赖项的最大时间缓存
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="obj"></param>
        ///// <param name="dep"></param>
        //public static void MaxInsert(string key, object obj, CacheDependency dep)
        //{
        //    if (obj != null)
        //    {
        //        _cache.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.AboveNormal, null);
        //    }
        //}

        ///// <summary>
        ///// Insert an item into the cache for the Maximum allowed time
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="obj"></param>
        //public static void Permanent(string key, object obj)
        //{
        //    Permanent(key, obj, null);
        //}

        //public static void Permanent(string key, object obj, CacheDependency dep)
        //{
        //    if (obj != null)
        //    {
        //        _cache.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.NotRemovable, null);
        //    }
        //}
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            return _cache[CachePrefix + key];
        }

        ///// <summary>
        ///// Return int of seconds * SecondFactor
        ///// </summary>
        //public static int SecondFactorCalculate(int seconds)
        //{
        //    // Insert method below takes integer seconds, so we have to round any fractional values
        //    return Convert.ToInt32(Math.Round((double)seconds * SecondFactor));
        //}
    }
}
