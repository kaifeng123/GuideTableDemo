using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ExportExcelTool
{
    class Program
    {

		static void Main(string[] args)
		{
			int type = 0;
			Console.Write("请选择导出类型\n0.导出客户端与服务端\n1.只导客户端\n2.只导服务端\n");
			string value = Console.ReadLine();
			int.TryParse(value, out type);
			ExportExcelTool.Main.exportType = (EExportPortType)type;

			float hour = 0;
            Console.Write("请输入希望生成最近几小时的配置表(支持小数，0全部生成)\n");
			value = Console.ReadLine();
			float.TryParse(value, out hour);

			Main main = new Main();
			main.Init();

			Console.Write("正在生成中，请稍候...\n");
			main.Run(hour);
			Console.WriteLine("生成结束！");
			Console.ReadLine();
		}

		public static T ParseEnum<T>(string value, T defaultValue) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
			if (string.IsNullOrEmpty(value)) return defaultValue;

			foreach (T item in Enum.GetValues(typeof(T)))
			{
				int va = (int)EExportDataType.CSharp;
				if (item.ToString().ToLower().Equals(value.Trim().ToLower())) return item;
			}
			return defaultValue;
		}

		public static List<int> ToListInt(string str)
		{
			string[] arr = str.Split(',');
			List<int> list = new List<int>();
			for (int i = 0; i < arr.Length; i++)
			{
				int value = 0;
				int.TryParse(arr[i], out value);
				list.Add(value);
			}
			return list;
		}
	}
}
