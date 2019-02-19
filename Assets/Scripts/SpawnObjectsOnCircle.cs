using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnObjectsOnCircle : MonoBehaviour
{
    private bool _dirty = true;
    private int _cachedAmountOfPoints = -1;
    private Vector3[] _points = new Vector3[0];

    [SerializeField] private int _amountOfPoints = 8;
    [SerializeField] private GameObject _gameObjectPrefab = null;
    [SerializeField] private float _circleRadius = 20;
    [SerializeField] private Vector3 _originInLocalSpace = Vector3.zero;

    private Vector3[] Points
    {
        get
        {
            if (_cachedAmountOfPoints == _amountOfPoints && !_dirty) return _points;

            _points = new Vector3[_amountOfPoints];
            float deltaAngle = Mathf.PI * 2 / _amountOfPoints;
            float currentAngle = -deltaAngle;
            for (int i = 0; i < _amountOfPoints; i++)
            {
                currentAngle += deltaAngle;
                float x = Mathf.Cos(currentAngle) * _circleRadius;
                float y = Mathf.Sin(currentAngle) * _circleRadius;
                _points[i] = new Vector3( x + 180, y, 0) + _originInLocalSpace + transform.position;
            }

            _cachedAmountOfPoints = _amountOfPoints;
            return _points;
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int index = 0; index < _amountOfPoints; index++)
        {
            int nextIndex = index + 1;
            if (nextIndex >= _amountOfPoints) nextIndex = 0;
            Gizmos.DrawLine(Points[index], Points[nextIndex]);
            Gizmos.DrawWireCube(Points[index], Vector3.one);
        }
    }

    private void OnValidate()
    {
        _amountOfPoints = Mathf.Max(3, _amountOfPoints);
        _circleRadius = Mathf.Max(0, _circleRadius);
        _dirty = true;
    }

    public void SpawnObjects()
    {
        if (_gameObjectPrefab == null) return;
        foreach (var point in _points)
            Instantiate(_gameObjectPrefab, point, _gameObjectPrefab.transform.rotation, transform);
    }
}
