using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace EduWeb
{
    public class DB
    {
        private SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString);

        public bool Connected { get; private set; }

        public DB() { Connected = false; }

        public bool Connect()
        {
            try { con.Open(); }
            catch { return false; }
            Connected = true;
            return true;
        }

        public bool NonQuery(string command) { return NonQuery(command, null, null, null, null, null); }
        public bool NonQuery(string command, object p1) { return NonQuery(command, p1, null, null, null, null); }
        public bool NonQuery(string command, object p1, object p2) { return NonQuery(command, p1, p2, null, null, null); }
        public bool NonQuery(string command, object p1, object p2, object p3) { return NonQuery(command, p1, p2, p3, null, null); }
        public bool NonQuery(string command, object p1, object p2, object p3, object p4) { return NonQuery(command, p1, p2, p3, p4, null); }
        public bool NonQuery(string command, object[] ps) { return NonQuery(command, null, null, null, null, ps); }
        private bool NonQuery(string command, object p1, object p2, object p3, object p4, object[] ps)
        {
            SqlCommand com = new SqlCommand(command, con);
            if (p1 != null) com.Parameters.Add(new SqlParameter("p1", p1));
            if (p2 != null) com.Parameters.Add(new SqlParameter("p2", p2));
            if (p3 != null) com.Parameters.Add(new SqlParameter("p3", p3));
            if (p4 != null) com.Parameters.Add(new SqlParameter("p4", p4));
            if (ps != null)
            {
                SqlParameter[] paras = new SqlParameter[ps.Length];
                for (int i = 0; i < ps.Length; i++)
                    paras[i] = new SqlParameter("p" + (i+1), ps[i]);
                com.Parameters.AddRange(paras);
            }
            try { com.ExecuteNonQuery(); }
            catch { return false; }
            finally { com.Dispose(); }
            return true;
        }

        public SqlDataReader Query(string command) { return Query(command, null, null, null, null, null); }
        public SqlDataReader Query(string command, object p1) { return Query(command, p1, null, null, null, null); }
        public SqlDataReader Query(string command, object p1, object p2) { return Query(command, p1, p2, null, null, null); }
        public SqlDataReader Query(string command, object p1, object p2, object p3) { return Query(command, p1, p2, p3, null, null); }
        public SqlDataReader Query(string command, object p1, object p2, object p3, object p4) { return Query(command, p1, p2, p3, p4, null); }
        public SqlDataReader Query(string command, object[] ps) { return Query(command, null, null, null, null, ps); }
        private SqlDataReader Query(string command, object p1, object p2, object p3, object p4, object[] ps)
        {
            SqlCommand com = new SqlCommand(command, con);
            if (p1 != null) com.Parameters.Add(new SqlParameter("p1", p1));
            if (p2 != null) com.Parameters.Add(new SqlParameter("p2", p2));
            if (p3 != null) com.Parameters.Add(new SqlParameter("p3", p3));
            if (p4 != null) com.Parameters.Add(new SqlParameter("p4", p4));
            if (ps != null)
            {
                SqlParameter[] paras = new SqlParameter[ps.Length];
                for (int i = 1; i <= ps.Length; i++)
                    paras[i] = new SqlParameter("p" + i, ps[i]);
                com.Parameters.AddRange(paras);
            }
            SqlDataReader reader;
            try { reader = com.ExecuteReader(); }
            catch { return null; }
            finally { com.Dispose(); }
            return reader;
        }

        public void Disconnect()
        {
            try { con.Close(); }
            catch { }
        }
    }
}