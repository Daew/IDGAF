using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EduWeb
{
    public partial class EditComment : System.Web.UI.Page
    {
        Edu m;
        string message = "";
        bool error = false;
        bool edit = true;
        int id, article;
        string author, subject, body;

        protected void Page_Load(object sender, EventArgs e)
        {
            m = (Edu)Master;
            if (!m.db.Connected) { error = true; message = "Cannot connect to database."; goto End; }
            SqlDataReader reader = m.db.Query("SELECT [id], [article], [author], [subject], [body] FROM [Comment] WHERE [id]=@p1", Request.QueryString["id"]);
            if (reader == null || !reader.HasRows) { error = true; message = "There is no such comment."; goto End; }
            reader.Read();
            id = (int)reader["id"];
            article = (int)reader["article"];
            author = (string)reader["author"];
            subject = (string)reader["subject"];
            body = (string)reader["body"];
            reader.Close();
            if (m.userLvl == Edu.Lvls.Guest) { message = "Log in first."; error = true; goto End; } // not logged in
            else if (m.userLvl >= Edu.Lvls.Mod) ; // mod or admin, can edit all
            else if (Session["email"] == reader["author"]) ; // person who wrote the comment
            else
            {
                SqlDataReader sqldr = m.db.Query("SELECT [user] FROM " + Globals.DbName + ".[Article] WHERE [id]=@p1", article);
                sqldr.Read();
                if (sqldr["user"] == Session["email"]) { sqldr.Close(); edit = false; } // writer of article where the comment is, can only delete
                else { message = "You don't have the necessary level to be here."; sqldr.Close(); error = true; goto End; }
            }
            if (IsPostBack)
            {
                error = true; // 4 out of 5 possible cases
                if (Request.Form["submit"] == "Edit")
                    if (edit)
                        if (m.db.NonQuery("UPDATE " + Globals.DbName + ".[Comment] SET [subject]=@p1, [body]=@p2 WHERE [id]=@p3", Text.DeHTML(Request.Form["subject"]), Text.DeHTML(Request.Form["body"]), Request.Form["id"]))
                        { message = "Comment edited successfully."; error = false; }
                        else message = "Database error.";
                    else message = "You cannot edit this comment.";
                else if (Request.Form["submit"] == "Delete")
                    if (m.db.NonQuery("UPDATE " + Globals.DbName + ".[Comment] SET [subject]='DELETED', [body]='DELETED' WHERE [id]=@p1", Request.Form["id"])) message = "Comment content deleted.";
                    else message = "Database error.";
                goto End;
            }
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
            Response.Write("<input type=\"text\" name=\"subject\" placeholder=\"Subject\" value=\""+subject+"\" />\n"+
                Text.Limited.Replace("</textarea>", body+"</textarea>")+ // comment text into textarea
                "<input type=\"hidden\" name=\"id\" value=\""+id+"\"/>\n"+
                "<input type=\"submit\" name=\"submit\" value=\"Edit\" />\n" +
                "<input type=\"submit\" name=\"submit\" value=\"Delete\" />\n");
        End: ;
        }
    }
}