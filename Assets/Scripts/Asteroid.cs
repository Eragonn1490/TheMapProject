using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private float _orbitSpeed;
    private float _rotationSpeed;
    private Vector3 _rotationDirection;
    private Transform _parent;
    private bool _isClockwise;

    private void Update()
    {
        transform.RotateAround(_parent.position, _isClockwise ? _parent.forward : -_parent.forward, _orbitSpeed * Time.deltaTime);
        transform.Rotate(_rotationDirection, _rotationSpeed * Time.deltaTime);
    }

    public void Init(float speed, float rotationSpeed, Transform parent, bool isClockwise)
    {
        _orbitSpeed = speed;
        _rotationSpeed = rotationSpeed;
        _parent = parent;
        _isClockwise = isClockwise;
        _rotationDirection = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }
}