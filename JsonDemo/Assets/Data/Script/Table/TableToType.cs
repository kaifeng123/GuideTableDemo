using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TableToType
{
	public static int ToInt(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return 0;
		}
		try
		{
			return int.Parse(str);
		}
		catch
		{
			throw new Exception(string.Format("{0}不能转换为int类型", str));
		}
	}

	public static float ToFloat(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return 0f;
		}
		try
		{
			return float.Parse(str);
		}
		catch
		{
			throw new Exception(string.Format("{0}不能转换为float类型", str));
		}
	}

	public static long ToLong(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return 0;
		}
		try
		{
			return long.Parse(str);
		}
		catch
		{
			throw new Exception(string.Format("{0}不能转换为long类型", str));
		}
	}

	public static bool ToBool(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return false;
		}
		try
		{
			return bool.Parse(str);
		}
		catch
		{
			throw new Exception(string.Format("{0}不能转换为bool类型", str));
		}
	}

	public static T ToEnum<T>(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return (T)(object)0;
		}
		try
		{
			return (T)(object)int.Parse(str);
		}
		catch
		{
			throw new Exception(string.Format("{0}不能转换为枚举{1}类型", str, typeof(T).Name));
		}
	}

	public static T ToObject<T>(string str) where T : class, ICfgObject, new()
	{
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}

		try
		{
			T obj = new T();
			obj.AutoParse(str.Split('_'));
			return obj;
		}
		catch(Exception e)
		{
			throw new Exception(string.Format("{0}不能转换为{1}类型", str, typeof(T).Name) + "-->" + e.Message);
		}
	}

	public static List<int> ToListInt(string str)
	{
		List<int> list = new List<int>();
		if (string.IsNullOrEmpty(str))
		{
			return list;
		}

		try
		{
			string[] arr = str.Split(',');
			for (int i = 0; i < arr.Length; i++)
			{
				list.Add(ToInt(arr[i]));
			}
			return list;
		}
		catch (Exception e)
		{
			throw new Exception(string.Format("{0}不能转换为List<int>类型", str) + "-->" + e.Message);
		}
	}

	public static List<float> ToListFloat(string str)
	{
		List<float> list = new List<float>();
		if (string.IsNullOrEmpty(str))
		{
			return list;
		}

		try
		{
			string[] arr = str.Split(',');
			for (int i = 0; i < arr.Length; i++)
			{
				list.Add(ToFloat(arr[i]));
			}
			return list;
		}
		catch (Exception e)
		{
			throw new Exception(string.Format("{0}不能转换为List<float>类型", str) + "-->" + e.Message);
		}
	}

	public static List<string> ToListString(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return new List<string>();
		}

		string[] arr = str.Split(',');
		return arr.ToList();
	}

	public static List<T> ToListEnum<T>(string str)
	{
		List<T> list = new List<T>();
		if (string.IsNullOrEmpty(str))
		{
			return list;
		}

		try
		{
			string[] arr = str.Split(',');
			for (int i = 0; i < arr.Length; i++)
			{
				list.Add(ToEnum<T>(arr[i]));
			}
			return list;
		}
		catch (Exception e)
		{
			throw new Exception(string.Format("{0}不能转换为枚举List<{1}>类型", str, typeof(T).Name) + "-->" + e.Message);
		}
	}

	public static List<T> ToListObject<T>(string str) where T : class, ICfgObject, new()
	{
		List<T> list = new List<T>();
		if (string.IsNullOrEmpty(str))
		{
			return list;
		}

		try
		{
			string[] arr = str.Split('|');
			for (int i = 0; i < arr.Length; i++)
			{
				T t = ToObject<T>(arr[i]);
				if (t != null)
				{
					list.Add(t);
				}
			}
			return list;
		}
		catch (Exception e)
		{
			throw new Exception(string.Format("{0}不能转换为List<{1}>类型", str, typeof(T).Name) + "-->" + e.Message);
		}
	}
}