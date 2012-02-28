using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EduWeb.Content
{
    public partial class Review : System.Web.UI.Page
    {
        Edu m;
        string message = "";
        bool error = false;
        SqlDataReader reader;

        protected void Page_Load(object sender, EventArgs e)
        {
            m = (Edu)Master;
            if (!m.db.Connected) { error = true; message = "Cannot connect to database."; goto End; }
            if (m.userLvl < Edu.Lvls.Mod) { message = "You don't have the necessary level to be here."; error = true; goto End; }
            if (IsPostBack)
            {
                if (Request.Form["submit"] == "Approve")
                    if (m.db.NonQuery("UPDATE " + Globals.DbName + ".[Article] SET [approved]=1, [time]=@p1 WHERE [id]=@p2", DateTime.Now, Request.Form["id"])) message = "Article approved successfully.";
                    else message = "Database error.";
                else if (Request.Form["submit"] == "Delete")
                    if (m.db.NonQuery("DELETE FROM " + Globals.DbName + ".[Article] WHERE id=@p1 AND [approved]=0", Request.Form["id"])) message = "Article permanently deleted.";
                    else message = "Database error.";
                error = true; // not error, simulates something like display=false; there is no unapproved article with this id anymore
                goto End;
            }
            reader = m.db.Query("SELECT [id], [user], [time], [name], [summary], [body] FROM " + Globals.DbName + ".[Article] WHERE [id]=@p1 AND [approved]=0", Request.QueryString["id"]);
            if (reader == null || !reader.HasRows) { error = true; message = "There is no such unapproved article."; goto End; }
        End: ;
        }

        protected void Write()
        {
            if (message != "")
            {
                Response.Write("<h4>" + message + "</h4>\n"+
                    "<a href=\"/Content/Manage.aspx\">Back to article list</a>");
                if (error)
                    goto End;
            }
            reader.Read();
            Response.Write(String.Format(
                "<h3>{0}</h3>\n" +
                "<h1>{1}</h1>\n" +
                "<h5>By {2}</h5>" +
                "<em>{3}</em>\n" +
                "<div>{4}</div>\n"+
                "<input type=\"hidden\" name=\"id\" value=\"{5}\" />\n"+
                "<input type=\"submit\" name=\"submit\" value=\"Approve\" />\n" +
                "<input type=\"submit\" name=\"submit\" value=\"Delete\" />\n",
                reader["time"], reader["name"], reader["user"], reader["summary"], Text.BBtoHTML(reader["body"].ToString(), false), reader["id"]
            ));
            reader.Close();
        End: ;
        }
    }
}