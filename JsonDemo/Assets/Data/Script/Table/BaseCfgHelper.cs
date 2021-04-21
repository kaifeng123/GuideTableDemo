using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public abstract class BaseCfgHelper<T, U> : PureSingleton<T>
	where T : new()
	where U : class, ICfg, new()
{

	protected Dictionary<string, U> cfgDictionary = new Dictionary<string, U>();

	protected List<U> cfgList = new List<U>();

	public BaseCfgHelper()
	{
		LoadConfig<U>(ref cfgList);
		AnalysisConfig();
	}

	public void LoadConfig<U>(ref List<U> cfgList) where U : class, ICfg, new()
	{
		string cfgName = typeof(U).Name;
		byte[] bytes = Resources.Load<TextAsset>(cfgName).bytes;
		//byte[] bytes = AssetMgr.LoadConfig(cfgName);

		if (bytes.Length == 0)
		{
			return;
		}
		MemoryStream ms = new MemoryStream(bytes);
		BinaryReader br = new BinaryReader(ms, Encoding.UTF8);
		int _linecount = br.ReadInt32();

		for (int i = 0; i < _linecount; i++)
		{
			//string strid = br.ReadString();
			string strcontent = br.ReadString();

			U cfg = new U();
			try
			{
				cfg.AutoParse(strcontent.Split('\t'));
				cfg.Init();
				cfgList.Add(cfg);
			}
			catch(Exception e)
			{
				Debug.LogError(string.Format("{0}表解析错误：{1}，行数据：{2}", cfgName, e.Message, strcontent));
			}
		}

		ms.Close();
		br.Close();
	}

	protected virtual void AnalysisConfig()
	{
		cfgList.ForEach(cfg => {
			if (cfgDictionary.ContainsKey(cfg.GetKey()))
			{
				cfgDictionary[cfg.GetKey()] = cfg;
			}
			else
			{
				cfgDictionary.Add(cfg.GetKey(), cfg);
			}
		});

		Init();
	}

	protected virtual void Init()
	{
		//特殊初始化时，子类可复写这里
	}

	public virtual U GetCfg(long key)
	{
		return GetCfg(key.ToString());
	}

	public virtual U GetCfg(int key)
	{
		return GetCfg(key.ToString());
	}

	public virtual U GetCfg(string key)
	{
		if (string.IsNullOrEmpty(key) || !cfgDictionary.ContainsKey(key))
		{
			//OutLog.LogError("获取配置表失败：Config:", typeof(U).Name, "key:", key);
			return null;
		}
		else
		{
			return cfgDictionary[key];
		}
	}

	public List<U> GetAllCfg()
	{
		return cfgList;
	}

	public int CfgCount {
		get { return cfgList.Count; }
	}

	public bool Contains(int key)
	{
		return Contains(key.ToString());
	}

	public bool Contains(string key)
	{
		if (cfgDictionary.ContainsKey(key))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public virtual U GetLast()
	{
		return cfgList.GetLast();
	}

	public virtual U GetFirst()
	{
		return cfgList[0];
	}
}
