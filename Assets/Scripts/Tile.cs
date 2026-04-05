using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isOccupied = false;

    public void SetOccupied(bool occupied)
    {
        // Set the occupied state of the tile
        isOccupied = occupied;
    }
}
