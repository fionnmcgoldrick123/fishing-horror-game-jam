using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;


    private Rigidbody2D rb;
    private Animator anim;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleHorizontalMovement();
        HandleAnimations();
        HandleFlip();
    }

    private void HandleHorizontalMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.linearVelocityY);
        rb.linearVelocity = movement;
    }

    private void HandleAnimations()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        anim.SetFloat("xVelocity", Mathf.Abs(horizontalInput));
    }

    private void HandleFlip()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput > 0 && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (horizontalInput < 0 && transform.localScale.x > 0)
        {
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}
