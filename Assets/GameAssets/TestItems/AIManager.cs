using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    static public AIManager Instance { get; private set; }

    public List<GameObject> leftTeamPlayers { get; private set; }
    public List<GameObject> rightTeamPlayers { get; private set; }

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
}
