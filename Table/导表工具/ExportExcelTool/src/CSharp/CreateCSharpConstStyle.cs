using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExportExcelTool
{
	/// <summary>
	/// 生成为C#端常量样式
	/// </summary>
	public class CreateCSharpConstStyle
	{
		public static void CreateFile(TableInfo info)
		{
			string cfgname = info.createName;
			CreateBytesStyle.CreateConstBytes(Main.csDataPath, info, cfgname);
			CreateCSharpScripts(Main.csScriptPath, info, cfgname);
		}

		private static void CreateCSharpScripts(string filePath, TableInfo info, string cfgname)
		{
			string folder = info.folderName;

			filePath += folder + "\\";
			if (!Directory.Exists(filePath))
			{
				Directory.CreateDirectory(filePath);
			}

			string[] types = info.types;
			string[] keys = info.keys;
			string[] enumKeys = info.enumKeys;

			string fileName = filePath + cfgname + ".cs";
			List<string> cutOut = CreateCSharp.GetNotCreateArray(fileName, cfgname);

			if (types.Length != keys.Length)
			{
				Main.AddErrorLog(string.Format("{0}配置表第 2 行与第 4 行长度不一", info.excelName));
				return;
			}

			FileStream luaStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
			//utf8 无bom
			Encoding _utf8NoMark = new UTF8Encoding(false);
			StreamWriter luaStreamWriter = new StreamWriter(luaStream, _utf8NoMark);

			CreateLua.WriteLineN(luaStreamWriter, "using UnityEngine;");
			CreateLua.WriteLineN(luaStreamWriter, "using System.Collections.Generic;");
			CreateLua.WriteLineN(luaStreamWriter, CreateCSharpUtil.GetSummary(info.excelName, false));

			string configStr =
@"public static class $class_name
{
	$content

	public static void AutoParse(string[] source){
$auto_content
	}
";
			configStr = configStr.Replace("$class_name", cfgname);
			string content = "";
			string autoContent = "";

			for (int i = 0; i < info.lines.Count; i++)
			{
				string[] lineArr = info.lines[i];

				string typeValue = lineArr[Array.IndexOf(info.keys, enumKeys[0])];
				string keyValue = lineArr[Array.IndexOf(info.keys, enumKeys[1])];
				string desValue = lineArr[Array.IndexOf(info.keys, enumKeys[2])];
				if (!string.IsNullOrEmpty(keyValue))
				{
					content += CreateCSharpUtil.GetSummary(desValue, true) + "\r\n";
					content += Utils.ToFormat("\tpublic static {0} {1}{ get; private set; }", CreateCSharpUtil.GetType(typeValue), keyValue) + "\r\n";

					autoContent += string.Format("\t\t{0} = {1};", keyValue, string.Format(CreateCSharpUtil.GetToType(typeValue), "source[" + i + "]")) + "\r\n";
				}
			}
			autoContent += "\t\tInit();";
			configStr = configStr.Replace("$content", content);
			configStr = configStr.Replace("$auto_content", autoContent);

			CreateLua.WriteLineN(luaStreamWriter, configStr);

			//写入非自动生成代码
			if (cutOut == null)
			{
				CreateLua.WriteLineN(luaStreamWriter, NonautomaticCreate());
			}
			else
			{
				for (int i = 0; i < cutOut.Count; i++)
				{
					CreateLua.WriteLineN(luaStreamWriter, cutOut[i]);
				}
			}

			CreateLua.WriteLineN(luaStreamWriter, "}");

			luaStreamWriter.Close();
			luaStream.Close();

			Console.WriteLine("--导出【" + cfgname + ".cs】成功！");
		}

		private static string NonautomaticCreate()
		{
			string normalStyle =
@"
	//非自动生成代码 Start
	private static void Init() {

	}
	//非自动生成代码 End";
			return normalStyle;
		}
	}
}