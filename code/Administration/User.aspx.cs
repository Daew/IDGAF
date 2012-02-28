using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EduWeb.Administration
{
    public partial class User : System.Web.UI.Page
    {
        Edu m;
        string message = "";
        bool error = false;
        SqlDataReader reader;

        protected void Page_Load(object sender, EventArgs e)
        {
            m = (Edu)Master;
            if (!m.db.Connected) { error = true; message = "Cannot connect to database."; goto End; }
            if (m.userLvl != Edu.Lvls.Admin) { message = "You don't have the necessary level to be here."; error = true; goto End; }
            if (IsPostBack)
            {
                error = true; // 3 out of 4 possible cases
                if (Request.Form["submit"] == "Edit")
                    if (m.db.NonQuery("UPDATE " + Globals.DbName + ".[User] SET [lvl]=@p1 WHERE [email]=@p2", Request.Form["lvl"], Request.Form["email"]))
                    { message = "User edited successfully."; error = false; }
                    else message = "Database error.";
                else if (Request.Form["submit"] == "Delete")
                    if (m.db.NonQuery("DELETE FROM " + Globals.DbName + ".[User] WHERE [email]=@p1", Request.Form["email"])) message = "User permanently deleted.";
                    else message = "Database error.";
                goto End;
            }
            reader = m.db.Query("SELECT [email], [lvl] FROM " + Globals.DbName + ".[User] WHERE [email]=@p1", Request.QueryString["email"]);
            if (reader == null || !reader.HasRows) { error = true; message = "There is no such user."; goto End; }
        End: ;
        }

        protected void Write()
        {
            if (message != "")
            {
                Response.Write("<h4>" + message + "</h4>\n" +
                    "<a href=\"/Administration/Users.aspx\">Back to user list</a>");
                if (error)
                    goto End;
            }
            reader.Read();
            Response.Write(String.Format(
                "<table>"+
                "<tr><td>Email:</td><td><input type=\"text\" readonly=\"readonly\" name=\"email\" value=\"{0}\" /></td></tr>\n" +
                "<tr><td>Level:</td><td><select name=\"lvl\">\n"+
                "   <option {1} value=\"1\">Reader</option>\n"+
                "   <option {2} value=\"2\">Writer</option>\n" +
                "   <option {3} value=\"3\">Mod</option>\n" +
                "</select></td></tr>\n" +
                "<tr><td></td><td><input type=\"submit\" name=\"submit\" value=\"Edit\" /></td></tr>\n" +
                "<tr><td></td><td><input type=\"submit\" name=\"submit\" value=\"Delete\" /></td></tr>\n"+
                "</table>",
                reader["email"], Selected("1"), Selected("2"), Selected("3")
            ));
            reader.Close();
        End: ;
        }

        protected string Selected(string value)
        {
            if (reader["lvl"].ToString() == value)
                return "selected=\"selected\"";
            else
                return "";
        }
    }
}