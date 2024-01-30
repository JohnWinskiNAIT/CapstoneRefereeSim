using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] BasicHockeyAi[] hockeyAi;
    player[] players;
    // Start is called before the first frame update
    void Start()
    {
        //grabs all the players in each hockey ai script
        for (var index = 0; index < hockeyAi.Length; index++)
        {
            players[index] = hockeyAi[index].GetThisPlayer();
            players[index].playerIndex = index;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

}
