using UnityEngine;
using System;
using System.Collections;
using RichToolKits;
//using RStructure;

public class UTxInitConfigs : MonoBehaviour {

	// Use this for initialization
	void Awake () 
	{
		RConsole.OnInitConsole("");
        Application.targetFrameRate = 1000;
		MessagerCenter.Register("InitConfig", InitConfig);
		MessagerCenter<bool>.Register("SetDebug", SetDebug);
		// MessagerCenter<bool>.Register("QCToolsOn", QCToolsOn);
		// MessagerCenter<bool>.Register("TruckToolsOn", QCTruckToolsOn);
		MessagerCenter<string>.Register("ec", ec);
		MessagerCenter.Broadcast("InitConfig");
		//Debug.Log("初始化开始  in  " + this.name);
		//Debug.Log("初始化开始  in  " + this);
	}
	void InitConfig()
	{
		try
		{
			ConfigManager.SetConfigPath(Application.dataPath + "/Configs/");
			bool bSuccess = ConfigManager.LoadConfig("GlobleVars.xml");
			if(!bSuccess) 
			{
				Debug.LogError(Application.dataPath + "/Configs/GlobleVars.xml is not exist!");
				return;				
			}
//			Configuration cfg = ConfigManager.GetConfig("GlobleVars.xml");//
//			string w,h,fullscreen,debug;
//			cfg.Get("iScreenWidth", out w);
//			cfg.Get("iScreenHeight", out h);
//			cfg.Get("bFullScreen", out fullscreen);
//			cfg.Get("iDebugLevel", out debug);
//			int iScreenWidth = int.Parse(w);
//			int iScreenHeight = int.Parse(h);//
//			bool bFullScreen = bool.Parse(fullscreen);
//			Screen.SetResolution(iScreenWidth, iScreenHeight, bFullScreen);//			
//          GlobleVars.SetString("sMacDcode", ToolKitsVersion.GetPublicCodeOfThisMachine());
//			Debug.Log("DebugLv = " + GlobleVars.GetInt("iDebugLevel").ToString());
		}
		catch(Exception ex)
		{
            Debug.LogException(ex);
		}
	}

	void Update () 
	{
		RConsole.OnUpdate();
		if(Input.GetKeyDown(KeyCode.Space))
		{
			RConsole.OnInitConsole("");
		}
		if(Input.GetKeyDown(KeyCode.D))
		{
			Debug.Log(MessagerCenter.a());
		}
	}
    public void SetDebug(bool debugOn)
	{
		RConsole.IsConsoleOn = debugOn;
		
//		int a = 5;
//		string s = string.Format("{0:00}",a);
//		Debug.Log("a=" + s);
	}
	public void ec(string code)
	{
/*		if(RichToolKits.DeviceUtils.EnterExtendCode(code))
			Debug.LogError("Extend Code Error!");
		else
			Debug.Log("Extend Code Successed!");*/
	}


	//public void QCTruckToolsOn(bool toolsOn)
	//{
	//    SOxTruckManager.truckEditorTools = toolsOn;
	//}


	//public void QCToolsOn(bool toolsOn)
	//{
	//    QCEditorTools.bQCEditorTools = toolsOn;
	//}

	void OnGUI()
	{
		RConsole.OnDisplay();	
	}
	void OnDestroy()
	{
		MessagerCenter.UnRegister("InitConfig", InitConfig);
		MessagerCenter<bool>.UnRegister("SetDebug", SetDebug);
//		MessagerCenter<bool>.UnRegister("QCToolsOn", QCToolsOn);
	}



//	// 通过代码进行Unity与CoreObject的绑定
//	public bool AttatchUnityObject(string unityObjectName, Type type)
//	{
//		GameObject go = GameObject.Find(unityObjectName);
//		if (go == null)
//		{
//			Debug.LogError("Unity Object [" + unityObjectName + "] is not found ! ");
//			return false;
//		}
//		//go.AddComponent(type);
//		Component com = go.AddComponent(type);
//		FieldInfo fi = com.GetType().GetField("isShadowInstance");
//		fi.SetValue(com, true);
//		
//		return true;
//	}
}
