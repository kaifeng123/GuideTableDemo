using System;
using System.Collections.Generic;

namespace ExportExcelTool {
	/// <summary>
	/// 导出为常量样式
	/// </summary>
	class CreateLuaConstStyle {
		public static List<string> CreateLuaFile(TableInfo info) {

			string cfgname = "E" + info.createName;

            string[] enumKeys = info.enumKeys;


            string enumStr = "";
			enumStr += "---@class " + cfgname + " " + info.excelName + "[Enum]\r\n";
			enumStr += string.Format("{0} = ", cfgname) + "{" + "\r\n";

			for (int i = 0; i < info.lines.Count; i++) {
                string[] lineArr = info.lines[i];

				string key = lineArr[Array.IndexOf(info.keys, enumKeys[1])];
				if (!string.IsNullOrEmpty(key)) {
					string type = lineArr[Array.IndexOf(info.keys, enumKeys[3])];
					string idstr = lineArr[Array.IndexOf(info.keys, enumKeys[0])];
					idstr = CreateLuaType.ToType(type, idstr);
					idstr = idstr.Substring(0, idstr.Length - 1);
					enumStr += string.Format("\t{0} = {1},    --{2}",
						lineArr[Array.IndexOf(info.keys, enumKeys[1])], idstr, Utils.FiltrationString(lineArr[Array.IndexOf(info.keys, enumKeys[2])])) + "\r\n";
				}
			}
			enumStr += "}" + "\r\n";

			Console.WriteLine("------导出【" + cfgname + ".lua】成功！");

			return new List<string> { enumStr };
		}
	}
}
