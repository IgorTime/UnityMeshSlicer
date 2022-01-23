using IgorTime.MeshSlicer;
using UnityEngine;

public class SliceTool
{
	public static bool Slice(Slicable objectToSlice, Plane plane) =>
		Slice(objectToSlice, plane, out _, out _);

	public static bool Slice(Slicable objectToSlice,
	                         Plane plane,
	                         out GameObject part1,
	                         out GameObject part2)
	{
		part1 = null;
		part2 = null;

		var mesh = objectToSlice.Mesh;
		var isSolid = objectToSlice.IsSolid;
		var input = new IgorTime.MeshSlicer.SliceInput(mesh, 
		                                               objectToSlice.MainMaterials,
		                                               objectToSlice.SliceMaterial,
		                                               isSolid);

		var hasSlice = Slicer.Slice(input, plane, out var output);
		if (!hasSlice)
		{
			return false;
		}

		part1 = CreateObject($"{objectToSlice.name}_slice1", output.Mesh1, output.Materials1, objectToSlice);
		part2 = CreateObject($"{objectToSlice.name}_slice2", output.Mesh2, output.Materials2, objectToSlice);
		return true;

		GameObject CreateObject(string name, Mesh mesh, Material[] materials, Slicable original)
		{
			var result = new GameObject(name);

			result.transform.SetPositionAndRotation(objectToSlice.transform.position,
			                                        objectToSlice.transform.rotation);

			var meshFilter = result.AddComponent<MeshFilter>();
			meshFilter.mesh = mesh;

			var meshRenderer = result.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterials = materials;

			var meshCollider = result.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = mesh;
			meshCollider.convex = true;

			var slicable = result.AddComponent<Slicable>();
			slicable.IsSolid = objectToSlice.IsSolid;
			slicable.SliceMaterial = original.SliceMaterial;

			result.AddComponent<Rigidbody>();
			result.layer = LayerMask.NameToLayer("Slicable");
			return result;
		}
	}
}