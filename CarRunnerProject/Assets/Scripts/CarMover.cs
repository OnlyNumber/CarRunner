using UnityEngine;

public class CarMover : MonoBehaviour
{
    [SerializeField] private Transform _car;
    [SerializeField] private float _speed;

    private void Update()
    {
        _car.transform.position = _car.transform.position + Vector3.forward * _speed * Time.deltaTime;
    }

    public Vector3 GetCarPosition()
    {
        return _car.transform.position;
    }

}
