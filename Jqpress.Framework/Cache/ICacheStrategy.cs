using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jqpress.Framework.Cache
{
    /// <summary>
    /// 公共缓存策略接口
    /// </summary>
    public interface ICacheStrategy
    {
        /// <summary>
        /// 添加指定ID的对象
        /// </summary>
        /// <param name="objId">缓存键</param>
        /// <param name="o">缓存对象</param>
        void AddObject(string objId, object o);
        /// <summary>
        /// 添加指定ID的对象
        /// </summary>
        /// <param name="objId">缓存键</param>
        /// <param name="o">缓存对象</param>
        /// <param name="expire">到期时间,单位:秒</param>
        void AddObject(string objId, object o, int expire);
        /// <summary>
        /// 添加指定ID的对象(关联指定文件组)
        /// </summary>
        /// <param name="objId">缓存键</param>
        /// <param name="o">缓存对象</param>
        /// <param name="files">关联的文件名</param>
        void AddObjectWithFileChange(string objId, object o, string[] files);
        /// <summary>
        /// 添加指定ID的对象(关联指定键值组)
        /// </summary>
        /// <param name="objId">缓存键</param>
        /// <param name="o">缓存对象</param>
        /// <param name="dependKey">依赖键</param>
        void AddObjectWithDepend(string objId, object o, string[] dependKey);
        /// <summary>
        /// 移除指定ID的对象
        /// </summary>
        /// <param name="objId">缓存键</param>
        void RemoveObject(string objId);
        /// <summary>
        /// 返回指定ID的对象
        /// </summary>
        /// <param name="objId">缓存键</param>
        /// <returns></returns>
        object RetrieveObject(string objId);
        /// <summary>
        /// 返回指定ID的对象 泛型接口
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        T RetrieveObject<T>(string objId);
        /// <summary>
        /// 到期时间,单位：秒
        /// </summary>
        int TimeOut { set; get; }
        /// <summary>
        /// 清空的有缓存数据
        /// </summary>
        void FlushAll();
    }
}
