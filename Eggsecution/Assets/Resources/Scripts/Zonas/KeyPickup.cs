using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerMovement>() == null) return;
        if(ZoneManager.Instance == null) return;

        ZoneManager.Instance.AddKey();
        Destroy(gameObject);

    }
}
