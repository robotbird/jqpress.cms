using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Core.Domain;
using Jqpress.Core.Repositories.Repository;
using Jqpress.Core.Repositories.IRepository;

namespace Jqpress.Core.Services
{

    /// <summary>
    /// 连接管理
    /// </summary>
    public class LinkService
    {
        private ILinkRepository _linkRepository;

        #region 构造函数
        /// <summary>
        /// 构造器方法
        /// </summary>
        public LinkService()
            : this(new LinkRepository())
        {
        }
        /// <summary>
        /// 构造器方法
        /// </summary>
        /// <param name="linkRepository"></param>
        public LinkService(ILinkRepository linkRepository)
        {
            this._linkRepository = linkRepository;
        }
        #endregion

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public  int InsertLink(LinkInfo link)
        {
            link.LinkId =  _linkRepository.Insert(link);
            return link.LinkId;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public  int UpdateLink(LinkInfo link)
        {
            return  _linkRepository.Update(link);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="termid"></param>
        /// <returns></returns>
        public  int Delete(int linkId)
        {
            return _linkRepository.Delete(new LinkInfo() { LinkId = linkId});
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="linkId"></param>
        /// <returns></returns>
        public LinkInfo GetLink(int linkId)
        {

            return _linkRepository.GetById(linkId);
        }
        /// <summary>
        /// 获取连接列表
        /// </summary>
        /// <param name="position"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public  List<LinkInfo> GetLinkList(int position, int status)
        {
            return GetLinkList(-1, position, status);
        }

        /// <summary>
        /// 获取连接列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public  List<LinkInfo> GetLinkList(int type, int position, int status)
        {
            List<LinkInfo> list = _linkRepository.Table.ToList();

            if (type != -1)
            {
                list = list.FindAll(link => link.Type == type);
            }

            if (position != -1)
            {
                list = list.FindAll(link => link.Position == position);
            }

            if (status != -1)
            {
                list = list.FindAll(link => link.Status == status);
            }

            return list;
        }

        /// <summary>
        /// 获取连接列表
        /// </summary>
        /// <returns></returns>
        public  List<LinkInfo> GetLinkList()
        {
            return _linkRepository.Table.ToList();
        }
    }
}
