using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private float fixedY = 0.5f;
    private Plane dragPlane;
    private Vector3 offset;

    public bool isDraggable = true;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        if (!isDraggable) return;
        dragPlane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            offset = transform.position - hitPoint;
            isDragging = true;
        }
    }

    void OnMouseDrag()
    {
        if (!isDraggable || !isDragging) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 targetPos = hitPoint + offset;
            transform.position = new Vector3(targetPos.x, fixedY, targetPos.z);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        Collider[] hitZones = Physics.OverlapBox(transform.position, transform.localScale / 2f, Quaternion.identity);

        foreach (var col in hitZones)
        {
            if (col.CompareTag("Zone") && IsFullyInsideXZ(col))
            {
                SetPlacement(col.transform.position.x, col.transform.position.y + 0.6f, col.transform.position.z);
                break;
            }
        }
    }

    bool IsFullyInsideXZ(Collider zone)
    {
        Bounds zoneBounds = zone.bounds;
        Bounds myBounds = GetComponent<Collider>().bounds;

        Vector3 min = myBounds.min;
        Vector3 max = myBounds.max;

        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(min.x, zoneBounds.center.y, min.z);
        corners[1] = new Vector3(max.x, zoneBounds.center.y, min.z);
        corners[2] = new Vector3(min.x, zoneBounds.center.y, max.z);
        corners[3] = new Vector3(max.x, zoneBounds.center.y, max.z);

        foreach (Vector3 corner in corners)
        {
            if (!zoneBounds.Contains(corner))
                return false;
        }

        return true;
    }

    public void SetPlacement(float x, float y, float z)
    {
        isDraggable = false;
        transform.position = new Vector3(x, y, z);
    }
}
