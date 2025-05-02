using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Camera _mainCamera;
    [SerializeField] private GameObject[] _cameraPositions;

    [SerializeField] private GameObject[] _panels;
    [SerializeField] private TextMeshProUGUI _levelText;
    private string[] _levelNames = { "Condition A", "Condition B", "Condition C" };
    [SerializeField] private TextMeshProUGUI _prevBtnText;
    [SerializeField] private TextMeshProUGUI _nextBtnText;

    private int _currentLevel = 0;


    private void Start()
    {
        _mainCamera = Camera.main;
        _mainCamera.transform.position = _cameraPositions[_currentLevel].transform.position;
        _mainCamera.transform.rotation = _cameraPositions[_currentLevel].transform.rotation;
    }

    public void PrevLevel()
    {
        _panels[_currentLevel].SetActive(false);
        _currentLevel--;
        UpdateLevel();
    }

    public void NextLevel()
    {
        _panels[_currentLevel].SetActive(false);
        _currentLevel++;
        UpdateLevel();
    }

    private void UpdateLevel()
    {
        if (_currentLevel > 2) _currentLevel = 0;
        if (_currentLevel < 0) _currentLevel = 2;
        _panels[_currentLevel].SetActive(true);
        _levelText.text = _levelNames[_currentLevel];
        _prevBtnText.text = "-> " + _levelNames[_currentLevel - 1 < 0 ? 2 : _currentLevel - 1];
        _nextBtnText.text = "-> " + _levelNames[_currentLevel + 1 > 2 ? 0 : _currentLevel + 1];

        _mainCamera.transform.position = _cameraPositions[_currentLevel].transform.position;
        _mainCamera.transform.rotation = _cameraPositions[_currentLevel].transform.rotation;
    }

    public void ResetStats()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        GameObject[] zones = GameObject.FindGameObjectsWithTag("Zone");

        foreach (GameObject cube in cubes)
        {
            cube.GetComponent<DraggableObject>().ResetStats(_currentLevel);
        }

        foreach (GameObject zone in zones)
        {
            zone.GetComponent<Collider>().enabled = true;
            zone.GetComponent<Renderer>().material = Resources.Load<Material>("Cyan");
        }
    }
}
