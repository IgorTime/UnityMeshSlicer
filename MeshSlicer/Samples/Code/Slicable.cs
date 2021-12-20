using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class Slicable : MonoBehaviour
{
	[SerializeField] private bool isSolid;

	public Mesh Mesh { get; private set; }
	public Material Material { get; private set; }

	public bool IsSolid
	{
		get => isSolid;
		set => isSolid = value;
	}

	private void Awake()
	{
		var meshFilter = GetComponent<MeshFilter>();
		Mesh = meshFilter.mesh;

		var meshRenderer = GetComponent<MeshRenderer>();
		Material = meshRenderer.material;
	}
}