using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
       
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Debug.Log($"{collision.gameObject.name}가 점프대를 밟음!");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
