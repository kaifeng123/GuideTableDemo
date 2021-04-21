using System;
using System.Collections.Generic;
using System.IO;

namespace ExportExcelTool {
	public enum EExportPortType
	{
		All = 0,	//客户端与服务端
		Client = 1,	//只导客户端
		Server = 2,	//只导服务端
	}

	public class Main {
		public static EExportPortType exportType = EExportPortType.All;

		/// <summary>
		/// 生成最近几小时修改的表
		/// </summary>
        public static float lastModifiedTime;
        //xlsx文件目录
        private string xlsxPath = "";
        //lua文件目录
        public static string luaPath = "";
		//C# 脚本目录
		public static string csScriptPath = "";
		//C# 数据目录
		public static string csDataPath = "";
		//Java 脚本目录
		public static string javaScriptPath = "";
		//Java 数据目录
		public static string javaDataPath = "";
		//所有xlsx文件名字
		private List<string> listFiles = new List<string>();
		private void initPara() {
			string startPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			INI _ini = new INI(INI.getDefaultName());
			xlsxPath = startPath + _ini.ReadValue("Table", "XlsxPath");
			luaPath = startPath + _ini.ReadValue("Table", "LuaPath");
			csScriptPath = startPath + _ini.ReadValue("Table", "CSScriptPath");
			csDataPath = startPath + _ini.ReadValue("Table", "CSDataPath");
			javaScriptPath = startPath + _ini.ReadValue("Table", "JavaScriptPath");
			javaDataPath = startPath + _ini.ReadValue("Table", "JavaDataPath");
		}
		private void EnumerateFiles(string _path) {
			try {
				string dirPath = _path;
				if (!Directory.Exists(dirPath)) {
					Console.WriteLine(_path + "文件夹不存在");
					return;
				}
				List<string> files = new List<string>(Directory.EnumerateFiles(dirPath));
				foreach (var fil in files) {
					if ((fil.EndsWith("xls") || fil.EndsWith("xlsx")) && fil.IndexOf("~$") < 0) {
						if (fil.IndexOf("属性编号对应") < 0) {
							if (Main.lastModifiedTime == 0 || IsTodayChanges(fil)) {
								listFiles.Add(fil);
							}
						}
					}
				}
				List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
				foreach (var dir in dirs) {
					if (dir.IndexOf(".svn") > 0) {
						continue;
					}
					EnumerateFiles(dir);
				}
			} catch (UnauthorizedAccessException UAEx) {
				Console.WriteLine(UAEx.Message);
			} catch (PathTooLongException PathEx) {
				Console.WriteLine(PathEx.Message);
			}
		}

		public void Init() {
			initPara();
			listFiles.Clear();
		}

		public void Run(float value) {
			lastModifiedTime = value;
			EnumerateFiles(xlsxPath);
			CreateClassDic();
		}

		private void CreateClassDic() {
			TableContent.Clear();
			for (int i = 0; i < listFiles.Count; i++) {
				TableContent.AnalysisTable(listFiles[i]);
			}

			//导出lua文件
			List<string> luaImportList = new List<string>();
			foreach (var vvv in TableContent.luaDictTable) {
                if (vvv.Value.lines.Count > 0)
                {
                    luaImportList.AddRange(CreateLua.CreateLuaFile(vvv.Value));
				}
            }
			if (lastModifiedTime == 0)
			{
				if (luaImportList.Count > 0)
				{
					CreateLua.ImportLuaRequire(luaImportList);
				}
			}

			//导出C#文件
			foreach (var vvv in TableContent.cSharpDictTable)
			{
				if (vvv.Value.lines.Count > 0)
				{
					CreateCSharp.CreateFile(vvv.Value);
				}
			}

			//导出java文件
			List<string> javaImportList = new List<string>();
			foreach (var vvv in TableContent.javaDictTable)
			{
				if (vvv.Value.lines.Count > 0)
				{
					CreateJava.CreateFile(vvv.Value);
				}
			}
		}

		public static void AddErrorLog(string str) {
            Console.WriteLine("错误!错误!--" + str);
            Console.ReadLine();
        }

		/// <summary>
		/// 今天是否修改过
		/// </summary>
		/// <param name="_fullPath"></param>
		/// <returns></returns>
		public static bool IsTodayChanges(string _fullPath) {
			FileInfo fileInfo = new FileInfo(_fullPath);
			long lastTime = (int)(Main.lastModifiedTime * 60 * 60 * 1000);
			TimeSpan ts = DateTime.Now - fileInfo.LastWriteTime;
			long diffTime = (long)ts.TotalMilliseconds;
			return diffTime < lastTime;
		}
	}
}
