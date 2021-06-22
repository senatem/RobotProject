using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RobotProject
{
    public class SqlCommunication
    {
        private MySqlConnection? _connection;

        public void Connect()
        {
            const string connCommand = "server=localhost;user=root;database=TEST;port=3306;password=Elba_Project2021";
            _connection = new MySqlConnection(connCommand);
            try
            {
                _connection.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, @"Sql Bağlantı Hatası", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
            }
        }

        public void Disconnect()
        {
            _connection?.Close();
        }

        public Product? Select(string column, string value)
        {
            try
            {
                string cmdString = $"SELECT * FROM Test WHERE {column}={value};";
                MySqlCommand cmd = new MySqlCommand(cmdString, _connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                Product? res = null;
                if (rdr.Read())
                {
                    res = new Product(rdr.GetInt32("Yukseklik"), rdr.GetInt32("Uzunluk"), rdr.GetInt32("Tip"),
                        rdr.GetFloat("Toplam_Siparis_Miktar"), rdr.GetString("Yontem_Kodu"));
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
            string cmdString = $"SELECT Toplam_Siparis_Miktar FROM Test WHERE Siparis_No={orderNo};";
            MySqlCommand cmd = new MySqlCommand(cmdString, _connection);
            object res = cmd.ExecuteScalar();
            return res != null ? Convert.ToInt32(res) : 0;
        }

        public List<string> GetOrders()
        {
            string cmdString = $"SELECT Siparis_No FROM Test;";
            List<string> res = new List<string>();
            MySqlCommand cmd = new MySqlCommand(cmdString, _connection);
            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                ((IList) res).Add(rdr.GetString("Siparis_No"));
            }

            rdr.Close();
            return res;
        }

        public Pallet? GetPallet(string orderNo)
        {
            string cmdString = $"SELECT Palet_Yuksekligi, Palet_uzunlugu, Toplam_Siparis_Miktar, Tip  FROM Test WHERE Siparis_No={orderNo};";
            MySqlCommand cmd = new MySqlCommand(cmdString, _connection);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                Pallet res = new Pallet(rdr.GetInt32("Palet_Yuksekligi"), rdr.GetInt32("Palet_Uzunlugu"),
                    rdr.GetInt32("Tip"), rdr.GetInt32("Toplam_Siparis_Miktar"));
                rdr.Close();
                return res;

            }
            rdr.Close();
            return null;
        }
    }
}