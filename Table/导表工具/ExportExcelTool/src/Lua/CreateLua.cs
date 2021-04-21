using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExportExcelTool {
	class CreateLua {
		public static List<string> CreateLuaFile(TableInfo info) {
			string luaPath = Main.luaPath;

			string folder = info.folderName;

			luaPath += folder + "\\";
			if (!Directory.Exists(luaPath)) {
				Directory.CreateDirectory(luaPath);
			}

			List<string> importList = new List<string>();
			///导出类型
			/// 1.普通导
			/// 2.添加一个枚举类
			/// 3.只导出枚举类
			/// 4.常量表，例如系统常量表
			if (info.exportType == 1) {
				List<string> list1 = CreateLuaConfigStyle.CreateLuaFile(luaPath, info);
				importList.AddRange(list1);
			} else if (info.exportType == 2 || info.exportType == 3) {
				List<string> list1 = CreateLuaEnumStyle.CreateLuaFile(info);
				importList.AddRange(list1);

				if (info.exportType == 2) {//导出为枚举与配置表
					List<string> list2 = CreateLuaConfigStyle.CreateLuaFile(luaPath, info);
					importList.AddRange(list2);
				}
			}else if (info.exportType == 4) {
				List<string> list2 = CreateLuaConstStyle.CreateLuaFile(info);
				importList.AddRange(list2);
			}
			return importList;
		}

		/// <summary>
		/// 导出helper配置
		/// </summary>
		/// <param name="list"></param>
		public static void ImportLuaRequire(List<string> list) {
			string luaPath = Main.luaPath;

			if (list.Count == 1 && list[0].IndexOf("CfgcmdHelper") != -1) {//如果是cmd文件，直接返回
				return;
			}
			string luafilename = luaPath + "SingleConfig.lua";

			if (File.Exists(luafilename)) {
				File.Delete(luafilename);
			}

			FileStream luaStream = new FileStream(luafilename, FileMode.CreateNew, FileAccess.Write);
			//utf8 无bom
			Encoding _utf8NoMark = new UTF8Encoding(false);
			StreamWriter luaStreamWriter = new StreamWriter(luaStream, _utf8NoMark);

			WriteLineN(luaStreamWriter,
@"---
--- 配置表单例统一获取类
--- 此类由工具生成，不可修改.
---
SingleConfig = {}"
			);
			for (int i = 0; i < list.Count; i++) {
				if (!string.IsNullOrEmpty(list[i]) && list[i].Contains("[Enum]")) {
					WriteLineN(luaStreamWriter, list[i]);
				}
			}

			for (int i = 0; i < list.Count; i++) {
				if (!string.IsNullOrEmpty(list[i]) && !list[i].Contains("[Enum]")) {
					WriteLineN(luaStreamWriter, list[i]);
				}
			}

			luaStreamWriter.Close();
			luaStream.Close();
		}

		public static void WriteLineN(StreamWriter luaStreamWriter, string luaFilename) {
			luaStreamWriter.Write(luaFilename + "\r\n");
		}

		public static List<string> GetNotCreateArray(string luafilename, string cfgname) {

			if (File.Exists(luafilename)) {
				string[] allLines = File.ReadAllLines(luafilename);
				File.Delete(luafilename);
				List<string> allList = allLines.ToList();
				int startIndex = allList.IndexOf("--非自动生成代码 Start");
				int endIndex = allList.IndexOf("--非自动生成代码 End");
				if (startIndex == -1 || endIndex == -1) {
					Main.AddErrorLog(cfgname + ".lua文件存在格式错误，请检查");
				}else {
					List<string> cutOut = allList.GetRange(startIndex, endIndex - startIndex + 1);
					return cutOut;
				}
			}
			return null;
		}

		public static string GetKeyValue(string key, string[] keys, string[] values) {
			for (int i = 0; i < keys.Length; i++) {
				if (key == keys[i]) {
					return values[i];
				}
			}
			return "";
		}
	}
}
