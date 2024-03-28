using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    static public AIManager Instance { get; private set; }

    public List<GameObject> LeftTeamPlayers { get; private set; }

    public List<GameObject> RightTeamPlayers { get; private set; }

    private GameObject puckObject;

    [SerializeField]
    float ignoreTimer;

    // Start is called before the first frame update
    private void Awake()
    {
        LeftTeamPlayers = new List<GameObject>();
        RightTeamPlayers = new List<GameObject>();
        if (Instance == null)
        {
            Instance = gameObject.GetComponent<AIManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (puckObject.GetComponent<PuckManager>().ignoredTime > ignoreTimer)
        {
            GameObject redirectPlayer = null;
            float distance = float.PositiveInfinity;

            for (int i = 0; i < LeftTeamPlayers.Count; i++)
            {
                if ((LeftTeamPlayers[i].transform.position - puckObject.transform.position).magnitude < distance)
                {
                    redirectPlayer = LeftTeamPlayers[i];
                    distance = (redirectPlayer.transform.position - puckObject.transform.position).magnitude;
                }
            }
            if (redirectPlayer != null)
            {
                redirectPlayer.GetComponent<ZoneAIController>().DeclarePosition(puckObject.transform.position);
            }
            for (int i = 0; i < RightTeamPlayers.Count; i++)
            {
                if ((RightTeamPlayers[i].transform.position - puckObject.transform.position).magnitude < distance)
                {
                    redirectPlayer = RightTeamPlayers[i];
                    distance = (redirectPlayer.transform.position - puckObject.transform.position).magnitude;
                }
            }
            if (redirectPlayer != null)
            {
                redirectPlayer.GetComponent<ZoneAIController>().DeclarePosition(puckObject.transform.position);
            }

            puckObject.GetComponent<PuckManager>().ResetTime();
        }
    }

    public void PuckCallback(GameObject puck)
    {
        puckObject = puck;
    }
}
