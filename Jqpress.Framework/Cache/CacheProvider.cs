/*
 * 缓存方式扩展，缓存Tag(命名空间)的实现
 * 给TagKey分组,不能分多层组
 * http://huacnlee.com/blog/group-caches-with-tag-or-namespace
 * 
 * 
 * 再论缓存删除 - 用正则匹配 CacheKey 批量删除缓存
 * http://huacnlee.com/blog/howto-remove-cache-by-regular-expressions\.
 * 
 * 
 * memcached批量删除方案探讨
 * http://it.dianping.com/memcached_item_batch_del.htm
 * 
 * 
 * 效率：tag>cache>regex>xml
 * 不需要清除的缓存越多,cache 相对效率越低
 * cache和regex 原理一样，效率接近,Key越大，cache 越快
 * cache,tag:  key<10时:tag 与cache 差距极大， key=10时:tag 快20倍以上，key=20时:tag 快10倍以上，key=50时:tag 快5倍以上，key=100时:tag 快4倍左右， key=200时:tag 快3倍左右，  key=500时:tag快一倍左右， key=1000时: tag 略快， key=5000:当有其它缓存时，tag 明显要快， key=10000 接近 ，key>20000 cache 略快
 * 单台用cache,多台用tag
 */
using System;
using System.Xml;

namespace Jqpress.Framework.Cache
{
    public class CacheProvider
    {
        private static XmlElement objectXmlMap;
        private static ICacheStrategy cs;
        private static volatile CacheProvider instance = null;
        private static object lockHelper = new object();
        private static XmlDocument rootXml = new XmlDocument();

        /// <summary>
        /// 是否使用memcached
        /// </summary>
        private static bool applyMemCached = false;
        /// <summary>
        /// 是否使用redis
        /// </summary>
        private static bool applyRedis = false;

        private static ICacheStrategy cachedStrategy;

        /// <summary>
        /// 构造函数
        /// </summary>
        private CacheProvider()
        {
            //if (MemCachedConfigs.GetConfig() != null && MemCachedConfigs.GetConfig().ApplyMemCached)
            //    applyMemCached = true;
            //if (RedisConfigs.GetConfig() != null && RedisConfigs.GetConfig().ApplyRedis)
            //    applyRedis = true;

            if (applyMemCached || applyRedis)
            {
                try
                {
                    cs = cachedStrategy = (ICacheStrategy)Activator.CreateInstance(Type.GetType("MemCache.EntLib." + (applyMemCached ? "MemCachedStrategy" : "RedisStrategy") + ", MemCache.EntLib", false, true));
                }
                catch
                {
                    throw new Exception("请检查MemCache.EntLib.dll文件是否被放置在bin目录下并配置正确");
                }
            }
            else
            {
                cs = new DefaultCacheStrategy();
                if (rootXml.HasChildNodes)
                    rootXml.RemoveAll();

                objectXmlMap = rootXml.CreateElement("Cache");
                //建立内部XML文档.
                rootXml.AppendChild(objectXmlMap);
            }    
            
        }

        /// <summary>
        /// 单体模式返回当前类的实例
        /// </summary>
        /// <returns></returns>
        public static CacheProvider GetCacheService()
        {
            if (instance == null)
            {
                lock (lockHelper)
                {
                    if (instance == null)
                    {
                        instance = new CacheProvider();
                    }
                }
            }
            return instance;
        }    
       

        /// <summary>
        /// 在XML映射文档中的指定路径,加入当前对象信息
        /// </summary>
        /// <param name="xpath">分级对象的路径 </param>
        /// <param name="o">被缓存的对象</param>
        public virtual void AddObject(string xpath, object o)
        {
            lock (lockHelper)
            {
                if (applyMemCached || applyRedis)
                {
                    //向缓存加入新的对象
                    cs.AddObject(xpath, o);
                }
                else
                {
                    //当缓存到期时间为0或负值,则不再放入缓存
                    if (cs.TimeOut <= 0) return;

                    //整理XPATH表达式信息
                    string newXpath = PrepareXpath(xpath);
                    int separator = newXpath.LastIndexOf("/");
                    //找到相关的组名
                    string group = newXpath.Substring(0, separator);
                    //找到相关的对象
                    string element = newXpath.Substring(separator + 1);

                    XmlNode groupNode = objectXmlMap.SelectSingleNode(group);

                    //建立对象的唯一键值, 用以映射XML和缓存对象的键
                    string objectId = "";

                    XmlNode node = objectXmlMap.SelectSingleNode(PrepareXpath(xpath));
                    if (node != null)
                    {
                        objectId = node.Attributes["objectId"].Value;
                    }

                    if (objectId == "")
                    {
                        groupNode = CreateNode(group);
                        objectId = Guid.NewGuid().ToString();
                        //建立新元素和一个属性 for this perticular object
                        XmlElement objectElement = objectXmlMap.OwnerDocument.CreateElement(element);
                        XmlAttribute objectAttribute = objectXmlMap.OwnerDocument.CreateAttribute("objectId");
                        objectAttribute.Value = objectId;
                        objectElement.Attributes.Append(objectAttribute);
                        //为XML文档建立新元素
                        groupNode.AppendChild(objectElement);
                    }
                    else
                    {
                        //建立新元素和一个属性 for this perticular object
                        XmlElement objectElement = objectXmlMap.OwnerDocument.CreateElement(element);
                        XmlAttribute objectAttribute = objectXmlMap.OwnerDocument.CreateAttribute("objectId");
                        objectAttribute.Value = objectId;
                        objectElement.Attributes.Append(objectAttribute);
                        //为XML文档建立新元素
                        groupNode.ReplaceChild(objectElement, node);
                    }

                    //向缓存加入新的对象
                    cs.AddObject(objectId, o);
                }
            }
        }

        /// <summary>
        /// 在XML映射文档中的指定路径,加入当前对象信息
        /// </summary>
        /// <param name="xpath">分级对象的路径 </param>
        /// <param name="o">被缓存的对象</param>
        /// <param name="o">到期时间,单位:秒</param>
        public virtual void AddObject(string xpath, object o, int expire)
        {
            lock (lockHelper)
            {
                if (applyMemCached || applyRedis)
                {
                    //向缓存加入新的对象
                    cs.AddObject(xpath, o, expire);
                }
                else
                {
                    //当缓存到期时间为0或负值,则不再放入缓存
                    if (cs.TimeOut <= 0) return;

                    //整理XPATH表达式信息
                    string newXpath = PrepareXpath(xpath);
                    int separator = newXpath.LastIndexOf("/");
                    //找到相关的组名
                    string group = newXpath.Substring(0, separator);
                    //找到相关的对象
                    string element = newXpath.Substring(separator + 1);

                    XmlNode groupNode = objectXmlMap.SelectSingleNode(group);

                    //建立对象的唯一键值, 用以映射XML和缓存对象的键
                    string objectId = "";

                    XmlNode node = objectXmlMap.SelectSingleNode(PrepareXpath(xpath));
                    if (node != null)
                    {
                        objectId = node.Attributes["objectId"].Value;
                    }

                    if (objectId == "")
                    {
                        groupNode = CreateNode(group);
                        objectId = Guid.NewGuid().ToString();
                        //建立新元素和一个属性 for this perticular object
                        XmlElement objectElement = objectXmlMap.OwnerDocument.CreateElement(element);
                        XmlAttribute objectAttribute = objectXmlMap.OwnerDocument.CreateAttribute("objectId");
                        objectAttribute.Value = objectId;
                        objectElement.Attributes.Append(objectAttribute);
                        //为XML文档建立新元素
                        groupNode.AppendChild(objectElement);
                    }
                    else
                    {
                        //建立新元素和一个属性 for this perticular object
                        XmlElement objectElement = objectXmlMap.OwnerDocument.CreateElement(element);
                        XmlAttribute objectAttribute = objectXmlMap.OwnerDocument.CreateAttribute("objectId");
                        objectAttribute.Value = objectId;
                        objectElement.Attributes.Append(objectAttribute);
                        //为XML文档建立新元素
                        groupNode.ReplaceChild(objectElement, node);
                    }

                    //向缓存加入新的对象
                    cs.AddObject(objectId, o, expire);
                }
            }
        }

        /// <summary>
        /// 在XML映射文档中的指定路径,加入当前对象信息
        /// </summary>
        /// <param name="xpath">分级对象的路径 </param>
        /// <param name="o">被缓存的对象</param>
        public virtual void AddObject(string xpath, object o, string[] files)
        {
            xpath = xpath.Replace(" ", "_SPACE_");    //如果xpath中出现空格，则将空格替换为_SPACE_
            lock (lockHelper)
            {
                if (applyMemCached || applyRedis)
                {
                    //向缓存加入新的对象
                    cs.AddObject(xpath, o);
                }
                else
                {
                    //当缓存到期时间为0或负值,则不再放入缓存
                    if (cs.TimeOut <= 0) return;

                    //整理XPATH表达式信息
                    string newXpath = PrepareXpath(xpath);
                    int separator = newXpath.LastIndexOf("/");
                    //找到相关的组名
                    string group = newXpath.Substring(0, separator);
                    //找到相关的对象
                    string element = newXpath.Substring(separator + 1);

                    XmlNode groupNode = objectXmlMap.SelectSingleNode(group);
                    //建立对象的唯一键值, 用以映射XML和缓存对象的键
                    string objectId = "";

                    XmlNode node = objectXmlMap.SelectSingleNode(PrepareXpath(xpath));
                    if (node != null)
                    {
                        objectId = node.Attributes["objectId"].Value;
                    }
                    if (objectId == "")
                    {
                        groupNode = CreateNode(group);
                        objectId = Guid.NewGuid().ToString();
                        //建立新元素和一个属性 for this perticular object
                        XmlElement objectElement = objectXmlMap.OwnerDocument.CreateElement(element);
                        XmlAttribute objectAttribute = objectXmlMap.OwnerDocument.CreateAttribute("objectId");
                        objectAttribute.Value = objectId;
                        objectElement.Attributes.Append(objectAttribute);
                        //为XML文档建立新元素
                        groupNode.AppendChild(objectElement);
                    }
                    else
                    {
                        //建立新元素和一个属性 for this perticular object
                        XmlElement objectElement = objectXmlMap.OwnerDocument.CreateElement(element);
                        XmlAttribute objectAttribute = objectXmlMap.OwnerDocument.CreateAttribute("objectId");
                        objectAttribute.Value = objectId;
                        objectElement.Attributes.Append(objectAttribute);
                        //为XML文档建立新元素
                        groupNode.ReplaceChild(objectElement, node);
                    }

                    //向缓存加入新的对象
                    cs.AddObjectWithFileChange(objectId, o, files);
                }
            }
        }

#if NET4 
        private static Hashtable htMapFile = new Hashtable();
#endif

        /// <summary>
        /// 取得指定XML路径下的数据项
        /// </summary>
        /// <param name="xpath">分级对象的路径</param>
        /// <returns></returns>
        public virtual object RetrieveObject(string xpath)
        {
            try
            {
#if NET4       
                if (GeneralConfigs.GetConfig().Webgarden > 1 && Environment.Version.Major >= 4)
                {
                    //.net4框架下基于mmap实现跨进程共享信息，来实现当前web园进程内缓存更新后，其它web园进程无法得到信息已修改的标记
                    //方法摘要：通过htMapFile表记录共享内存的文件信息，这样可以提升访问共享信息的命中率（之前直接声明的方式命中率非常低且容易过多申请共享内存造成内存紧张）
                    //通过在共享内存中保存进程ID的方式，如果当前进程ID未出现在共享内存中，则直接将进程ID放到内享内存中，同时返回NULL，这样前端就会从数据库或文件中再次载入数据。
                    //如当前进程ID出现在了共享内存中，则标识该进程中的当前键值的缓存数据已更新过，则直接从缓存中获取数据并返回该数据信息。
                    lock (lockHelper)
                    {
                        //强制移除缓存（将共享内存中数据清空）后，查看指定缓存键的共享内存数据变化
                        //if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["removecache"]))
                        //    RemoveObject("/xzsyw/TemplateIDList");

                        MemoryMappedFile file = htMapFile[xpath] as MemoryMappedFile;
                        if (file == null)
                        {
                            file = MemoryMappedFile.CreateOrOpen(xpath, 512, MemoryMappedFileAccess.ReadWrite);// MemoryMappedFileOptions.DelayAllocatePages, new MemoryMappedFileSecurity(), HandleInheritability.Inheritable);
                            htMapFile.Add(xpath, file);
                        }
                        int processId = System.Diagnostics.Process.GetCurrentProcess().Id;
                        using (BinaryReader br = new BinaryReader(file.CreateViewStream()))
                        {
                            string brstr = br.ReadString().Trim().Replace("none", "");
                            if (!brstr.Contains("_" + processId + "_"))
                            {
                                using (BinaryWriter bw = new BinaryWriter(file.CreateViewStream()))
                                {
                                    bw.Write(Utils.CutString("_" + processId + "_" + brstr, 0, 512));
                                }
                                if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["showdetail"]))
                                    System.Web.HttpContext.Current.Response.Write("<br/>write xpath: " + xpath + "  process :" + processId + ", old process: " + brstr);
                                return null;
                            }
                            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["showdetail"]))
                                System.Web.HttpContext.Current.Response.Write("<br/>output write xpath: " + xpath + "  process :" + processId + ", old process: " + brstr);
                        }
                    }
                }
#endif
                if (applyMemCached || applyRedis)
                {
                    //向缓存加入新的对象
                    return cs.RetrieveObject(xpath);
                }
                else
                {
                    XmlNode node = objectXmlMap.SelectSingleNode(PrepareXpath(xpath));
                    if (node != null)
                        return cs.RetrieveObject(node.Attributes["objectId"].Value);

                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 通过指定的路径删除缓存中的对象
        /// </summary>
        /// <param name="xpath">分级对象的路径</param>
        public virtual void RemoveObject(string xpath)
        {
            lock (lockHelper)
            {
                try
                {
#if NET4      
                    if (GeneralConfigs.GetConfig().Webgarden > 1 && Environment.Version.Major >= 4)
                    {
                        //.net4框架下基于mmap实现跨进程共享信息，来实现当前web园进程内缓存更新后，其它web园进程无法得到信息已修改的标记
                        //方法摘要：通过htMapFile表记录共享内存的文件信息，这样可以提升访问共享信息的命中率（之前直接声明的方式命中率非常低且容易过多申请共享内存造成内存紧张）
                        //通过直接置空共享内存中数据(写入"none")，这样当别的进程再访问该共享内存时，发现共享内存中已为空（"即当前进程缓存数据要重新加载",详情参见上面的RetrieveObject(string xpath)）        
                        MemoryMappedFile file = htMapFile[xpath] as MemoryMappedFile;
                        if (file == null)
                        {
                            file = MemoryMappedFile.CreateOrOpen(xpath, 512, MemoryMappedFileAccess.ReadWrite);// MemoryMappedFileOptions.DelayAllocatePages, new MemoryMappedFileSecurity(), HandleInheritability.Inheritable);
                            htMapFile.Add(xpath, file);
                        }
                        using (BinaryWriter bw = new BinaryWriter(file.CreateViewStream()))
                        {
                            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["showdetail"]))
                                System.Web.HttpContext.Current.Response.Write("<br/>xpath: " + xpath);
                            bw.Write("none");
                        }
                    }
#endif
                    if (applyMemCached || applyRedis)
                    {
                        //移除相应的缓存项
                        cs.RemoveObject(xpath);
                    }
                    else
                    {
                        XmlNode result = objectXmlMap.SelectSingleNode(PrepareXpath(xpath));
                        //检查路径是否指向一个组或一个被缓存的实例元素
                        if (result.HasChildNodes)
                        {
                            //删除所有对象和子结点的信息
                            XmlNodeList objects = result.SelectNodes("*[@objectId]");
                            string objectId = "";
                            foreach (XmlNode node in objects)
                            {
                                objectId = node.Attributes["objectId"].Value;
                                node.ParentNode.RemoveChild(node);
                                //删除对象
                                cs.RemoveObject(objectId);
                            }
                        }
                        else
                        {
                            //删除元素结点和相关的对象
                            string objectId = result.Attributes["objectId"].Value;
                            result.ParentNode.RemoveChild(result);
                            cs.RemoveObject(objectId);
                        }
                    }

                }
                catch//如出错误表明当前路径不存在
                { }
            }
        }

        /// <summary>
        /// 返回指定ID的对象 泛型接口
        /// </summary>
        /// <param name="objId"></param>
        /// <typeparam name="T">返回数据的类型</typeparam>
        /// <returns></returns>
        public virtual T RetrieveObject<T>(string objId)
        {
            object o = RetrieveObject(objId);
            return o != null ? (T)o : default(T);
        }
        /// <summary>
        /// 对象树形分级对象节点
        /// </summary>
        /// <param name="xpath">分级路径 location</param>
        /// <returns></returns>
        private XmlNode CreateNode(string xpath)
        {
            lock (lockHelper)
            {
                string[] xpathArray = xpath.Split('/');
                string root = "";
                XmlNode parentNode = objectXmlMap;
                //建立相关节点
                for (int i = 1; i < xpathArray.Length; i++)
                {
                    XmlNode node = objectXmlMap.SelectSingleNode(root + "/" + xpathArray[i]);
                    // 如果当前路径不存在则建立,否则设置当前路径到它的子路径上
                    if (node == null)
                    {
                        XmlElement newElement = objectXmlMap.OwnerDocument.CreateElement(xpathArray[i]);
                        parentNode.AppendChild(newElement);
                    }
                    //设置低一级的路径
                    root = root + "/" + xpathArray[i];
                    parentNode = objectXmlMap.SelectSingleNode(root);
                }
                return parentNode;
            }
        }

        /// <summary>
        /// 整理 xpath 确保 '/'被删除 is removed
        /// </summary>
        /// <param name="xpath">分级地址</param>
        /// <returns></returns>
        private string PrepareXpath(string xpath)
        {
            lock (lockHelper)
            {
                string[] xpathArray = xpath.Split('/');
                xpath = "/Cache";
                foreach (string s in xpathArray)
                {
                    if (s != "")
                    {
                        xpath = xpath + "/" + s;
                    }
                }
                return xpath;
            }
        }

        /// <summary>
        /// 加载指定的缓存策略
        /// </summary>
        /// <param name="ics"></param>
        public void LoadCacheStrategy(ICacheStrategy ics)
        {
            lock (lockHelper)
            {    
                cs = ics;
            }
        }

        /// <summary>
        /// 加载默认的缓存策略
        /// </summary>
        public void LoadDefaultCacheStrategy()
        {
            lock (lockHelper)
            {
                //当使用MemCached或redis时
                if (applyMemCached || applyRedis)
                {
                    cs = cachedStrategy;
                }
                else
                {
                    cs = new DefaultCacheStrategy();
                }
            }
        }

        /// <summary>
        /// 清空的有缓存数据, 注: 考虑效率问题，建议仅在需要时（如后台管理）使用.
        /// </summary>
        public void FlushAll()
        {
            cs.FlushAll();
        }

    }
}
