using UnityEngine;

namespace IgorTime.MeshSlicer
{
	public static class Slicer
	{
		private static readonly ISliceStrategy mainThreadSlicer = new MainThreadSliceStrategy();

		public static bool Slice(in SliceInput input, Plane plane, out SliceOutput output)
		{
			return mainThreadSlicer.Slice(input, plane, out output);
		}
	}
}