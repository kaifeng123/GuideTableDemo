using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class UIText : MonoBehaviour
{
    public TextAsset asset;
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        MemoryStream ms = new MemoryStream(asset.bytes);
        BinaryReader br = new BinaryReader(ms, Encoding.UTF8);
        int _linecount = br.ReadInt32();
        //Debug.Log(br.ReadString());
        //Debug.Log(_linecount);
        for (int i = 0; i < _linecount; i++)
        {
            Debug.Log(br.ReadString());
        }
       // Resources
        // Debug.Log(br.ReadString());
        List<string> strList = new List<string>() {"周","亚","威" };
        var str= string.Join(",",strList);
        Debug.Log(str);
        text.text = JsonDemoCfgHelper.instance.GetFirst()._name.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
