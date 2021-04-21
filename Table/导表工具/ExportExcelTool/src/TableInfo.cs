using System.Collections.Generic;
using System.Data;

namespace ExportExcelTool
{
	public enum EExportDataType
	{
		Lua,	//客户端Lua
		CSharp,	//客户端C#
		Java,	//服务端Java
	}

    public class TableInfo
    {
		private int typeLineIndex;       //决定是根据哪一行导出Key
		private int typeRowIndex;       //决定是根据哪一列来导出样式(服务端 2，客户端C# 3，Lua 4)
		public string excelPath;     //excel文件路径
        public string excelName;     //excel文件名字
        public string sheetName;    //Sheet表名
        public string folderName;   //要放置的文件夹名
        public string createName;     //要生成的表名

		public string[] types = new string[] { };             //客户端类型列表
		public string[] keys = new string[] { };              //KEY列表
        public string[] annotations = new string[] { };       //注释列表
        public List<string[]> lines = new List<string[]>();  //表数据[行[列]]

        public int exportType;              //导出类型
		public string[] groupKeys = new string[] { };        //组合KEY
        public string[] enumKeys = new string[] { };         //枚举KEY
		public string interfaceName = "";    //要实现的接口

		private int colLen;         //列长度
        private bool[] rowsValid;   //记录对应列是否有效
        private int validLen;       //有效长度

        public TableInfo(string fullName, DataTable dataTable, EExportDataType exportType)
        {
			switch (exportType)
			{
				case EExportDataType.CSharp:
					this.typeLineIndex = 1;
					this.typeRowIndex = 2;
					break;
				case EExportDataType.Lua:
					this.typeLineIndex = 1;
					this.typeRowIndex = 3;
					break;
				case EExportDataType.Java:
					this.typeLineIndex = 2;
					this.typeRowIndex = 4;
					break;
			}

            this.excelPath = fullName;
            this.sheetName = dataTable.TableName;
            this.excelName = GetFileName(fullName) + "-" + this.sheetName;
            this.folderName = dataTable.Columns[0].ToString();
            if (this.folderName.StartsWith("Column"))
            {
                Main.AddErrorLog("错误：" + excelName + "配置表表头未填写文件夹名");
            }
            this.createName = dataTable.Columns[1].ToString();
            if (this.createName.EndsWith("_1"))
            {
                this.createName = this.createName.Replace("_1", "");
            }

            AnalysisTableType(dataTable);
            if (this.exportType == 0)
            {
                return;
            }

            AnalysisTableTitle(dataTable);
            AnalysisTableRows(dataTable);
        }

        /// <summary>
        /// 解析单元格，获取导出类型及KEY
        /// </summary>
        /// <param name="dataTable"></param>
        private void AnalysisTableType(DataTable dataTable)
        {
            try
            {
				if (dataTable.Columns.Count <= this.typeRowIndex)
				{
					return;
				}
                string tempStr = dataTable.Columns[this.typeRowIndex].ToString();
				for(int i = 1; i < 10; i++)
				{
					if (tempStr.EndsWith("_" + i))
					{
						tempStr = tempStr.Replace("_" + i, "");
						break;
					}
				}
                string[] arr = tempStr.Split(',');

				if (arr.Length > 0 && !string.IsNullOrEmpty(arr[0]))
				{
					int aa;
					int.TryParse(arr[0], out aa);
					exportType = aa;
				}
				if (arr.Length > 1 && !string.IsNullOrEmpty(arr[1]))
                {
                    groupKeys = arr[1].Split('_');
                }
                if (arr.Length > 2 && !string.IsNullOrEmpty(arr[2]))
                {
                    enumKeys = arr[2].Split('_');
                }
				if (arr.Length > 3 && !string.IsNullOrEmpty(arr[3]))
				{
					interfaceName = arr[3];
				}
			}
            catch
            {
                Main.AddErrorLog(string.Format("请检查{0}文件第一行第{1}列是否配置正确！", excelName, this.typeRowIndex + 1));
            }
        }

        /// <summary>
        /// 解析表头
        /// </summary>
        /// <param name="dataTable"></param>
        private void AnalysisTableTitle(DataTable dataTable)
        {
            //解析到KEY长度
            colLen = dataTable.Columns.Count;

            //拿出有效长度
            rowsValid = new bool[colLen - 1];
            for (int i = 1; i < colLen; i++)
            {
                string typevalid = dataTable.Rows[typeLineIndex][i].ToString().ToLower();
				string keyvalid = dataTable.Rows[3][i].ToString().ToLower();
				if (string.IsNullOrEmpty(typevalid) || typevalid == "no" || string.IsNullOrEmpty(keyvalid))
                {
					rowsValid[i - 1] = false;
				}
				else
				{
					rowsValid[i - 1] = true;
					validLen++;
				}
            }

            //声明有效数组长度
            types = new string[validLen];
			keys = new string[validLen];
            annotations = new string[validLen];

            //解析类型、KEY、注释
            int validIndex = 0;
            for (int i = 1; i < colLen; i++)
            {
                if (rowsValid[i - 1])
                {
                    //解析客户端类型
                    string ctype = dataTable.Rows[typeLineIndex][i].ToString();
                    types[validIndex] = ctype.Trim();

					//解析key
					string key = dataTable.Rows[3][i].ToString();
                    keys[validIndex] = key.Trim();

                    //解析注释
                    string annotation = dataTable.Rows[0][i].ToString();
                    annotations[validIndex] = annotation.Trim();

                    validIndex++;
                }
            }
        }

        /// <summary>
        /// 解析表数据
        /// </summary>
        /// <param name="dataTable"></param>
        private void AnalysisTableRows(DataTable dataTable)
        {
            int count = dataTable.Rows.Count;
            int lineIndex = 0;
            for (int i = 4; i < count; i++)
            {
                string lineValid = dataTable.Rows[i][0].ToString().Trim();
                if (lineValid.ToLower() == "no" || lineValid.ToLower() == "0")  //行遇到no或0不导出这一行
                {
                    continue;
                }
                //string line1Valid = dataTable.Rows[i][1].ToString().Trim();
                //if (string.IsNullOrEmpty(line1Valid))  //第一列为空也不导出
                //{
                //    continue;
                //}

                lines.Add(new string[validLen]);
                int validIndex = 0;
                for (int j = 1; j < colLen; j++)
                {
                    if (rowsValid[j - 1])
                    {
                        lines[lineIndex][validIndex] = dataTable.Rows[i][j].ToString().Trim();
                        validIndex++;
                    }
                }

                if (lineValid.ToLower() == "end")   //行遇到end结束后续数据
                {
                    break;
                }
                lineIndex++;
            }
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(string path)
        {
            string file = path;
            if (file.LastIndexOf('.') >= 0)
                file = file.Substring(0, file.LastIndexOf('.'));
            if (file.LastIndexOf('\\') >= 0)
                file = file.Substring(file.LastIndexOf('\\') + 1);
            return file;
        }
    }
}
