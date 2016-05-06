using UnityEngine;
using System.Collections;

public class NotDestroyObject : MonoBehaviour
{ 
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}


