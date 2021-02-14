using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpeed : MonoBehaviour
{

    public float speedLimit = 50;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude < 10)
        {
            float speedx = Random.Range(-speedLimit, speedLimit);
            float speedy = Random.Range(-speedLimit, speedLimit);
            rb.velocity = new Vector2(speedx, speedy);

        }
    }

    public float SpeedLimit
    {
        get { return speedLimit; }
        set { SpeedLimit = speedLimit; }
    }
}
