using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;

public class LazerReciever : MonoBehaviour
{
    bool isPointedAt = false;    //is the raycast looking at it
    bool isActivated;
    Button button;

    private void Start()
    {
        button = GetComponentInParent<Button>();
    }

    private void Update()
    {
        PointedAtUpdate();
    }

    public void PointedAtUpdate()
    {
        if (isPointedAt)
        {
            Debug.Log($"Is being pointed at");
        }
    }

    public void Activate()
    {
        //Debug.Log($"Is being Activated");

        //button.clickable.clicked += true;

        //m_OnClick.m_PersistentCalls.m_Calls;

        //m_OnClick.m_PersistentCalls.m_Calls.Array.data[0].m_Target

        //button.onClick.GetPersistentMethodName(0);

        Debug.Log($"{button.onClick.GetPersistentMethodName(0)}"); 

        button.onClick.Invoke();
    }
}
