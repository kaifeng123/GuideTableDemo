using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportExcelTool
{
	public class CreateJavaJsonStyle
	{
		public static void CreateFile(TableInfo info)
		{
			List<Dictionary<string, object>> dir = new List<Dictionary<string, object>>();

			string filePath = Main.javaDataPath;
			
			string cfgname = info.createName + "Config";
			string fileName = filePath + cfgname + ".xml";

			string[] annotation = info.annotations;
			string[] types = info.types;
			string[] keys = info.keys;

			if (!Directory.Exists(filePath))
			{
				Main.AddErrorLog("未找到目标路径，path:" + filePath);
			}

			if (File.Exists(fileName)) File.Delete(fileName);
			FileStream luaStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
			//utf8 无bom
			Encoding _utf8NoMark = new UTF8Encoding(false);
			StreamWriter luaStreamWriter = new StreamWriter(luaStream, _utf8NoMark);

			CreateLua.WriteLineN(luaStreamWriter, "[");

			List<string> verifyKey = new List<string>();
			for (int i = 0; i < info.lines.Count; i++)
			{
				string[] lineArr = info.lines[i];

				string luaLine = "{";
				for (int j = 0; j < keys.Length; j++)
				{
					luaLine += CreateJavaType.ToType(types[j], keys[j], j < lineArr.Length ? lineArr[j] : "") + ",";
				}
				luaLine = luaLine.Substring(0, luaLine.Length - 1);
				
				if (i == info.lines.Count - 1)//最后一行
				{
					luaLine += "}";//最后一行，不要逗号
				}
				else
				{
					luaLine += "},";
				}
				CreateLua.WriteLineN(luaStreamWriter, luaLine);
			}

			CreateLua.WriteLineN(luaStreamWriter, "]");

			luaStreamWriter.Close();
			luaStream.Close();

			Console.WriteLine("--导出【" + cfgname + ".xml】成功！");
		}
	}
}
