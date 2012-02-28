using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace EduWeb.User
{
    public partial class Default : System.Web.UI.Page
    {
        Edu m;
        byte response = 0; // 0x01 login, 0x02 register, 0x04 success, 0x08 logged
        string error = "";
        SHA512CryptoServiceProvider sha = new SHA512CryptoServiceProvider();

        protected void Page_Load(object sender, EventArgs e)
        {
            m = (Edu)Master;
            if (m.userLvl > Edu.Lvls.Guest)
            {
                response += 8;
                if (IsPostBack && Request.Form["submit"] == "Logout") { Session.Clear(); response += 4; }
                goto End;
            }
            if (IsPostBack)
            {
                if (Request.Form["submit"] == "Login")
                {
                    response += 1;
                    if (!m.db.Connected) { error = "Cannot connect to database."; goto End; }
                    SqlDataReader reader = m.db.Query("SELECT [email] FROM " + Globals.DbName + ".[User] WHERE [email]=@p1 AND [pass]=@p2",
                        Request.Form["l_mail"], sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Request.Form["l_pwd"])));
                    if (reader == null || !reader.HasRows) { error = "Login failed, wrong mail or password."; goto End; }
                    reader.Read();
                    Session["email"] = reader["email"];
                    reader.Close();
                    response += 4;
                }
                else if (Request.Form["submit"] == "Register")
                {
                    response += 2;
                    if (!m.db.Connected) { error = "Cannot connect to database."; goto End; }
                    SqlDataReader reader = m.db.Query("SELECT [email] FROM " + Globals.DbName + ".[User] WHERE [email]=@p1", Request.Form["r_mail"]);
                    if (!(reader == null || !reader.HasRows)) { error = "This email is already taken."; goto End; }
                    reader.Close();
                    if (!m.db.NonQuery("INSERT INTO " + Globals.DbName + ".[User] VALUES (@p1, @p2, 1)",
                        Text.DeHTML(Request.Form["r_mail"]), sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Request.Form["r_pwd"]))))
                    { error = "Database error. Try again later."; goto End; }
                    response += 4;
                }
            }
        End: ;
        }

        protected void Write()
        {
            string
                logoutForm =
                    "<p>You are logged in as " + Session["email"] + ".</p>\n" +
                    "<input type=\"submit\" name=\"submit\" value=\"Logout\" />\n",

                loginForm =
                    "<div style=\"width:50%; float:left; text-align:center\">\n" +
                    "    <h2 class=\"section\" style=\"margin-top:0\">Login</h2>\n" +
                    "    <input type=\"email\" name=\"l_mail\" placeholder=\"Email\" /><br />\n" +
                    "    <input type=\"password\" name=\"l_pwd\" placeholder=\"Password\" /><br />\n" +
                    "    <input id=\"login\" type=\"submit\" name=\"submit\" value=\"Login\" />\n" +
                    "</div>\n" +
                    "<div style=\"width:50%; float:right; text-align:center\">\n" +
                    "    <h2 class=\"section\" style=\"margin-top:0\">Register</h2>\n" +
                    "    <input type=\"email\" name=\"r_mail\" id=\"mail\" placeholder=\"Email\" /><br />\n" +
                    "    <input type=\"email\" id=\"r_mailc\" placeholder=\"Email again\" /><br />\n" +
                    "    <input type=\"password\" name=\"r_pwd\" id=\"pwd\" placeholder=\"Password\" /><br />\n" +
                    "    <input type=\"password\" id=\"r_pwdc\" placeholder=\"Password again\" /><br />\n" +
                    "    <input id=\"register\" type=\"submit\" name=\"submit\" value=\"Register\" onclick=\"return Confirm();\" />\n" +
                    "</div>\n";

            switch (response)
            {
                case 0x00: // no request
                    Response.Write(loginForm);
                    break;

                case 0x01: // login fail
                case 0x02: // register fail
                    Response.Write("<h4>"+error+"</h4>\n" + loginForm);
                    break;

                case 0x05: // login success
                    Response.Write(logoutForm + "<br/><script type=\"text/javascript\">setTimeout(\"location.href='/'\", 5000);</script>\nRedirecting you in 5 seconds.<br />\n");
                    break;

                case 0x06: // register success
                    Response.Write("<h4>Registration successful, you may now login.</h4>\n" + loginForm);
                    break;

                case 0x08: // logged in, no request
                    Response.Write(logoutForm);
                    break;

                case 0x0c: // logout (success)
                    Response.Write("<h4>Logged out successfuly.</h4>" + loginForm);
                    break;
            }
        }
    }
}