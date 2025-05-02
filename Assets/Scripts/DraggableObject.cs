using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    [Header("Drag Settings")]
    private Camera _mainCamera;
    private bool _isDragging = false;
    private float _fixedY = 0.5f;
    private Plane _dragPlane;
    private Vector3 _offset;
    private bool _isDraggable = true;

    [Header("Timer")]
    private bool _isTimerOn = false;
    private TimeSpan _timePlaying;
    private float _elapsedTime = 0f;
    private int _clics = 0;
    private int _errors = 0;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _errorsText;
    [SerializeField] private TextMeshProUGUI _clicsText;

    private void Start()
    {
        _mainCamera = Camera.main;
        _timerText.text = "Temps : 00:00.00";
    }

    private void OnMouseDown()
    {
        if (!_isDraggable) return;
        IncrementClics();
        if (!_isTimerOn) { BeginTimer(); }
        _dragPlane = new Plane(Vector3.up, new Vector3(0, _fixedY, 0));

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (_dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            _offset = transform.position - hitPoint;
            _isDragging = true;
        }
    }

    private void OnMouseDrag()
    {
        if (!_isDraggable || !_isDragging) return;

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (_dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 targetPos = hitPoint + _offset;
            transform.position = new Vector3(targetPos.x, _fixedY, targetPos.z);
        }
    }

    private void OnMouseUp()
    {
        if (!_isDraggable) return;
        _isDragging = false;

        Collider[] hitZones = Physics.OverlapBox(transform.position, transform.localScale / 2f, Quaternion.identity);

        foreach (var col in hitZones)
        {
            if (col.CompareTag("Zone") && IsFullyInsideXZ(col))
            {
                SetPlacement(col.transform.position.x, col.transform.position.y + 0.6f, col.transform.position.z);
                EndTimer();
                break;
            }
        }
        if (_isDraggable) IncrementErrors();
    }

    private bool IsFullyInsideXZ(Collider zone)
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

    private void SetPlacement(float x, float y, float z)
    {
        _isDraggable = false;
        transform.position = new Vector3(x, y, z);
    }
    private void IncrementErrors()
    {
        _errors++;
        _errorsText.text = "Erreurs : " + _errors;
    }
    private void IncrementClics()
    {
        _clics++;
        _clicsText.text = "Clics :" + _clics;
    }


    private void BeginTimer()
    {
        _isTimerOn = true;
        _elapsedTime = 0f;
        StartCoroutine(UpdateTimer());
    }

    private void EndTimer()
    {
        _isTimerOn = false;
    }
    private IEnumerator UpdateTimer()
    {
        while (_isTimerOn)
        {
            _elapsedTime += Time.deltaTime;
            _timePlaying = TimeSpan.FromSeconds(_elapsedTime);
            string timePlayingStr = "Time: " + _timePlaying.ToString("mm':'ss'.'ff");
            _timerText.text = timePlayingStr;

            yield return null;
        }
    }
}
