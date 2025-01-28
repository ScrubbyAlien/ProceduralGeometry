using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TileBuilder
{
    public void Ground(ref MeshBuilder builder)
    {
        builder.VertexMatrix = translateToCenter;
        builder.TextureMatrix = 
            // Matrix4x4.Scale(new(0.25f, 0.25f, 1f)) *
            // Matrix4x4.Translate(new(1f, -2f, 0f)) 
            Matrix4x4.Translate(new(0.25f, 0.5f, 0f)) *
            Matrix4x4.Scale(new(0.25f, 0.25f, 1f))
            ;
        Quad ground = Square1x1(ref builder);
        builder.AddQuad(ground, false);
    }
}
