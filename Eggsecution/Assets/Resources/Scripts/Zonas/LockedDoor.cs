using UnityEngine;

public class LockedDoor : MonoBehaviour
{
   private void OnCollisionEnter2D(Collision2D collision)
   {
        if (collision.collider.GetComponentInParent<PlayerMovement>() == null) return;
        if (ZoneManager.Instance == null) return;

        if (ZoneManager.Instance.UseKey())  
        {
            Debug.Log(name + " abierta");
            gameObject.SetActive(false);    
            gameObject.SetActive(false);    
        }
        else
        {
            Debug.Log(name + ": necesitas una llave");
        }
   }
}
