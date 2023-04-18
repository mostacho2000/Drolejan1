using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   public int vidas;
   public int puntos;
   public int numGranadas;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    

}
