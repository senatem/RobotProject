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
            const string connCommand = "server=localhost;user=root;database=TEST;port=3306;password=elbaproject";
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

        public Product Select(string column, string value)
        {
            string cmdString = "SELECT * FROM Test" + " WHERE " + column + "=" + value;
            MySqlCommand cmd = new MySqlCommand(cmdString, _connection);
            MySqlDataReader rdr = cmd.ExecuteReader();

            rdr.Read();
            Product res = new Product(rdr.GetInt32("Yukseklik"), rdr.GetInt32("Uzunluk"), rdr.GetInt32("Tip"), rdr.GetFloat("Toplam_Siparis_Miktar"), rdr.GetString("Yontem_Kodu"));
            rdr.Close();
            return res;
        }
    }
}