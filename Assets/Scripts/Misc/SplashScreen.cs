using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] protected float _delay = 3f;

    private void Start()
    {
        Invoke("End",_delay);
    }

    void End()
    {
        SceneManager.LoadScene(1);
    }
}
