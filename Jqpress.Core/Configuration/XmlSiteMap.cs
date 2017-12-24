using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.Routing;
using System.Web;
using System.Xml;

namespace Jqpress.Core.Configuration
{
    public class XmlSiteMap
    {
        public XmlSiteMap()
        {
            RootNode = new SiteMapNode();
        }

        public SiteMapNode RootNode { get; set; }

        public virtual void LoadFrom(string physicalPath)
        {
            string filePath = HttpContext.Current.Server.MapPath(physicalPath);
            string content = File.ReadAllText(filePath);

            if (!string.IsNullOrEmpty(content))
            {
                using (var sr = new StringReader(content))
                {
                    using (var xr = XmlReader.Create(sr,
                            new XmlReaderSettings
                            {
                                CloseInput = true,
                                IgnoreWhitespace = true,
                                IgnoreComments = true,
                                IgnoreProcessingInstructions = true
                            }))
                    {
                        var doc = new XmlDocument();
                        doc.Load(xr);

                        if ((doc.DocumentElement != null) && doc.HasChildNodes)
                        {
                            XmlNode xmlRootNode = doc.DocumentElement.FirstChild;
                            Iterate(RootNode, xmlRootNode);
                        }
                    }
                }
            }
        }

        private static void Iterate(SiteMapNode siteMapNode, XmlNode xmlNode)
        {
            PopulateNode(siteMapNode, xmlNode);

            foreach (XmlNode xmlChildNode in xmlNode.ChildNodes)
            {
                if (xmlChildNode.LocalName.Equals("siteMapNode", StringComparison.InvariantCultureIgnoreCase))
                {
                    var siteMapChildNode = new SiteMapNode();
                    siteMapNode.ChildNodes.Add(siteMapChildNode);

                    Iterate(siteMapChildNode, xmlChildNode);
                }
            }
        }

        private static void PopulateNode(SiteMapNode siteMapNode, XmlNode xmlNode)
        {
            //title
            
           siteMapNode.Title = GetStringValueFromAttribute(xmlNode, "title");

            //routes, url
            string controllerName = GetStringValueFromAttribute(xmlNode, "controller");
            string actionName = GetStringValueFromAttribute(xmlNode, "action");
            string url = GetStringValueFromAttribute(xmlNode, "url");
            if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName))
            {
                siteMapNode.ControllerName = controllerName;
                siteMapNode.ActionName = actionName;

                siteMapNode.RouteValues = new RouteValueDictionary()
                                          {
                                              {"area", "Admin"}
                                          };
            }
            else if (!string.IsNullOrEmpty(url))
            {
                siteMapNode.Url = url;
            }

            //Icon
            siteMapNode.Icon = GetStringValueFromAttribute(xmlNode, "Icon");

         
                siteMapNode.Visible = true;
        }

        private static string GetStringValueFromAttribute(XmlNode node, string attributeName)
        {
            string value = null;

            if (node.Attributes.Count > 0)
            {
                XmlAttribute attribute = node.Attributes[attributeName];

                if (attribute != null)
                {
                    value = attribute.Value;
                }
            }

            return value;
        }
    }
}
