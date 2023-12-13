using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    void Awake()
    {
        instance = this;
    }
    public AlphaGrilMove mAlphaGrilMove;
    public SessionListManager mSessionListManager;

}
