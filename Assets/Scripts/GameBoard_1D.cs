using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class GameBoard_1D : MonoBehaviour
{
    [SerializeField]private Tilemap currentState;
    [SerializeField]private Tilemap nextState;
    [SerializeField]private Tilemap backTiles;
    [SerializeField]private Tile aliveTile;
    [SerializeField]private Tile deadTile;
    [SerializeField]private Pattern pattern;
    [SerializeField]private float updateInterval = 0.05f;

    //limites de la simulación
    [SerializeField]private int limitX;
    [SerializeField]private int limitY;
    [SerializeField]private TMP_InputField inputFieldX;
    [SerializeField]private TMP_InputField inputFieldY;

    [SerializeField] private TMP_InputField ruleInputField; // El campo para cambiar la regla
    private int ruleNumber = 30;

    private HashSet<Vector3Int> aliveCells;
    private HashSet<Vector3Int> cellsToCheck;

    public int population { get; private set; }
    public int iteration { get; private set;}
    public float time { get; private set;}

    private void Awake()
    {
        aliveCells = new HashSet<Vector3Int>();
        cellsToCheck = new HashSet<Vector3Int>();
        FillWithDeadTiles();
    }

    private void Start()
    {
        SetPatern(pattern);
    }
    private void SetPatern(Pattern pattern)
    {
        Clear();

        Vector2Int center = pattern.GetCenter();

        for (int i = 0; i < pattern.cells.Length; i++)
        {
            Vector3Int cell = (Vector3Int)(pattern.cells[i] - center);
            currentState.SetTile(cell, aliveTile);
            aliveCells.Add(cell);
        }

        population = aliveCells.Count;
    }

    public void Clear()
    {
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
        aliveCells.Clear();
        cellsToCheck.Clear();
        population = 0;
        iteration = 0;
        time = 0;
    }

    private void OnEnable()
    {
        StartCoroutine(Simulate());
    }

    private IEnumerator Simulate()
    {
        var interval = new WaitForSeconds(updateInterval);
        yield return interval;

        while (enabled)
        {
            UpdateState();
            
            population = aliveCells.Count;
            iteration++;
            time += updateInterval;

            yield return interval;
        }
    }
    
    private void UpdateState()
    {
        HashSet<Vector3Int> newAliveCells = new HashSet<Vector3Int>();  // Nuevo conjunto para las celdas vivas
        cellsToCheck.Clear();

        foreach (Vector3Int cell in aliveCells)
        {
            Vector3Int leftNeighbor = cell + new Vector3Int(-1, 0, 0);
            Vector3Int rightNeighbor = cell + new Vector3Int(1, 0, 0);

            // Verifica que estén dentro de los límites
            if (Mathf.Abs(leftNeighbor.x) <= limitX)
                cellsToCheck.Add(leftNeighbor);

            if (Mathf.Abs(rightNeighbor.x) <= limitX)
                cellsToCheck.Add(rightNeighbor);

            if (Mathf.Abs(cell.x) <= limitX) 
                cellsToCheck.Add(cell);
        }

        foreach (Vector3Int cell in cellsToCheck)// Evalua el estado de las celdas para la siguiente generación
        {
            Vector3Int leftNeighbor = cell + new Vector3Int(-1, 0, 0);
            Vector3Int rightNeighbor = cell + new Vector3Int(1, 0, 0);

            bool newAliveState = ApplyRule(IsAlive(leftNeighbor), IsAlive(cell), IsAlive(rightNeighbor));// Implementa la regla 30

            Vector3Int lowerCell = cell + new Vector3Int(0, -1, 0);// Cambia el estado de la celda en el nivel inferior

            if (lowerCell.y < -limitY)// Ignora cualquier celda que esté por debajo del límite inferior
                continue;  

            if (newAliveState)
            {
                nextState.SetTile(lowerCell, aliveTile);
                newAliveCells.Add(lowerCell);
            }
            else
                nextState.SetTile(lowerCell, deadTile);
        }

        aliveCells = newAliveCells; // Actualiza las celdas vivas para la siguiente iteración

        // Intercambiamos los Tilemaps
        Tilemap temp = currentState;
        currentState = nextState;
        nextState = temp;
    }

    private bool ApplyRule(bool leftAlive, bool currentAlive, bool rightAlive)
    {
        // Convierte la regla en un número binario (un array de 8 bits)
        int binaryState = (leftAlive ? 4 : 0) + (currentAlive ? 2 : 0) + (rightAlive ? 1 : 0);

        // Convierte la regla a binario y verifica si el bit correspondiente está activado
        return ((ruleNumber >> binaryState) & 1) == 1;
    }

    public bool IsAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == aliveTile;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Detecta el clic Derecho
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Convierte la posición del ratón a coordenadas del mundo
            Vector3Int cellPos = currentState.WorldToCell(mouseWorldPos); // Convierte las coordenadas del mundo a coordenadas del Tilemap

            TileBase currentTile = currentState.GetTile(cellPos);

            if (Mathf.Abs(cellPos.x) <= limitX && Mathf.Abs(cellPos.y) <= limitY) //revisa que este dentro del limite
            {
                if (currentTile == aliveTile)// La tile ya está viva
                {
                    currentState.SetTile(cellPos, deadTile); //La tile deja de estar viva
                    aliveCells.Remove(cellPos); // Remueve la celda de las celdas vivas
                }
                else// Si no hay una tile viva, procede a colocar una nueva tile viva
                {
                    currentState.SetTile(cellPos, aliveTile); // Coloca una tile viva donde se hizo clic
                    aliveCells.Add(cellPos); // Añade la celda a las celdas vivas
                }
            }
        }
    }

    public void UpdateLimits() //actualiza los límites
    {
        if (int.TryParse(inputFieldX.text, out int newX))
            limitX = newX/2;

        if (int.TryParse(inputFieldY.text, out int newY))
            limitY = newY/2;
    }

    public void FillWithDeadTiles()
    {
        UpdateLimits(); //actualiza los límites
        backTiles.ClearAllTiles();
        Clear();

        for (int x = -limitX; x <= limitX; x++) // Llena el área dentro de los límites con deadTiles
        {
            for (int y = -limitY; y <= limitY; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                backTiles.SetTile(cell, deadTile);
            }
        }
    }

    public void SetRuleFromInput()
    {
        if (int.TryParse(ruleInputField.text, out int newRule)) //convierte el input en un numero
            ruleNumber = newRule; // Asigna la nueva regla
        else
            Debug.LogError("El valor ingresado no es un número válido");
    }
}
