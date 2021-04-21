using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExportExcelTool
{
	public class CreateCSharp
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
				CreateCSharpConfigStyle.CreateFile(info);
			}else if (info.exportType == 4)
			{
				CreateCSharpConstStyle.CreateFile(info);
			}
		}

		public static List<string> GetNotCreateArray(string luafilename, string cfgname)
		{

			if (File.Exists(luafilename))
			{
				string[] allLines = File.ReadAllLines(luafilename);
				File.Delete(luafilename);
				List<string> allList = allLines.ToList();
				int startIndex = allList.IndexOf("\t//非自动生成代码 Start");
				int endIndex = allList.IndexOf("\t//非自动生成代码 End");
				if (startIndex == -1 || endIndex == -1)
				{
					Main.AddErrorLog(cfgname + ".cs文件存在格式错误，请检查");
				}
				else
				{
					List<string> cutOut = allList.GetRange(startIndex, endIndex - startIndex + 1);
					return cutOut;
				}
			}
			return null;
		}
	}
}
