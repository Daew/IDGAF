using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace EduWeb
{
    public class Nav
    {
        private List<XmlDocument> maps = new List<XmlDocument>();

        public void AddSiteMap(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            maps.Add(doc);
        }

        public string Menu()
        {
            string response = "";

            foreach (XmlDocument doc in maps)
            {
                XmlNode root = doc.ChildNodes[1].FirstChild;
                response += String.Format("<a href=\"{0}\">{1}</a>\n", Url(root), Title(root));
                foreach (XmlNode node in doc.ChildNodes[1].FirstChild.ChildNodes)
                {
                    response += String.Format("<a href=\"{0}\">{1}</a>\n", Url(node), Title(node));
                    if (node.HasChildNodes)
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            response += String.Format("<a class=\"child\" href=\"{0}\">{1}</a></li>\n", Url(childNode), Title(childNode));
                            if (childNode.HasChildNodes)
                                foreach (XmlNode grandchildNode in childNode.ChildNodes)
                                    response += String.Format("<a class=\"grandchild\" href=\"{0}\">{1}</a></li>\n", Url(grandchildNode), Title(grandchildNode));
                        }
                }
                response += "<br/>\n";
            }

            return response;
        }

        public static string Paging(int start, int perPage, int total, bool age)
        {
            return String.Format(
                "<table class=\"paging\"><tr>" +
                "<td><a href=\"?start=1\">{0}</a></td>" +
                "<td><a href=\"?start={4}\">{1}</a></td>" +
                "<td><a href=\"?start={5}\">{2}</a></td>" +
                "<td><a href=\"?start={6}\">{3}</a></td>" +
                "</tr></table>",
                age?"Newest":"First", age?"Newer":"Previous", age?"Older":"Next", age?"Oldest":"Last",
                start>perPage?start-perPage:1, start+perPage, total>perPage?total-perPage:1);
        }

        private string Url(XmlNode node)
        {
            return node.Attributes["url"].InnerText;
        }
        
        private string Title(XmlNode node)
        {
            return node.Attributes["title"].InnerText;
        }
    }
}