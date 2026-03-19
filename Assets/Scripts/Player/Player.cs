using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool canMove = true;

    [Header("Fishing")]
    [SerializeField] public bool canFish = false;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isCasting = false;


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
        HandleFishingInput();

    }

    private void HandleHorizontalMovement()
    {
        if (!canMove) return;

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
        if (!canMove) return;

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

    private void HandleFishingInput()
    {
        if (canFish && Input.GetKeyDown(KeyCode.E))
        {
            if (canMove)
            {
                Debug.Log("Fishing action triggered!");
                anim.SetBool("isFishing", true);
                canMove = false;
            }
            else
            {
                Debug.Log("Stopped fishing!");
                anim.SetBool("isFishing", false);
                canMove = true;
            }
        }

        CheckForCastInput();
    }

    private void CheckForCastInput()
    {
        if (!canFish || canMove) return;

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isCasting)
        {
            Debug.Log("Casting action triggered!");
            isCasting = true;
            anim.SetBool("isPrepping", true);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && isCasting)
        {
            Debug.Log("Stopped casting!");
            isCasting = false;
            anim.SetBool("isPrepping", false);
            anim.SetTrigger("Casting");
        }
    }
}
