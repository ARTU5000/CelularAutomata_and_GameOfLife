using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() //Inicia con el timeScale en 0 para poder pintar antes de iniciar la simulación
    {
        Time.timeScale = 0.0f;
    }

    
    public void Play() //Activa la simulación
    {
        Time.timeScale = 1.0f;
    }

    public void Pause() //Pausa la simulación
    {
        Time.timeScale = 0.0f;
    }
}
