using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] BasicHockeyAi[] hockeyAi;
    HockeyPlayer[] players;
    private InputAction passAction;
    [SerializeField]
    InputActionAsset inputActions;

    private void Awake()
    {
        players = new HockeyPlayer[2];
        passAction = inputActions.FindActionMap("Testing").FindAction("Pass");
    }
    void Start()
    {

        //test = hockeyAi.Length;
        //grabs all the players in each hockey ai script
        for (var index = 0; index < hockeyAi.Length; index++)
        {

            players[index] = hockeyAi[index].gameObject.GetComponent<HockeyPlayer>();
            players[index].playerIndex = index;
        }
    }
    void Update()
    {
        if (passAction.WasPressedThisFrame())
        {
            Debug.Log("passed");
            hockeyAi[0].PassPuck(players[1]);
        }
    }
    #region Eanble and Disable 
    private void OnEnable()
    {
        passAction.Enable();
    }
    private void OnDisable()
    {
        passAction.Disable();
    }
    #endregion
}
