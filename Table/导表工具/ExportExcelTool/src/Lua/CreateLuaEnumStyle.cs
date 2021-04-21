using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportExcelTool {
	/// <summary>
	/// 导出为枚举样式
	/// </summary>
	class CreateLuaEnumStyle {
		public static List<string> CreateLuaFile(TableInfo info) {

			string cfgname = "E" + info.createName;
			string enumStr = "";

			string[] enumKeys = info.enumKeys;

			enumStr += "---@class " + cfgname + " " + info.excelName + "[Enum]\r\n";
			enumStr += string.Format("{0} = ", cfgname) + "{" + "\r\n";
			for (int i = 0; i < info.lines.Count; i++) {
                string[] lineArr = info.lines[i];

				if (lineArr.Length > 0 && (lineArr[0] == "NO" || lineArr[0] == "0")) continue;
				string key = lineArr[Array.IndexOf(info.keys, enumKeys[1])];
				if (!string.IsNullOrEmpty(key)) {
					enumStr += string.Format("\t{0} = {1},    --{2}", 
						lineArr[Array.IndexOf(info.keys, enumKeys[1])], lineArr[Array.IndexOf(info.keys, enumKeys[0])], Utils.FiltrationString(lineArr[Array.IndexOf(info.keys, enumKeys[2])])) + "\r\n";
				} 
			}
			enumStr += "}" + "\r\n";

			Console.WriteLine("------导出【" + cfgname + ".lua】成功！");

			return new List<string> { enumStr };
		}
	}
}
