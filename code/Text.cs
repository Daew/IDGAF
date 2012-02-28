using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace EduWeb
{
    public static class Text
    {
        public static readonly string Complete =
            "<script type=\"text/javascript\">"+
                "function textInsert(text) {"+
                "document.getElementById(\"bbcodeTextarea\").value += text; }" +
            "</script>"+
            "<div>" +
                "<input type=\"button\" onclick=\"textInsert('[b][/b]');\" value=\"Bold\" />" +
                "<input type=\"button\" onclick=\"textInsert('[i][/i]');\" value=\"Italic\" />" +
                "<input type=\"button\" onclick=\"textInsert('[u][/u]');\" value=\"Underscore\" />" +
                "<input type=\"button\" onclick=\"textInsert('[url=www.example.com][/url]');\" value=\"Url\" />" +
                "<input type=\"button\" onclick=\"textInsert('[img=www.example.com/pic.jpg]');\" value=\"Image\" />" +
                "<input type=\"button\" onclick=\"textInsert('[quote=person][/quote]');\" value=\"Quote\" />" +
                "<input type=\"button\" onclick=\"textInsert('[size=6][/size]');\" value=\"Size\" />" +
                "<input type=\"button\" onclick=\"textInsert('[color=#000000][/color]');\" value=\"Color\" />" +
                "<input type=\"button\" onclick=\"textInsert('[list][item][/item][item][/item][/list]');\" value=\"List\" />" +
                "<br /><textarea id=\"bbcodeTextarea\" name=\"body\" rows=\"15\" cols=\"80\"></textarea><br />" +
            "</div>";

        public static readonly string Limited =
            "<script type=\"text/javascript\">" +
                "function textInsert(text) {" +
                "document.getElementById(\"bbcodeTextarea\").value += text; }" +
            "</script>" +
            "<div>" +
                "<input type=\"button\" onclick=\"textInsert('[b][/b]');\" value=\"Bold\" />" +
                "<input type=\"button\" onclick=\"textInsert('[i][/i]');\" value=\"Italic\" />" +
                "<input type=\"button\" onclick=\"textInsert('[u][/u]');\" value=\"Underscore\" />" +
                "<input type=\"button\" onclick=\"textInsert('[url=www.example.com][/url]');\" value=\"Url\" />" +
                "<input type=\"button\" onclick=\"textInsert('[quote=person][/quote]');\" value=\"Quote\" />" +
                "<input type=\"button\" onclick=\"textInsert('[color=#000000][/color]');\" value=\"Color\" />" +
                "<input type=\"button\" onclick=\"textInsert('[list][item][/item][item][/item][/list]');\" value=\"List\" />" +
                "<br /><textarea id=\"bbcodeTextarea\" name=\"body\" rows=\"12\" cols=\"60\"></textarea><br />" +
            "</div>";

        public static string BBtoHTML(string text, bool limited)
        {
            string temp = DeHTML(text);
            temp = Regex.Replace(temp, "\\[b\\](?<text>.*)\\[/b\\]", "<strong>${text}</strong>");
            temp = Regex.Replace(temp, "\\[i\\](?<text>.*)\\[/i\\]", "<em>${text}</em>");
            temp = Regex.Replace(temp, "\\[u\\](?<text>.*)\\[/u\\]", "<span style=\"text-decoration:underline\">${text}</span>");
            temp = Regex.Replace(temp, "\\[url=(?<href>.*)\\](?<text>.*)\\[/url\\]", "<a href=\"http://${href}\">${text}</a>");
            if (!limited) temp = Regex.Replace(temp, "\\[img=(?<src>[^\\]]*)\\]", "<img alt=\"Image\" src=\"http://${src}\">");
            temp = Regex.Replace(temp, "\\[quote=(?<person>[^\\]]*)\\](?<text>.*)\\[/quote\\]", "<div class=\"quote\"><h5>${person} said:</h5><p>${text}</p></div>");
            temp = Regex.Replace(temp, "\\[quote=(?<person>[^\\]]*)\\](?<text>.*)\\[/quote\\]", "<div class=\"quote\"><h5>${person} said:</h5><p> something</p></div>");
            if (!limited) temp = Regex.Replace(temp, "\\[size=(?<size>[^\\]]*)\\](?<text>.*)\\[/size\\]", "<h${size}>${text}</h${size}>");
            temp = Regex.Replace(temp, "\\[color=(?<color>.*)\\](?<text>[^\\]]*)\\[/color\\]", "<span style=\"color:${color}\">${text}</span>");
            temp = temp.Replace("[list]", "<ul>");
            temp = temp.Replace("[/list]", "</ul>");
            temp = temp.Replace("[item]", "<li>");
            temp = temp.Replace("[/item]", "</li>");
            temp = temp.Replace("\n", "<br />");
            return temp;
        }

        public static string DeHTML(string text)
        {
            string temp = text;
            temp = temp.Replace("<", "&lt;");
            temp = temp.Replace(">", "&gt;");
            return temp;
        }
    }
}