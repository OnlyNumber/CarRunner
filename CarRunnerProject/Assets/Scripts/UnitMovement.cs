using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    public void MoveToTarget(Vector3 target, float speed, float rotationSpeed)
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        Vector3 dir = target - transform.position;

        Vector3 moveDirection = new Vector3(dir.x, 0f, dir.z).normalized;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
