using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jqpress.Core.Common
{
    /// <summary>
    ///前面分页类
    /// </summary>
    public class Pager
    {
        private Pager()
        {
        }

        /// <summary>
        /// 当前页
        /// </summary>
        public static int PageIndex
        {
            get
            {
                return Jqpress.Framework.Web.PressRequest.GetQueryInt("page", 1);
            }
        }


        public static string CreateHtml(int pageSize, int recordCount, string url)
        {
            string html = string.Empty;

            if (recordCount == 0)
            {
                return html;
            }

            int pageCount = recordCount / pageSize;


            if (recordCount % pageSize > 0)
            {
                pageCount += 1;
            }
            string total = string.Empty;
            string left = string.Empty;
            string right = string.Empty;
            string center = string.Empty;


            //显示首页 上一页
            if (PageIndex != 1)
            {
                left += "<a href=\"" + string.Format(url, 1) + "\">首页</a>";
                left += "<a href=\"" + string.Format(url, PageIndex - 1) + "\">上一页</a>";
            }

            //显示尾页 下一页
            if (PageIndex < pageCount && pageCount > 1)
            {
                right += "<a href=\"" + string.Format(url, PageIndex + 1) + "\">下一页</a>";
                right += "<a href=\"" + string.Format(url, pageCount) + "\">尾页</a>";
            }

            int min = 1;	//要显示的页面数最小值
            int max = pageCount;   	//要显示的页面数最大值

            if (pageCount > 5)
            {
                if (PageIndex > 2 && PageIndex < (pageCount - 1))
                {
                    min = PageIndex - 2;
                    max = PageIndex + 2;
                }
                else if (PageIndex <= 2)
                {
                    min = 1;
                    max = 5;
                }

                else if (PageIndex >= (pageCount - 1))
                {
                    min = pageCount - 4;
                    max = pageCount;
                }
            }


            //循环显示数字
            for (int i = min; i <= max; i++)
            {
                if (PageIndex == i)	//如果是当前页，用粗体和红色显示
                {
                    center += "<span class=\"current\">" + i + "</span>";
                }
                else
                {
                    center += "<a href=\"" + string.Format(url, i) + "\">" + i + "</a>";
                }
            }

            //  total = string.Format("<span class=\"total\">共有<strong>{0}</strong>条</span>", recordCount);

            html = "<div  class= \"pager\">";
            html += "<div>";
            html += (total + left + center + right);
            html += "</div>";
            html += "</div>";


            return html;

        }
    }
}
