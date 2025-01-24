using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TileBuilder
{
    public void Ground(ref MeshBuilder builder)
    {
        builder.VertexMatrix = translateToCenter;
        Quad ground = Square1x1(ref builder);
        builder.AddQuad(ground, false);
    }
}
