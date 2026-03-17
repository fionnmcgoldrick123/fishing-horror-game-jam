using System.Runtime.CompilerServices;
using UnityEngine;

public class FishingZone : MonoBehaviour
{

    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("Player entered fishing zone");

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("Player exited fishing zone");
        }
    }
}
