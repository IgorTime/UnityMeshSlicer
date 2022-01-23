using UnityEngine;

namespace IgorTime.MeshSlicer
{
	public readonly struct SlicerTriangle
	{
		public readonly SlicerVertex V1;
		public readonly SlicerVertex V2;
		public readonly SlicerVertex V3;
		public readonly bool IsPositive;

		public SlicerTriangle(in Vector3 v1, in Vector3 v2, in Vector3 v3,
		                      in Vector3 n1, in Vector3 n2, in Vector3 n3,
		                      in Vector2 uv1, in Vector2 uv2, in Vector2 uv3,
		                      in int id1, in int id2, in int id3,
		                      in bool isPositive)
		{
			V1 = SlicerVertex.Create(v1, n1, uv1, id1);
			V2 = SlicerVertex.Create(v2, n2, uv2, id2);
			V3 = SlicerVertex.Create(v3, n3, uv3, id3);
			IsPositive = isPositive;
		}

		public SlicerTriangle(in SlicerVertex v1, 
		                      in SlicerVertex v2,
		                      in SlicerVertex v3,
		                      in bool isPositive)
		{
			V1 = v1;
			V2 = v2;
			V3 = v3;
			IsPositive = isPositive;
		}
	}
}