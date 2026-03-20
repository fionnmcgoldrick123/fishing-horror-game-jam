using UnityEngine;

public class FishingZone : MonoBehaviour
{

    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player entered fishing zone");

        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.CanFish = true;
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Player exited fishing zone");

        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.CanFish = false;
            }
        }
    }
}
