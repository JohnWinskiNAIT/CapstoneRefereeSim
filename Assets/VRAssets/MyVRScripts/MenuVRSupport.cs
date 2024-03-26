using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuVRSupport : MonoBehaviour
{
    [SerializeField] GameObject canvasParent;
    Canvas canvas;
    public bool isVRActive = true;
    [SerializeField] Transform canvasWorldTrans;
    [SerializeField] Transform canvasScreenTrans;

    [SerializeField] Camera cam;

    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        isVRActive = GameUtilities.VREnabled();
        //CanvasStart();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isVRActive)
            {
                isVRActive = false;
            }
            else
            {
                isVRActive = true;
            }
        }

        CanvasStart();
    }

    public void CanvasStart()
    {
        if (isVRActive)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvasParent.transform.SetPositionAndRotation(canvasWorldTrans.position, canvasWorldTrans.rotation);
            canvasParent.transform.localScale = new Vector3(canvasWorldTrans.lossyScale.x, canvasWorldTrans.lossyScale.y, canvasWorldTrans.lossyScale.z);

            canvas.worldCamera = cam;
        }
        else
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasParent.transform.SetPositionAndRotation(canvasScreenTrans.position, canvasScreenTrans.rotation);
            canvasParent.transform.localScale = new Vector3(canvasScreenTrans.lossyScale.x, canvasScreenTrans.lossyScale.y, canvasScreenTrans.lossyScale.z);
        }
    }
}
