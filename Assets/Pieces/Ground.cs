using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TileBuilder
{
    public void Ground(ref MeshBuilder builder, bool stone = false)
    {
        builder.VertexMatrix = translateToCenter;
        if (stone)
        {
            builder.TextureMatrix = 
                Matrix4x4.Translate(new(0.75f, 0.25f)) *
                Matrix4x4.Scale(new(0.25f, 0.25f))
                ;
        }
        else
        {
            builder.TextureMatrix = 
                Matrix4x4.Translate(new(0.25f, 0.5f, 0f)) *
                Matrix4x4.Scale(new(0.25f, 0.25f, 1f))
                ;
        }
        Quad ground = Square1x1(ref builder, 0.02f);
        builder.AddQuad(ground, false);
    }
}
