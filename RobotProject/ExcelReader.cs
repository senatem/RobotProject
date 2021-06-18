using System.Data;
using System.IO;
using ExcelDataReader;

namespace RobotProject
{
    public class ExcelReader
    {
        private readonly FileStream _stream;
        private readonly IExcelDataReader _reader;
        private readonly DataSet _table;
        

        public ExcelReader(string file)
        {
            _stream = File.Open(file, FileMode.Open, FileAccess.Read);

            _reader = ExcelReaderFactory.CreateOpenXmlReader(_stream);
            _table = _reader.AsDataSet();
        }

        public DataTable Find(string[] fields, int[] values)
        {
            var res = _table.Tables[0];
            for (var i=0; i<fields.Length; i++){
                res = res.Select(fields[i] + " = " + values[i]).CopyToDataTable();
            }

            return res;
        }

        public void Dispose()
        {
            _reader.Close();
            _stream.Close();
        }
    }
}