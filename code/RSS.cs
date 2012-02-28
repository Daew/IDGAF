using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace EduWeb
{
    public static class RSS
    {
        public static string Headers(string title, string link, string description)
        {
            string temp =
                "<?xml version=\"1.0\" ?>\n" +
                "<rss version=\"2.0\">\n" +
                "<channel>\n";
            temp += "<title>" + title + "</title>\n";
            temp += "<link>" + link + "</link>\n";
            temp += "<description>" + description + "</description>\n";
            return temp;
        }

        public static string Item(string title, string link, string description, string author, DateTime date)
        {
            string temp = "<item>\n";
            temp += "<title>" + title + "</title>\n";
            temp += "<link>" + link + "</link>\n";
            temp += "<description>" + description + "</description>\n";
            temp += "<author>" + author + "</author>\n";
            temp += "<pubDate>" + date + "</pubDate>\n";
            temp += "</item>\n";
            return temp;
        }

        public static string End()
        {
            return  "</channel>\n" +
                    "</rss>\n";
        }
    }
}