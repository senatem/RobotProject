using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RobotProject
{
    public class SqlCommunication
    {
        private SqlConnection _cnn;

        public void Connect()
        {
            string connectionString =
                @"Data Source=.\SQLEXPRESS;Initial Catalog=ELBA_SERVER;Integrated Security=False;User ID=sa;Password=acrobat;MultipleActiveResultSets=True;";
            _cnn = new SqlConnection(connectionString);
            _cnn.Open();
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
                    res = new Product(rdr.GetInt32(rdr.GetOrdinal("Yukseklik")), rdr.GetInt32(rdr.GetOrdinal("Uzunluk")), rdr.GetInt32(rdr.GetOrdinal("Tip")),
                        rdr.GetFloat(rdr.GetOrdinal("Toplam_Siparis_Miktar")), rdr.GetString(rdr.GetOrdinal("Yontem_Kodu")));
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
                Pallet res = new Pallet(rdr.GetInt32(rdr.GetOrdinal("Palet_Yuksekligi")), rdr.GetInt32(rdr.GetOrdinal("Palet_Uzunlugu")),
                    rdr.GetInt32(rdr.GetOrdinal("Tip")), rdr.GetInt32(rdr.GetOrdinal("Toplam_Siparis_Miktar")));
                rdr.Close();
                return res;

            }
            rdr.Close();
            return null;
        }
    }
}