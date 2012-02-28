using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EduWeb
{
    public partial class Article : System.Web.UI.Page
    {
        Edu m;
        string message = "";
        bool error = false;
        SqlDataReader reader;

        protected void Page_Load(object sender, EventArgs e)
        {
            m = (Edu)Master;
            if (!m.db.Connected) { error = true; message = "Cannot connect to database."; goto End; }
            if (IsPostBack)
            {
                if (m.userLvl == Edu.Lvls.Guest) message = "Please log in to post comments.";
                else
                {
                    object[] paras = new object[5];
                    paras[0] = Request.QueryString["id"];
                    paras[1] = Session["email"];
                    paras[2] = Text.DeHTML(Request.Form["subject"]);
                    paras[3] = Text.DeHTML(Request.Form["body"]);
                    paras[4] = DateTime.Now;
                    if (m.db.NonQuery("INSERT INTO " + Globals.DbName + ".[Comment] ([article], [author], [subject], [body], [time]) VALUES (@p1, @p2, @p3, @p4, @p5)", paras))
                        message = "Comment inserted successfully.";
                    else
                        message = "Error occurred. Please try again.";
                }
            }
            reader = m.db.Query("SELECT [id], [user], [time], [name], [summary], [body], [comments] FROM " + Globals.DbName + ".[Article] WHERE [id]=@p1 AND [approved]=1", Request.QueryString["id"]);
            if (reader == null || !reader.HasRows) { error = true; message = "There is no such article."; goto End; }
        End: ;
        }

        protected void Write()
        {
            if (message != "")
            {
                Response.Write("<h4>"+message+"</h4>\n");
                if (error)
                    goto End;
            }
            reader.Read();
            Response.Write(String.Format(
                "<div class=\"article\">"+
                "   <h3>{0}</h3>\n" +
                "   <h1>{1}</h1>\n" +
                "   <h5>By {2}</h5>\n" +
                "   <em>{3}</em>\n" +
                "   <div>{4}</div>\n"+
                "</div>",
                reader["time"], reader["name"], reader["user"], reader["summary"], Text.BBtoHTML(reader["body"].ToString(), false)
            ));
            reader.Close();
            Comments();
        End: ;
        }

        protected void Comments()
        {
            Response.Write("<h2 class=\"section\">Comments:</h2>\n");
            SqlDataReader sqldr = m.db.Query("SELECT [id], [article], [author], [subject], [body], [time] FROM " + Globals.DbName + ".[Comment] WHERE [article]=@p1", Request.QueryString["id"]);
            if (sqldr == null || !sqldr.HasRows) { Response.Write("<h4>There are no comments yet.</h4>\n"); goto Input; }
            while(sqldr.Read())
                Response.Write(String.Format(
                    "<div class=\"comment\">\n"+
                    "   <h5>{0}</h5>"+
                    "   <h4>{1}'s opinion:</h4>\n"+
                    "   <a href=\"/EditComment.aspx?id={2}\">Edit/Delete</a>\n"+
                    "   <h3>{3}</h3>\n"+
                    "   <p>{4}</p>\n"+
                    "</div>\n",
                    sqldr["time"], sqldr["author"], sqldr["id"], sqldr["subject"], Text.BBtoHTML(sqldr["body"].ToString(), true)
                ));
        Input: ;
            Response.Write(
                    "<h2 class=\"section\">Insert comment:</h2>" +
                    "<input type=\"text\" name=\"subject\" placeholder=\"Subject\" />"+
                    Text.Limited+
                    "<input type=\"submit\" />"
                );
        }
    }
}