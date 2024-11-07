using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonsMap : MonoBehaviour
{
    public GameObject PlayMap;
    public GameObject PlayPause;
    public GameObject pause;
    public GameObject clear;
    public GameObject iteration;
    public GameObject AIPlay;
    public GameObject Change;

    // Start is called before the first frame update
    void Start() //Inicia con el timeScale en 0 para poder pintar antes de iniciar la simulación
    {
        Time.timeScale = 1.0f;
        PlayMap.SetActive(true); // Activa PlayMap al inicio
        PlayPause.SetActive(false); // Desactiva PlayPause al inicio
        pause.SetActive(false); // Desactiva Pause al inicio
        clear.SetActive(false); // Desactiva clear al inicio
        AIPlay.SetActive(false); // Desactiva la IA al inicio
        iteration.SetActive(true); // Activa iteration al inicio
        Change.SetActive(true); // Activa el cambio de tamaño al inicio
    }

    public void Play() //Activa la simulación
    {
        Time.timeScale = 1.0f;
        
        PlayMap.SetActive(false); 
        PlayPause.SetActive(false); 
        pause.SetActive(true);
        clear.SetActive(false); 
        iteration.SetActive(false);
        AIPlay.SetActive(false);
        Change.SetActive(false);
    }

    public void Pause() //Pausa la simulación
    {
        Time.timeScale = 0.0f;
        PlayMap.SetActive(false); 
        pause.SetActive(false); 
        PlayPause.SetActive(true); 
        clear.SetActive(true); 
        iteration.SetActive(false);
        Change.SetActive(false);
    }

    public void Clear()
    {
        PlayMap.SetActive(false); 
        pause.SetActive(false); 
        PlayPause.SetActive(true); 
        clear.SetActive(true); 
        iteration.SetActive(true);
        AIPlay.SetActive(false);
        Change.SetActive(true);
    }

    public void Iterate() 
    {
        PlayMap.SetActive(true); 
        pause.SetActive(false); 
        PlayPause.SetActive(false); 
        clear.SetActive(false); 
        iteration.SetActive(true);
        Change.SetActive(true);
        AIPlay.SetActive(false);
    }

    public void change() 
    {
        PlayMap.SetActive(true); 
        pause.SetActive(false); 
        PlayPause.SetActive(false); 
        clear.SetActive(false); 
        iteration.SetActive(true);
        Change.SetActive(true);
        AIPlay.SetActive(false);
    }
}
