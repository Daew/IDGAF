using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EduWeb
{
    public partial class Feed : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DB db = new DB();
            db.Connect();
            Response.Clear();
            Response.ContentType = "application/rss+xml";
            string name = Request.QueryString["name"];
            SqlDataReader reader;
            if (name != null && name != "")
            {
                SqlDataReader cat = db.Query("SELECT [name], [description] FROM " + Globals.DbName + ".[Category] WHERE [name]=@p1", name);
                cat.Read();
                Response.Output.Write(RSS.Headers(Globals.WebTitle + ", " + name, Globals.WebUrl + "Categories/Category.aspx?name=" + name, (string)cat["description"]));
                cat.Close();
                reader = db.Query("SELECT TOP 5 [id], [user], [time], [name], [summary] FROM " + Globals.DbName + ".[Article] WHERE [approved]=1 AND [category]=@p1 ORDER BY [time] DESC", name);
            }
            else
            {
                Response.Output.Write(RSS.Headers(Globals.WebTitle, Globals.WebUrl, Globals.WebDesc));
                reader = db.Query("SELECT TOP 10 [id], [user], [time], [name], [summary] FROM " + Globals.DbName + ".[Article] WHERE [approved]=1 ORDER BY [time] DESC");
            }
            while (reader.Read())
                Response.Output.Write(RSS.Item((string)reader["name"], Globals.WebUrl + "Article.aspx?id=" + reader["id"], (string)reader["summary"], (string)reader["user"], (DateTime)reader["time"]));
            reader.Close();
            Response.Output.Write(RSS.End());
            Response.Output.Flush();
            Response.Output.Close();
            Response.End();
        }
    }
}