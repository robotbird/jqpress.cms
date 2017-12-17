using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Blog.Data.IData;

namespace Jqpress.Blog.Data
{
    public class DatabaseProvider
    {
        private DatabaseProvider()
        { }

        private static IDataProvider _instance = null;
        private static object lockHelper = new object();

        static DatabaseProvider()
        {
            GetProvider();
        }

        private static void GetProvider()
        {
            try
            {
                string dataprovider = string.Format("Jqpress.Blog.Data.{0}.DataProvider, Jqpress.Blog", Jqpress.Framework.Configuration.ConfigHelper.DbType);
                _instance = (IDataProvider)Activator.CreateInstance(Type.GetType(dataprovider, false, true));
            }
            catch
            {
                throw new Exception("请检查web.config中Dbtype节点数据库类型是否正确，例如：SqlServer、Access、MySql,SqlLite");
            }
        }

        public static IDataProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockHelper)
                    {
                        if (_instance == null)
                        {
                            GetProvider();
                        }
                    }
                }
                return _instance;
            }
        }
        public static void ResetDbProvider()
        {
            _instance = null;
        }
    }
}
