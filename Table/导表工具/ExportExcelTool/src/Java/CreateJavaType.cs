using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportExcelTool
{
	public class CreateJavaType
	{
		public static string ToType(string type, string key, string value)
		{
			type = type.ToLower();
			switch (type)
			{
				case "int":
					int a;
					int.TryParse(value, out a);
					return Utils.ToFormat("\"{0}\":{1}", key, a);
				case "long":
					long b;
					long.TryParse(value, out b);
					return Utils.ToFormat("\"{0}\":{1}", key, b);
				case "float":
					float c;
					float.TryParse(value, out c);
					return Utils.ToFormat("\"{0}\":{1}", key, c);
				case "string":
				default:
					return Utils.ToFormat("\"{0}\":\"{1}\"", key, value.Replace("\"", "\\\""));
			}
		}
	}
}
