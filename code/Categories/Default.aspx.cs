using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EduWeb.Categories
{
    public partial class Default : System.Web.UI.Page
    {
        Edu m;
        bool error = false;
        string message = "";
        SqlDataReader reader;

        protected void Page_Load(object sender, EventArgs e)
        {
            m = (Edu)Master;
            if (!m.db.Connected) { error = true; message = "Cannot connect to database."; goto End; }
            reader = m.db.Query("SELECT [name], [description] FROM " + Globals.DbName + ".[Category]");
            if (reader == null || !reader.HasRows) { error = true; message = "There are no categories."; goto End; }
        End: ;
        }

        protected void Write()
        {
            if (message != "")
            {
                Response.Write("<h4>" + message + "</h4>\n");
                if (error)
                    goto End;
            }
            while (reader.Read())
                Response.Write(String.Format(
                    "<div class=\"category\">\n" +
                    "   <h3>{0}</h3>\n" +
                    "   <em>{1}</em><br />\n" +
                    "   <a href=\"/Feed.aspx?name={0}\" class=\"rss\">RSS Feed</a>\n" +
                    "   <a href=\"/Categories/Category.aspx?name={0}\">Show</a>\n" +
                    "</div>\n", reader["name"], reader["description"]));
            reader.Close();
        End: ;
        }
    }
}