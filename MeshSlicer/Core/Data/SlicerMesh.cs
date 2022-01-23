using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace IgorTime.MeshSlicer
{
	public class SlicerMesh
	{
		[StructLayout(LayoutKind.Sequential)]
		private readonly struct InternalVertexData
		{
			private readonly Vector3 p;
			private readonly Vector3 n;
			private readonly Vector2 uv0;
			
			public InternalVertexData(in Vector3 p, in Vector3 n, in Vector2 uv0)
			{
				this.p = p;
				this.n = n;
				this.uv0 = uv0;
			}
		}

		private readonly VertexAttributeDescriptor[] verticesLayout =
		{
			new VertexAttributeDescriptor(VertexAttribute.Position),
			new VertexAttributeDescriptor(VertexAttribute.Normal),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2)
		};

		private const int DEFAULT_CAPACITY = 2048;
		private readonly List<int> allTriangles = new List<int>(DEFAULT_CAPACITY * 2);

		private readonly Dictionary<int, SlicerVertex> idToVertexData =
			new Dictionary<int, SlicerVertex>(DEFAULT_CAPACITY);

		private readonly Dictionary<Material, List<int>> materialToIndices =
			new Dictionary<Material, List<int>>();

		private readonly IndicesCache indicesCache = 
			new IndicesCache(buffersCount: 5, DEFAULT_CAPACITY);

		private int nextVertexId = int.MaxValue;
		public const int EMPTY_VERTEX_ID = -1;

		public void Clear()
		{
			allTriangles.Clear();
			idToVertexData.Clear();
			indicesCache.ClearCache();
			ResetNextVertexId();
		}

		private int AddVertex(SlicerVertex v)
		{
			if (idToVertexData.TryGetValue(v.ID, out var vData))
			{
				return vData.Index;
			}

			if (v.ID == EMPTY_VERTEX_ID)
			{
				v.ID = GetNextVertexId();
			}

			v.Index = idToVertexData.Count;
			idToVertexData[v.ID] = v;
			return v.Index;
		}

		public void AddTriangle(in SlicerVertex v1,
		                        in SlicerVertex v2,
		                        in SlicerVertex v3,
		                        Material material)
		{
			var i1 = AddVertex(v1);
			var i2 = AddVertex(v2);
			var i3 = AddVertex(v3);
			
			if (!materialToIndices.TryGetValue(material, out var trianglesCollection))
			{
				var cacheIndex = materialToIndices.Count;
				trianglesCollection = indicesCache.GetIndicesByIndex(cacheIndex);
				materialToIndices.Add(material, trianglesCollection);
			}
			
			trianglesCollection.Add(i1);
			trianglesCollection.Add(i2);
			trianglesCollection.Add(i3);
		}

		public void AddTriangle(in SlicerTriangle slicerTriangle, 
		                        Material material)
		{
			AddTriangle(slicerTriangle.V1, slicerTriangle.V2, slicerTriangle.V3, material);
		}

		public Mesh ToUnityMesh(out Material[] materials)
		{
			allTriangles.Clear();
			foreach (var indices in materialToIndices.Values)
			{
				if (indices.Count == 0)
				{
					break;
				}
				
				allTriangles.AddRange(indices);
			}
			
			var vertexCount = idToVertexData.Count;
			var vertexBufferData = new NativeArray<InternalVertexData>(vertexCount, Allocator.Temp);
			foreach(var vertexData in idToVertexData.Values)
			{
				var i = vertexData.Index;
				vertexBufferData[i] = new InternalVertexData(vertexData.Position,
				                                             vertexData.Normal,
				                                             vertexData.UV0);
			}

			var result = new Mesh();
			result.SetVertexBufferParams(vertexCount, verticesLayout);
			result.SetVertexBufferData(vertexBufferData, 0, 0, vertexBufferData.Length);
			result.SetIndexBufferParams(allTriangles.Count, IndexFormat.UInt32);
			result.SetIndexBufferData(allTriangles, 0, 0, allTriangles.Count);
			
			result.subMeshCount = materialToIndices.Count;
			materials = new Material[result.subMeshCount];
			
			var subMeshIndex = 0;
			var indicesOffset = 0;
			foreach (var kv in materialToIndices)
			{
				var indices = kv.Value;
				if (indices.Count == 0)
				{
					break;
				}

				materials[subMeshIndex] = kv.Key;
				result.SetSubMesh(subMeshIndex, new SubMeshDescriptor(indicesOffset, indices.Count));
				indicesOffset += indices.Count;
				subMeshIndex++;
			}
			
			return result;
		}

		public int GetNextVertexId() => nextVertexId--;

		private void ResetNextVertexId()
		{
			nextVertexId = int.MaxValue;
		}
	}
}