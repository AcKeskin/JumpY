using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private CircleCollider2D collider;
    public float xVelocity = 3, yVelocity = 5,offset = 3;
    public int score, timer;
    public float scalingFactor = 1;
    public GameEvents ge;
    public bool dead = false, boosted = false;
    Vector2 screen;
    // Start is called before the first frame update
    void Start()
    {
        ge = GameObject.FindObjectOfType<GameEvents>();
        screen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        score = 0;
        timer = 0;
        rb = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<CircleCollider2D>();
        scalingFactor =  1920.0f / (float)Screen.height;
        transform.localScale *= scalingFactor;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            if (Input.GetKeyDown(KeyCode.A) )
            {
                rb.velocity = new Vector2(-xVelocity, yVelocity);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {

                rb.velocity = new Vector2(xVelocity, yVelocity);
            } 
            if(Input.touchCount>0)
            {
                if (Input.touches[Input.touches.Length - 1].position.x < Screen.width / 2)
                {
                    rb.velocity = new Vector2(-xVelocity, yVelocity);
                }
                else if (Input.touches[Input.touches.Length - 1].position.x > Screen.width / 2)
                {

                    rb.velocity = new Vector2(xVelocity, yVelocity);
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject go = collision.gameObject;
        if (go.tag == "Bait")
        {
            collider.isTrigger = true;
            ge.kill(collision.gameObject,0);
            score += 40;
            collider.isTrigger = false;
        }
        if (go.tag == "Boost")
        {
            collider.isTrigger = true;
            ge.kill(collision.gameObject, 0);
            score += 45;
            collider.isTrigger = false;
        }
        if (go.tag == "Bounds")
        {
            go = collision.contacts[0].collider.gameObject;
            if (go.name == "BottomCollider")
            {
                collision.collider.isTrigger = true;
                collision.contacts[0].collider.gameObject.transform.position -= new Vector3(0,offset, 0);
                Die();
            }
            else if (go.name == "LeftCollider" || go.name == "RightCollider")
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
            else if (go.name == "TopCollider")
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            }
        }
        if (collision.gameObject.tag == "Enemy")
        {
            if (!boosted)
                killEverything();
            else
            {
                ge.kill(collision.gameObject, 0);
                score += 65;
            }
        }
    }
    public void Die()
    {
        dead = true;
        rb.velocity = new Vector2(0, yVelocity);
        collider.isTrigger = true;
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        if(score > PlayerPrefs.GetInt("HighScore"))
            PlayerPrefs.SetInt("HighScore", score);
        foreach (GameObject go in allObjects)
            if (go.activeInHierarchy && go.tag != "Bounds")
            {
                if (go.GetComponent<Rigidbody2D>() != null)
                {
                    go.GetComponent<Rigidbody2D>().velocity = new Vector2(0, yVelocity);
                    go.GetComponent<Collider2D>().isTrigger = true;
                    go.GetComponent<Rigidbody2D>().gravityScale = rb.gravityScale;
                }

            }
    }

    public void killEverything()
    {
        dead = true;
        collider.isTrigger = true;
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
            if (go.activeInHierarchy && go.tag != "Bounds")
            {
                ge.kill(go,0);

            }
        ge.kill(gameObject,0);
    }

    public IEnumerator Boost()
    {
        timer++;
        while (timer > 1)
        {
            timer--;
            yield return new WaitForSeconds(1f);
        }
        GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<TrailRenderer>().material.color = Color.white;
        boosted = false;
        ge.UndoEnemyColors();
    }
}
