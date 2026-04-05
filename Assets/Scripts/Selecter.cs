using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VRButtonPoker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to a UI button and invoke its onClick event
        if (other.CompareTag("UI"))
        {
            Debug.Log("Button poked: " + other.gameObject.name);
            other.GetComponent<Button>().onClick.Invoke();
        }
    }
}
