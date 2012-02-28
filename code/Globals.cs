using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.XPath;

namespace EduWeb
{
    public static class Globals
    {
        public static readonly string WebTitle, WebUrl, WebDesc, DbName;
        public static readonly int PerPage;
        
        static Globals()
        {
            XPathDocument doc = new XPathDocument(new System.Web.UI.Page().Server.MapPath("/config.xml"));
            XPathNavigator nav = doc.CreateNavigator();
            WebTitle = nav.SelectSingleNode("/config/web/title").Value;
            WebUrl = nav.SelectSingleNode("/config/web/url").Value;
            WebDesc = nav.SelectSingleNode("/config/web/desc").Value;
            DbName = nav.SelectSingleNode("/config/db/name").Value;
            PerPage = nav.SelectSingleNode("/config/content/perPage").ValueAsInt;
        }
    }
}