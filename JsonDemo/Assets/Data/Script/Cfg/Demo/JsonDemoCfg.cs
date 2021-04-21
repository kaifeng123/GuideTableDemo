using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ../table/测试-测试(demo)
/// </summary>
public class JsonDemoCfg : ICfg
{
	/// <summary>
	/// 道具ID
	/// </summary>
	public int id{ get; protected set; }
	/// <summary>
	/// 名称
	/// </summary>
	public string _name{ get; protected set; }


	public  void AutoParse(string[] source){
		this.id = TableToType.ToInt(source[0]);
		this._name = source[1];

	}


	//非自动生成代码 Start
	public string GetKey(){
		return id.ToString();
	}

	public void Init() {

	}
	//非自动生成代码 End
}
