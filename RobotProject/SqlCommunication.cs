using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using RobotProject.Form2Items;

namespace RobotProject
{
    public class SqlCommunication
    {
        private SqlConnection _cnn;

        public void Connect()
        {
            // Data Source=190.190.200.100,1433;Network Library=DBMSSOCN;Initial Catalog=myDataBase;User ID=myUsername;Password=myPassword;
            try
            {
                _cnn.Close();
            } 
            catch (Exception) {//ignore
            }
            
            string connectionString =
                //@"Data Source=.\SQLEXPRESS;Initial Catalog=ELBA_SERVER;Integrated Security=False;User ID=sa;Password=acrobat;MultipleActiveResultSets=True;";

                @"Data Source=10.100.11.148;Network Library=DBMSSOCN;Initial Catalog=ELBA_Server;User ID=sa;Password=acrobat;";
            _cnn = new SqlConnection(connectionString);
            try
            {
                _cnn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Kaynak: SQL bağlantısı ." + ex.Message, @"Bağlantı Hatası", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
            }
        }

        public void Disconnect()
        {
            _cnn.Close();
        }

        public Product? Select(string column, string value)
        {
            try
            {
                string cmdString = $"SELECT * FROM Ambalaj WHERE {column}={value};";
                SqlCommand cmd = new SqlCommand(cmdString, _cnn);
                SqlDataReader rdr = cmd.ExecuteReader();

                Product? res = null;
                if (rdr.Read())
                {
                    var y = rdr.GetInt32(rdr.GetOrdinal("Yukseklik"));
                    var u = rdr.GetInt32(rdr.GetOrdinal("Uzunluk"));
                    var t = rdr.GetInt32(rdr.GetOrdinal("Tip"));
                    var m = rdr.GetDouble(rdr.GetOrdinal("Toplam_Siparis_Miktar"));

                    res = new Product(y, u,t, (float) m, rdr.GetString(rdr.GetOrdinal("Yontem_Kodu")));
                }

                rdr.Close();
                return res;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return null;
        }

        public int GetOrderSize(long orderNo)
        {
            string cmdString = $"SELECT Toplam_Siparis_Miktar FROM Ambalaj WHERE Siparis_No={orderNo};";
            SqlCommand cmd = new SqlCommand(cmdString, _cnn);
            object res = cmd.ExecuteScalar();
            return res != null ? Convert.ToInt32(res) : 0;
        }

        public List<string> GetOrders()
        {
            string cmdString = $"SELECT Siparis_No FROM Ambalaj;";
            List<string> res = new List<string>();
            SqlCommand cmd = new SqlCommand(cmdString, _cnn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                ((IList) res).Add(rdr.GetString(rdr.GetOrdinal("Siparis_No")));
            }

            rdr.Close();
            return res;
        }

        public Pallet? GetPallet(string orderNo)
        {
            string cmdString = $"SELECT Palet_Yuksekligi, Palet_uzunlugu, Toplam_Siparis_Miktar, Tip  FROM Ambalaj WHERE Siparis_No={orderNo};";
            SqlCommand cmd = new SqlCommand(cmdString, _cnn);
            SqlDataReader rdr = cmd.ExecuteReader();
            
            if (rdr.Read())
            {
                var yontem = rdr.GetString(rdr.GetOrdinal("Yontem_Kodu"));
                var t = rdr.GetInt32(rdr.GetOrdinal("Tip"));
                var m = rdr.GetDouble(rdr.GetOrdinal("Toplam_Siparis_Miktar"));
                var yparse = 0;
                var uparse = 0;
                
                if (int.Parse(yontem) == 1)
                {
                    var px = rdr.GetInt32(rdr.GetOrdinal("Yukseklik"));
                    var py = rdr.GetInt32(rdr.GetOrdinal("Uzunluk"));
                    string[] fields = {"YontemKodu", "Tip", "Yukseklik", "Uzunluk"};
                    int[] values = {int.Parse(yontem), t, px - px % 100, py - py % 100};
                    
                    yparse = (int) ConnectionManager.Calculator.Er.Find(fields, values).Rows[0]["Palet H"];
                    uparse = (int) ConnectionManager.Calculator.Er.Find(fields, values).Rows[0]["Palet L"];
                }
                else
                {
                    var y = rdr.GetString(rdr.GetOrdinal("Palet_Yuksekligi"));
                    var u = rdr.GetString(rdr.GetOrdinal("Palet_Uzunlugu"));
                    if (!int.TryParse(y, out yparse)) yparse = 0;
                    if (!int.TryParse(u, out uparse)) uparse = 0;
                }

                Pallet res = new Pallet(yparse, uparse,
                    t, Convert.ToInt32(m));
                rdr.Close();
                return res;
            }
            rdr.Close();
            return null;
        }
    }
}