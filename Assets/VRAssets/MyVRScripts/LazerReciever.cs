using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;

public class LazerReciever : MonoBehaviour
{
    bool isPointedAt = false;    //is the raycast looking at it
    bool isActivated;
    Button button;
    Vector3 hitPos = Vector3.zero;
    Vector3 center;
    Vector3 distFromCenter;
    Vector2 distFromCenterSimplified;

    GameObject cursorRepresentative;

    private void Start()
    {
        center = transform.position;
        button = GetComponentInParent<Button>();
    }

    private void Update()
    {

    }

    public void PointedAt(Vector3 hitPosition)
    {
        hitPos = hitPosition;

        if (isPointedAt)
        {
            Debug.Log($"Is being pointed at");
        }
    }

    public Vector3 DistFromCenter()
    {
        distFromCenter = hitPos - center;
        return distFromCenter;
    }

    public Vector2 DistFromCenter2D()
    {
        cursorRepresentative.transform.position = hitPos;
        //distFromCenterSimplified = new Vector2(distFromCenter.);
        return distFromCenterSimplified;
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
