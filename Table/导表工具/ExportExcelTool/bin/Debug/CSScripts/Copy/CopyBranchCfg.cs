using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// LocalData/G-关卡-支线
/// </summary>
public class CopyBranchCfg : ICopyCfg
{
	
	/// <summary>
	/// id
	/// </summary>
	public int id{ get; protected set; }

	/// <summary>
	/// 支线章
	/// </summary>
	public int evolve{ get; protected set; }

	/// <summary>
	/// VIP等级
	/// </summary>
	public List<int> vipLevel{ get; protected set; }

	/// <summary>
	/// 名称
	/// </summary>
	public string _sectionName{ get; protected set; }

	/// <summary>
	/// 说明
	/// </summary>
	public string _description{ get; protected set; }

	/// <summary>
	/// 章节(删）
	/// </summary>
	public int chapter{ get; protected set; }

	/// <summary>
	/// 难度
	/// </summary>
	public int difficulty{ get; protected set; }

	/// <summary>
	/// 入口坐标(删)
	/// </summary>
	public List<int> point{ get; protected set; }

	/// <summary>
	/// 地图
	/// </summary>
	public int mapId{ get; protected set; }

	/// <summary>
	/// 出生点
	/// </summary>
	public List<int> playerStart{ get; protected set; }

	/// <summary>
	/// 切波条件
	/// </summary>
	public string winCondition{ get; protected set; }

	/// <summary>
	/// 切波参数
	/// </summary>
	public string winArg{ get; protected set; }

	/// <summary>
	/// 第一波敌人
	/// </summary>
	public string enemyGroup1{ get; protected set; }

	/// <summary>
	/// 奖励ID
	/// </summary>
	public int rewardId{ get; protected set; }

	/// <summary>
	/// 扫荡额外奖励id
	/// </summary>
	public int extraRewardId{ get; protected set; }

	/// <summary>
	/// 消耗入场券数量
	/// </summary>
	public int needTicket{ get; protected set; }

	/// <summary>
	/// 剧情卡牌1
	/// </summary>
	public string _scriptCard{ get; protected set; }


	public override void AutoParse(string[] source){
		this.id = TableToType.ToInt(source[0]);
		this.evolve = TableToType.ToInt(source[1]);
		this.vipLevel = TableToType.ToListInt(source[2]);
		this._sectionName = source[3];
		this._description = source[4];
		this.chapter = TableToType.ToInt(source[5]);
		this.difficulty = TableToType.ToInt(source[6]);
		this.point = TableToType.ToListInt(source[7]);
		this.mapId = TableToType.ToInt(source[8]);
		this.playerStart = TableToType.ToListInt(source[9]);
		this.winCondition = source[10];
		this.winArg = source[11];
		this.enemyGroup1 = source[12];
		this.rewardId = TableToType.ToInt(source[13]);
		this.extraRewardId = TableToType.ToInt(source[14]);
		this.needTicket = TableToType.ToInt(source[15]);
		this._scriptCard = source[16];

	}

	//非自动生成代码 Start
	public string GetKey(){
		return id.ToString();
	}

	public void Init() {

	}
	//非自动生成代码 End
}
