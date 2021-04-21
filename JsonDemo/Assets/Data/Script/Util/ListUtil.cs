using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ListUtil {
	/// <summary>
	/// 获取list最后一个item
	/// </summary>
	public static T GetLast<T>(this List<T> list) {
		return list[list.Count - 1];
	}

	public static List<object> ToObjectList<T>(this List<T> list) {
		List<object> datas = new List<object>();
		list.ForEach(a => datas.Add(a));
		return datas;
	}
	public static bool BoolKey<T,T1>(this Dictionary<T, T1> dic,T Key)
	{
		return dic.ContainsKey(Key);
	}
	/// <summary>
	/// 列表为空
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="List"></param>
	/// <returns></returns>
	public static bool ListIsNoll<T>(this List<T> List)
	{
		return List == null || List.Count == 0;
	}
}
