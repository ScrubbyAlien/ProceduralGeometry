using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public partial class TileBuilder
{
    private const float Rad225 = 22.5f * Mathf.Deg2Rad;
    private readonly float moundAngle;
    
    private readonly float innerBottomZ;
    private readonly float innerTopZ;
    private readonly float innerBottomX;
    private readonly float innerTopX;
    
    private readonly float outerBottomZ;
    private readonly float outerTopZ;
    private readonly float outerBottomX;
    private readonly float outerTopX;

    public TileBuilder()
    {
        moundAngle = Mathf.Atan(2f) * Mathf.Rad2Deg;
        
        innerBottomZ = Mathf.Sin(Rad225) * 0.15f;
        innerTopZ = Mathf.Sin(Rad225) * 0.155f;
        innerBottomX = Mathf.Cos(Rad225) * 0.15f;
        innerTopX = Mathf.Cos(Rad225) * 0.155f;

        outerBottomZ = Mathf.Sin(Rad225) * 0.18f;
        outerTopZ = Mathf.Sin(Rad225) * 0.175f;
        outerBottomX = Mathf.Cos(Rad225) * 0.18f;
        outerTopX = Mathf.Cos(Rad225) * 0.175f;
        
        
    }

    public void Path(ref MeshBuilder builder, in int[] neighborProperties)
    {
        bool[] surroundingPaths = new bool[neighborProperties.Length];
        for (int i = 0; i < neighborProperties.Length; i++)
        {
            surroundingPaths[i] = neighborProperties[i] is 1 or 3;
        }
        
        // get number of surrounding paths
        switch (surroundingPaths.Count(p => p))
        {
            case 0:
                PathDefault(ref builder);
                break;
            case 1:
                PathDeadEnd(ref builder, Array.IndexOf(surroundingPaths, true));
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    
    }
    
    private void PathDefault(ref MeshBuilder builder)
    {
        for (int i = 0; i < 360; i += 45)
        {
            builder.VertexMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(i, Vector3.up));
            builder.TextureMatrix =
                Matrix4x4.Translate(new(0f, 0.25f, 0f)) *
                Matrix4x4.Scale(new(0.25f, 0.25f, 1))
                ;
                

            Matrix4x4 rotate45cc = Matrix4x4.Rotate(Quaternion.AngleAxis(45, new (0f, -1, 0f)));
            
            Vector3 bottomRightInner = new(innerBottomX, 0, innerBottomZ);
            Vector3 bottomLeftInner = rotate45cc.MultiplyPoint(bottomRightInner);
            Vector3 topRightInner = new(innerTopX, 0.01f, innerTopZ);
            Vector3 topLeftInner = rotate45cc.MultiplyPoint(topRightInner);

            Vector3 tangentInner = (bottomRightInner - bottomLeftInner).normalized;
            Vector3 binormalInner = (topRightInner - bottomRightInner).normalized;

            Vector3 normalInner = Vector3.Cross(-tangentInner, binormalInner);
            
            int a = builder.AddVertex(bottomLeftInner, normalInner, new(0, 0));
            int b = builder.AddVertex(topLeftInner, normalInner, new(0, 1));
            int c = builder.AddVertex(topRightInner, normalInner, new(1, 1));
            int d = builder.AddVertex(bottomRightInner, normalInner, new(1, 0));
            
            builder.AddQuad(a, b, c, d);
            
            Vector3 bottomRightOuter = new(outerBottomX, 0, outerBottomZ);
            Vector3 bottomLeftOuter = rotate45cc.MultiplyPoint(bottomRightOuter);
            Vector3 topRightOuter = new(outerTopX, 0.01f, outerTopZ);
            Vector3 topLeftOuter = rotate45cc.MultiplyPoint(topRightOuter);
            
            Vector3 tangentOuter = (bottomRightOuter - bottomLeftOuter).normalized;
            Vector3 binormalOuter = (topRightOuter - bottomRightOuter).normalized;

            Vector3 normalOuter = Vector3.Cross(tangentOuter, binormalOuter);
            
            int e = builder.AddVertex(bottomRightOuter, normalOuter, new(0, 0));
            int f = builder.AddVertex(topRightOuter, normalOuter, new(0, 1));
            int g = builder.AddVertex(topLeftOuter, normalOuter, new(1, 1));
            int h = builder.AddVertex(bottomLeftOuter, normalOuter, new(1, 0));
            
            builder.AddQuad(e, f, g, h);
            
            int ii = builder.AddVertex(topRightInner, new(0, 1, 0), new(0, 0));
            int j = builder.AddVertex(topLeftInner, new(0, 1, 0), new(0, 1));
            int k = builder.AddVertex(topLeftOuter, new(0, 1, 0), new(1, 1));
            int l = builder.AddVertex(topRightOuter, new(0, 1, 0), new(1, 0));
            
            builder.AddQuad(ii, j, k, l);

            builder.TextureMatrix =
                Matrix4x4.Translate(new(0.25f, 0.25f, 0f)) *
                Matrix4x4.Scale(new(0.25f, 0.25f, 1))
            ;
            
            int m = builder.AddVertex(new(0, 0.0001f, 0), new(0, 1, 0), new(0, 0));
            int n = builder.AddVertex(bottomLeftInner, new(0, 1, 0), new(0, 1));
            int o = builder.AddVertex(bottomRightInner, new(0, 1, 0), new(1, 1));
            
            builder.AddTriangle(m, n, o);
        }
    }
    
    private void PathDeadEnd(ref MeshBuilder builder, int direction)
    {
        
        
        for (float i = -22.5f - 180f; i < -22.5; i += 45)
        {
            builder.VertexMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(i - 90 * direction, Vector3.up));
            builder.TextureMatrix =
                Matrix4x4.Translate(new(0f, 0.25f, 0f)) *
                Matrix4x4.Scale(new(0.25f, 0.25f, 1))
                ;
            
            Matrix4x4 rotate45cc = Matrix4x4.Rotate(Quaternion.AngleAxis(45, new (0f, -1, 0f)));
            
            Vector3 bottomRightInner = new(innerBottomX, 0, innerBottomZ);
            Vector3 bottomLeftInner = rotate45cc.MultiplyPoint(bottomRightInner);
            Vector3 topRightInner = new(innerTopX, 0.01f, innerTopZ);
            Vector3 topLeftInner = rotate45cc.MultiplyPoint(topRightInner);

            Vector3 tangentInner = (bottomRightInner - bottomLeftInner).normalized;
            Vector3 binormalInner = (topRightInner - bottomRightInner).normalized;

            Vector3 normalInner = Vector3.Cross(-tangentInner, binormalInner);
            
            int a = builder.AddVertex(bottomLeftInner, normalInner, new(0, 0));
            int b = builder.AddVertex(topLeftInner, normalInner, new(0, 1));
            int c = builder.AddVertex(topRightInner, normalInner, new(1, 1));
            int d = builder.AddVertex(bottomRightInner, normalInner, new(1, 0));
            
            builder.AddQuad(a, b, c, d);
            
            Vector3 bottomRightOuter = new(outerBottomX, 0, outerBottomZ);
            Vector3 bottomLeftOuter = rotate45cc.MultiplyPoint(bottomRightOuter);
            Vector3 topRightOuter = new(outerTopX, 0.01f, outerTopZ);
            Vector3 topLeftOuter = rotate45cc.MultiplyPoint(topRightOuter);
            
            Vector3 tangentOuter = (bottomRightOuter - bottomLeftOuter).normalized;
            Vector3 binormalOuter = (topRightOuter - bottomRightOuter).normalized;

            Vector3 normalOuter = Vector3.Cross(tangentOuter, binormalOuter);
            
            int e = builder.AddVertex(bottomRightOuter, normalOuter, new(0, 0));
            int f = builder.AddVertex(topRightOuter, normalOuter, new(0, 1));
            int g = builder.AddVertex(topLeftOuter, normalOuter, new(1, 1));
            int h = builder.AddVertex(bottomLeftOuter, normalOuter, new(1, 0));
            
            builder.AddQuad(e, f, g, h);
            
            int ii = builder.AddVertex(topRightInner, new(0, 1, 0), new(0, 0));
            int j = builder.AddVertex(topLeftInner, new(0, 1, 0), new(0, 1));
            int k = builder.AddVertex(topLeftOuter, new(0, 1, 0), new(1, 1));
            int l = builder.AddVertex(topRightOuter, new(0, 1, 0), new(1, 0));
            
            builder.AddQuad(ii, j, k, l);

            builder.TextureMatrix =
                Matrix4x4.Translate(new(0.25f, 0.25f, 0f)) *
                Matrix4x4.Scale(new(0.25f, 0.25f, 1))
            ;
            
            int m = builder.AddVertex(new(0, 0.0001f, 0), new(0, 1, 0), new(0, 0));
            int n = builder.AddVertex(bottomLeftInner, new(0, 1, 0), new(0, 1));
            int o = builder.AddVertex(bottomRightInner, new(0, 1, 0), new(1, 1));
            
            builder.AddTriangle(m, n, o);
        }
        
        builder.VertexMatrix = 
            Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * direction, Vector3.up)) *
            Matrix4x4.Translate(new(0f, 0.0001f, -0.15f)) *
            Matrix4x4.Scale(new(0.5f, 1f, 0.3f))
            ;
        Quad floor = Square1x1(ref builder);
        builder.AddQuad(floor);


        for (int i = 0; i < 2; i++)
        {
            builder.VertexMatrix = 
                Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * direction, Vector3.up)) *
                Matrix4x4.Translate(new(-0.25f, 0f, 0f)).inverse *
                Matrix4x4.Rotate(Quaternion.AngleAxis(180 * i, Vector3.up)) *
                Matrix4x4.Translate(new(-0.25f, 0f, 0f))
                ;


            Vector3 bottomLeftInner = new(0f, 0f, 0.15f);
            Vector3 topLeftInner = new(0f, 0.01f, 0.155f);
            Vector3 topRightInner = new(0.5f, 0.01f, 0.155f);
            Vector3 bottomRightInner = new(0.5f, 0f, 0.15f);

            Vector3 tangentInner = (bottomRightInner - bottomLeftInner).normalized;
            Vector3 binormalInner = (topRightInner - bottomRightInner).normalized;

            Vector3 normalInner = Vector3.Cross(-tangentInner, binormalInner);
            
            
            Vector3 bottomRightOuter = new(0.5f, 0f, 0.18f);
            Vector3 topRightOuter = new(0.5f, 0.01f, 0.175f);
            Vector3 topLeftOuter = new(0f, 0.01f, 0.175f);
            Vector3 bottomLeftOuter = new(0f, 0f, 0.18f);
            
                
            Vector3 tangentOuter = (bottomRightOuter - bottomLeftOuter).normalized;
            Vector3 binormalOuter = (topRightOuter - bottomRightOuter).normalized;

            Vector3 normalOuter = Vector3.Cross(tangentOuter, binormalOuter);
                
                
            
            int a = builder.AddVertex(bottomLeftInner, normalInner, new(0, 0));
            int b = builder.AddVertex(topLeftInner, normalInner, new(0, 1));
            int c = builder.AddVertex(topRightInner, normalInner, new(1, 1));
            int d = builder.AddVertex(bottomRightInner, normalInner, new(1, 0));
            builder.AddQuad(a, b, c, d);
            
            int e = builder.AddVertex(bottomRightOuter, normalOuter, new(1, 0));
            int f = builder.AddVertex(topRightOuter, normalOuter, new(1, 1));
            int g = builder.AddVertex(topLeftOuter, normalOuter, new(0, 1));
            int h = builder.AddVertex(bottomLeftOuter, normalOuter, new(0, 0));
            builder.AddQuad(e, f, g, h);
            
            int ii = builder.AddVertex(topLeftInner, Vector3.up, new(1, 0));
            int j = builder.AddVertex(topLeftOuter, Vector3.up, new(1, 1));
            int k = builder.AddVertex(topRightOuter, Vector3.up, new(0, 1));
            int l = builder.AddVertex(topRightInner, Vector3.up, new(0, 0));
            builder.AddQuad(ii, j, k, l);
            
        }
        
    
    }
}