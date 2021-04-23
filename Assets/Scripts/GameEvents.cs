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
    public GameObject Enemy, Boost, Bait, canvas, how2;
    public GameObject EnemyExplosion, BoostExplosion, BaitExplosion, PlayerExplosion;
    public Text scoreText, timeText, instruct_l, instruct_r;
    public Image left_h, right_h;
    //public int obj_limit = 0;
    private int div = 350, res = 0;
    float speedLimit = 50,height,width;
    bool dead, starting;
    private List<GameObject> enemies, baits, boosts;
    private int enemyCount = 0, baitCount = 0, boostCount = 0;
    Vector2 screen;
    // Start is called before the first frame update
    void Start()
    {
        starting = true;
        screen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        pc = Player.GetComponent<PlayerController>();
        timeText.enabled = false;
        StartCoroutine("DelayedStart");
        orange = new Color32(252, 127, 3, 255);
        enemies = new List<GameObject>();
        baits = new List<GameObject>();
        boosts = new List<GameObject>();
        enemyCount++;
        boostCount++;
        baitCount += 2;
        for (int i = 0; i < enemyCount; i++)
        {
            enemies.Add(Spawn(Enemy));
            boosts.Add(Spawn(Boost));
        }
        for (int i = 0; i < baitCount; i++)
        {
            baits.Add(Spawn(Bait));
        }
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
        else if(pc == null)
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
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
        if (go == Enemy && pc.boosted)
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
            scoreText.text = "You have a new high score !!\n" + hs;
            scoreText.verticalOverflow = VerticalWrapMode.Overflow;
            PlayerPrefs.SetInt("HighScore", hs);
        }
        else
        {
            scoreText.text = "Your score is " + pc.score + "\n Your highest score is\n" + hs;
        }
        //PlayerPrefs.SetInt("HighScore", 0);
        scoreText.alignment = TextAnchor.MiddleCenter;
        scoreText.fontSize = scoreText.fontSize + 15;
        scoreText.color = Color.yellow;

    }

    private void introScene()
    {
        RectTransform rt = canvas.GetComponent<RectTransform>();
        left_h.rectTransform.sizeDelta = new Vector2(rt.sizeDelta.x / 2, rt.sizeDelta.y);
        left_h.rectTransform.position = new Vector3(rt.position.x - (rt.sizeDelta.x / 4), rt.position.y, rt.position.z);
        right_h.rectTransform.sizeDelta = new Vector2(rt.sizeDelta.x / 2, rt.sizeDelta.y);
        right_h.rectTransform.position = new Vector3(rt.position.x + (rt.sizeDelta.x / 4), rt.position.y, rt.position.z);
        instruct_l.rectTransform.sizeDelta = new Vector2(rt.sizeDelta.x / 3, rt.sizeDelta.y); ;
        instruct_l.rectTransform.position = new Vector3(rt.position.x - (rt.sizeDelta.x / 4), rt.position.y, rt.position.z);
        instruct_r.rectTransform.sizeDelta = new Vector2(rt.sizeDelta.x / 3, rt.sizeDelta.y);
        instruct_r.rectTransform.position =  new Vector3(rt.position.x + (rt.sizeDelta.x / 4), rt.position.y, rt.position.z);
    }

    private void gamestuff()
    {
        if (pc.score / div > res)
        {
            res++;
            int r = Random.Range(1, 101);
            if (r > 60 * (enemies.Count / 4))
                enemyCount++;
            else if (r > 20 * (baits.Count / 6))
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
        timeText.enabled = true;
        Time.timeScale = 0f;
        int time_to = 7;
        if(PlayerPrefs.GetInt("HighScore",0) < 1)
            introScene();
        else
        {
            time_to = 4;
            instruct_l.enabled = false;
            instruct_r.enabled = false;
            right_h.enabled = false;
            left_h.enabled = false;
        }
        float pauseEndTime = Time.realtimeSinceStartup + time_to;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {

            if (PlayerPrefs.GetInt("HighScore", 0) < 1 && (int)(pauseEndTime - Time.realtimeSinceStartup) == 3)
            {
                instruct_l.enabled = false;
                instruct_r.enabled = false;
                right_h.enabled = false;
                left_h.enabled = false;
            }
            timeText.text = "Starting in\n" + (int)(pauseEndTime - Time.realtimeSinceStartup);
            yield return 0;
        }
        timeText.enabled = false;
        Time.timeScale = 1;
        starting = false;
    }
}
