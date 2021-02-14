using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height, Camera.main.orthographicSize * 2.0f, 0.1f);
    }

}
