using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject EnemyExplosion, BoostExplosion, BaitExplosion, PlayerExplosion;
    Vector2 screen;
    GameObject[] explosion;
    public int expo_count;
    public Text hs;
    private Vector3 canvas_pos,init;
    public float speed = 3f;
    int highestscore;
    // Start is called before the first frame update
    void Start()
    {
        canvas_pos = GameObject.Find("deneme").transform.position;
        screen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        init = new Vector3(canvas_pos.x + (hs.rectTransform.rect.width / 2) + Screen.width/2, canvas_pos.y, canvas_pos.z);
        expo_count = 0;
        explosion = new GameObject[4];
        explosion[0] = EnemyExplosion;
        explosion[1] = BoostExplosion;
        explosion[2] = BaitExplosion;
        explosion[3] = PlayerExplosion;
        Debug.Log(canvas_pos.x + " == " + hs.rectTransform.rect.width + " ||| " + init.x + " &&&&  "+ hs.transform.position.x);
        hs.transform.position = init;
        Debug.Log("asdsadasd " + hs.transform.position.x);
        Debug.Log( hs.transform.position.x);
        highestscore = PlayerPrefs.GetInt("HighScore",-1);
        if(highestscore < 0)
        {
            hs.text = "Enjoy your first run <3";
        }
        else
        {
            hs.text = "Highest score: " + highestscore;
        }
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(hs.transform.position.x);
        expo_count = GameObject.FindGameObjectsWithTag("Explosion").Length;
        if (expo_count < 3)
        {
            float which = Random.Range(0, 4);
           explode(explosion[(int)which]);
        }
        else if (expo_count < 10)
        {
            float percent = Random.Range(0, 100);
            if (percent < 30)
            {
                float which = Random.Range(0, 4);
                explode(explosion[(int)which]);
            }

        }
        if(hs.transform.position.x < canvas_pos.x - (hs.rectTransform.rect.width / 2))
        {
            hs.transform.position = init;
        }
        hs.transform.position = new Vector3(hs.transform.position.x-speed, hs.transform.position.y, hs.transform.position.z);
    }

    GameObject explode(GameObject go)
    {
        float x = Random.Range(-screen.x, screen.x);
        float y = Random.Range(-screen.y, screen.y);
        var expo = Instantiate(go) as GameObject;
        var main = expo.GetComponent<ParticleSystem>().main;
        main.simulationSpeed = 0.5f;
        expo.transform.position = new Vector3(x, y);
        return expo;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
