using System;
using Grid;
using UnityEngine;
[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(GridTile))]
public class Piece : MonoBehaviour
{

    private TileBuilder tileBuilder;
    private MeshBuilder meshBuilder;
    private GridTile tile;

    private MeshFilter filter;
    private MeshRenderer renderer;
    private Mesh mesh;

    private void Start()
    {
        tileBuilder = new TileBuilder();
        meshBuilder = new MeshBuilder();
        tile = GetComponent<GridTile>();

        filter = GetComponent<MeshFilter>();
        mesh = new Mesh() { name = "Piece" };
        filter.sharedMesh = mesh;
    }

    private void Update()
    {
        meshBuilder.Clear();
        
        int state = 0;
        if (tile.GetProperty(MyTile.Path)) state += 1;
        if (tile.GetProperty(MyTile.House)) state += 2;

        tileBuilder.Ground(ref meshBuilder);
        switch (state)
        {
            case 1:
                // path
            case 2:
                // house
                tileBuilder.House(ref meshBuilder);
                break;
            case 3:
                // path and house
            default:
                break;
        }
        meshBuilder.Build(ref mesh);
    }

    private void OnDrawGizmos() {
        GridTile _tile = GetComponent<GridTile>();
        bool draw = true;
        if (_tile.GetProperty(MyTile.Path) && _tile.GetProperty(MyTile.House)) {
            Gizmos.color = Color.black;
        } else if (_tile.GetProperty(MyTile.Path)) {
            Gizmos.color = Color.red;
        } else if (_tile.GetProperty(MyTile.House)) {
            draw = false;
            // Gizmos.color = Color.blue;
        } else {
            draw = false;
            // Gizmos.color = Color.green;
        }
        
        if (draw) Gizmos.DrawCube(transform.position, new Vector3(1, 0.1f, 1));
    }
}