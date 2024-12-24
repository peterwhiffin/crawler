using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotation;

    private void Update()
    {
        transform.Rotate(0f, 0f, rotation * Time.deltaTime);
    }
}
