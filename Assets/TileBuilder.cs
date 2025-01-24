using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public partial class TileBuilder
{
    private static Matrix4x4 translateToCenter = Matrix4x4.Translate(new(-0.5f, 0, -0.5f));

    private static Quad Square1x1(ref MeshBuilder builder)
    {
        int w00 = builder.AddVertex(new(0, 0, 0), new(0, 1, 0), new(0, 0));
        int w01 = builder.AddVertex(new(0, 0, 1), new(0, 1, 0), new(0, 1));
        int w11 = builder.AddVertex(new(1, 0, 1), new(0, 1, 0), new(1, 1));
        int w10 = builder.AddVertex(new(1, 0, 0), new(0, 1, 0), new(1, 0));
        return new Quad(w00, w01, w11, w10);
    }
}
