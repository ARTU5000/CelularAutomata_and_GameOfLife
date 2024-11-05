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
    [SerializeField]private Tile iceTile;
    [SerializeField]private Pattern pattern;
    [SerializeField]private float updateInterval = 0.05f;

    //limites de la simulación
    [SerializeField]private int limitX;
    [SerializeField]private int limitY;
    [SerializeField]private TMP_InputField inputFieldX;
    [SerializeField]private TMP_InputField inputFieldY;

    [SerializeField] private TMP_InputField ruleInputField; // El campo para cambiar la regla
    private int ruleNumber = 30;

    [SerializeField]private HashSet<Vector3Int> aliveCells;
    private HashSet<Vector3Int> cellsToCheck;

    [SerializeField]public int population { get; private set; }
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
        for (int x = -limitX; x <= limitX; x++) // Llena el área dentro de los límites con deadTiles
        {
            for (int y = -limitY; y <= limitY; y++)
            {
                int rand = Random.Range(0, 15);
                Tile introTile = lavaTile;
                if (rand >= 0 && rand < 5)
                    {introTile = waterTile;}
                else if (rand >= 5 && rand < 8)
                    {introTile = sandTile;}
                else if (rand >= 8 && rand < 13)
                    {introTile = grassTile;}
                else if (rand >= 13 && rand < 14)
                    {introTile = rockTile;}
                else if (rand >= 14 && rand < 15)
                    {introTile = lavaTile;}

                Vector3Int cell = new Vector3Int(x, y, 0);
                currentState.SetTile(cell, introTile);
                aliveCells.Add(cell);
            }
        }
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
    // Asegurarse de que se verifiquen todas las celdas dentro de los límites en cada iteración
    cellsToCheck.Clear();
    CheckLimitTiles();

    // Cambiando al siguiente estado
    foreach (Vector3Int cell in cellsToCheck)
    {
        if (Mathf.Abs(cell.x) > limitX || Mathf.Abs(cell.y) > limitY) // revisa que esté dentro del límite
        {
            continue;
        }

        TileBase currentTile = currentState.GetTile(cell);

        int waterNeighbors = CountSpecificNeighbors(cell, waterTile);
        int sandNeighbors = CountSpecificNeighbors(cell, sandTile);
        int grassNeighbors = CountSpecificNeighbors(cell, grassTile);
        int rockNeighbors = CountSpecificNeighbors(cell, rockTile);
        int iceNeighbors = CountSpecificNeighbors(cell, iceTile);
        int lavaNeighbors = CountSpecificNeighbors(cell, lavaTile);

        // Condiciones de cambio de tiles
        if (currentTile == waterTile)
        {
            if (waterNeighbors == 8)
            {
                float rand = Random.value;
                if (rand <= 0.1f)
                {
                    nextState.SetTile(cell, iceTile); // Cambia de agua a hielo
                    aliveCells.Add(cell);
                }
                else if (rand > 0.1f && rand <= 0.2f)
                {
                    nextState.SetTile(cell, rockTile); // Cambia de agua a roca
                    aliveCells.Add(cell);
                }
                else if (rand > 0.2f && rand <= 0.4f)
                {
                    nextState.SetTile(cell, grassTile); // Cambia de agua a pasto
                    aliveCells.Add(cell);
                }
                else if (grassNeighbors >= 6)
                {
                    nextState.SetTile(cell, rockTile); // Cambia de pasto a roca
                    aliveCells.Add(cell);
                }
                else
                {
                    nextState.SetTile(cell, currentTile); // Mantiene agua si no se cumple ninguna probabilidad
                    aliveCells.Add(cell);
                }
            }
            else if (iceNeighbors >= 3 && Random.value <= 0.05f)
            {
                nextState.SetTile(cell, iceTile); // Cambia de agua a hielo con probabilidad
                aliveCells.Add(cell);
            }
            else if (rockNeighbors >= 3 && Random.value <= 0.05f)
            {
                nextState.SetTile(cell, rockTile); // Cambia de agua a hielo con probabilidad
                aliveCells.Add(cell);
            }
            else if (waterNeighbors >= 3 && Random.value <= 0.25f)
            {
                nextState.SetTile(cell, sandTile); // Cambia de agua a arena
                aliveCells.Add(cell);
            }
            else
            {
                nextState.SetTile(cell, currentTile); // Mantiene el estado actual si no se cumplen condiciones
                aliveCells.Add(cell);
            }
        }
        else if (currentTile == sandTile)
        {
            if (sandNeighbors >= 4)
            {
                nextState.SetTile(cell, grassTile); // Cambia de arena a pasto
                aliveCells.Add(cell);
            }
            else if (sandNeighbors < 3)
            {
                nextState.SetTile(cell, waterTile); // Regresa a agua si tiene menos de 3 vecinos de arena
                aliveCells.Add(cell);
            }
            else
            {
                nextState.SetTile(cell, currentTile); // Mantiene el estado actual
                aliveCells.Add(cell);
            }
        }
        else if (currentTile == grassTile)
        {
            if (grassNeighbors >= 7)
            {
                nextState.SetTile(cell, rockTile); // Cambia de pasto a roca
                aliveCells.Add(cell);
            }
            else if (grassNeighbors < 2)
            {
                nextState.SetTile(cell, sandTile); // Regresa a arena si tiene menos de 3 vecinos de pasto
                aliveCells.Add(cell);
            }
            else
            {
                nextState.SetTile(cell, currentTile); // Mantiene el estado actual
                aliveCells.Add(cell);
            }
        }
        else if (currentTile == rockTile)
        {
            float rand = Random.value;
            if (rockNeighbors >= 8 )
            {
                nextState.SetTile(cell, lavaTile); // Cambia de roca a lava
                aliveCells.Add(cell);
            }
            else if (rockNeighbors == 7 && lavaNeighbors == 1)
            {
                nextState.SetTile(cell, lavaTile); // Regresa a pasto si tiene menos de 3 vecinos de roca
                aliveCells.Add(cell);
            }
            else if (rockNeighbors < 1)
            {
                nextState.SetTile(cell, grassTile); // Regresa a pasto si tiene menos de 3 vecinos de roca
                aliveCells.Add(cell);
            }
            else
            {
                nextState.SetTile(cell, currentTile); // Mantiene el estado actual
                aliveCells.Add(cell);
            }
        }
        else if (currentTile == lavaTile)
        {
            if (rockNeighbors + lavaNeighbors < 7)
            {
                nextState.SetTile(cell, rockTile); // Cambia de roca a lava
                aliveCells.Add(cell);
            }
            else
            {
                nextState.SetTile(cell, currentTile); // Mantiene el estado actual
                aliveCells.Add(cell);
            }
        }
        else
        {
            nextState.SetTile(cell, currentTile); // Mantiene el estado actual
            aliveCells.Add(cell);
        }
    }

    // Intercambiamos los Tilemaps
    Tilemap temp = currentState;
    currentState = nextState;
    nextState = temp;
    nextState.ClearAllTiles();
}

private int CountSpecificNeighbors(Vector3Int cell, Tile specificTile)
{
    int count = 0;

    for (int x = -1; x <= 1; x++)
    {
        for (int y = -1; y <= 1; y++)
        {
            if (x == 0 && y == 0) continue; // Evitar la celda central

            Vector3Int neighbor = cell + new Vector3Int(x, y, 0);

            if (currentState.GetTile(neighbor) == specificTile)
            {
                count++;
            }
        }
    }

    return count;
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
                int rand = Random.Range(0, 15);
                Tile introTile = lavaTile;
                if (rand >= 0 && rand < 5)
                    {introTile = waterTile;}
                else if (rand >= 5 && rand < 9)
                    {introTile = sandTile;}
                else if (rand >= 9 && rand < 12)
                    {introTile = grassTile;}
                else if (rand >= 12 && rand < 14)
                    {introTile = rockTile;}
                else if (rand >= 14 && rand < 15)
                    {introTile = lavaTile;}

                Vector3Int cell = new Vector3Int(x, y, 0);
                currentState.SetTile(cell, introTile);
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
