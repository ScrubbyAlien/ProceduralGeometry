using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TileBuilder
{
    public void House(ref MeshBuilder builder)
    {
        for (int i = 0; i < 4; i++)
        {
            builder.VertexMatrix = 
                Matrix4x4.Scale(new(0.5f, 0.2f, 0.5f)) * 
                Matrix4x4.Rotate(Quaternion.AngleAxis(i * 90, Vector3.up)) * 
                translateToCenter * 
                Matrix4x4.Rotate(Quaternion.AngleAxis(90, Vector3.forward));
            Quad wall = Square1x1(ref builder);
            builder.AddQuad(wall);
        }

        for (int i = 0; i < 2; i++)
        {
            builder.VertexMatrix = 
                Matrix4x4.Rotate(Quaternion.AngleAxis(i * 180, Vector3.up)) *
                Matrix4x4.Translate(new(-0.25f, 0.2f, -0.25f)) *
                Matrix4x4.Rotate(Quaternion.AngleAxis(45, Vector3.forward)) *
                Matrix4x4.Scale(new(0.25f * Mathf.Sqrt(2), 1, 0.5f));
            Quad roof = Square1x1(ref builder);
            builder.AddQuad(roof, false);


            builder.VertexMatrix =
                Matrix4x4.Rotate(Quaternion.AngleAxis(i * 180, Vector3.up)) *
                Matrix4x4.Translate(new(-0.25f, 0.2f, -0.25f)) *
                Matrix4x4.Scale(new(0.5f, 0.5f, 0f)) *
                Matrix4x4.Rotate(Quaternion.AngleAxis(90, Vector3.left))
                ;
            int a = builder.AddVertex(new(0, 0, 0), new(0, 1, 0), new(0, 1));
            int b = builder.AddVertex(new(0.5f, 0, 0.5f), new(0, 1, 0), new(0, 0));
            int c = builder.AddVertex(new(1, 0, 0), new(0, 1, 0), new(1, 0));
            builder.AddTriangle(a, b, c);


        }
    }
}
