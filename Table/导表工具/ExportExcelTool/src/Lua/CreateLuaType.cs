using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportExcelTool {
	class CreateLuaType {

		public static string ToType(string type, string value) {
			type = type.ToLower();
			switch (type) {
				case "int":
				case "long":
					int va = 0;
					int.TryParse(value, out va);
					return Utils.ToFormat("{0},", va);
				case "float":
					float va1 = 0;
					float.TryParse(value, out va1);
					return Utils.ToFormat("{0},", va1);
				case "string":
					return Utils.ToFormat("'{0}',", value);
				case "bool":
					int bol = 0;
					int.TryParse(value, out bol);
					return Utils.ToFormat("{0},", bol > 0 ? "true" : "false");
				default:
					return SpecialType(type, value);
			}
		}

		// 解析特殊样式
		/*
			list<int>					1,2,3						{1, 10, 10}
			list<list<int>>				1_10_10,2_20_10				{{1, 10, 10}, {2, 20, 10}}
			map<int,int>				1_2,2_2,3_2,4_2,5_2			{["a"] = "b", ["c"] = "d", ["e"] = "r"}
			keyvalue<int,int>				1_2							{key = 10, value = 10}
		 	list<keyvalue<int,int>>		1_2,2_2,3_2,4_2,5_2			{{key = a, value = b}, {key = c, value = d}}
			map<int,keyvalue<int,int>>		1:10_10,2:20_10				{[1] = {key = 10, value = 10}, [2] = {key = 10, value = 20}}
			map<int,list<int>>			1:10_10_10_10,2:20_3_3_5	{[1] = {10, 10, 10}, [2] = {10, 10, 10}}
			keyvalue<int,list<int>>		1:10_10_10_10				{key = 1, value = {10, 10, 10, 10}}

		*/
		public static string SpecialType(string type, string value) {
			string result = "";
			string content = "";
			string[] arr = null;
			string[] childArr = null;
			string[] typeArr = null;
			switch (type) {
				//list<int>		600002,600003,600004	{1, 10, 10}
				case "list<int>":
				case "list<float>":
				case "list<string>":
					return GetList(value, type) + ",";
				//list<list<int>>	1_10_10,2_20_10		{{1, 10, 10}, {2, 20, 10}}
				case "list<list<int>>":
				case "list<list<float>>":
				case "list<list<string>>":
					arr = value.Split(',');
					content = "";
					for (int i = 0; i < arr.Length; i++) {
						content += Utils.ToFormat("{0},", GetList(arr[i], type));
					}
					content = content.Substring(0, content.Length - 1);
					result = Utils.ToFormat("{{0}},", content);
					break;
				//map<int,int>		a_b,c_d,e_r,a_t,t_w		{["a"] = "b", ["c"] = "d", ["e"] = "r"}
				case "map<int,int>":
				case "map<float,float>":
				case "map<string,string>":
				case "map<int,float>":
				case "map<float,int>":
				case "map<float,string>":
				case "map<string,float>":
				case "map<int,string>":
				case "map<string,int>":
					content = "";
					arr = value.Split(',');
					for (int i = 0; i < arr.Length; i++) {
						content += GetMap(arr[i], type) + ",";
					}
					content = content.Substring(0, content.Length - 1);
					result = Utils.ToFormat("{{0}},", content);
					break;
				//keyvalue<int,int>	a_b		{key = 10, value = 10}
				case "keyvalue<int,int>":
				case "keyvalue<float,float>":
				case "keyvalue<string,string>":
				case "keyvalue<int,float>":
				case "keyvalue<float,int>":
				case "keyvalue<float,string>":
				case "keyvalue<string,float>":
				case "keyvalue<int,string>":
				case "keyvalue<string,int>":
					return Getkeyvalue(value, type) + ",";
				//list<keyvalue<int,int>>		a_b,c_d,e_r,a_t,t_w		{{key = a, value = b}, {key = c, value = d}}
				case "list<keyvalue<int,int>>":
				case "list<keyvalue<float,float>>":
				case "list<keyvalue<string,string>>":
				case "list<keyvalue<int,float>>":
				case "list<keyvalue<float,int>>":
				case "list<keyvalue<float,string>>":
				case "list<keyvalue<string,float>>":
				case "list<keyvalue<int,string>>":
				case "list<keyvalue<string,int>>":
					arr = value.Split(',');
					content = "";
					for (int i = 0; i < arr.Length; i++) {
						string va = Getkeyvalue(arr[i], type);
						if (va != "nil") {
							content += Utils.ToFormat("{0},", va);
						}
					}
					if(content != "") {
						content = content.Substring(0, content.Length - 1);
					}
					result = Utils.ToFormat("{{0}},", content);
					break;
				//map<int,keyvalue<int,int>>		1:10_10,2:20_10		{[1] = {key = 10, value = 10}, [2] = {key = 10, value = 20}}
				case "map<int,keyvalue<int,int>>":
					arr = value.Split(',');
					content = "";
					for (int i = 0; i < arr.Length; i++) {
						childArr = new string[2];
						childArr[0] = arr[i].Substring(0, arr[i].IndexOf(':'));
						childArr[1] = arr[i].Substring(childArr[0].Length + 1);
						if (childArr.Length == 2) {
							string va = Getkeyvalue(childArr[1], type);
							if (va != "nil") {
								content += Utils.ToFormat("[{0}] = {1},", childArr[0], va);
							}
						}
					}
					if (content != "") {
						content = content.Substring(0, content.Length - 1);
					}
					result = Utils.ToFormat("{{0}},", content);
					break;
				//map<int,list<int>>		1:10_10_10_10,2:20_3_3_5	{[1] = {10, 10, 10}, [2] = {10, 10, 10}}
				case "map<int,list<int>>":
				case "map<float,list<float>>":
				case "map<string,list<string>>":
				case "map<int,list<float>>":
				case "map<float,list<int>>":
				case "map<int,list<string>>":
				case "map<string,list<int>>":
				case "map<float,list<string>>":
				case "map<string,list<float>>":
					arr = value.Split(',');
					content = "";
					for (int i = 0; i < arr.Length; i++) {
						childArr = arr[i].Split(':');
						typeArr = type.Split(',');
						if (childArr.Length == 2) {
							if (IsString(typeArr[0])) {
								content += Utils.ToFormat("['{0}']={1},", childArr[0], GetList(childArr[1], typeArr[1]));
							} else {
								content += Utils.ToFormat("[{0}]={1},", childArr[0], GetList(childArr[1], typeArr[1]));
							}
						}
					}
					content = content.Substring(0, content.Length - 1);
					result = Utils.ToFormat("{{0}},", content);
					break;
				//keyvalue<int,list<int>>		1:10_10_10_10		{key = 1, value = {10, 10, 10, 10}}
				case "keyvalue<int,list<int>>":
				case "keyvalue<float,list<float>>":
				case "keyvalue<string,list<string>>":
				case "keyvalue<int,list<float>>":
				case "keyvalue<float,list<int>>":
				case "keyvalue<int,list<string>>":
				case "keyvalue<string,list<int>>":
				case "keyvalue<float,list<string>>":
				case "keyvalue<string,list<float>>":
					arr = value.Split(':');
					typeArr = type.Split(',');
					if (arr.Length == 2) {
						if (IsString(typeArr[0])) {
							content = Utils.ToFormat("key='{0}',value={1}", arr[0], GetList(arr[1], typeArr[1]));
						} else {
							content = Utils.ToFormat("key={0},value={1}", arr[0], GetList(arr[1], typeArr[1]));
						}
					}
					result = Utils.ToFormat("{{0}},", content);
					break;
				default:
					return Utils.ToFormat("'{0}',", value);
			}
			return result;
		}

		public static string Getkeyvalue(string value, string type) {
			if (string.IsNullOrEmpty(value)) {
				return "nil";
			}
			string[] typeArr = type.Split(',');
			string a1 = IsString(typeArr[0]) ? "'{0}'" : "{0}";
			string a2 = IsString(typeArr[1]) ? "'{1}'" : "{1}";
			string content = "";
			string[] arr = null;
			if (value.IndexOf(":") != -1) {
				arr = value.Split(':');
			} else {
				arr = value.Split('_');
			}
			if (arr.Length == 2) {
				content = string.Format("key=" + a1 + ",value=" + a2, arr[0], arr[1]);
			}
			return Utils.ToFormat("{{0}}", content);
		}

		public static string GetList(string value, string type) {
			string content = "";
			string[] arr = null;
			if (value.IndexOf("_") != -1) {
				arr = value.Split('_');
			} else {
				arr = value.Split(',');
			}
			if (arr.Length > 0) {
				for (int i = 0; i < arr.Length; i++) {
					if (IsString(type)) {
						content += string.Format("'{0}',", arr[i]);
					} else {
						content += arr[i] + ',';
					}
				}
				content = content.Substring(0, content.Length - 1);
			}
			return Utils.ToFormat("{{0}}", content);
		}

		public static string GetMap(string value, string type) {
			string[] typeArr = type.Split(',');
			string a1 = IsString(typeArr[0]) ? "'{0}'" : "{0}";
			string a2 = IsString(typeArr[1]) ? "'{1}'" : "{1}";
			string content = "";
			string[] arr = null;
			if (value.IndexOf(":") != -1) {
				arr = value.Split(':');
			} else {
				arr = value.Split('_');
			}
			
			if (arr.Length == 2) {
				content = Utils.ToFormat("[" + a1 + "] = " + a2, arr[0], arr[1]);
			}
			return Utils.ToFormat("{0}", content);
		}

		public static bool IsString(string type) {
			return type.IndexOf("string") != -1;
		}

		public static string GetType(string type, string value) {
			type = type.ToLower();
			switch (type) {
				case "int":
				case "long":
				case "float":
					return "number";
				case "string":
					return "string";
				case "bool":
					return "bool";
				case "list<int>":
				case "list<float>":
				case "map<int,int>":
				case "map<float,float>":
				case "map<int,float>":
				case "map<float,int>":
					return "table<number, number>";
				case "map<float,string>":
				case "map<int,string>":
				case "list<string>":
					return "table<number, string>";
				case "map<string,int>":
				case "map<string,float>":
					return "table<string, number>";
				case "map<string,string>":
					return "table<string, string>";
				case "keyvalue<int,int>":
				case "keyvalue<float,float>":
				case "keyvalue<string,string>":
				case "keyvalue<int,float>":
				case "keyvalue<float,int>":
				case "keyvalue<float,string>":
				case "keyvalue<string,float>":
				case "keyvalue<int,string>":
				case "keyvalue<string,int>":
					return "KeyValue";
				case "list<keyvalue<int,int>>":
				case "list<keyvalue<float,float>>":
				case "list<keyvalue<string,string>>":
				case "list<keyvalue<int,float>>":
				case "list<keyvalue<float,int>>":
				case "list<keyvalue<float,string>>":
				case "list<keyvalue<string,float>>":
				case "list<keyvalue<int,string>>":
				case "list<keyvalue<string,int>>":
				case "map<int,keyvalue<int,int>>":
					return "table<number, KeyValue>";
				case "list<list<int>>":
				case "list<list<float>>":
				case "map<int,list<int>>":
				case "map<float,list<float>>":
				case "map<int,list<float>>":
				case "map<float,list<int>>":
				case "keyvalue<int,list<int>>":
				case "keyvalue<float,list<float>>":
				case "keyvalue<float,list<int>>":
				case "keyvalue<int,list<float>>":
					return "table<number, table<number, number>>";
				case "map<int,list<string>>":
				case "map<float,list<string>>":
				case "keyvalue<int,list<string>>":
				case "keyvalue<float,list<string>>":
				case "list<list<string>>":
					return "table<number, table<number, string>>";
				case "map<string,list<int>>":				
				case "map<string,list<float>>":				
				case "keyvalue<string,list<int>>":				
				case "keyvalue<string,list<float>>":
					return "table<string, table<number, number>>";
				case "keyvalue<string,list<string>>":
				case "map<string,list<string>>":
					return "table<string, table<number, string>>";
				default:
					return SpecialType(type, value);
			}
		}
	}
}
