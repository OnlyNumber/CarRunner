using Unity.VisualScripting;
using UnityEngine;

public class TurretAimLine : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    [SerializeField] private float _lineLength;

    private void Update()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position + transform.forward * _lineLength);
    }
}
