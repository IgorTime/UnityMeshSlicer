using UnityEngine;

namespace IgorTime.MeshSlicer
{
	public interface ISliceStrategy
	{
		public void Slice(Mesh mesh,
		                  Plane plane,
		                  bool isSolid,
		                  Material[] mainMaterials,
		                  Material sliceMaterial,
		                  out Mesh out1,
		                  out Material[] materials1,
		                  out Mesh out2,
		                  out Material[] materials2);
	}
}