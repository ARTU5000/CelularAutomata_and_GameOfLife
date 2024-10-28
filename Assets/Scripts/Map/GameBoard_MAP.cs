using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class GameBoard_Map : MonoBehaviour
{
    [SerializeField]private Tilemap currentState;
    [SerializeField]private Tilemap nextState;
    [SerializeField]private Tilemap backTiles;
    [SerializeField]private Tile sandTile;
    [SerializeField]private Tile waterTile;
    [SerializeField]private Tile grassTile;
    [SerializeField]private Tile rockTile;
    [SerializeField]private Tile lavaTile;
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
            currentState.SetTile(cell, sandTile);
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
        // Juntando las celdas a revisar, limitadas por los bordes
        foreach (Vector3Int cell in aliveCells)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector3Int neighborCell = cell + new Vector3Int(x, y, 0);

                    if (Mathf.Abs(neighborCell.x) <= limitX && Mathf.Abs(neighborCell.y) <= limitY) // revisa que este dentro del limite
                    {
                        cellsToCheck.Add(neighborCell);
                    }
                }
            }
        }

        // Cambiando al siguiente estado, limitado por los bordes
        foreach (Vector3Int cell in cellsToCheck)
        {
            if (Mathf.Abs(cell.x) > limitX || Mathf.Abs(cell.y) > limitY) // revisa que este dentro del limite
            {
                continue;
            }

            int neighbors = CountNeighbors(cell);
            bool alive = IsAlive(cell);

            if (!alive && neighbors == 2)
            {
                nextState.SetTile(cell, sandTile);
                aliveCells.Add(cell);
            }
            else if (alive && (neighbors == 2))
            {
                nextState.SetTile(cell, waterTile);
                aliveCells.Remove(cell);
            }
            else if (alive && (neighbors > 2 && neighbors <= 4))
            {
                nextState.SetTile(cell, grassTile);
            }
            else if (alive && (neighbors > 4 && neighbors < 7))
            {
                nextState.SetTile(cell, waterTile);
            }
            else if (alive && (neighbors == 7))
            {
                nextState.SetTile(cell, rockTile);
            }
            else if (alive && (neighbors == 8))
            {
                nextState.SetTile(cell, lavaTile);
            }
            else
            {
                nextState.SetTile(cell, currentState.GetTile(cell));
            }
        }

        // Intercambiamos los Tilemaps
        Tilemap temp = currentState;
        currentState = nextState;
        nextState = temp;
        nextState.ClearAllTiles();
    }

    private int CountNeighbors(Vector3Int cell)
    {
        int count = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighbor = cell + new Vector3Int (x, y, 0);

                if (x == 0 && y == 0)
                {
                    continue;
                }
                else if (IsAlive(neighbor))
                {
                    count++;
                }
            }
        }

        return count;
    }

    public bool IsAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == sandTile;
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
                if (currentTile == sandTile)// La tile ya está viva
                {
                    currentState.SetTile(cellPos, waterTile); //La tile deja de estar viva
                    aliveCells.Remove(cellPos); // Remueve la celda de las celdas vivas
                }
                else// Si no hay una tile viva, procede a colocar una nueva tile viva
                {
                    currentState.SetTile(cellPos, sandTile); // Coloca una tile viva donde se hizo clic
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
                backTiles.SetTile(cell, waterTile);
                currentState.SetTile(cell, waterTile);
                aliveCells.Add(cell);
            }
        }
    }

    void CheckLimitTiles()
    {
        for (int x = -limitX; x <= limitX; x++)
        {
            for (int y = -limitY; y <= limitY; y++)
            {
            Vector3Int cell = new Vector3Int(x, y, 0);
            cellsToCheck.Add(cell);
            }
        }
    }
}
