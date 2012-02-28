using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EduWeb
{
    public partial class Edu : System.Web.UI.MasterPage
    {
        public Nav nav = new Nav();
        public DB db = new DB();

        public enum Lvls { Guest = 0, Reader, Writer, Mod, Admin }
        public Lvls userLvl;

        protected void Page_Init()
        {
            if (!db.Connect()) { userLvl = Lvls.Guest; goto End; }
            if (Session.Count == 0)
                userLvl = Lvls.Guest;
            else
            {
                System.Data.SqlClient.SqlDataReader reader = db.Query("SELECT email, lvl FROM " + Globals.DbName + ".[User] WHERE email=@p1", Session["email"]);
                if (!reader.HasRows || reader == null) { userLvl = Lvls.Guest; goto End; }
                reader.Read();
                userLvl = (Lvls)int.Parse(reader["lvl"].ToString());
                reader.Dispose();
            }
        End:
            nav.AddSiteMap(Server.MapPath("/User/"+userLvl.ToString()+".sitemap"));
        }

        protected void Page_Unload()
        {
            db.Disconnect();
        }

        protected void Sidemenu()
        {
            Response.Write(nav.Menu());
        }

        protected void Title()
        {
            Response.Write(Globals.WebTitle);
        }

        protected void Description()
        {
            Response.Write(Globals.WebDesc);
        }

        protected void Url()
        {
            Response.Write(Globals.WebUrl);
        }
    }
}