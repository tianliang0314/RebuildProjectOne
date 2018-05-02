using UnityEngine;
using System;
using System.Collections;
using wcore;

/// <summary>
/// 焊版定位
/// </summary>
public class DCxWeldingPlateControl : SMxMonoBehaviour
{
    void Awake()
    {
        GameObject steelBlockPlane = GameObject.Find("SteelBlockPlane");
        if (steelBlockPlane != null)
        {
            steelBlockPlane.AddComponent<WBxWeldingBedManagerCO2>();

        }
        steelBlockPlane.GetComponent<WBxWeldingBedManagerCO2>().Awake();
        steelBlockPlane.GetComponent<WBxWeldingBedManagerCO2>().Start();
    }

    public override void SceneObjectStart()
    {
        //GameObject father = GameObject.Find("WeldedPlatePos");
        //gameObject.transform.parent = father.transform;
        //gameObject.transform.localPosition = new Vector3(0, 0, 0);
        //gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    void OnDestroy()
    {

    }
}

