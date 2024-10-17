using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    [SerializeField]private Tilemap currentState;
    [SerializeField]private Tilemap nextState;
    [SerializeField]private Tile aliveTile;
    [SerializeField]private Tile deadTile;
    [SerializeField]private Pattern pattern;
    [SerializeField]private float updateInterval = 0.05f;

    private HashSet<Vector3Int> aliveCells;
    private HashSet<Vector3Int> cellsToCheck;

    public int population { get; private set; }
    public int iteration { get; private set;}
    public float time { get; private set;}

    private void Awake()
    {
        aliveCells = new HashSet<Vector3Int>();
        cellsToCheck = new HashSet<Vector3Int>();
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
        //Juntando las celdas a revisar
        foreach (Vector3Int cell in aliveCells)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    cellsToCheck.Add(cell + new Vector3Int (x, y, 0));
                }
            }
        }

        //cambiando al siguiente estado
        foreach (Vector3Int cell in cellsToCheck)
        {
            int neighbors = CountNeighbors(cell);
            bool alive = IsAlive(cell);

            if (!alive && neighbors == 3)
            {
                nextState.SetTile(cell, aliveTile);
                aliveCells.Add(cell);
            }
            else if (alive && (neighbors < 2 || neighbors > 3))
            {
                nextState.SetTile(cell, deadTile);
                aliveCells.Remove(cell);
            }
            else
            {
                nextState.SetTile(cell, currentState.GetTile(cell));
            }
        }

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
        return currentState.GetTile(cell) == aliveTile;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detecta el clic izquierdo
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Convierte la posición del ratón a coordenadas del mundo
            Vector3Int cellPos = currentState.WorldToCell(mouseWorldPos); // Convierte las coordenadas del mundo a coordenadas del Tilemap

            TileBase currentTile = currentState.GetTile(cellPos);

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
