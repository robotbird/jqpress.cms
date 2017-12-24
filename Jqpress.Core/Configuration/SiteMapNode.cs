using System.Collections.Generic;
using System.Web.Routing;


namespace Jqpress.Core.Configuration
{
    public class SiteMapNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMapNode"/> class.
        /// </summary>
        public SiteMapNode()
        {
            RouteValues = new RouteValueDictionary();
            ChildNodes = new List<SiteMapNode>();
        }

        public string Title { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public RouteValueDictionary RouteValues { get; set; }

        public string Url { get; set; }

        public IList<SiteMapNode> ChildNodes { get; set; }

        public string Icon { get; set; }

        public bool Visible { get; set; }
    }

}
