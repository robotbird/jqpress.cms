using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Framework.DbProvider;
using Jqpress.Framework.Configuration;
using Jqpress.Core.Domain;
using Jqpress.Core.Domain.Enum;
using Jqpress.Core.Repositories.IRepository;

namespace Jqpress.Core.Repositories.Repository
{
    public partial class LinkRepository:ILinkRepository
    {
        
        /// <summary>
        /// insert link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public int Insert(LinkInfo link)
        {
            string cmdText = string.Format(@"insert into [{0}links]
                            (
                            [type],[linkname],[linkurl],[position],[target],[description],[sortnum],[status],[createtime]
                            )
                            values
                            (
                            @type,@linkname,@linkurl,@position,@target,@description,@sortnum,@status,@createtime
                            )", ConfigHelper.Tableprefix);

            using (var conn = new DapperHelper().OpenConnection())
            {
                conn.Execute(cmdText, link);
                //return conn.Query<int>(string.Format("select top 1 [linkid] from [{0}links]  order by [linkid] desc", ConfigHelper.Tableprefix), null).First();
                return conn.Query<int>(string.Format("select  [linkid] from [{0}links]  order by [linkid] desc limit 1", ConfigHelper.Tableprefix), null).First();//for sqlite
            }

        }
        /// <summary>
        /// update link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public int Update(LinkInfo link)
        {
            string cmdText = string.Format(@"update [{0}links] set
                                [type]=@type,
                                [linkname]=@linkname,
                                [linkurl]=@linkurl,
                                [position]=@position,
                                [target]=@target,
                                [description]=@description,
                                [sortnum]=@sortnum,
                                [status]=@status,
                                [createtime]=@createtime
                                where linkid=@linkid", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
               return conn.Execute(cmdText, link);
            }
        }
        /// <summary>
        /// delete link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public int Delete(LinkInfo link)
        {
            string cmdText = string.Format("delete from [{0}links] where [linkid] = @linkid", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
                return conn.Execute(cmdText, new { categoryid = link.LinkId });
            }
        }
        public LinkInfo GetById(object Id) 
        {
            string cmdText = string.Format("select * from [{0}links] where [linkid]=@linkid ", ConfigHelper.Tableprefix);
            using(var conn = new DapperHelper().OpenConnection())
            {
                return conn.Query<LinkInfo>(cmdText, new {linkid=(int)Id }).First();
            }
        }
        /// <summary>
        /// 获取全部链接
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<LinkInfo> Table
        {
            get
            {
                string cmdText = string.Format("select * from [{0}links]  order by [sortnum] asc,[linkid] asc", ConfigHelper.Tableprefix);
                using (var conn = new DapperHelper().OpenConnection())
                {
                    var list = conn.Query<LinkInfo>(cmdText, null);
                    return list;
                }
            }

        }
    }
}
