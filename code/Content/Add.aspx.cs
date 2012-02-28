using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EduWeb.Content
{
    public partial class Add : System.Web.UI.Page
    {
        Edu m;
        string error = "";
        SqlDataReader reader;

        protected void Page_Load(object sender, EventArgs e)
        {
            m = (Edu)Master;
            if (!m.db.Connected) { error = "Cannot connect to database."; goto End; }
            if (m.userLvl < Edu.Lvls.Writer) { error = "Your user level needs to be at least Writer to add content."; goto End; }
            reader = m.db.Query("SELECT [name] FROM [Category]");
            if (!reader.HasRows || reader == null) { error = "There are no categories yet."; goto End; }
            if (IsPostBack)
            {
                object[] parameters = new object[6];
                parameters[0] = Session["email"];
                parameters[1] = DateTime.Now;
                parameters[2] = Text.DeHTML(Request.Form["name"]);
                parameters[3] = Text.DeHTML(Request.Form["summary"]);
                parameters[4] = Text.DeHTML(Request.Form["body"]);
                parameters[5] = Request.Form["category"];
                parameters[6] = Request.Form["comments"];
                if (!m.db.NonQuery("INSERT INTO " + Globals.DbName + ".[Article] ([user], [time], [name], [summary], [body], [approved], [category], [comments]) VALUES (@p1, @p2, @p3, @p4, @p5, 0, @p6, @p7)", parameters))
                { error = "Database error."; goto End; }
            }
            End: ;
        }

        protected void Write()
        {
            if (error != "") { Response.Write("<h4>" + error + "</h4>\n"); goto End; }
            if (IsPostBack)
                Response.Write("<h4>Article saved successfully.</h4>");
            Response.Write(
                    "<input type=\"text\" name=\"name\" placeholder=\"Name\" /><br />\n"+
                    "<select name=\"category\">\n");
            while (reader.Read())
                Response.Write(String.Format(
                    "<option value=\"{0}\">{0}</option>\n", reader["name"]));
            reader.Close();
            Response.Write(
                    "</select><br />\n"+
                    "<textarea name=\"summary\" placeholder=\"Summary of the article, shows on front page\" rows=\"6\" cols=\"60\"></textarea><br />\n" +
                    Text.Complete+
                    "Allow comments: Yes<input type=\"radio\" name=\"comments\" value=\"1\"/> No<input type=\"radio\" name=\"comments\" value=\"0\"/><br />\n"+
                    "<input type=\"submit\" />\n"
                );
        End: ;
        }
    }
}