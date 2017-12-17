using System;
using System.Web;
using System.IO;
using Commons.Collections;
using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;
using Jqpress.Framework.Cache;
namespace Jqpress.Framework.ViewEngine.NVelocityEngine
{
   public class NVelocityHelper
    {
        private VelocityEngine velocity = null;
        private IContext context = null;
        /// <summary>
        /// 模板缓存时间 秒
        /// </summary>
        private int templateCacheTime = 5 * 60;
        /// <summary>
        /// 模板文件路径
        /// </summary>
        private string templatePath;

        public NVelocityHelper(string templatePath)
        {
            this.templatePath = templatePath;
            velocity = new VelocityEngine();

            //使用设置初始化VelocityEngine
            ExtendedProperties props = new ExtendedProperties();

            props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, templatePath);
            props.AddProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");
            props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_CACHE, true); //是否缓存 
            props.AddProperty("file.resource.loader.modificationCheckInterval", (Int64)30); //缓存时间(秒)

            //   props.AddProperty(RuntimeConstants.OUTPUT_ENCODING, "gb2312");
            //    props.AddProperty(RuntimeConstants.RESOURCE_LOADER, "file");

            //  props.SetProperty(RuntimeConstants.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl\\,NVelocity");

            velocity.Init(props);
            //RuntimeConstants.RESOURCE_MANAGER_CLASS 
            //为模板变量赋值
            context = new VelocityContext();

        }

        /// <summary>
        /// 给模板变量赋值
        /// </summary>
        /// <param name="key">模板变量</param>
        /// <param name="value">模板变量值</param>
        public void Put(string key, object value)
        {
            //if (context == null)
            //    context = new VelocityContext();
            context.Put(key, value);
        }

        /// <summary>
        /// 生成字符
        /// </summary>
        /// <param name="templatFileName">模板文件名</param>
        public string BuildString(string templateFile)
        {
            //从文件中读取模板
            // NVelocity.Template template = velocity.GetTemplate(templateFile);
            NVelocity.Template template = GetTemplate(templateFile);

            //合并模板
            StringWriter writer = new StringWriter();
            template.Merge(context, writer);
            return writer.ToString();
        }

        /// <summary>
        /// 缓存模板对象
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public NVelocity.Template GetTemplate(string templateFile)
        {
            CacheProvider cache = CacheProvider.GetCacheService();
            templatePath = templatePath.Replace("\\", "").Replace(":", "").Replace("_","");
           // string xpath = String.Format("/Template/{0}/{1}", templatePath, templateFile);
            string xpath = String.Format("/Template/{0}", templateFile);
            NVelocity.Template template = (NVelocity.Template)cache.RetrieveObject(xpath);
            if (template == null)
            {
                //VelocityEngine velocity = new VelocityEngine();
                //ExtendedProperties props = new ExtendedProperties();
                //props.AddProperty(RuntimeConstants.RESOURCE_LOADER, "file");
                //props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, templatePath);
                //props.AddProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");
                //props.AddProperty(RuntimeConstants.OUTPUT_ENCODING, "utf-8");
                //velocity.Init(props);
                template = velocity.GetTemplate(templateFile);

                if (template == null)
                    throw new NullReferenceException("模板目录不存在。");

                cache.AddObject(xpath, template, templateCacheTime);
            }
            return template;
        }

        /// <summary>
        /// 显示模板
        /// </summary>
        /// <param name="templatFileName">模板文件名</param>
        public void Display(string templateFile, NVelocityHelper th)
        {

            //全局
            // th.Put(TagFields.QUERY_COUNT, querycount);
            //th.Put(TagFields.PROCESS_TIME, DateTime.Now.Subtract(starttick).TotalMilliseconds / 1000);

            // HttpContext.Current.Response.Clear();
            //   HttpContext.Current.Response.Write(writer.ToString());
            HttpContext.Current.Response.Write(th.BuildString(templateFile));

            //  HttpContext.Current.Response.End();
        }
    }
}
