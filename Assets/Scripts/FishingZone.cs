using System.Runtime.CompilerServices;
using UnityEngine;

public class FishingZone : MonoBehaviour
{

    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player entered fishing zone");

        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.canFish = true;
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Player exited fishing zone");

        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.canFish = false;
            }
        }
    }
}
