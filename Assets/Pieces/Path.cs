using System;
using System.Collections;
using System.Collections.Generic;
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
            // path eller path and house
            surroundingPaths[i] = neighborProperties[i] is 1 or 3;
        }

        // get number of surrounding paths
        List<int> connections = Connections(surroundingPaths);

        switch (connections.Count)
        {
            case 0:
                PathDefault(ref builder);
                break;
            case 1:
                PathDeadEnd(ref builder, Array.IndexOf(surroundingPaths, true));
                break;
            case 2:
                if (Math.Abs(connections[0] - connections[1]) == 2) // rak
                {
                    PathStraight(ref builder, connections[0]);
                }
                else // inte rak, aka hörn
                {
                    PathCorner(ref builder, connections[0], connections[1]);
                }
                break;
            case 3:
                int middleConnection = (Array.IndexOf(surroundingPaths, false) + 2) % 4;
                PathTCrossing(ref builder, middleConnection);
                break;
            case 4:
                PathFourWay(ref builder);
                break;
        }

    }

    private void PathDefault(ref MeshBuilder builder)
    {
        PizzaSlices(ref builder, 8, 0);
    }

    private void PathDeadEnd(ref MeshBuilder builder, int direction)
    { 
        PizzaSlices(ref builder, 4, -180 - (90 * direction));
        
        Floor(ref builder, new(0f, -0.15f, 0.5f, 0.3f), direction);
    }

    private void PathStraight(ref MeshBuilder builder, int direction)
    {
        builder.TextureMatrix =
            Matrix4x4.Translate(new(0.25f, 0.25f, 0f)) *
            Matrix4x4.Scale(new(0.25f, 0.25f, 1))
            ;
        
        Floor(ref builder, new(-0.5f, -0.15f, 1f, 0.3f), direction);
    }

    private void PathCorner(ref MeshBuilder builder, int connection1, int connection2)
    {
        Floor(ref builder, new(0.175f, -0.15f, 0.325f, 0.3f), connection1);
        Floor(ref builder, new(0.175f, -0.15f, 0.325f, 0.3f), connection2);
        
        if (connection1 == 0 && connection2 == 3) // special
        {
            PizzaSlices(ref builder, 2, -90);    
            Floor(ref builder, new(0f, -0.15f, 0.175f, 0.15f), connection2, true);
            Floor(ref builder, new(-0.175f, -0.15f, 0.175f, 0.15f), (connection1 + 2) % 4, true);
            InvertedCorner(ref builder, new(0, 0, 0.175f, 0.175f), connection2);
        }
        else
        {
            PizzaSlices(ref builder, 2, -180 - 90 * connection1);
            
            Floor(ref builder, new(0f, -0.15f, 0.175f, 0.15f), connection1, true);
            Floor(ref builder, new(-0.175f, -0.15f, 0.175f, 0.15f), (connection2 + 2) % 4, true);
            InvertedCorner(ref builder, new(0, 0, 0.175f, 0.175f), connection1);
        }
        
    }

    private void PathTCrossing(ref MeshBuilder builder, int middleConnection)
    {
        int leftConnection = (middleConnection + 1) % 4;
        int rightConnection = (middleConnection - 1) % 4;
        
        Floor(ref builder, new(0.175f, -0.15f, 0.325f, 0.3f), leftConnection);
        Floor(ref builder, new(0.175f, -0.15f, 0.325f, 0.3f), middleConnection);
        Floor(ref builder, new(0.175f, -0.15f, 0.325f, 0.3f), rightConnection);
        
        Floor(ref builder, new(-0.175f, -0.15f, 0.35f, 0.15f), rightConnection, true);
        
        InvertedCorner(ref builder, new(0f, 0f, 0.175f, 0.175f), middleConnection);
        InvertedCorner(ref builder, new(0f, 0f, 0.175f, 0.175f), rightConnection);
    }

    private void PathFourWay(ref MeshBuilder builder)
    {
        Floor(ref builder, new(0.175f, -0.15f, 0.325f, 0.3f), 0);
        Floor(ref builder, new(0.175f, -0.15f, 0.325f, 0.3f), 1);
        Floor(ref builder, new(0.175f, -0.15f, 0.325f, 0.3f), 2);
        Floor(ref builder, new(0.175f, -0.15f, 0.325f, 0.3f), 3);
        
        InvertedCorner(ref builder, new(0f, 0f, 0.175f, 0.175f), 0);
        InvertedCorner(ref builder, new(0f, 0f, 0.175f, 0.175f), 1);
        InvertedCorner(ref builder, new(0f, 0f, 0.175f, 0.175f), 2);
        InvertedCorner(ref builder, new(0f, 0f, 0.175f, 0.175f), 3);
    }
    
    private void PizzaSlices(ref MeshBuilder builder, int numSlices, float startAngle) {
        
        for (float i = startAngle - 22.5f; i < startAngle - 22.5f + numSlices * 45 ; i += 45)
        {
            builder.VertexMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(i, Vector3.up));
            builder.TextureMatrix =
                Matrix4x4.Translate(new(0f, 0.25f, 0f)) *
                Matrix4x4.Scale(new(0.25f, 0.25f, 1))
                ;

            Matrix4x4 rotate45cc = Matrix4x4.Rotate(Quaternion.AngleAxis(45, new(0f, -1, 0f)));

            Vector3 bottomRightInner = new(innerBottomX, 0, innerBottomZ);
            Vector3 bottomLeftInner = rotate45cc.MultiplyPoint(bottomRightInner);
            Vector3 topRightInner = new(innerTopX, 0.01f, innerTopZ);
            Vector3 topLeftInner = rotate45cc.MultiplyPoint(topRightInner);

            Vector3 tangentInner = (bottomRightInner - bottomLeftInner).normalized;
            Vector3 binormalInner = (topRightInner - bottomRightInner).normalized;

            Vector3 normalInner = Vector3.Cross(-tangentInner, binormalInner);

            int a = builder.AddVertex(bottomLeftInner, normalInner, new(0, 0), 0.02f);
            int b = builder.AddVertex(topLeftInner, normalInner, new(0, 1), 0.02f);
            int c = builder.AddVertex(topRightInner, normalInner, new(1, 1), 0.02f);
            int d = builder.AddVertex(bottomRightInner, normalInner, new(1, 0), 0.02f);

            builder.AddQuad(a, b, c, d);

            Vector3 bottomRightOuter = new(outerBottomX, 0, outerBottomZ);
            Vector3 bottomLeftOuter = rotate45cc.MultiplyPoint(bottomRightOuter);
            Vector3 topRightOuter = new(outerTopX, 0.01f, outerTopZ);
            Vector3 topLeftOuter = rotate45cc.MultiplyPoint(topRightOuter);

            Vector3 tangentOuter = (bottomRightOuter - bottomLeftOuter).normalized;
            Vector3 binormalOuter = (topRightOuter - bottomRightOuter).normalized;

            Vector3 normalOuter = Vector3.Cross(tangentOuter, binormalOuter);

            int e = builder.AddVertex(bottomRightOuter, normalOuter, new(0, 0), 0.02f);
            int f = builder.AddVertex(topRightOuter, normalOuter, new(0, 1), 0.02f);
            int g = builder.AddVertex(topLeftOuter, normalOuter, new(1, 1), 0.02f);
            int h = builder.AddVertex(bottomLeftOuter, normalOuter, new(1, 0), 0.02f);

            builder.AddQuad(e, f, g, h);

            int ii = builder.AddVertex(topRightInner, new(0, 1, 0), new(0, 0), 0.02f);
            int j = builder.AddVertex(topLeftInner, new(0, 1, 0), new(0, 1), 0.02f);
            int k = builder.AddVertex(topLeftOuter, new(0, 1, 0), new(1, 1), 0.02f);
            int l = builder.AddVertex(topRightOuter, new(0, 1, 0), new(1, 0), 0.02f);

            builder.AddQuad(ii, j, k, l);

            builder.TextureMatrix =
                Matrix4x4.Translate(new(0.25f, 0.25f, 0f)) *
                Matrix4x4.Scale(new(0.25f, 0.25f, 1))
                ;

            int m = builder.AddVertex(new(0, 0.0001f, 0), new(0, 1, 0), new(0, 0), 0.02f);
            int n = builder.AddVertex(bottomLeftOuter, new(0, 1, 0), new(0, 1), 0.02f);
            int o = builder.AddVertex(bottomRightOuter, new(0, 1, 0), new(1, 1), 0.02f);

            builder.AddTriangle(m, n, o);
        }
    }

    private void Floor(ref MeshBuilder builder, Rect rect, int direction, bool halfFloor = false)
    {
        builder.TextureMatrix =
            Matrix4x4.Translate(new(0.25f, 0.25f, 0f)) *
            Matrix4x4.Scale(new(0.25f, 0.25f, 1))
            ;
        
        builder.VertexMatrix =
            Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * direction, Vector3.up)) *
            Matrix4x4.Translate(new(rect.x, 0.0001f, rect.y)) *
            Matrix4x4.Scale(new(rect.width, 1f, rect.height))
            ;
        Quad floor = Square1x1(ref builder, 0.02f);
        builder.AddQuad(floor);

        // walls
        builder.TextureMatrix =
            Matrix4x4.Translate(new(0f, 0.25f, 0f)) *
            Matrix4x4.Scale(new(0.25f, 0.25f, 1))
            ;
        
        for (int i = 0; i < (halfFloor ? 1 : 2); i++)
        {
            builder.VertexMatrix =
                Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * direction, Vector3.up)) *
                MeshBuilder.RotateDisplacedAxis(180 * i, new(rect.x + (rect.width / 2), 0f, -rect.y - (rect.height / 2)),
                    Vector3.up);
            ;


            Vector3 bottomLeftInner = new(rect.x, 0f, rect.y);
            Vector3 topLeftInner = new(rect.x, 0.01f, rect.y - 0.005f);
            Vector3 topRightInner = new(rect.x + rect.width, 0.01f, rect.y - 0.005f);
            Vector3 bottomRightInner = new(rect.x + rect.width, 0f, rect.y);

            Vector3 tangentInner = (bottomRightInner - bottomLeftInner).normalized;
            Vector3 binormalInner = (topRightInner - bottomRightInner).normalized;

            Vector3 normalInner = Vector3.Cross(tangentInner, binormalInner);


            Vector3 bottomRightOuter = new(rect.x + rect.width, 0f, rect.y - 0.03f);
            Vector3 topRightOuter = new(rect.x + rect.width, 0.01f, rect.y - 0.025f);
            Vector3 topLeftOuter = new(rect.x, 0.01f, rect.y - 0.025f);
            Vector3 bottomLeftOuter = new(rect.x, 0f, rect.y - 0.03f);


            Vector3 tangentOuter = (bottomRightOuter - bottomLeftOuter).normalized;
            Vector3 binormalOuter = (topRightOuter - bottomRightOuter).normalized;

            Vector3 normalOuter = Vector3.Cross(-tangentOuter, binormalOuter);

            int a = builder.AddVertex(bottomLeftInner, normalInner, new(0, 0), 0.02f);
            int b = builder.AddVertex(topLeftInner, normalInner, new(0, 1), 0.02f);
            int c = builder.AddVertex(topRightInner, normalInner, new(1, 1), 0.02f);
            int d = builder.AddVertex(bottomRightInner, normalInner, new(1, 0), 0.02f);
            builder.AddQuad(d, c, b, a);

            int e = builder.AddVertex(bottomRightOuter, normalOuter, new(1, 0), 0.02f);
            int f = builder.AddVertex(topRightOuter, normalOuter, new(1, 1), 0.02f);
            int g = builder.AddVertex(topLeftOuter, normalOuter, new(0, 1), 0.02f);
            int h = builder.AddVertex(bottomLeftOuter, normalOuter, new(0, 0), 0.02f);
            builder.AddQuad(h, g, f, e);

            int ii = builder.AddVertex(topLeftInner, Vector3.up, new(1, 0), 0.02f);
            int j = builder.AddVertex(topLeftOuter, Vector3.up, new(1, 1), 0.02f);
            int k = builder.AddVertex(topRightOuter, Vector3.up, new(0, 1), 0.02f);
            int l = builder.AddVertex(topRightInner, Vector3.up, new(0, 0), 0.02f);
            builder.AddQuad(l, k, j, ii);

        }
    }

    private void InvertedCorner(ref MeshBuilder builder, Rect rect, int direction)
    {
        builder.TextureMatrix =
            Matrix4x4.Translate(new(0.25f, 0.25f, 0f)) *
            Matrix4x4.Scale(new(0.25f, 0.25f, 1))
            ;
        
        builder.VertexMatrix =
            Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * direction, Vector3.up)) *
            Matrix4x4.Translate(new(rect.x, 0.0001f, rect.y)) *
            Matrix4x4.Scale(new(rect.width, 1f, rect.height))
            ;
        Quad floor = Square1x1(ref builder, 0.02f);
        builder.AddQuad(floor);
        
        // Wall
        builder.TextureMatrix =
            Matrix4x4.Translate(new(0f, 0.25f, 0f)) *
            Matrix4x4.Scale(new(0.25f, 0.25f, 1))
            ;
        
        builder.VertexMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(-90 * direction, Vector3.up));
        
        Vector3 bottomLeft = new(0.15f, 0f, rect.height);
        Vector3 topLeft = new(0.155f, 0.01f, rect.height);
        Vector3 topRight = new(rect.width, 0.01f, 0.155f);
        Vector3 bottomRight = new(rect.width, 0f, 0.15f);
        
        Vector3 tangent = (bottomRight - bottomLeft).normalized;
        Vector3 binormal = (topRight - bottomRight).normalized;

        Vector3 normal = Vector3.Cross(-tangent, binormal);
        
        int a = builder.AddVertex(bottomLeft, normal, new(0, 0), 0.02f);
        int b = builder.AddVertex(topLeft, normal, new(0, 1), 0.02f);
        int c = builder.AddVertex(topRight, normal, new(1, 1), 0.02f);
        int d = builder.AddVertex(bottomRight, normal, new(1, 0), 0.02f);
        builder.AddQuad(a, b, c, d);

        int e = builder.AddVertex(topLeft, Vector3.up, new(0, 0), 0.02f);
        int f = builder.AddVertex(new(rect.width, 0.01f, rect.height), Vector3.up, new(0, 1), 0.02f);
        int g = builder.AddVertex(topRight, Vector3.up, new(1, 0), 0.02f);
        
        builder.AddTriangle(e, f, g);

    }

    private static List<int> Connections(bool[] paths)
    {
        List<int> connections = new List<int>();
        for (int i = 0; i < paths.Length; i++)
        {
            if (paths[i]) connections.Add(i);
        }
        return connections;
    }

    
}