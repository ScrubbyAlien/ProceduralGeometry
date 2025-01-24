using Grid;
using UnityEngine;

[RequireComponent(typeof(GridTile))]
public class Piece : MonoBehaviour {
    private void OnDrawGizmos() {
        GridTile tile = GetComponent<GridTile>();
        if (tile.GetProperty(MyTile.Path) && tile.GetProperty(MyTile.House)) {
            Gizmos.color = Color.black;
        } else if (tile.GetProperty(MyTile.Path)) {
            Gizmos.color = Color.red;
        } else if (tile.GetProperty(MyTile.House)) {
            Gizmos.color = Color.blue;
        } else {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawCube(transform.position, new Vector3(1, 0.1f, 1));
    }
}