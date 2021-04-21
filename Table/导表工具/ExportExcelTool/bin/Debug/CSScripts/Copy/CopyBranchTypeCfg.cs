using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// LocalData/G-关卡-支线段
/// </summary>
public class CopyBranchTypeCfg : ICfg
{
	
	/// <summary>
	/// 章
	/// </summary>
	public int evolve{ get; protected set; }

	/// <summary>
	/// 剧本
	/// </summary>
	public int Script{ get; protected set; }

	/// <summary>
	/// 开放等级
	/// </summary>
	public int unlock{ get; protected set; }

	/// <summary>
	/// 支线段名
	/// </summary>
	public string _evolveName{ get; protected set; }

	/// <summary>
	/// 支线段的ICON
	/// </summary>
	public string icon{ get; protected set; }


	public  void AutoParse(string[] source){
		this.evolve = TableToType.ToInt(source[0]);
		this.Script = TableToType.ToInt(source[1]);
		this.unlock = TableToType.ToInt(source[2]);
		this._evolveName = source[3];
		this.icon = source[4];

	}

	//非自动生成代码 Start
	public string GetKey(){
		return evolve.ToString();
	}

	public void Init() {

	}
	//非自动生成代码 End
}
