using UnityEngine;

namespace IgorTime.MeshSlicer
{
	public interface ISliceStrategy
	{
		public void Slice(Mesh mesh,
		                  Plane plane,
		                  bool isSolid,
		                  out Mesh out1,
		                  out Mesh out2);
	}
}