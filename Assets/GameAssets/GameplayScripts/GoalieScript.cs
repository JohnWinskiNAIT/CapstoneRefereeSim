using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalieScript : MonoBehaviour
{
    [SerializeField] bool letScore;
    player goaliePlayer;
    // Start is called before the first frame update
    void Start()
    {
        //goalies are players too
        goaliePlayer = new player();
    }

    // Update is called once per frame
    void Update()
    {
        if (goaliePlayer.puckPossesion)
        {
            if (letScore)
            {
                //insert goal manager score thing here.
            }
            else
            {
                //insert save mechanic here.
            }
        }
    }

}
