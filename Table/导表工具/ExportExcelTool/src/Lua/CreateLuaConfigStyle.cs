using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExportExcelTool {
	/// <summary>
	/// 导出为配置表样式
	/// </summary>
	class CreateLuaConfigStyle {
		public static List<string> CreateLuaFile(string luaPath, TableInfo info) {
			string pCfgname = info.createName;

            string[] annotation = info.annotations;
            string[] types = info.types;
            string[] keys = info.keys;

			string cfgname = "Cfg" + pCfgname;
			string cfgtablename = cfgname + "Table";
			string luafilename = luaPath + cfgname + ".lua";
			List<string> cutOut = CreateLua.GetNotCreateArray(luafilename, cfgname);

			if (types.Length != keys.Length) {
				Main.AddErrorLog(string.Format("{0}配置表第 2 行与第 4 行长度不一", info.excelName));
				return new List<string> { };
			}

			FileStream luaStream = new FileStream(luafilename, FileMode.CreateNew, FileAccess.Write);
			//utf8 无bom
			Encoding _utf8NoMark = new UTF8Encoding(false);
			StreamWriter luaStreamWriter = new StreamWriter(luaStream, _utf8NoMark);

            CreateLua.WriteLineN(luaStreamWriter, "\r\n--" + info.excelName) ;
			CreateConfigFun(luaStreamWriter, cfgname, annotation, types, keys);

			string[] getKey = info.groupKeys;
			if (cutOut == null) {
				CreateLua.WriteLineN(luaStreamWriter, "--非自动生成代码 Start");
				CreateLua.WriteLineN(luaStreamWriter, "--非自动生成代码 End");
			} else {
				for (int i = 0; i < cutOut.Count; i++) {
					CreateLua.WriteLineN(luaStreamWriter, cutOut[i]);
				}
			}
            string annotationStr = "";

            CreateLua.WriteLineN(luaStreamWriter, "\r\n");
			CreateLua.WriteLineN(luaStreamWriter, string.Format("local {0} = ", cfgtablename) + "{");
			List<string> verifyKey = new List<string>();
			for (int i = 0; i < info.lines.Count; i++) {
                string[] lineArr = info.lines[i];

				string luaLine = "";
				string keyString = "";
				for (int j = 0; j < keys.Length; j++) {
					if (j == 0) {
						if (getKey.Length > 0 && !string.IsNullOrEmpty(getKey[0])) {
							List<string> tempkeys = new List<string>();
							annotationStr = "";
							for (int k = 0; k < getKey.Length; k++) {
								tempkeys.Add(CreateLua.GetKeyValue(getKey[k], keys, lineArr));
								annotationStr += CreateLua.GetKeyValue(getKey[k], keys, annotation) + ",";
							}
							keyString = string.Join("_", tempkeys);
						} 

						if (getKey.Length > 1) {
							keyString = "'" + keyString + "'";
						}else {
							string keyType = "";
							if (getKey.Length == 1 && !string.IsNullOrEmpty(getKey[0])) {
								keyString = lineArr[Array.IndexOf(keys, getKey[0])];
								keyType = types[Array.IndexOf(keys, getKey[0])];
							}else {
								keyString = lineArr[j];
								keyType = types[j];
							}
							if (keyType != "INT" && keyType != "LONG") {
								keyString = "'" + keyString + "'";
							}
						}

						luaLine = string.Format("[{0}]=", keyString) + "{";
						if (verifyKey.IndexOf(keyString) != -1) {
							Main.AddErrorLog(string.Format("在【{0}.xls】中发现重复Key【{1}】，请检查", info.excelName, keyString));
						}else {
							verifyKey.Add(keyString);
						}
					}
					if (!string.IsNullOrEmpty(types[j]) && types[j] != "NO") {
						luaLine += CreateLuaType.ToType(types[j], j < lineArr.Length ? lineArr[j] : "");
					}
				}
				luaLine += "},";
				luaLine = luaLine.Replace(",}", "}").Replace(", }", "}");
				if (!string.IsNullOrEmpty(keyString)) {
					CreateLua.WriteLineN(luaStreamWriter, luaLine);
				}
			}

			CreateLua.WriteLineN(luaStreamWriter, "}");
			CreateLua.WriteLineN(luaStreamWriter, "return " + cfgtablename);

			luaStreamWriter.Close();
			luaStream.Close();

			Console.WriteLine("------导出【" + cfgname + ".lua】成功！");

			return new List<string> { CreateConfigHelperFun(luaPath, info.folderName, cfgname, info.excelName, info.groupKeys, annotationStr) };
		}


		private static void CreateConfigFun(StreamWriter luaStreamWriter, string fileName, string[] annotation, string[] types, string[] keys) {
			string configStr =
@"---@class $config_name
$config_name = BaseClass()

function $config_name:ctor(data)
$content
	data = nil
end
";
			configStr = configStr.Replace("$config_name", fileName);
			string ann = "";
			int diff = 0;
			for (int i = 0; i < keys.Length; i++) {
				if (!string.IsNullOrEmpty(types[i]) && types[i] != "NO") {
					string annot = Utils.FiltrationString(annotation[i]);
					ann += string.Format("\t---@type {0}", CreateLuaType.GetType(types[i], "")) +"\r\n";
					ann += string.Format("\tself.{0} = data[{1}] --{2}", keys[i], (i + 1) - diff, annot) + "\r\n";
				} else {
					diff++;
				}
			}
			ann = ann.Replace(",}", "}").Replace(", }", "}");
			CreateLua.WriteLineN(luaStreamWriter, configStr.Replace("$content", ann));
		}

		/// <summary>
		/// 生成configHelper文件
		/// </summary>
		/// <param name="fileName"></param>
		private static string CreateConfigHelperFun(string luaPath, string folder, string cfgname, string xlsName, string[] keyArr, string annotationStr) {
			string helperName = cfgname + "Helper";
			if (helperName == "CfgcmdHelper") {//CMD不生成
				return "CfgcmdHelper";
			}
			string luafilename = luaPath + helperName + ".lua";
			List<string> cutOut = CreateLua.GetNotCreateArray(luafilename, helperName);

			FileStream luaStream = new FileStream(luafilename, FileMode.CreateNew, FileAccess.Write);
			//utf8 无bom
			Encoding _utf8NoMark = new UTF8Encoding(false);
			StreamWriter luaStreamWriter = new StreamWriter(luaStream, _utf8NoMark);

			string configStr =
@"
--$xlsName
---@class $helperName
local $helperName = BaseClass()

---@type table<key, {}>
local _sourceTable	--源数据

---@type table<key, $cfgname>
local _classTable = {}	--解析后的数据

function $helperName:ctor()
	_sourceTable = reload('luaconfig.$folder.$cfgname')
end

--初始化所有配置数据
function $helperName:InitAllConfig()
	if table.length(_sourceTable) > 0 then
		for k, v in pairs(_sourceTable) do
			_classTable[k] = $cfgname.New(v)
			_sourceTable[k] = nil
		end
	end
end

--获取整张配置表数据
---@return table<key, $cfgname>
function $helperName:GetTable()
	self:InitAllConfig()
	return _classTable;
end

---@param key string or number 配置表ID
---@param value bool 不传或true时，如果未找到则返回第一条数据(防止未找到报错问题)
---@return $cfgname
function $helperName:Get(key, value)
	if _classTable[key] then
		return _classTable[key]
	elseif _sourceTable[key] then
		_classTable[key] = $cfgname.New(_sourceTable[key])
		_sourceTable[key] = nil
	else
		if value == nil or value == true then
			LogColor(Color.red, '从$cfgnameTable配置表获取Key错误 Key:%s  允许容错处理返回第一条数据', key)
			if table.length(_classTable) > 0 then
				return table.first(_classTable)
			elseif table.length(_sourceTable) > 0 then
				local v, k = table.first(_sourceTable)
				_classTable[k] = $cfgname.New(v)
				_sourceTable[k] = nil
				return _classTable[k]
			end
		else
			--LogColor(Color.yellow, '从$cfgnameTable配置表获取Key错误 Key:%s', Key)
		end
	end
	return _classTable[key]
end
";

			configStr = configStr.Replace("$xlsName", xlsName);
			configStr = configStr.Replace("$helperName", helperName);
			configStr = configStr.Replace("$folder", folder);
			configStr = configStr.Replace("$cfgname", cfgname);
			CreateLua.WriteLineN(luaStreamWriter, configStr);

			#region 如果有组合key时
			//如果有组合key时
			if (keyArr.Length > 0) {
				string[] annotationArr = annotationStr.Split(',');
				CreateLua.WriteLineN(luaStreamWriter, "--根据组合Key获取配置数据");
				for (int i = 0; i < keyArr.Length; i++) {
					CreateLua.WriteLineN(luaStreamWriter, string.Format("---@param {0} number {1}", keyArr[i], Utils.FiltrationString(annotationArr[i])));
				}
configStr = @"---@param value bool 不传或true时，如果未找到则返回第一条数据(防止未找到报错问题)
---@return $cfgname
function $helperName:GetOfKey($keyParam1, value)
	if $keyParam3 then
		LogError('组合Key不可为nil, $keyParam4', $keyParam1)
	end
	return self:Get($keyParam2, value)
end
";
				string str1 = string.Join(", ", keyArr);
				string str2 = string.Join(" .. '_' .. ", keyArr);
				configStr = configStr.Replace("$keyParam1", string.Join(", ", keyArr));

				string[] keyArr2 = new string[keyArr.Length];
				for(int i = 0; i < keyArr.Length; i++) {
					keyArr2[i] = string.Format("({0} or 0)", keyArr[i]);
				}
				configStr = configStr.Replace("$keyParam2", string.Join(" .. '_' .. ", keyArr2));

				string[] keyArr3 = new string[keyArr.Length];
				for (int i = 0; i < keyArr.Length; i++) {
					keyArr3[i] = string.Format("{0} == nil", keyArr[i]);
				}
				configStr = configStr.Replace("$keyParam3", string.Join(" or ", keyArr3));

				string[] keyArr4 = new string[keyArr.Length];
				for (int i = 0; i < keyArr.Length; i++) {
					keyArr4[i] = string.Format("{0}:%s", keyArr[i]);
				}
				configStr = configStr.Replace("$keyParam4", string.Join(", ", keyArr4));

				configStr = configStr.Replace("$helperName", helperName);
				configStr = configStr.Replace("$cfgname", cfgname);
				CreateLua.WriteLineN(luaStreamWriter, configStr);
			}
			#endregion

			if (cutOut == null) {
				CreateLua.WriteLineN(luaStreamWriter, "--非自动生成代码 Start\r\n");
				CreateLua.WriteLineN(luaStreamWriter, "--非自动生成代码 End");
			} else {
				for (int i = 0; i < cutOut.Count; i++) {
					CreateLua.WriteLineN(luaStreamWriter, cutOut[i]);
				}
			}

			CreateLua.WriteLineN(luaStreamWriter, "return " + helperName);
			
			luaStreamWriter.Close();
			luaStream.Close();

			string helperStr =
@"
local _$cfgname
---@return $helperName $xlsName
function SingleConfig.$cfgname()
    if _$cfgname == nil then
        _$cfgname = reload('luaconfig.$folder.$helperName').New()
	end
	return _$cfgname
end";
			cfgname = cfgname.Substring(3);
			helperStr = helperStr.Replace("$xlsName", xlsName);
			helperStr = helperStr.Replace("$cfgname", cfgname);
			helperStr = helperStr.Replace("$helperName", helperName);
			helperStr = helperStr.Replace("$folder", folder);
			return helperStr;
		}
	}
}
