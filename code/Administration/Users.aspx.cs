using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EduWeb.Administration
{
    public partial class Users : System.Web.UI.Page
    {
        Edu m;
        bool error = false;
        string message = "";
        SqlDataReader reader;
        int start = 1,
            num,
            perPage = Globals.PerPage*2;

        protected void Page_Load(object sender, EventArgs e)
        {
            m = (Edu)Master;
            if (Request.QueryString["start"] != null)
                if (int.TryParse(Request.QueryString["start"], out start))
                {
                    if (start < 1) start = 1;
                }
                else start = 1;
            if (!m.db.Connected) { error = true; message = "Cannot connect to database."; goto End; }
            if (m.userLvl != Edu.Lvls.Admin) { message = "You don't have the necessary level to be here."; error = true; goto End; }
            SqlDataReader temp = m.db.Query("SELECT COUNT(*) AS num FROM " + Globals.DbName + ".[User]");
            temp.Read();
            num = (int)temp["num"];
            temp.Close();
            reader = m.db.Query(
                "SELECT [email], [lvl] FROM" +
                "( SELECT [email], [lvl], ROW_NUMBER() OVER (ORDER BY [email] DESC) AS rownum FROM " + Globals.DbName + ".[User] WHERE [lvl] < 4)" +
                "AS temp WHERE rownum BETWEEN @p1 AND (@p1 +@p2-1)", start, perPage);
            if (reader == null || !reader.HasRows) { error = true; message = "Database error."; goto End; }
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
            Response.Write("<table style=\"width:100%\" border=\"1\">\n" +
                "<tr><th>Email</th><th>Level</th><th>Edit</th></tr>");
            while (reader.Read())
                Response.Write(String.Format(
                    "<tr>\n" +
                        "<td>{0}</td>\n" +
                        "<td>{1}</td>\n" +
                        "<td><a href=\"/Administration/User.aspx?email={0}\">Edit</a></td>\n" +
                    "</tr>\n",
                    reader["email"], reader["lvl"]));
            Response.Write("</table>\n");
            reader.Close();
        End: ;
            Response.Write(Nav.Paging(start, perPage, num, false));
        }
    }
}