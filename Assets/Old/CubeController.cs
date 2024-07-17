using UnityEngine;

public class CubeController : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed of the cube

    void Update()
    {
        // Get input from horizontal and vertical axes
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveVertical = Input.GetAxis("Vertical");     // W/S or Up/Down

        // Calculate the direction based on input
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Move the cube
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }
}
