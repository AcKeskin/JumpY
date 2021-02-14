using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    public int time = 5;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().boosted = true;
            collision.gameObject.GetComponent<PlayerController>().timer = time;
            collision.gameObject.GetComponent<PlayerController>().StartCoroutine("Boost");
            collision.gameObject.GetComponent<SpriteRenderer>().color = new Color32(0,188,212,255) ;
        }
    }
}
