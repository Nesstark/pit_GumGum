using UnityEngine;

public class Balance : MonoBehaviour
{
    public Transform body;

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = body.position.x;
        pos.z = body.position.z;
        transform.position = pos;
    }
}
