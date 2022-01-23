using UnityEngine;

namespace IgorTime.MeshSlicer
{
	public static class Slicer
	{
		private static readonly ISliceStrategy mainThreadSlicer = new MainThreadSliceStrategy();

		public static void Slice(Mesh mesh,
		                         Plane plane,
		                         bool isSolid,
		                         Material[] mainMaterials,
		                         Material sliceMaterial,
		                         out Mesh part1,
		                         out Material[] materials1,
		                         out Mesh part2,
		                         out Material[] materials2)
		{
			mainThreadSlicer.Slice(mesh, 
			                       plane,
			                       isSolid,
			                       mainMaterials,
			                       sliceMaterial,
			                       out part1,
			                       out materials1,
			                       out part2,
			                       out materials2);
		}
	}
}