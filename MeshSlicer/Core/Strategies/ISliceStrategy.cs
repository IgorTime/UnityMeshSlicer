using UnityEngine;

namespace IgorTime.MeshSlicer
{
	public interface ISliceStrategy
	{
		public bool Slice(SliceInput input, Plane plane, out SliceOutput output);
	}
}