using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public partial class TileBuilder
{
    public void Market(ref MeshBuilder builder, in int[] neighborProperties)
    {
        for (int i = 0; i < neighborProperties.Length; i++)
        {
            if (neighborProperties[i] == 1)
            {
                Matrix4x4 transform =
                    Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * i, Vector3.up)) *
                    Matrix4x4.Translate(new(0.5f, 0f, 0f));
                
                DeadEnd(ref builder, -180, transform);
            }
        }
        
        Stall(ref builder, new(-0.25f, 0, 0.25f), 45);
        Stall(ref builder, new(0.25f, 0, 0.25f), -45);
        Stall(ref builder, new(-0.25f, 0f, -0.25f), 135);
        Stall(ref builder, new(0.25f, 0, -0.25f), -135);
    }

    private void DeadEnd(ref MeshBuilder builder, float startAngle, Matrix4x4 transform)
    {
        builder.OpenVertexGroup("deadend");
        PizzaSlices(ref builder, 4, startAngle);
        builder.ApplyMatrixToGroup(transform, "deadend");
        builder.CloseVertexGroup();
    }

    private void Stall(ref MeshBuilder builder, Vector3 position, float rotation)
    { 
        // front board
        builder.VertexMatrix = 
            Matrix4x4.Translate(position) *
            Matrix4x4.Rotate(Quaternion.AngleAxis(rotation, Vector3.down));
        builder.TextureMatrix =
            Matrix4x4.Translate(new(0.5f, 0.625f)) *
            Matrix4x4.Scale(new(0.25f, 0.125f));
        
        int a = builder.AddVertex(new(-0.15f, 0f, -0.075f), Vector3.back, new(0, 0));
        int b = builder.AddVertex(new(-0.15f, 0.05f, -0.075f), Vector3.back, new(0, 1));
        int c = builder.AddVertex(new(0.15f, 0.05f, -0.075f), Vector3.back, new(1, 1));
        int d = builder.AddVertex(new(0.15f, 0f, -0.075f), Vector3.back, new(1, 0));
        
        builder.AddDualFacedQuad(a, b, c, d);

        
        // top board
        int e = builder.AddVertex(new(-0.15f, 0.05f, -0.075f), Vector3.up, new(0, 0));
        int f = builder.AddVertex(new(-0.15f, 0.05f, -0.01f), Vector3.up, new(0, 1));
        int g = builder.AddVertex(new(0.15f, 0.05f, -0.01f), Vector3.up, new(1, 1));
        int h = builder.AddVertex(new(0.15f, 0.05f, -0.075f), Vector3.up, new(1, 0));

        builder.AddDualFacedQuad(e, f, g, h);

        // sides
        for (int i = -1; i < 2; i += 2)
        {
            int ii = builder.AddVertex(new(-0.15f * i, 0f, 0.075f), Vector3.left, new(0, 0));
            int j = builder.AddVertex(new(-0.15f * i, 0.05f, 0.075f), Vector3.left, new(0, 1));
            int k = builder.AddVertex(new(-0.15f * i, 0.05f, -0.075f), Vector3.left, new(1, 1));
            int l = builder.AddVertex(new(-0.15f * i, 0f, -0.075f), Vector3.left, new(1, 0));
            builder.AddDualFacedQuad(ii, j, k, l);
        }

        
        // awning
        builder.TextureMatrix = 
            Matrix4x4.Translate(new(0.5f, 0.25f)) *
            Matrix4x4.Scale(new(0.25f, 0.25f));
        
        Vector3 awningBL = new(-0.18f, 0.13f, 0.1f);
        Vector3 awningTL = new(-0.18f, 0.17f, -0.1f);
        Vector3 awningTR = new(0.18f, 0.17f, -0.1f);
        Vector3 awningBR = new(0.18f, 0.13f, 0.1f);

        Vector3 tangent = awningBR - awningBL;
        Vector3 binormal = awningTL - awningBL;
        Vector3 normal = Vector3.Cross(-tangent, binormal);


        int m = builder.AddVertex(awningBL, normal, new(0, 0));
        int n = builder.AddVertex(awningTL, normal, new(0, 1));
        int o = builder.AddVertex(awningTR, normal, new(1, 1));
        int p = builder.AddVertex(awningBR, normal, new(1, 0));

        builder.AddDualFacedQuad(m, n, o, p);
        
        builder.TextureMatrix =
            Matrix4x4.Translate(new(0.5f, 0.625f)) *
            Matrix4x4.Scale(new(0.25f, 0.125f));
        
        Pole(ref builder, new(-0.15f, 0, -0.075f), 0.164f);
        Pole(ref builder, new(-0.15f, 0, 0.075f), 0.134f);
        Pole(ref builder, new(0.15f, 0, 0.075f), 0.134f);
        Pole(ref builder, new(0.15f, 0, -0.075f), 0.164f);
        
    }

    private void Pole(ref MeshBuilder builder, Vector3 position, float height)
    {
        Matrix4x4 vm = builder.VertexMatrix;
        
        builder.OpenVertexGroup("pole");
        for (int i = 0; i < 4; i++)
        {
            builder.VertexMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * i, Vector3.up));
            int a = builder.AddVertex(new(-0.003f, 0f, -0.003f), Vector3.back, new(0, 0));
            int b = builder.AddVertex(new(-0.003f, height, -0.003f), Vector3.back, new(0, 1));
            int c = builder.AddVertex(new(0.003f, height, -0.003f), Vector3.back, new(1, 1));
            int d = builder.AddVertex(new(0.003f, 0f, -0.003f), Vector3.back, new(1, 0));
            builder.AddQuad(a, b, c, d);
        }
        builder.CloseVertexGroup();
        builder.ApplyMatrixToGroup(
            vm * Matrix4x4.Translate(position), 
            "pole");

        builder.VertexMatrix = vm;
    }
}
