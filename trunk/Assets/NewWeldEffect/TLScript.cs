using UnityEngine;
using System.Collections;
using wcore;

public class TLScript : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DCxCO2WeldingControl.isPress = true;
        }
        if (DCxCO2WeldingControl.isPress)
        {
			transform.GetChild(0).Translate(Vector3.right*0.00012f, Space.World);
        } 

    }
}
