using System.Data;
using System.IO;
using ExcelDataReader;

namespace RobotProject
{
    public class ExcelReader
    {
        private readonly FileStream _stream;
        private IExcelDataReader reader;
        private DataSet table;
        

        public ExcelReader(string file)
        {
            _stream = File.Open(file, FileMode.Open, FileAccess.Read);

            reader = ExcelReaderFactory.CreateOpenXmlReader(_stream);
            table = reader.AsDataSet();
        }

        public DataTable Find(string[] fields, int[] values)
        {
            var res = table.Tables[0];
            for (var i=0; i<fields.Length; i++){
                res = res.Select(fields[i] + " = " + values[i]).CopyToDataTable();
            }

            return res;
        }

        public void Dispose()
        {
            reader.Close();
            _stream.Close();
        }
    }
}