using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EduWeb.Content
{
    public partial class Manage : System.Web.UI.Page
    {
        Edu m;
        string message = "";
        bool error = false;
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
            if (m.userLvl < Edu.Lvls.Mod) { message = "You don't have the necessary level to be here."; error = true; goto End; }
            SqlDataReader temp = m.db.Query("SELECT COUNT(*) AS num FROM " + Globals.DbName + ".[Article] WHERE [approved]=0");
            temp.Read();
            num = (int)temp["num"];
            temp.Close();
            reader = m.db.Query(
                "SELECT [id], [user], [time], [name], [summary], [comments] FROM" +
                "( SELECT [id], [user], [time], [name], [summary], [comments], [approved], ROW_NUMBER() OVER (ORDER BY [time] ASC) AS rownum FROM " + Globals.DbName + ".[Article] WHERE [approved]=0 )" +
                "AS temp WHERE rownum BETWEEN @p1 AND (@p1 +@p2-1)", start, perPage);
            if (reader == null || !reader.HasRows) { error = true; message = "There are no articles."; goto End; }
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
                "<tr><th>Author</th><th>Date</th><th>Name</th><th>Summary</th><th>Comments</th><th>Action</th></tr>\n");
            while (reader.Read())
                Response.Write(String.Format(
                    "<tr>\n" +
                        "<td>{0}</td>\n" +
                        "<td>{1}</td>\n" +
                        "<td>{2}</td>\n" +
                        "<td>{3}</td>\n" +
                        "<td>{4}</td>\n" +
                        "<td><a href=\"/Content/Review.aspx?id={5}\">Read</a></td>\n" +
                    "</tr>\n",
                    reader["user"], reader["time"], reader["name"], reader["summary"], reader["comments"], reader["id"]));
            Response.Write("</table>\n");
            reader.Close();
        End: ;
            Response.Write(Nav.Paging(start, perPage, num, true));
        }
    }
}