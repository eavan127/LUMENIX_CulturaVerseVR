using UnityEngine;
using UnityEngine.InputSystem; 

public class Test_Script : MonoBehaviour 
{
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) 
        {
            // Safety check: ensure Camera.main actually exists
            if (Camera.main == null)
            {
                Debug.LogError("Error: No Camera found with the 'MainCamera' tag!");
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("SUCCESS! You clicked on: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("Ray fired, but didn't hit anything. Check your Collider size!");
            }
        }
    }
}
