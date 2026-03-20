using UnityEngine;

public class ExitShop : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;

    private PlayerController player;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player entered exit shop zone");

        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.CanExitShop = true;
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Player exited exit shop zone");

        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.CanExitShop = false;
            }
        }
    }
}
