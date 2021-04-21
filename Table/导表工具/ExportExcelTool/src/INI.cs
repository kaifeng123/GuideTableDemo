using System.Collections;

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

class INI
{
    [System.Runtime.InteropServices.DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string section,string key,string val
        ,string filePath);
    [System.Runtime.InteropServices.DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section,string key
        ,string def,System.Text.StringBuilder retVal,int size,string filePath);
    private string sPath = null;

    private static string defaultName="";

    public static string getDefaultName()
    {
        if (defaultName == "")
        {
            defaultName = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "setting.ini";
        }
        return defaultName;
    }

    public INI(string path)
    {
        this.sPath = path;
    }

    public void Write(string section,string key,string value)
    {

        WritePrivateProfileString(section,key,value,sPath);
    }

    public string ReadValue(string section,string key)
    {
        System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
        GetPrivateProfileString(section,key,"",temp,255,sPath);
        return temp.ToString();
    }
}
