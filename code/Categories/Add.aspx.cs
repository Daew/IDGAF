using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EduWeb.Categories
{
    public partial class Add : System.Web.UI.Page
    {
        Edu m;
        string error = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            m = (Edu)Master;
            if (m.userLvl < Edu.Lvls.Writer) { error = "Your user level needs to be at least Writer to add content."; goto End; }
            if (IsPostBack)
            {
                if (!m.db.Connected) { error = "Cannot connect to database."; goto End; }
                if (!m.db.NonQuery("INSERT INTO " + Globals.DbName + ".[Category] ([name], [description]) VALUES (@p1, @p2)", Text.DeHTML(Request.Form["name"]), Text.DeHTML(Request.Form["description"]))) { error = "Database error."; goto End; }
            }
        End: ;
        }

        protected void Write()
        {
            if (error != "") { Response.Write("<h4>" + error + "</h4>\n"); goto End; }
            if (IsPostBack)
                Response.Write("<h4>Article saved successfully.</h4>");
            Response.Write(
                    "<input type=\"text\" name=\"name\" placeholder=\"Name\" /><br />\n" +
                    "<textarea name=\"description\" placeholder=\"Description of the category\" rows=\"6\" cols=\"60\"></textarea><br />\n" +
                    "<input type=\"submit\" />\n"
                );
        End: ;
        }
    }
}