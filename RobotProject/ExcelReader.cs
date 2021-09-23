using System;
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
            _table = _reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true,
                },
               // UseColumnDataType = false
            });
        }

        public DataTable Find(string[] fields, int[] values)
        {
            var j = 0;
            var res = _table.Tables[j];
            
            for (var i = 0; i < fields.Length; i++)
            {
                try
                {
                    res = res.Select(fields[i] + " = " + values[i]).CopyToDataTable();
                }
                catch (InvalidOperationException e)
                {
                    j += 1;
                    i = -1;
                    res = _table.Tables[j];
                }
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