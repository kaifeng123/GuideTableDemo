using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// LocalData/G-关卡-星星奖励
/// </summary>
public class StarRewardCfg : ICfg
{
	
	/// <summary>
	/// 节ID
	/// </summary>
	public int id{ get; protected set; }

	/// <summary>
	/// 需要星星数
	/// </summary>
	public int needStar{ get; protected set; }

	/// <summary>
	/// 奖励id
	/// </summary>
	public int reward{ get; protected set; }

	/// <summary>
	/// 需要星星数2
	/// </summary>
	public int needStar2{ get; protected set; }

	/// <summary>
	/// 奖励id2
	/// </summary>
	public int reward2{ get; protected set; }


	public  void AutoParse(string[] source){
		this.id = TableToType.ToInt(source[0]);
		this.needStar = TableToType.ToInt(source[1]);
		this.reward = TableToType.ToInt(source[2]);
		this.needStar2 = TableToType.ToInt(source[3]);
		this.reward2 = TableToType.ToInt(source[4]);

	}

	//非自动生成代码 Start
	public string GetKey(){
		return id.ToString();
	}

	public void Init() {

	}
	//非自动生成代码 End
}
