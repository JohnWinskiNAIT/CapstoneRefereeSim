using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    static public AIManager Instance { get; private set; }

    public List<GameObject> leftTeamPlayers { get; private set; }

    public List<GameObject> rightTeamPlayers { get; private set; }

    private GameObject puckObject;

    [SerializeField]
    float ignoreTimer;

    // Start is called before the first frame update
    private void Awake()
    {
        leftTeamPlayers = new List<GameObject>();
        rightTeamPlayers = new List<GameObject>();
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

            for (int i = 0; i < leftTeamPlayers.Count; i++)
            {
                if ((leftTeamPlayers[i].transform.position - puckObject.transform.position).magnitude < distance)
                {
                    redirectPlayer = leftTeamPlayers[i];
                    distance = (redirectPlayer.transform.position - puckObject.transform.position).magnitude;
                }
            }
            if (redirectPlayer != null)
            {
                Debug.Log("Test");
                redirectPlayer.GetComponent<ZoneAIController>().DeclarePosition(puckObject.transform.position);
            }
            for (int i = 0; i < rightTeamPlayers.Count; i++)
            {
                if ((rightTeamPlayers[i].transform.position - puckObject.transform.position).magnitude < distance)
                {
                    redirectPlayer = rightTeamPlayers[i];
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
