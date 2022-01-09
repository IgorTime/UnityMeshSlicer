using UnityEngine;

namespace IgorTime.MeshSlicer
{
	public struct SlicerVertex
	{
		public int ID;
		public Vector3 Position;
		public Vector3 Normal;
		public Vector2 UV0;
		public int Index;
		public static SlicerVertex Create(in Vector3 v,
		                                  in Vector3 n,
		                                  in Vector2 uv0,
		                                  in int id) =>
			new SlicerVertex
			{
				Position = v,
				Normal = n,
				UV0 = uv0,
				ID = id
			};
	}
	
	public static class SlicerVertexExtension
	{
		public static void Unpack(this in SlicerVertex vertex, 
		                          out Vector3 v, 
		                          out Vector3 n, 
		                          out Vector2 uv,
		                          out int id)
		{
			v = vertex.Position;
			n = vertex.Normal;
			uv = vertex.UV0;
			id = vertex.ID;
		}
	}
}