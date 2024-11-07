using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AI : MonoBehaviour
{
    [SerializeField] private Tilemap movementMap;  // Tilemap donde la IA se mueve
    [SerializeField] private Tilemap goalMap;      // Tilemap con la meta y la posicion de la IA
    [SerializeField] private Tile aiTile, metaTile;
    [SerializeField] private Tile sandTile, waterTile, grassTile, rockTile, lavaTile, iceTile;

    private Vector3Int aiPosition;
    private Vector3Int goalPosition;
    private float moveInterval = 0.2f;  // Intervalo de movimiento en segundos
    private int totalCost = 0;  // Costo total acumulado

    private void Start()
    {
        aiPosition = new Vector3Int(30, -30, 0);
        goalMap.SetTile(aiPosition, aiTile);
        goalPosition = new Vector3Int(-30, 30, 0);
        goalMap.SetTile(goalPosition, metaTile);
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

        Debug.Log("llegaste al final. Costo final: " + totalCost); //informa que ya llegó
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
}