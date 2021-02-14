using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class GameEvents : MonoBehaviour
{
    public GameObject Player;
    /*[Range(0.0f, 1.0f)]
    public float r;
    [Range(0.0f, 1.0f)]
    public float g;
    [Range(0.0f, 1.0f)]
    public float b;*/
    private Color orange;
    private PlayerController pc;
    public GameObject Enemy, Boost, Bait;
    public GameObject EnemyExplosion, BoostExplosion, BaitExplosion, PlayerExplosion;
    public Text scoreText;
    public Text timeText;
    //public int obj_limit = 0;
    private int div = 350, res = 0;
    float speedLimit = 50;
    bool dead,starting;
    private List<GameObject> enemies, baits, boosts;
    private int enemyCount = 0, baitCount = 0, boostCount = 0;
    Vector2 screen;
    // Start is called before the first frame update
    void Start()
    {
        orange = new Color32(252,127,3,255) ;
        enemies = new List<GameObject>();
        baits = new List<GameObject>();
        boosts = new List<GameObject>();
        screen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        pc = Player.GetComponent<PlayerController>();
        enemyCount++;
        boostCount++;
        baitCount += 2;
        timeText.enabled = false;
        for (int i = 0; i < enemyCount; i++)
        {
            enemies.Add(Spawn(Enemy));
            boosts.Add(Spawn(Boost));
        }
        for (int i = 0; i < baitCount; i++)
        {
            baits.Add(Spawn(Bait));
        }
        starting = true;
        StartCoroutine("DelayedStart");
        //obj_limit = enemyCount + baitCount + boostCount;
    }

    // Update is called once per frame
    void Update()
    {
            if (pc != null && !pc.dead)
            {
                gamestuff();
                scoreText.text = "Score: " + pc.score;
                if (pc.boosted && !timeText.enabled)
                {
                    timeText.enabled = true;
                }
                if (pc.boosted && timeText.enabled)
                {
                    timeText.text = "" + pc.timer;
                }
                else if (timeText.enabled && !starting)
                {
                    timeText.enabled = false;
                }
            }
            else if (!dead)
            {
                deadScreen();


            }
            else if (pc.dead)
            {
                if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            }
    }
    GameObject Spawn(GameObject go)
    {
        float x = Random.Range(-screen.x, screen.x);
        float y = Random.Range(-screen.y, screen.y);
        float dist = Vector2.Distance(new Vector2(x, y), Player.transform.position);
        while (dist < (3 * screen.x / 2))
        {
            x = Random.Range(-screen.x, screen.x);
            y = Random.Range(-screen.y, screen.y);
            dist = Vector2.Distance(new Vector2(x, y), Player.transform.position);
        }
        GameObject newBait = Instantiate(go) as GameObject;
        newBait.transform.position = new Vector2(x, y);

        newBait.transform.localScale *= pc.scalingFactor;

        Rigidbody2D baitRB = newBait.GetComponent<Rigidbody2D>();
        float speedx = Random.Range(-speedLimit, speedLimit);
        float speedy = Random.Range(-speedLimit, speedLimit);
        baitRB.velocity = new Vector2(speedx, speedy);
        newBait.AddComponent<ReSpeed>();
        if(go == Enemy && pc.boosted)
        {
            newBait.GetComponent<SpriteRenderer>().color = orange;
        }
        return newBait;
    }

    private void deadScreen()
    {
        //scoreText.enabled = false;
        dead = true;
        timeText.enabled = false;
        int hs = PlayerPrefs.GetInt("HighScore", -1);
        if (hs <= 0 || hs < pc.score)
        {
            hs = pc.score;
            scoreText.text = "You have a new high score !!\n" + hs ;
            scoreText.verticalOverflow = VerticalWrapMode.Overflow;
            PlayerPrefs.SetInt("HighScore",hs);
        }
        else 
        {
            scoreText.text = "Your score is " + pc.score + "\n Your highest score is\n" + hs ;
        }
        scoreText.alignment = TextAnchor.MiddleCenter;
        scoreText.fontSize = scoreText.fontSize + 15;
        scoreText.color = Color.yellow;
    }

    private void gamestuff()
    {
        if (pc.score / div > res)
        {
            res++;
            int r = Random.Range(1, 101);
            if (r > 60*(enemies.Count/4))
                enemyCount++;
            else if (r > 20 * (baits.Count/6))
                baitCount++;
            else
            {
                boosts.Add(Spawn(Boost));
            }
        }
        if (enemies.Count < enemyCount)
            for (int i = enemies.Count; i < enemyCount; i++)
            {
                enemies.Add(Spawn(Enemy));
            }

        if (baits.Count < baitCount)
            for (int i = baits.Count; i < baitCount; i++)
            {
                baits.Add(Spawn(Bait));
            }
    }

    public void kill(GameObject go, float offset)
    {
        if (go.tag == "Enemy")
        {
            enemies.Remove(go);
            GameObject expo = Instantiate(EnemyExplosion) as GameObject;
            expo.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + offset);
            Destroy(go);
        }
        else if (go.tag == "Bait")
        {
            baits.Remove(go);
            GameObject expo = Instantiate(BaitExplosion) as GameObject;
            expo.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + offset);
            Destroy(go);
        }
        else if (go.tag == "Boost")
        {
            boosts.Remove(go);
            GameObject expo = Instantiate(BoostExplosion) as GameObject;
            expo.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + offset);
            Destroy(go);
            foreach (GameObject e in enemies)
            {
                e.GetComponent<SpriteRenderer>().color = orange;
            }
        }
        else if (go.tag == "Player")
        {
            boosts.Remove(go);
            GameObject expo = Instantiate(PlayerExplosion) as GameObject;
            expo.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + offset);
            Destroy(go);
        }
    }

    public void UndoEnemyColors()
    {
        foreach (GameObject e in enemies)
        {
            e.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    IEnumerator DelaySeconds(float time)
    {
        yield return new WaitForSeconds(time);
    }

    IEnumerator DelayedStart()
    {
        Debug.Log("Delaay!");
        timeText.enabled = true;
        Time.timeScale = 0f;
        float pauseEndTime = Time.realtimeSinceStartup + 4;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            timeText.text = "Starting in\n" + (int)(pauseEndTime - Time.realtimeSinceStartup);
            yield return 0;
        }
        timeText.enabled = false;
        Time.timeScale = 1;
        starting = false;
    }
}
