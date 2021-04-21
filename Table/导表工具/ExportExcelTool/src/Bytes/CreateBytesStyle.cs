using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportExcelTool
{
	public class CreateBytesStyle
	{
		/// <summary>
		/// 创建配置二进制数据文件
		/// </summary>
		public static void CreateConfigBytes(string filePath, TableInfo info)
		{
			if (!Directory.Exists(filePath))
			{
				Main.AddErrorLog("未找到目标路径，path:" + filePath);
			}

			string cfgname = info.createName + "Cfg";
			string fileName = filePath + cfgname + ".bytes";

			if (File.Exists(fileName)) File.Delete(fileName);

			FileStream txtRead = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
			BinaryWriter bw = new BinaryWriter(txtRead, UTF8Encoding.UTF8);

			bw.Write(info.lines.Count);

			for (int i = 0; i < info.lines.Count; i++)
			{
				string[] lineArr = info.lines[i];
				string sid = lineArr[0];
				if (info.groupKeys != null && info.groupKeys.Length > 0)
				{
					string[] keyArrrr = new string[info.groupKeys.Length];
					for (int j = 0; j < info.groupKeys.Length; j++)
					{
						keyArrrr[j] = lineArr[int.Parse(info.groupKeys[j])];
					}
					//sid = string.Join("_", keyArrrr);
				}
				//bw.Write(sid);
				bw.Write(string.Join("\t", lineArr));
			}

			bw.Close();
			txtRead.Close();
			Console.WriteLine("--导出【" + cfgname + ".bytes】成功！");
		}

		/// <summary>
		/// 创建常量二进制数据文件
		/// </summary>
		public static void CreateConstBytes(string filePath, TableInfo info, string cfgname)
		{
			string fileName = filePath + cfgname + ".bytes";
			string[] enumKeys = info.enumKeys;

			if (File.Exists(fileName)) File.Delete(fileName);

			FileStream txtRead = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
			BinaryWriter bw = new BinaryWriter(txtRead, UTF8Encoding.UTF8);

			List<string> valueList = new List<string>();
			for (int i = 0; i < info.lines.Count; i++)
			{
				string[] lineArr = info.lines[i];

				string value = lineArr[Array.IndexOf(info.keys, enumKeys[3])];
				valueList.Add(value);
			}

			bw.Write(string.Join("\t", valueList));

			bw.Close();
			txtRead.Close();
			Console.WriteLine("--导出【" + cfgname + ".bytes】成功！");
		}
	}
}
