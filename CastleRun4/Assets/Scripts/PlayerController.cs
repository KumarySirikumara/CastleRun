using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Start() variables
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    // FSM
    private enum State {idle, running, jumping, falling, hurt}
    private State state = State.idle;

    // Inspector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource collect;
    [SerializeField] private AudioSource hurt;
    [SerializeField] private int score = 0;
    [SerializeField] private TextMeshProUGUI scoreCount;
    [SerializeField] private int health = 3;
    [SerializeField] private TextMeshProUGUI healthCount;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state != State.hurt)
        {
            Movement();
        }
        AnimationState();
        anim.SetInteger("state", (int)state);   // set animator based on enum state
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Coin")
        {
            collect.Play();
            Destroy(collision.gameObject);
            score += 1;
            scoreCount.text = score.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Obstacle")
        {
            hurt.Play();
            state = State.hurt;
            HandleHealth();
        }
    }

    private void HandleHealth()
    {
        health -= 1;
        healthCount.text = health.ToString();

        if(health <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");

        // moving left
        if(hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }
        // moving right
        else if(hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }
        // New added start
        else if(hDirection == 0 && coll.IsTouchingLayers(ground))
        {
            if (state == State.running || state == State.falling)
            {
                rb.velocity = Vector3.zero;                
            }
        }
        // New added end
        
        // jumping
        if(Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            state = State.jumping;
        }
    }

    private void AnimationState()
    {
        if(state == State.jumping)
        {
            if (rb.velocity.y < 0.1f)
            {
                state = State.falling;
            }
        }
        else if(state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        else if (state == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                state = State.idle;
            }
        }
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            //moving
            state = State.running;
        }
        else
        {
            state = State.idle;
        }
    }

    private void Footstep()
    {
        footstep.Play();
    }
}
