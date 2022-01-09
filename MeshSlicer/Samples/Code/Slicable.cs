using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class Slicable : MonoBehaviour
{
	[SerializeField] private bool isSolid;
	[SerializeField] private Material sliceMaterial;

	public Mesh Mesh { get; private set; }
	public Material MainMaterial { get; private set; }
	public Material SliceMaterial => sliceMaterial;

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
		MainMaterial = meshRenderer.sharedMaterial;
	}
}