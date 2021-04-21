using System.Text;
using System.Text.RegularExpressions;

namespace ExportExcelTool
{
	public class CreateCSharpUtil
	{
		/// <summary>
		/// </summary>
		/// <param name="summary"></param>
		/// <param name="retract">是否有缩进</param>
		/// <returns></returns>
		public static string GetSummary(string summary, bool retract)
		{
			string normalStyle =
@"
/// <summary>
/// {0}
/// </summary>";

			string retractStyle =
@"
	/// <summary>
	/// {0}
	/// </summary>";
			return string.Format(retract ? retractStyle : normalStyle, summary.Replace("\n", ""));
		}

		public static string getMd5String(byte[] buf)
		{
			System.Security.Cryptography.MD5 alg = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] data = alg.ComputeHash(buf);

			StringBuilder sBuilder = new StringBuilder();

			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			return sBuilder.ToString();
		}
		public static string getMd5String(string input)
		{
			return getMd5String(Encoding.UTF8.GetBytes(input)).ToUpper();
		}

		public static string GetType(string type)
		{
			if (type.Contains("E_"))//枚举
			{
				return type.Replace("E_", "");
			}
			else
			{
				return type;
			}
			switch (type)
			{
				case "int":
				case "long":
				case "float":
				case "string":
				case "bool":
					return type;
				case "list<int>":
					return "List<int>";
				case "list<string>":
					return "List<string>";
				default:
					return type;
			}
		}

		static Regex regex = new Regex(@"(?is)(?<=\<)([A-Za-z0-9_.]*)(?=\>)");
		public static string GetToType(string type)
		{
			switch (type)
			{
				case "int":
					return "TableToType.ToInt({0})";
				case "long":
					return "TableToType.ToLong({0})";
				case "float":
					return "TableToType.ToFloat({0})";
				case "bool":
					return "TableToType.ToBool({0})";
				case "string":
					return "{0}";
				case "List<int>":
					return "TableToType.ToListInt({0})";
				case "List<float>":
					return "TableToType.ToListFloat({0})";
				case "List<string>":
					return "TableToType.ToListString({0})";
				default:
					if (type.StartsWith("E_"))//枚举
					{
						string enumName = type.Substring(2);
						return string.Format("TableToType.ToEnum<{0}>", enumName) + "({0})";
					}
					else
					{
						string childType = regex.Match(type).Value;
						if (!string.IsNullOrEmpty(childType))
						{
							if (childType.Contains("E_"))//List<enum>列表枚举
							{
								string enumType = childType.Substring(2);
								return string.Format("TableToType.ToListEnum<{0}>", enumType) + "({0})";
							}
							else
							{
								return string.Format("TableToType.ToListObject<{0}>", childType) + "({0})";
							}
						}
						else
						{
							return string.Format("TableToType.ToObject<{0}>", type) + "({0})";
						}
					}
			}
		}

		public static string GetKeyToString(string key, string type, string excelName, string sheetName)
		{
			if (type.StartsWith("E_"))//是枚举
			{
				return string.Format("((int){0}).ToString()", key);
			}
			else
			{
				switch (type)
				{
					case "int":
					case "long":
					case "float":
						return string.Format("{0}.ToString()", key);
					case "string":
						return key;
				}
			}
			Main.AddErrorLog(string.Format("{0}表中{1}，{2}类型不能做为第一列的字段类型", excelName, sheetName, type));
			return "";
		}
	}
}
