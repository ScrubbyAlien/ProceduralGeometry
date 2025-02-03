using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MeshBuilder
{
    private readonly List<Vector3> vertices = new();
    private readonly Dictionary<int, string> vertexGroups = new();
    private string currentVertexGroup = "";
    private readonly List<Vector3> normals = new();
    private readonly List<Vector2> uvs = new();
    private readonly List<int> triangles = new();

    public Matrix4x4 VertexMatrix = Matrix4x4.identity;
    public Matrix4x4 TextureMatrix = Matrix4x4.identity;

    private Vector2 TweakedUV(Vector2 v2, float tweakAmount)
    {
        return new Vector2(
            v2.x > 0.5f ? v2.x - tweakAmount : v2.x + tweakAmount,
            v2.y > 0.5f ? v2.y - tweakAmount : v2.y + tweakAmount
            );
    }

public int AddVertex(Vector3 position, Vector3 normal, Vector2 uv, float tweak = 0) {
        int index = vertices.Count;
        if (currentVertexGroup != "")
        {
            vertexGroups.Add(index, currentVertexGroup);
        }
        vertices.Add(VertexMatrix.MultiplyPoint(position));
        normals.Add(VertexMatrix.MultiplyVector(normal));
        uvs.Add(TextureMatrix.MultiplyPoint(TweakedUV(uv, tweak * 3f)));
        return index;
    }

    public void AddQuad(Quad quad, bool preserveMatrix = true)
    {
        AddQuad(quad.v1, quad.v2, quad.v3, quad.v4, preserveMatrix);
    }
    
    public void AddQuad(int bottomLeft, int topLeft, int topRight, int bottomRight, bool preserveMatrix = true) {
        AddTriangle(bottomLeft, topLeft, topRight, preserveMatrix);
        AddTriangle(bottomLeft, topRight, bottomRight, preserveMatrix);
    }

    public void AddDualFacedQuad(int bottomLeft, int topLeft, int topRight, int bottomRight, bool preserveMatrix = true)
    {
        Matrix4x4 vm = VertexMatrix;
        Matrix4x4 tm = TextureMatrix;
        
        AddQuad(bottomLeft, topLeft, topRight, bottomRight, false);
        int a = AddVertex(vertices[bottomLeft], -normals[bottomLeft], uvs[bottomLeft]);
        int b = AddVertex(vertices[topLeft], -normals[topLeft], uvs[topLeft]);
        int c = AddVertex(vertices[topRight], -normals[topRight], uvs[topRight]);
        int d = AddVertex(vertices[bottomRight], -normals[bottomRight], uvs[bottomRight]);
        AddQuad(d, c, b, a, preserveMatrix);

        if (preserveMatrix)
        {
            VertexMatrix = vm;
            TextureMatrix = tm;
        }
        
    }

    public void AddTriangle(int first, int second, int third, bool preserveMatrix = true)
    {
        triangles.Add(first);
        triangles.Add(second);
        triangles.Add(third);

        if (!preserveMatrix)
        {
            VertexMatrix = Matrix4x4.identity;
            TextureMatrix = Matrix4x4.identity;
        }
    }

    public void Build(ref Mesh mesh) {
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
        mesh.MarkModified();
    }

    public void Clear()
    {
        vertexGroups.Clear();
        vertices.Clear();
        normals.Clear();
        uvs.Clear();
        triangles.Clear();
        VertexMatrix = Matrix4x4.identity;
        TextureMatrix = Matrix4x4.identity;
    }

    public static Matrix4x4 RotateDisplacedAxis(float angle, Vector3 origin, Vector3 axis)
    {
        return Matrix4x4.Translate(origin) *
               Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis)) *
               Matrix4x4.Translate(origin).inverse;
    }

    public void OpenVertexGroup(string groupName)
    {
        if (vertexGroups.ContainsValue(groupName))
        {
            foreach (int key in vertexGroups.Keys.ToArray())
            {
                if (vertexGroups[key] == groupName)
                {
                    vertexGroups.Remove(key);
                }
            }
        }
        currentVertexGroup = groupName;
    }

    public void CloseVertexGroup()
    {
        currentVertexGroup = "";
    }

    public void ApplyMatrixToGroup(Matrix4x4 matrix, string group)
    {
        List<int> indices = vertexGroups
                                .Where(pair => pair.Value == group)
                                .Select(pair => pair.Key)
                                .ToList();
        
        foreach (int index in indices)
        {
            vertices[index] = matrix.MultiplyPoint(vertices[index]);
            normals[index] = matrix.MultiplyVector(normals[index]);
        }
    }
}

public struct Quad
{
    public int v1, v2, v3, v4;

    public Quad(int v1, int v2, int v3, int v4)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
        this.v4 = v4;
    }
}

