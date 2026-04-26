using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    public Transform target;
    //public float rotationSpeed = 5f;

    void Update()
    {
        if (target == null)
        {
            return;
        }

        transform.LookAt(target.position);
    }
}