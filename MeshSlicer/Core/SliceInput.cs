using UnityEngine;

namespace IgorTime.MeshSlicer
{
	public readonly struct SliceInput
	{
		public readonly Mesh Mesh;
		public readonly Material[] Materials;
		public readonly Material SliceMaterial;
		public readonly bool Solid;

		public SliceInput(Mesh mesh, Material[] materials, Material sliceMaterial, bool solid)
		{
			Mesh = mesh;
			Materials = materials;
			SliceMaterial = sliceMaterial;
			Solid = solid;
		}
		
		public SliceInput(Mesh mesh, Material[] materials)
		{
			Mesh = mesh;
			Materials = materials;
			SliceMaterial = null;
			Solid = false;
		}
	}
}