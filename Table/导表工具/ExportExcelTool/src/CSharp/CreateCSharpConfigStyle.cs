using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExportExcelTool
{
	public class CreateCSharpConfigStyle
	{
		public static void CreateFile(TableInfo info)
		{
			CreateBytesStyle.CreateConfigBytes(Main.csDataPath, info);
			CreateCSharpScripts(Main.csScriptPath, info);
		}

		/// <summary>
		/// 创建C#脚本文件
		/// </summary>
		private static void CreateCSharpScripts(string filePath, TableInfo info)
		{
			if (!Directory.Exists(filePath))
			{
				Main.AddErrorLog("未找到目标路径，path:" + filePath);
			}

			string folder = info.folderName;

			filePath += folder + "\\";
			if (!Directory.Exists(filePath))
			{
				Directory.CreateDirectory(filePath);
			}

			string pCfgname = info.createName;

			string[] annotation = info.annotations;
			string[] types = info.types;
			string[] keys = info.keys;

			string cfgname = pCfgname + "Cfg";
			string fileName = filePath + cfgname + ".cs";
			List<string> cutOut = CreateCSharp.GetNotCreateArray(fileName, cfgname);

			if (types.Length != keys.Length)
			{
				Main.AddErrorLog(string.Format("{0}配置表第 2 行与第 4 行长度不一", info.excelName));
				return;
			}

			CreateCSharpHelperScripts(filePath,cfgname);

			FileStream luaStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
			//utf8 无bom
			Encoding _utf8NoMark = new UTF8Encoding(false);
			StreamWriter luaStreamWriter = new StreamWriter(luaStream, _utf8NoMark);

			CreateLua.WriteLineN(luaStreamWriter, "using UnityEngine;");
			CreateLua.WriteLineN(luaStreamWriter, "using System.Collections.Generic;");
			CreateLua.WriteLineN(luaStreamWriter, CreateCSharpUtil.GetSummary(info.excelName, false));
			CreateConfigFun(luaStreamWriter, cfgname, annotation, types, keys, info.interfaceName);

			string[] getKey = info.groupKeys;
			if (cutOut == null)
			{
				CreateLua.WriteLineN(luaStreamWriter, NonautomaticCreate(CreateCSharpUtil.GetKeyToString(keys[0], types[0], info.excelName, info.sheetName)));
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

		private static void CreateConfigFun(StreamWriter luaStreamWriter, string fileName, string[] annotation, string[] types, string[] keys, string interfaceName)
		{
			string configStr =
@"public class $class_name : $interfaceName
{
	$content

	public $override void AutoParse(string[] source){
$auto_content
	}
";
			configStr = configStr.Replace("$class_name", fileName);
			if (string.IsNullOrEmpty(interfaceName))
			{
				configStr = configStr.Replace("$interfaceName", "ICfg");
				configStr = configStr.Replace("$override", "");
			}
			else
			{
				configStr = configStr.Replace("$interfaceName", interfaceName);
				configStr = configStr.Replace("$override", "override");
			}
			
			string ann = "";
			for (int i = 0; i < keys.Length; i++)
			{
				ann += CreateCSharpUtil.GetSummary(annotation[i], true) + "\r\n";
				ann += Utils.ToFormat("\tpublic {0} {1}{ get; protected set; }", CreateCSharpUtil.GetType(types[i]), keys[i]) + "\r\n";
			}
			configStr = configStr.Replace("$content", ann);

			ann = "";
			for (int i = 0; i < keys.Length; i++)
			{
				ann += string.Format("\t\tthis.{0} = {1};", keys[i], 
					string.Format(CreateCSharpUtil.GetToType(types[i]), "source[" + i + "]")) + "\r\n";
			}
			configStr = configStr.Replace("$auto_content", ann);

			CreateLua.WriteLineN(luaStreamWriter, configStr);
		}

		/// <summary>
		/// 创建C#脚本单例文件
		/// </summary>
		private static void CreateCSharpHelperScripts(string filePath, string cfgName)
		{
			
			string cfgname = cfgName + "Helper";
			string fileName = filePath + cfgname + ".cs";
			
			if (File.Exists(fileName))
			{
				return;
			}

			FileStream luaStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
			//utf8 无bom
			Encoding _utf8NoMark = new UTF8Encoding(false);
			StreamWriter luaStreamWriter = new StreamWriter(luaStream, _utf8NoMark);

			CreateLua.WriteLineN(luaStreamWriter, "using System;");
			CreateLua.WriteLineN(luaStreamWriter, "using System.Collections.Generic;");
			CreateLua.WriteLineN(luaStreamWriter, "using System.Linq;");
			
			//CreateLua.WriteLineN(luaStreamWriter, CreateCSharpUtil.GetSummary(info.excelName, false));

			string configStr =
@"
/// <summary>
/// $summary
/// </summary>
public class $class_name : BaseCfgHelper<$class_name,$cfgItem_name>
{
	
}
";
			configStr = configStr.Replace("$class_name", cfgname);
			configStr = configStr.Replace("$summary", cfgName+"-工具类");
			configStr = configStr.Replace("$cfgItem_name", cfgName);
			CreateLua.WriteLineN(luaStreamWriter, configStr);

			luaStreamWriter.Close();
			luaStream.Close();
			Console.WriteLine("--导出【" + cfgname + ".cs】成功！");
		}

		private static string NonautomaticCreate(string key)
		{
			string normalStyle =
@"
	//非自动生成代码 Start
	public string GetKey(){
		return $key_id;
	}

	public void Init() {

	}
	//非自动生成代码 End";
			return normalStyle.Replace("$key_id", key);
		}

	}
}
