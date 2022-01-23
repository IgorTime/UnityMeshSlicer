using UnityEngine;

namespace IgorTime.MeshSlicer
{
	public readonly struct SliceOutput
	{
		public readonly Mesh Mesh1;
		public readonly Mesh Mesh2;
		public readonly Material[] Materials1;
		public readonly Material[] Materials2;

		public SliceOutput(Mesh mesh1, Mesh mesh2, Material[] materials1, Material[] materials2)
		{
			Mesh1 = mesh1;
			Mesh2 = mesh2;
			Materials1 = materials1;
			Materials2 = materials2;
		}
	}
}