using System.Collections.Generic;
using ExcelDataReader;
using System.Data;
using System.IO;

namespace ExportExcelTool
{

    class TableContent
    {
		public static Dictionary<string, TableInfo> cSharpDictTable = new Dictionary<string, TableInfo>();

		public static Dictionary<string, TableInfo> luaDictTable = new Dictionary<string, TableInfo>();

		public static Dictionary<string, TableInfo> javaDictTable = new Dictionary<string, TableInfo>();

		public static void Clear()
        {
			luaDictTable.Clear();
			cSharpDictTable.Clear();
		}

        /// <summary>
        /// 解析excel表
        /// </summary>
        /// <param name="excelPath"></param>
        /// <returns></returns>
        public static void AnalysisTable(string excelPath)
        {
			Stream stream = null;
			try
			{
				stream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
			}
			catch
			{
				Main.AddErrorLog("请检查EXCEL是否关闭.");
				return;
			}

			var reader = ExcelReaderFactory.CreateReader(stream);
            do { while (reader.Read()) { } } while (reader.NextResult());

            ExcelDataSetConfiguration configuration = new ExcelDataSetConfiguration { ConfigureDataTable = tableReader => new ExcelDataTableConfiguration { UseHeaderRow = true } };
            DataSet dataSet = reader.AsDataSet(configuration);

            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
				if (VerifyTable(dataSet.Tables[i]))
				{
					if (Main.exportType == EExportPortType.All || Main.exportType == EExportPortType.Client)
					{
						LoaderTable(new TableInfo(excelPath, dataSet.Tables[i], EExportDataType.CSharp), cSharpDictTable);//解析C#所要数据
					}
					if (Main.exportType == EExportPortType.All || Main.exportType == EExportPortType.Server)
					{
						LoaderTable(new TableInfo(excelPath, dataSet.Tables[i], EExportDataType.Java), javaDictTable);//解析Java所要数据
					}
				}
			}
        }

		/// <summary>
		/// 装入表中
		/// </summary>
		/// <param name="ti"></param>
		/// <param name="dir"></param>
		public static void LoaderTable(TableInfo ti, Dictionary<string, TableInfo> dir)
		{
			if (dir.ContainsKey(ti.createName))
			{
				TableInfo temp = dir[ti.createName];
				if (temp.keys.Length == ti.keys.Length)
				{
					dir[ti.createName].lines.AddRange(ti.lines);
				}
				else
				{
					Main.AddErrorLog(string.Format("发现存在两张相同表名:{0}，但字段不一致的情况。{1}中的{2} 与 {3}中的{4}表名相同", 
						ti.createName, temp.excelName, temp.sheetName, ti.excelName, ti.sheetName));
				}
			}
			else
			{
				dir.Add(ti.createName, ti);
			}
		}

		//验证表是否是正规表
		public static bool VerifyTable(DataTable dataTable)
		{
			if (dataTable.Rows.Count >= 3)
			{
				return dataTable.Rows[1][0].ToString() == "CLIENT" && dataTable.Rows[2][0].ToString() == "SERVER" &&
				dataTable.Rows[3][0].ToString() == "KEY";
			}
			return false;
		}
	}
}
