using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace OneWTech.RobertWelding.ProjectTools
{
    public class GlobalFunctions : MonoBehaviour
    {

        public static bool g_EnableSound = false;            //全局声音开关
        public static float g_SoundVolume = 1.0f;           //全局声音音量
        public static bool g_AllowClickButton = true;
        public static KeyCode g_ForbidKey = 0;
        public static int isForbid = 0;
        public static void ForbidKey(KeyCode n_Keycode)
        {
            g_ForbidKey = n_Keycode;
        }

        public static void AllowGetKey(KeyCode n_Keycode)
        {
            g_ForbidKey = 0;
        }
        public static bool GetKey(KeyCode n_Keycode)
        {
            if (g_ForbidKey != n_Keycode)
                return Input.GetKey(n_Keycode);
            else
                return false;
        }

        public static bool GetKeyDown(KeyCode n_Keycode)
        {
            if (g_ForbidKey != n_Keycode)
                return Input.GetKeyDown(n_Keycode);
            else
                return false;
        }

        public static bool GetKeyUp(KeyCode n_Keycode)
        {
            if (g_ForbidKey != n_Keycode)
                return Input.GetKeyUp(n_Keycode);
            else
                return false;
        }

        public static bool GetButton(string n_ButtonName)
        {
            if (g_AllowClickButton)
                return Input.GetButton(n_ButtonName);
            else
                return false;
        }

        public static bool GetButtonDown(string n_ButtonName)
        {
            if (g_AllowClickButton)
                return Input.GetButtonDown(n_ButtonName);
            else
                return false;
        }

        public static bool GetButtonUp(string n_ButtonName)
        {
            if (g_AllowClickButton)
                return Input.GetButtonUp(n_ButtonName);
            else
                return false;
        }

        public static bool GetMouseButton(int n_MouseButton)
        {
            if (g_AllowClickButton)
                return Input.GetMouseButton(n_MouseButton);
            else
                return false;
        }

        public static bool GetMouseButtonDown(int n_MouseButton)
        {
            if (g_AllowClickButton)
                return Input.GetMouseButtonDown(n_MouseButton);
            else
                return false;
        }

        public static bool GetMouseButtonUp(int n_MouseButton)
        {
            if (g_AllowClickButton)
                return Input.GetMouseButtonUp(n_MouseButton);
            else
                return false;
        }

        public static bool anyKey()
        {
            return Input.anyKey;
        }

        public static bool anyKeyDown()
        {
            return Input.anyKeyDown;
        }

        public static float GetAxis(string n_AxisName)
        {
            if (n_AxisName != null && n_AxisName != "")
                return Input.GetAxis(n_AxisName);
            else
                return 0f;
        }

        public static float GetAxisRaw(string n_AxisName)
        {
            if (n_AxisName != null && n_AxisName != "")
                return Input.GetAxisRaw(n_AxisName);
            else
                return 0f;
        }

        public static Touch GetTouch(int n_Index)
        {
            if (n_Index > 0)
                return Input.GetTouch(n_Index);
            else
                return Input.GetTouch(0);
        }

        public static Vector3 GetMousePosition()
        {
            return Input.mousePosition;
        }

        public static void ForBidAllKey(bool n_IsAllowClick)
        {
            if (n_IsAllowClick)
                g_AllowClickButton = false;
            else
                g_AllowClickButton = true;
            return;
        }



        public static string ReadXml(string n_FileName, string n_NodeName)
        {
            string FilePath = Application.dataPath + "/Configs/" + n_FileName + ".xml";
            if (File.Exists(FilePath))
            {
                string t_Data = "";
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load(FilePath);
                XmlNodeList xmlNode = XmlDoc.SelectSingleNode("Config").ChildNodes;

                foreach (XmlElement node in xmlNode)
                {
                    if (node.Name == n_NodeName)
                    {
                        t_Data = node.InnerText;
                        return t_Data;
                    }
                }
            }
            return "";
        }
        
        public static T ResourceLoad<T>(string n_Path = "") where T : class, new()
        {
            T t = default(T);
            if (typeof(T).Equals(typeof(GameObject)))
            {
                t = MonoBehaviour.Instantiate(Resources.Load(n_Path)) as T;
                if (t == null)
                    return null;
                GameObject t_Tmp = t as GameObject;
                if (t_Tmp.name.Length > 7)
                    t_Tmp.name = t_Tmp.name.Remove(t_Tmp.name.Length - 7);
                t = t_Tmp as T;
                return t;
            }
            else
            {
                t = Resources.Load(n_Path, typeof(T)) as T;
                if (t == null)
                    return null;
            }
            
            return t;
        }
    }
}