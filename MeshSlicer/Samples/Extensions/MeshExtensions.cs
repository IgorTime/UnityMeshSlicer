using System.Collections.Generic;
using UnityEngine;

public static class MeshExtensions
{
	public static void OptimizeSubMeshes(this Mesh mesh,
	                                     IList<Material> materials,
	                                     out Material[] optimizedMaterials)
	{
		var trianglesBuffer = new List<int>();
		var newSubMeshes = new Dictionary<string, List<int>>();
		var newMaterials = new Dictionary<string, Material>();

		for (var i = 0; i < mesh.subMeshCount; i++)
		{
			var key = materials[i].name;
			if (!newSubMeshes.TryGetValue(key, out var triangles))
			{
				newSubMeshes[key] = triangles = new List<int>();
				newMaterials[key] = materials[i];
			}

			mesh.GetTriangles(trianglesBuffer, i);
			triangles.AddRange(trianglesBuffer);
		}

		{
			mesh.subMeshCount = newSubMeshes.Count;
			optimizedMaterials = new Material [newSubMeshes.Count];

			var i = 0;
			foreach (var triangles in newSubMeshes)
			{
				mesh.SetTriangles(triangles.Value, i, false);
				optimizedMaterials[i++] = newMaterials[triangles.Key];
			}

			mesh.RecalculateBounds();
		}
	}
}