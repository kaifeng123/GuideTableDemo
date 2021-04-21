using UnityEngine;
using System.Collections;
//µ¥ÀıÀà
public abstract class PureSingleton<T> where T : new() {
	private static T _instance;
	public static T instance{
		get{
			if(_instance == null){
				_instance = new T();
			}
			return _instance;
		}
	}
}

