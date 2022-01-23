using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class Slicable : MonoBehaviour
{
	[SerializeField] private bool isSolid;
	[SerializeField] private Material sliceMaterial;

	public Mesh Mesh { get; private set; }
	public Material[] MainMaterials { get; private set; }

	public Material[] GetAllMaterials()
	{
		var hasDuplicated = HasDuplicatedMaterial();
		var count = !hasDuplicated ? MainMaterials.Length + 1 : MainMaterials.Length;
		var result = new Material[count];
		for (var i = 0; i < MainMaterials.Length; i++)
		{
			result[i] = MainMaterials[i];
		}

		if (!hasDuplicated)
		{
			result[result.Length - 1] = sliceMaterial;
		}

		return result;
	}

	private bool HasDuplicatedMaterial()
	{
		foreach (var material in MainMaterials)
		{
			if (material == SliceMaterial)
			{
				return true;
			}
		}

		return false;
	}

	public Material SliceMaterial
	{
		get => sliceMaterial;
		set => sliceMaterial = value;
	}

	public bool IsSolid
	{
		get => isSolid;
		set => isSolid = value;
	}

	private void Awake()
	{
		var meshFilter = GetComponent<MeshFilter>();
		Mesh = meshFilter.sharedMesh;

		var meshRenderer = GetComponent<MeshRenderer>();
		MainMaterials = meshRenderer.sharedMaterials;
	}
}