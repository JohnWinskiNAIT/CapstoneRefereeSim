using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stinkycontroller : MonoBehaviour
{
    [SerializeField] LazerEmitter emitter;

    [SerializeField] GameObject canvasParent;
    Canvas canvas;
    public bool isVrActive = false;
    [SerializeField] Transform canvasWorldTrans;
    [SerializeField] Transform canvasScreenTrans;

    [SerializeField] Camera cam;

    private void Start()
    {
        canvas = canvasParent.GetComponentInChildren<Canvas>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            emitter.Activate();
            //Debug.Log("MouseDown");
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            emitter.Deactivate();
            //Debug.Log("MouseUp");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isVrActive)
            {
                isVrActive = false;
            }
            else
            {
                isVrActive = true;
            }
        }

        CanvasUpdate();

    }
    public void CanvasUpdate()
    {
        if (isVrActive)
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
