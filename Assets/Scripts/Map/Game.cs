using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class Game : MonoBehaviour
{
    [SerializeField] private Tilemap movementMap;  // Tilemap donde la IA se mueve
    [SerializeField] private Tilemap goalMap;      // Tilemap con la meta y la posicion de la IA

    [SerializeField] private Tile aiTile;
    [SerializeField] private Tile playerTile;
    [SerializeField] private Tile metaTile;

    [SerializeField] private Tile sandTile;
    [SerializeField] private Tile waterTile;
    [SerializeField] private Tile grassTile;
    [SerializeField] private Tile rockTile;
    [SerializeField] private Tile lavaTile; 
    [SerializeField] private Tile iceTile;

    private Vector3Int aiPosition;
    private Vector3Int playerPosition;
    private Vector3Int goalPosition;

    private float moveInterval = 0.2f;  // Intervalo de movimiento en segundos
    private int totalCost = 0;  // Costo total acumulado
    private int playerTotalCost = 0;  // Costo total acumulado del jugador

    private void Start()
    {
        aiPosition = new Vector3Int(30, -30, 0);
        goalMap.SetTile(aiPosition, aiTile);
        goalPosition = new Vector3Int(0, 30, 0);
        goalMap.SetTile(goalPosition, metaTile);
        playerPosition = new Vector3Int(-30, -30, 0);
        goalMap.SetTile(playerPosition, playerTile);
    }

    public void Update()
    {
        MovePlayer();
    }

    public void StartAI()
    {
        StartCoroutine(MoveTowardsGoal());  // Inicia el movimiento de la IA al presionar el boton
    }

    private IEnumerator MoveTowardsGoal()
    {
        while (aiPosition != goalPosition)
        {
            Vector3Int nextPosition = GetNextPosition();
            if (nextPosition != aiPosition)
            {
                goalMap.SetTile(aiPosition, null);  // Borra la posicion actual de la IA
                aiPosition = nextPosition;  // Actualiza la posicion de la IA
                goalMap.SetTile(aiPosition, aiTile);  // Dibuja la IA en la nueva posicion

                // Obtiene el costo del siguiente movimiento y lo suma al costo total
                int stepCost = GetTileCost(aiPosition);
                totalCost += stepCost;

                // Imprime la posicion actual de la IA y el costo total
                Debug.Log($"La IA se movio a: {aiPosition}. Costo del movimiento: {stepCost}. Costo total: {totalCost}.");
            }

            yield return new WaitForSeconds(moveInterval);  // Espera el intervalo antes de moverse otra vez
        }

        Debug.Log("AI llego al final. Costo final: " + totalCost); //informa que ya lleg�
    }

    private Vector3Int GetNextPosition()
    {
        Vector3Int[] positionsToCheck = { //Agrega las posiciones de alrededor para revision
            aiPosition + Vector3Int.up,
            aiPosition + Vector3Int.down,
            aiPosition + Vector3Int.left,
            aiPosition + Vector3Int.right
        };

        Vector3Int bestPosition = aiPosition; //marca la posicion actual como la mejor
        int lowestCost = int.MaxValue; 

        foreach (Vector3Int pos in positionsToCheck)
        {
            if (IsCloserToGoal(pos)) //compara distancias
            {
                int cost = GetTileCost(pos); //revisa el costo
                if (cost < lowestCost)
                {
                    lowestCost = cost; //si es un costo menor al que se tenia se asigna como el mejor camino
                    bestPosition = pos;
                }
            }
        }

        return bestPosition;
    }

    private bool IsCloserToGoal(Vector3Int position)
    {
        float currentDistance = Vector3Int.Distance(aiPosition, goalPosition);
        float newDistance = Vector3Int.Distance(position, goalPosition);
        return newDistance < currentDistance; //regresa si la casilla revisada esta mas cerca
    }

    private int GetTileCost(Vector3Int position) //revisa el tipo de la casilla y asigna un valor de lo que cuesta atrvesarla
    {
        TileBase tile = movementMap.GetTile(position);

        if (tile == waterTile) return 10; //pasa por el agua, es cansado
        if (tile == sandTile) return 2; //pasa por la arena, es incomodo
        if (tile == grassTile) return 1; //pasa por el pasto, no hay problema
        if (tile == rockTile) return 5; //pasa por las rocas, es complicado
        if (tile == lavaTile) return 90; //pasa por la lava, es peligroso y no deberia escoger esta opcion
        if (tile == iceTile) return 3; //pasa por el frio, se desliza

        return int.MaxValue;  // Costo alto para tiles desconocidos
    }
    private void MovePlayer()
    {
        Vector3Int newPlayerPosition = playerPosition;

        if (Input.GetKeyDown(KeyCode.UpArrow) && playerPosition != goalPosition)
        {
            newPlayerPosition += Vector3Int.up;
            PlayerCost(newPlayerPosition);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && playerPosition != goalPosition)
        {
            newPlayerPosition += Vector3Int.down;
            PlayerCost(newPlayerPosition);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && playerPosition != goalPosition)
        {
            newPlayerPosition += Vector3Int.left;
            PlayerCost(newPlayerPosition);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && playerPosition != goalPosition)
        {
            newPlayerPosition += Vector3Int.right;
            PlayerCost(newPlayerPosition);
        }
    }

    private void PlayerCost(Vector3Int newPlayerPosition)
    {
        // Comprueba si el nuevo movimiento es válido dentro del mapa
        if (movementMap.HasTile(newPlayerPosition))
        {
            // Limpia la posición anterior y actualiza a la nueva
            goalMap.SetTile(playerPosition, null);
            playerPosition = newPlayerPosition;
            goalMap.SetTile(playerPosition, playerTile);   
            // Calcula el costo del movimiento y lo añade al total del jugador
            int stepCost = GetTileCost(playerPosition);
            playerTotalCost += stepCost;   
            // Imprime la posición actual del jugador y el costo total
            Debug.Log($"El jugador se movió a: {playerPosition}. Costo del movimiento: {stepCost}. Costo total: {playerTotalCost}.");
        }

        if (playerPosition == goalPosition)
            Debug.Log("llegaste al final. Costo final: " + playerTotalCost); //informa que ya lleg�
    }
}