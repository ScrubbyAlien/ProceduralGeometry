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
    private MeshRenderer meshRenderer;
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


        int[] neighborProperties = new int[4];
        for (int i = 0; i < 8; i += 2)
        {
            int nstate = 0;
            if (tile.GetNeighbourProperty(i, MyTile.Path)) nstate += 1;
            if (tile.GetNeighbourProperty(i, MyTile.House)) nstate += 2;
            neighborProperties[i / 2] = nstate;
        }
        
        tileBuilder.Ground(ref meshBuilder);
        switch (state)
        {
            case 1:
                tileBuilder.Path(ref meshBuilder, in neighborProperties);
                break;
            case 2:
                // house
                tileBuilder.House(ref meshBuilder);
                break;
            case 3:
                // path and house
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
            draw = false;
            // Gizmos.color = Color.cyan;
            // Gizmos.DrawSphere(new(0f, 0, 0f), 0.15f);
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