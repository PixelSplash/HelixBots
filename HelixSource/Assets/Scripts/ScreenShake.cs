using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private const float shakeRatio = 0.04f;
    private float _initialx;
    private float _initialy;
    [SerializeField]
    private const float _lifeTime = 1f;
    [SerializeField]
    private const float _rate = 0.05f;
    // Use this for initialization
    void Start()
    {
   
        _initialx = transform.position.x;
        _initialy = transform.position.y;
        StartCoroutine(Shake());
        StartCoroutine(KillScreenShake());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator Shake()
    {
        yield return new WaitForSeconds(_rate);
        transform.position = new Vector3(_initialx + Random.Range(-shakeRatio, shakeRatio), _initialy + Random.Range(-shakeRatio, shakeRatio), transform.position.z);
        StartCoroutine(Shake());
    }
    IEnumerator KillScreenShake()
    {
        yield return new WaitForSeconds(_lifeTime);
        transform.position = new Vector3(_initialx,_initialy, transform.position.z);
        Destroy(this.GetComponent<ScreenShake>());
    }
}
