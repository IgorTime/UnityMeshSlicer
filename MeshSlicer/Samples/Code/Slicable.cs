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
		var result = new Material[MainMaterials.Length + 1];
		for (var i = 0; i < MainMaterials.Length; i++)
		{
			result[i] = MainMaterials[i];
		}

		result[result.Length - 1] = sliceMaterial; 
		return result;
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