using System;
using System.Collections.Generic;
using UnityEngine;

namespace IgorTime.MeshSlicer
{
	public class MainThreadSliceStrategy : ISliceStrategy
	{
		private const int DEFAULT_CAPACITY = 2048;
		private readonly SlicerMesh meshPositive = new SlicerMesh();
		private readonly SlicerMesh meshNegative = new SlicerMesh();
		private readonly SlicerTriangle[] trianglesBuffer = new SlicerTriangle[3];

		private readonly List<Vector3> intersections = new List<Vector3>();
		private readonly List<Vector3> meshVertices = new List<Vector3>(DEFAULT_CAPACITY);
		private readonly List<Vector3> meshNormals = new List<Vector3>(DEFAULT_CAPACITY);
		private readonly List<Vector2> meshUv = new List<Vector2>(DEFAULT_CAPACITY);
		private readonly List<int> meshTriangles = new List<int>(DEFAULT_CAPACITY);

		public bool Slice(SliceInput input, Plane plane, out SliceOutput output)
		{
			var mesh = input.Mesh;
			var mainMaterials = input.Materials;

			if (mesh.subMeshCount != mainMaterials.Length)
			{
				throw new Exception("Different amount of materials and SubMeshes");
			}

			meshPositive.Clear();
			meshNegative.Clear();
			intersections.Clear();

			mesh.GetVertices(meshVertices);
			mesh.GetNormals(meshNormals);
			mesh.GetUVs(0, meshUv);
			var hasSlice = false;

			var indexOffset = 0;
			for (var subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
			{
				mesh.GetTriangles(meshTriangles, subMeshIndex);
				var material = mainMaterials[subMeshIndex];

				var trianglesCount = meshTriangles.Count;
				for (var i = 0; i < trianglesCount; i += 3)
				{
					var i1 = i;
					var i2 = i + 1;
					var i3 = i + 2;

					var v1 = meshVertices[meshTriangles[i1]];
					var v2 = meshVertices[meshTriangles[i2]];
					var v3 = meshVertices[meshTriangles[i3]];

					var n1 = meshNormals[meshTriangles[i1]];
					var n2 = meshNormals[meshTriangles[i2]];
					var n3 = meshNormals[meshTriangles[i3]];

					var uv1 = meshUv[meshTriangles[i2]];
					var uv2 = meshUv[meshTriangles[i3]];
					var uv3 = meshUv[meshTriangles[i3]];

					var v1Side = plane.GetSide(v1);
					var v2Side = plane.GetSide(v2);
					var v3Side = plane.GetSide(v3);

					var vertexData1 = SlicerVertex.Create(v1, n1, uv1, indexOffset + i1);
					var vertexData2 = SlicerVertex.Create(v2, n2, uv2, indexOffset + i2);
					var vertexData3 = SlicerVertex.Create(v3, n3, uv3, indexOffset + i3);

					if (v1Side == v2Side && v2Side == v3Side)
					{
						var meshData = v1Side ? meshPositive : meshNegative;
						meshData.AddTriangle(vertexData1, vertexData2, vertexData3, material);
						continue;
					}

					hasSlice = true;

					if (v1Side == v2Side)
					{
						ScenarioWhenThirdVertexAlone(plane,
						                             vertexData1,
						                             vertexData2,
						                             vertexData3,
						                             v1Side);
					}
					else if (v1Side == v3Side)
					{
						ScenarioWhenSecondVertexAlone(plane,
						                              vertexData1,
						                              vertexData2,
						                              vertexData3,
						                              v1Side);
					}
					else
					{
						ScenarioWhenFirstVertexAlone(plane,
						                             vertexData1,
						                             vertexData2,
						                             vertexData3,
						                             v1Side);
					}

					for (var j = 0; j < trianglesBuffer.Length; j++)
					{
						var meshData = trianglesBuffer[j].IsPositive
							? meshPositive
							: meshNegative;
						meshData.AddTriangle(trianglesBuffer[j], material);
					}
				}

				indexOffset += trianglesCount;
			}

			if (!hasSlice)
			{
				output = default;
				return false;
			}

			if (input.Solid)
			{
				FillEmptySpace(plane.normal, input.SliceMaterial);
			}

			var out1 = meshPositive.ToUnityMesh(out var materials1);
			var out2 = meshNegative.ToUnityMesh(out var materials2);
			output = new SliceOutput(out1, out2, materials1, materials2);
			return true;
		}

		private void ScenarioWhenFirstVertexAlone(in Plane plane,
		                                          in SlicerVertex vertex1,
		                                          in SlicerVertex vertex2,
		                                          in SlicerVertex vertex3,
		                                          in bool v1Side)
		{
			vertex1.Unpack(out var v1, out var n1, out var uv1, out var id1);
			vertex2.Unpack(out var v2, out var n2, out var uv2, out var id2);
			vertex3.Unpack(out var v3, out var n3, out var uv3, out var id3);

			var i1 = FindIntersection(v1, v2, plane, out var t1);
			var i2 = FindIntersection(v1, v3, plane, out var t2);
			var iUV1 = Vector2.Lerp(uv1, uv2, t1);
			var iUV2 = Vector2.Lerp(uv1, uv3, t2);
			var iN1 = Vector3.Lerp(n1, n2, t1);
			var iN2 = Vector3.Lerp(n1, n3, t2);
			var iID = SlicerMesh.EMPTY_VERTEX_ID;

			trianglesBuffer[0] =
				new SlicerTriangle(v1, i1, i2, n1, iN1, iN2, uv1, iUV1, iUV2, id1, iID, iID,
				                   v1Side);
			trianglesBuffer[1] =
				new SlicerTriangle(i1, v2, v3, iN1, n2, n3, iUV1, uv2, uv3, iID, id2, id3, !v1Side);
			trianglesBuffer[2] =
				new SlicerTriangle(i1, v3, i2, iN1, n3, iN2, iUV1, uv3, iUV2, iID, id3, iID,
				                   !v1Side);

			intersections.Add(i1);
			intersections.Add(i2);
		}

		private void ScenarioWhenSecondVertexAlone(in Plane plane,
		                                           in SlicerVertex vertex1,
		                                           in SlicerVertex vertex2,
		                                           in SlicerVertex vertex3,
		                                           in bool v1Side)
		{
			vertex1.Unpack(out var v1, out var n1, out var uv1, out var id1);
			vertex2.Unpack(out var v2, out var n2, out var uv2, out var id2);
			vertex3.Unpack(out var v3, out var n3, out var uv3, out var id3);

			var i1 = FindIntersection(v1, v2, plane, out var t1);
			var i2 = FindIntersection(v2, v3, plane, out var t2);
			var iUV1 = Vector2.Lerp(uv1, uv2, t1);
			var iUV2 = Vector2.Lerp(uv2, uv3, t2);
			var iN1 = Vector3.Lerp(n1, n2, t1);
			var iN2 = Vector3.Lerp(n2, n3, t2);
			var iID = SlicerMesh.EMPTY_VERTEX_ID;

			trianglesBuffer[0] =
				new SlicerTriangle(v1, i1, v3, n1, iN1, n3, uv1, iUV1, uv3, id1, iID, id3, v1Side);
			trianglesBuffer[1] =
				new SlicerTriangle(i1, i2, v3, iN1, iN2, n3, iUV1, iUV2, uv3, iID, iID, id3,
				                   v1Side);
			trianglesBuffer[2] =
				new SlicerTriangle(i1, v2, i2, iN1, n2, iN2, iUV1, uv2, iUV2, iID, id2, iID,
				                   !v1Side);

			intersections.Add(i1);
			intersections.Add(i2);
		}

		private void ScenarioWhenThirdVertexAlone(in Plane plane,
		                                          in SlicerVertex vertex1,
		                                          in SlicerVertex vertex2,
		                                          in SlicerVertex vertex3,
		                                          bool v1Side)
		{
			vertex1.Unpack(out var v1, out var n1, out var uv1, out var id1);
			vertex2.Unpack(out var v2, out var n2, out var uv2, out var id2);
			vertex3.Unpack(out var v3, out var n3, out var uv3, out var id3);

			var i1 = FindIntersection(v2, v3, plane, out var t1);
			var i2 = FindIntersection(v1, v3, plane, out var t2);
			var iUV1 = Vector2.Lerp(uv2, uv3, t1);
			var iUV2 = Vector2.Lerp(uv1, uv3, t2);
			var iN1 = Vector3.Lerp(n2, n3, t1);
			var iN2 = Vector3.Lerp(n1, n3, t2);
			var iID = SlicerMesh.EMPTY_VERTEX_ID;

			trianglesBuffer[0] =
				new SlicerTriangle(v1, v2, i1, n1, n2, iN1, uv1, uv2, iUV1, id1, id2, iID, v1Side);
			trianglesBuffer[1] =
				new SlicerTriangle(v1, i1, i2, n1, iN1, iN2, uv1, iUV1, iUV2, id1, iID, iID,
				                   v1Side);
			trianglesBuffer[2] =
				new SlicerTriangle(i1, v3, i2, iN1, n3, iN2, iUV1, uv3, iUV2, iID, id3, iID,
				                   !v1Side);

			intersections.Add(i1);
			intersections.Add(i2);
		}

		private static Vector3 FindIntersection(in Vector3 v1,
		                                        in Vector3 v2,
		                                        Plane plane,
		                                        out float t)
		{
			if (plane.SameSide(v1, v2))
			{
				throw new Exception("Both vertices on the same side");
			}

			var ray = new Ray(v1, v2 - v1);
			plane.Raycast(ray, out var distance);
			t = distance / Vector3.Distance(v1, v2);
			return ray.GetPoint(distance);
		}

		private void FillEmptySpace(in Vector3 planeNormal, Material material)
		{
			var middlePoint = CalculateMiddlePoint(intersections);
			var positiveMidPointVertexId = meshPositive.GetNextVertexId();
			var negativeMidPointVertexId = meshNegative.GetNextVertexId();

			//TODO figure out how to calculate UV 
			var emptyUV = Vector2.zero;
			var emptyID = SlicerMesh.EMPTY_VERTEX_ID;

			var count = intersections.Count;
			for (var i = 0; i < count - 1; i += 2)
			{
				var v1 = intersections[i];
				var v2 = intersections[i + 1];
				var v3 = middlePoint;
				var pID1 = emptyID;
				var pID3 = positiveMidPointVertexId;
				var nID1 = emptyID;
				var nID3 = negativeMidPointVertexId;
				var normal = ComputeNormal(v1, v2, v3);

				var dot = Vector3.Dot(planeNormal, normal);
				if (dot < 0)
				{
					normal = -normal;
					(v1, v3) = (v3, v1);
					(pID1, pID3) = (pID3, pID1);
					(nID1, nID3) = (nID3, nID1);
				}

				{
					var vertex1 = SlicerVertex.Create(v3, -normal, emptyUV, pID3);
					var vertex2 = SlicerVertex.Create(v2, -normal, emptyUV, emptyID);
					var vertex3 = SlicerVertex.Create(v1, -normal, emptyUV, pID1);
					meshPositive.AddTriangle(vertex1, vertex2, vertex3, material);
				}

				{
					var vertex1 = SlicerVertex.Create(v1, normal, emptyUV, nID1);
					var vertex2 = SlicerVertex.Create(v2, normal, emptyUV, emptyID);
					var vertex3 = SlicerVertex.Create(v3, normal, emptyUV, nID3);
					meshNegative.AddTriangle(vertex1, vertex2, vertex3, material);
				}
			}
		}

		private static Vector3 CalculateMiddlePoint(List<Vector3> points)
		{
			var middlePoint = new Vector3();
			var count = points.Count;
			for (var i = 0; i < count; i++)
			{
				middlePoint += points[i];
			}

			return middlePoint / count;
		}

		private static Vector3 ComputeNormal(in Vector3 vertex1,
		                                     in Vector3 vertex2,
		                                     in Vector3 vertex3)
		{
			var side1 = vertex2 - vertex1;
			var side2 = vertex3 - vertex1;
			var normal = Vector3.Cross(side1, side2);
			return normal;
		}
	}
}