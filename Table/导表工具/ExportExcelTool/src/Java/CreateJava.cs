using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExportExcelTool
{
	public class CreateJava
	{
		public static void CreateFile(TableInfo info)
		{
			List<string> importList = new List<string>();
			///导出类型
			/// 1.普通导
			/// 2.添加一个枚举类
			/// 3.只导出枚举类
			/// 4.常量表，例如系统常量表
			if (info.exportType == 1)
			{
				CreateJavaJsonStyle.CreateFile(info);
			}
		}

		public static void WriteLineN(StreamWriter luaStreamWriter, string luaFilename)
		{
			luaStreamWriter.Write(luaFilename + "\r\n");
		}
	}
}
