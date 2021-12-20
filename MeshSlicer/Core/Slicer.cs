using UnityEngine;

namespace IgorTime.MeshSlicer
{
	public static class Slicer
	{
		private static readonly ISliceStrategy mainThreadSlicer = new MainThreadSliceStrategy();

		public static void Slice(Mesh mesh,
		                         Plane plane,
		                         bool isSolid,
		                         out Mesh part1,
		                         out Mesh part2)
		{
			mainThreadSlicer.Slice(mesh, plane, isSolid, out part1, out part2);
		}
	}
}