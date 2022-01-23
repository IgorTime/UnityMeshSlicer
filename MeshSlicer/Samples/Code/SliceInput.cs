using UnityEngine;
using UnityEngine.Profiling;

public class SliceInput : MonoBehaviour
{
	[SerializeField] private Camera cam = default;
	[SerializeField] private LayerMask layerMask = default;
	[SerializeField] private float rayCastDistance = 10f;

	[Header("Debug:")] [SerializeField] private bool enableDebug;

	private RaycastHit? startHit;
	private RaycastHit? endHit;
	private RaycastHit? currentHit;

	private void Awake()
	{
		if (!cam)
		{
			cam = Camera.main;
		}
	}

	private void Update()
	{
		ProcessRayCastHits();
		TrySliceObject();
		ClearData();
	}

	private void ProcessRayCastHits()
	{
		if (Input.GetMouseButton(0))
		{
			var rayOrigin = Input.mousePosition;
			var ray = cam.ScreenPointToRay(rayOrigin);
			var isHit = Physics.Raycast(ray, out var hit, rayCastDistance, layerMask);
			if (isHit)
			{
				currentHit = hit;

				if (!startHit.HasValue)
				{
					startHit = hit;
				}
			}
			else
			{
				if (!endHit.HasValue)
				{
					endHit = currentHit;
				}

				currentHit = null;
			}
		}
	}

	private void ClearData()
	{
		if (Input.GetMouseButtonUp(0))
		{
			startHit = null;
			endHit = null;
			currentHit = null;
		}
	}

	private void TrySliceObject()
	{
		if (!startHit.HasValue ||
		    !endHit.HasValue)
		{
			return;
		}

		var startHitPoint = startHit.Value.point;
		var endHitPoint = endHit.Value.point;
		var objectToSlice = startHit.Value.collider.GetComponent<Slicable>();

		startHit = null;
		endHit = null;

		if (!objectToSlice)
		{
			return;
		}

		var forward = cam.transform.forward;
		var sliceVector = startHitPoint - endHitPoint;
		var inNormal = Vector3.Cross(sliceVector, forward);
		var inPont = Vector3.Lerp(startHitPoint, endHitPoint, 0.5f);

		var matrix = objectToSlice.transform.worldToLocalMatrix;
		var plane = new Plane(matrix.MultiplyVector(inNormal),
		                      matrix.MultiplyPoint(inPont));

		var isSliced = SliceTool.Slice(objectToSlice, plane);
		if (isSliced)
		{
			Destroy(objectToSlice.gameObject);
		}
	}
}