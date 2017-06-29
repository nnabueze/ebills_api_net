using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data;
using System.Collections;

namespace EbillsApi.Models.Repository
{

    public class IgrRepository
    {
        private MySql.Data.MySqlClient.MySqlConnection conn;

        public IgrRepository()
        {
            string myContectionString;
            myContectionString = "server=127.0.0.1;uid=root;pwd=;database=igr;convert zero datetime=True";

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myContectionString;
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {

                throw ex;
            }
        }

        //saving IGR
        public long saveIgr(Igr igrToSave)
        {
            try
            {
                String sqlString = "INSERT INTO igrs(igr_key,state_name,igr_code,igr_abbre,logo,created_at,updated_at)VALUES('" + igrToSave.Igr_Key + "','" + igrToSave.State_name + "','" + igrToSave.Igr_code + "','" + igrToSave.Igr_abbre + "','" + igrToSave.Logo + "','" + igrToSave.created_at.ToString("yyyy-MM-dd HH:mm:ss") + "','" + igrToSave.Updated_at.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString,conn);
                cmd.ExecuteNonQuery();
                long id = cmd.LastInsertedId;
                return id;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {

                throw ex;
            }           
        }

        //getting  a single result
        public Igr getIgr(string igrKey)
        {
            Igr p = new Igr();
            MySql.Data.MySqlClient.MySqlDataReader mySqlReader = null;

            String sqlString = "SELECT * FROM igrs WHERE igr_key = '" + igrKey +"'";
            try
            {
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);
                using (mySqlReader = cmd.ExecuteReader())
                {
                    if (mySqlReader.Read())
                    {
                        p.Id = mySqlReader.GetInt32(0);
                        p.Igr_abbre = (string) mySqlReader["igr_abbre"];
                        p.Igr_code = (string)mySqlReader["igr_code"];
                        p.Igr_Key = (string)mySqlReader["Igr_Key"];
                        p.Logo = (string)mySqlReader["Logo"];
                        p.State_name = (string)mySqlReader["State_name"];
                        p.created_at = (DateTime)mySqlReader["created_at"];
                        p.Updated_at = (DateTime)mySqlReader["updated_at"];


                        return p;
                    }
                    else
                    {
                        return null;
                    }
                     
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {

                throw ex;
            }
        }

        //return list of IGR
        public IEnumerable<Igr> GetIgrs()
        {
            MySql.Data.MySqlClient.MySqlDataReader mySqlReader = selectData();

            IList<Igr> IgrArrayList = new List<Igr>();

            while(mySqlReader.Read())
            {
                Igr p = new Igr();
                p.Id = mySqlReader.GetInt32(0);
                p.Igr_abbre = (string) mySqlReader["igr_abbre"];
                p.Igr_code = (string)mySqlReader["igr_code"];
                p.Igr_Key = (string)mySqlReader["Igr_Key"];
                p.Logo = (string)mySqlReader["Logo"];
                p.State_name = (string)mySqlReader["State_name"];
                p.created_at = (DateTime)mySqlReader["created_at"];
                p.Updated_at = (DateTime)mySqlReader["updated_at"];

                IgrArrayList.Add(p);
            }

            return IgrArrayList;
        }


        //reusable function select
        private MySql.Data.MySqlClient.MySqlDataReader selectData(int? ID = null)
        {
            MySql.Data.MySqlClient.MySqlDataReader mySqlReader = null;

            String sqlString = null;

            if (ID != null)
            {
                sqlString = "SELECT * FROM igrs WHERE ID = " + ID;
            }
            else
            {
                sqlString = "SELECT * FROM igrs";
            }


            try
            {
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString,conn);
                mySqlReader = cmd.ExecuteReader();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {

                throw ex;
            }

            return mySqlReader;

        }

        //getting list of Mda
        public IList<Item> ListMda(string biller)
        {
            Igr IgrInfo = getIgr(biller);            
            IList<Item> ItemArrayList = new List<Item>();

            MySql.Data.MySqlClient.MySqlDataReader mySqlReader = null;
            string sqlString = "SELECT * FROM mdas WHERE igr_id =" + IgrInfo.Id;

            try
            {
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);
                using (mySqlReader = cmd.ExecuteReader())
                {
                    while(mySqlReader.Read())
                    {
                        Item Mda = new Item();
                        Mda.Name = (string) mySqlReader["mda_name"];
                        Mda.value = (string)mySqlReader["mda_key"];

                        ItemArrayList.Add(Mda);
                    }
                     
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {

                throw ex;
            }

            return ItemArrayList;

        }


        //getting list of Subhead
        public IList<Item> ListSubhead(int biller)
        {            
            IList<Item> ItemArrayList = new List<Item>();

            MySql.Data.MySqlClient.MySqlDataReader mySqlReader = null;
            string sqlString = "SELECT * FROM subheads WHERE mda_id =" + biller;

            try
            {
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);
                using (mySqlReader = cmd.ExecuteReader())
                {
                    while(mySqlReader.Read())
                    {
                        Item Mda = new Item();
                        Mda.Name = (string) mySqlReader["subhead_name"];
                        Mda.value = (string)mySqlReader["subhead_key"];

                        ItemArrayList.Add(Mda);
                    }
                     
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {

                throw ex;
            }

            return ItemArrayList;

        }

        //getting  a single Mda result
        public Mda GetMda(string mdaKey)
        {
            Mda p = new Mda();
            MySql.Data.MySqlClient.MySqlDataReader mySqlReader = null;

            String sqlString = "SELECT * FROM mdas WHERE mda_key = '" + mdaKey + "'";
            try
            {
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);
                using (mySqlReader = cmd.ExecuteReader())
                {
                    if (mySqlReader.Read())
                    {
                        p.Id = mySqlReader.GetInt32(0);
                        p.igr_id = mySqlReader.GetInt32("igr_id");
                        p.mda_category = (string)mySqlReader["mda_category"];
                        p.mda_key = (string)mySqlReader["mda_key"];
                        p.mda_name = (string)mySqlReader["mda_name"];
                        p.created_at = (DateTime)mySqlReader["created_at"];
                        p.Updated_at = (DateTime)mySqlReader["updated_at"];


                        return p;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {

                throw ex;
            }
        }

        //getting  a single tin result
        public Dictionary<string,string> TinVerify(string tin)
        {
            Dictionary<string, string> p = new Dictionary<string, string>();
            MySql.Data.MySqlClient.MySqlDataReader mySqlReader = null;

            String sqlString = "SELECT * FROM tins WHERE tin_no = '" + tin + "' OR temporary_tin= '" + tin + "'";
            try
            {
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);
                using (mySqlReader = cmd.ExecuteReader())
                {
                    if (mySqlReader.Read())
                    {

                        p.Add("name", (string)mySqlReader["name"]);
                        p.Add("phone", (string)mySqlReader["phone"]);
                        p.Add("Tin",tin);


                        return p;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {

                throw ex;
            }
        }
    }
}