using System.Collections.Generic;

namespace IgorTime.MeshSlicer
{
	public class IndicesCache
	{
		private readonly int bufferCapacity;
		private readonly List<List<int>> indicesCache;

		public IndicesCache(int buffersCount, int bufferCapacity)
		{
			this.bufferCapacity = bufferCapacity;
			indicesCache = new List<List<int>>(buffersCount);

			ResizeCacheIfNeeded(buffersCount);
		}
		
		public void ClearCache()
		{
			foreach (var indicesBuffer in indicesCache)
			{
				indicesBuffer.Clear();
			}
		}
		
		public List<int> GetIndicesByIndex(int index)
		{
			ResizeCacheIfNeeded(index);
			return indicesCache[index];
		}

		private void ResizeCacheIfNeeded(int size)
		{
			var cacheCount = indicesCache.Count;
			if (cacheCount <= size)
			{
				indicesCache.Add(new List<int>(bufferCapacity));
			}
		}
	}
}