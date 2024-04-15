using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable enable
public class LazerReciever : MonoBehaviour
{
    bool isPointedAt = false;    //is the raycast looking at it
    bool isActivated;
    Button? button;
    Toggle? toggle;
    Slider? slider;

    bool isBttn;
    bool isTogg;
    bool isSlid;

    Collider coll;
    Vector3 hitPos = Vector3.zero;
    Vector3 center;
    Vector2 center2D;

    Vector2 distFromCenterSimplified;
    Vector2 hitPos2D;

    [SerializeField] GameObject cursorParent;

    [SerializeField] GameObject? bottLeftBound;  //empty that wont move, just there to tell us where the bounds are in 2D

    Vector3 bottLeftPos;    //just to shorten the code writing.
    Vector2 bottLeftPos2D;  //just in case there is any weird z axis shenanigans. 

    private void Start()
    {
        coll = GetComponent<Collider>();
        center = transform.position;

        if (bottLeftBound != null) bottLeftPos = bottLeftBound.transform.localPosition;

        bottLeftPos2D = new Vector2(bottLeftPos.x, bottLeftPos.y);

        if (GetComponentInParent<Button>() != null)
        {
            button = GetComponentInParent<Button>();
            isBttn = true;
        }

        if (GetComponentInParent<Toggle>() != null)
        {
            toggle = GetComponentInParent<Toggle>();
            isTogg = true;
        }

        if (GetComponentInParent<Slider>() != null)
        {
            slider = GetComponentInParent<Slider>();
            isSlid = true;
        }
    }

    private void Update()
    {
        //Debug.Log($"Button:{isBttn}, Toggle:{isTogg}, Slider:{isSlid}, ParentName: {gameObject.GetComponentInParent<RectTransform>().gameObject.name}");
    }

    public void PointedAt(Vector3 hitPosition)
    {
        hitPos = hitPosition;
        Debug.Log($"{transform.name} is being pointed at");

        Button buttonPointedAt = transform.GetComponentInParent<Button>();
        if(buttonPointedAt != null )
        {
            GameObject callingObject = buttonPointedAt.transform.parent.gameObject;
            if( callingObject != null )
            {
                callingObject.GetComponentInChildren<TextMeshProUGUI>().fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.3f);
                callingObject.GetComponent<Button>().Select();
            }
        }
        //button.OnPointerEnter()
        //PointerEventData

        
        //button.Select();
        //TextMeshProUGUI text = button.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        //text.fontMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 0.3f);
    }

    public Vector3 DistFromCenter()
    {
        if (hitPos == Vector3.zero)
        {
            Debug.Log("HitPosition = Zero. Reciever has not been pointed at ever.");
        }
        Vector3 distFromCenter = hitPos - center;
        return distFromCenter;
    }

    public Vector2 CursorPos2D()
    {   //returns the position of the pointer as if the canvas was a 2d flat screen.
        cursorParent.transform.position = hitPos;
        return cursorParent.transform.localPosition;
    }

    public Vector2 CursorPosScreen()
    {   //Returns cursor position as if it was measuring from Bottom Left of the screen
        return CursorPos2D() - bottLeftPos2D;
    }



    public void Activate()
    {   //Makes the button react as if it has been clicked. Runs all the OnClick() events  in the Button component

        if (isBttn)
        {
            Debug.Log($"{button.onClick.GetPersistentMethodName(0)}");  //returns name of the method we are calling
            button.onClick.Invoke();
        }
        else if (isTogg)
        {
            Debug.Log($"Toggle Switch");
            toggle.isOn = !toggle.isOn;
        }
        else if (isSlid)
        {
            //handle localPos = new vector2 (hitpos.x, 0)
            Debug.Log($"Toggle Switch");
            toggle.isOn = !toggle.isOn;
        }
    }
}
