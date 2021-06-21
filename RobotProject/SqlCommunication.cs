using System;
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
            string cmdString = $"SELECT * FROM Test WHERE {column}={value}";
            MySqlCommand cmd = new MySqlCommand(cmdString, _connection);
            MySqlDataReader rdr = cmd.ExecuteReader();

            Product? res = null;
            if (rdr.Read())
            {
                res = new Product(rdr.GetInt32("Yukseklik"), rdr.GetInt32("Uzunluk"), rdr.GetInt32("Tip"), rdr.GetFloat("Toplam_Siparis_Miktar"), rdr.GetString("Yontem_Kodu"));
            }
            rdr.Close();
            return res;
        }

        public int GetOrderSize(int orderNo)
        {
            string cmdString = $"SELECT Toplam_Siparis_Miktar FROM Test WHERE Siparis_No={orderNo}";
            MySqlCommand cmd = new MySqlCommand(cmdString);
            object res = cmd.ExecuteScalar();
            return res != null ? Convert.ToInt32(res) : 0;
        }
    }
}