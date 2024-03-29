﻿using ModelPainterCore.Model.DCM;
using ModelPainterCore.Model.OBJ;
using ModelPainterCore.Model.P3D;
using ModelPainterCore.Model.TBL;
using ModelPainterCore.Model.TCN;
using ModelPainterCore.Render;
using ModelPainterCore.Util;
using OpenTK.Mathematics;

namespace ModelPainterCore.Model;

public class ModelBakery
{
    public static (VboData ModelData, Dictionary<uint, Guid> IdMap) BakeModelParts(List<ModelPart> modelParts,
        float dialation = 0)
    {
        var objectIdMap = new Dictionary<uint, Guid>();
        var startingId = 1u;

        var vertices = new List<VboVertex>();
        var matrices = new MatrixStack();

        matrices.RotateY(MathF.PI);
        matrices.Scale(-1, 1, 1);

        foreach (var part in modelParts)
            part.Render(matrices, vertices, dialation, objectIdMap, ref startingId);

        return (PackVboData(vertices), objectIdMap);
    }

    public static (VboData ModelData, Dictionary<uint, Guid> IdMap) BakeTabula(TabulaModel tbl, float dialation = 0)
    {
        return BakeModelParts(tbl.GetModelParts(dialation), dialation);
    }

    public static (VboData ModelData, Dictionary<uint, Guid> IdMap) BakeTechne(TechneModel tcn, float dialation = 0)
    {
        return BakeModelParts(tcn.GetModelParts(dialation), dialation);
    }

    public static (VboData ModelData, Dictionary<uint, Guid> IdMap) BakeStudioModel(StudioModel model,
        float dialation = 0)
    {
        var objectIdMap = new Dictionary<uint, Guid>();
        var startingId = 1u;

        var vertices = new List<VboVertex>();
        var matrices = new MatrixStack();

        matrices.Translate(0, 1.5F, 0);
        matrices.Scale(-1, -1, -1);

        foreach (var part in model.Children)
            part.Render(matrices, vertices, dialation, objectIdMap, ref startingId);

        return (PackVboData(vertices), objectIdMap);
    }

    public static (VboData ModelData, Dictionary<uint, Guid> IdMap) BakeP3dModel(P3dModel model)
    {
        var objectIdMap = new Dictionary<uint, Guid>();
        var startingId = 1u;

        var vertices = new List<VboVertex>();
        var matrices = new MatrixStack();

        matrices.Scale(1, -1, 1);
        matrices.Translate(0, -1.5F, 0);
        matrices.RotateY(MathF.PI);

        foreach (var part in model.Meshes)
            part.Render(matrices, vertices, objectIdMap, ref startingId);

        return (PackVboData(vertices), objectIdMap);
    }

    public static (VboData ModelData, Dictionary<uint, Guid> IdMap) BakeObjModel(ObjModel model)
    {
        var objectIdMap = new Dictionary<uint, Guid>();
        var startingId = 1u;

        var vertices = new List<VboVertex>();
        var matrices = new MatrixStack();

        matrices.Scale(1, -1, 1);
        matrices.Translate(0, -1.5F, 0);

        var entry = matrices.Peek();
        foreach (var part in model.ObjFile.Faces)
        {
            var verts = part.Vertices;
            if (verts.Count is < 3 or > 4)
                continue;

            for (var i = 0; i < verts.Count; i++)
            {
                var v = model.ObjFile.Vertices[verts[i].Vertex - 1];
                var n = model.ObjFile.VertexNormals[verts[i].Normal - 1];
                var t = model.ObjFile.TextureVertices[verts[i].Texture - 1];
                vertices.Add(ModelUtil.Bake(entry, new Vector3(v.Position.X, v.Position.Y, v.Position.Z),
                    new Vector2(t.X, t.Y), new Vector3(n.X, n.Y, n.Z), startingId));
            }

            if (verts.Count == 3)
                vertices.Add(vertices[^1]);
        }

        return (PackVboData(vertices), objectIdMap);
    }

    private static VboData PackVboData(IReadOnlyList<VboVertex> vertices)
    {
        var v = new Vector3[vertices.Count];
        var n = new Vector3[vertices.Count];
        var t = new Vector2[vertices.Count];
        var id = new uint[vertices.Count];
        var e = new uint[(vertices.Count / 2) * 3];

        for (var i = 0; i < vertices.Count; i++)
        {
            v[i] = vertices[i].Position;
            n[i] = vertices[i].Normal;
            t[i] = vertices[i].Tex;
            id[i] = vertices[i].ObjectId;
        }

        // Convert quads to tris
        var elementIndex = 0;
        for (var i = 0; i < vertices.Count; i += 4)
        {
            e[elementIndex + 0] = (uint)(i + 0);
            e[elementIndex + 1] = (uint)(i + 1);
            e[elementIndex + 2] = (uint)(i + 2);
            e[elementIndex + 3] = (uint)(i + 0);
            e[elementIndex + 4] = (uint)(i + 2);
            e[elementIndex + 5] = (uint)(i + 3);

            elementIndex += 6;
        }

        var packed = new VboData(v, n, t, id, e);
        return packed;
    }
}