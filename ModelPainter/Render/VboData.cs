﻿using OpenTK;

namespace ModelPainter.Render;

public record VboData(Vector3[] Vertices, Vector3[] Normals, Vector2[] TexCoords, uint[] Ids, uint[] Elements);