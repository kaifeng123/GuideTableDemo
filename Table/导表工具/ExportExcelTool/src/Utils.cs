using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportExcelTool {
	public class Utils {
		public static string ToFormat(string str, params object[] args) {
			for (int i = 0; i < args.Length; i++) {
				str = str.Replace("{" + i + "}", args[i].ToString());
			}
			return str;
		}

		public static string FiltrationString(string str) {
			return str.Replace("\n", "  ").Replace("\r", "  ").Replace("\r\n", "");
		}

		/// <summary>
		/// 获取文件名
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetFileName(string path) {
			string file = path;
			if (file.LastIndexOf('.') >= 0)
				file = file.Substring(0, file.LastIndexOf('.'));
			if (file.LastIndexOf('\\') >= 0)
				file = file.Substring(file.LastIndexOf('\\') + 1);
			return file;
		}
	}
}
