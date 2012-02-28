using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EduWeb
{
    public partial class Default : System.Web.UI.Page
    {
        Edu m;
        string message = "";
        bool error = false;
        SqlDataReader reader;
        int start = 1,
            num,
            perPage = Globals.PerPage;

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
            SqlDataReader temp = m.db.Query("SELECT COUNT(*) AS num FROM " + Globals.DbName + ".[Article] WHERE [approved]=1");
            temp.Read();
            num = (int)temp["num"];
            temp.Close();
            reader = m.db.Query(
                "SELECT [id], [user], [time], [name], [summary] FROM"+
                "( SELECT [id], [user], [time], [name], [summary], [approved], ROW_NUMBER() OVER (ORDER BY [time] DESC) AS rownum FROM " + Globals.DbName + ".[Article] WHERE [approved]=1 )" +
                "AS temp WHERE rownum BETWEEN @p1 AND (@p1 +@p2-1)", start, perPage);
            if (reader == null || !reader.HasRows) { error = true; message = "There are no articles."; goto End; }
        End: ;
        }

        protected void Write()
        {
            if (message != "")
            {
                Response.Write("<h4>"+message+"</h4>\n");
                if(error)
                    goto End;
            }
            while (reader.Read())
                Response.Write(String.Format(
                    "<div class=\"article\">\n" +
                    "   <h4>{0}</h4>\n" +
                    "   <h2>{1}</h2>\n" +
                    "   <h6>By {2}</h6>\n" +
                    "   <em>{3}</em><br />\n" +
                    "   <a href=\"/Article.aspx?id={4}\">Read more...</a>\n" +
                    "</div>\n", reader["time"], reader["name"], reader["user"], reader["summary"], reader["id"]));
            reader.Close();
        End: ;
            Response.Write(Nav.Paging(start, perPage, num, true));
        }
    }
}