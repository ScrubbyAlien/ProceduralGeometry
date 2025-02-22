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
        
        tileBuilder.Ground(ref meshBuilder, state == 3);
        switch (state)
        {
            case 1:
                tileBuilder.Path(ref meshBuilder, in neighborProperties);
                break;
            case 2:
                // house
                uint tileID = (uint) Mathf.RoundToInt(tile.Id.magnitude * Vector2.Angle(tile.Id, Vector2.left));
                uint tileSeed = tileID * (tileID + 100) * (tileID + 300);
                tileBuilder.House(ref meshBuilder, tileSeed);
                break;
            case 3:
                tileBuilder.Market(ref meshBuilder, in neighborProperties);
                // path and house
                break;
        }
        
        meshBuilder.Build(ref mesh);
    }
}