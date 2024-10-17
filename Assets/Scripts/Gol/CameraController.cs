using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public float speed; // Velocidad de movimiento de la cámara
    public Slider zoom; // El slider para controlar el zoom
    public Camera camera; 

    void Start()
    {
        zoom.minValue = 10f;
        zoom.maxValue = 150f;
        zoom.value = camera.orthographicSize;

        zoom.onValueChanged.AddListener(SetZoom);
    }

    void Update()
    {
        Vector3 move = new Vector3();

        if (Input.GetKey(KeyCode.W)) // Arriba
            move.y += 1;
        if (Input.GetKey(KeyCode.S)) // Abajo
            move.y -= 1;
        if (Input.GetKey(KeyCode.A)) // Izquierda
            move.x -= 1;
        if (Input.GetKey(KeyCode.D)) // Derecha
            move.x += 1;

        transform.position += move * speed * Time.deltaTime; //movimiento de la camara
    }
  
    void SetZoom(float value)// ajusta el zoom de la camara
    {
        camera.orthographicSize = value;
    }

    public void CenterCamera()// regresa la camara al punto de origen
    {
        transform.position = new Vector3(0, 0, transform.position.z);
    }
}
