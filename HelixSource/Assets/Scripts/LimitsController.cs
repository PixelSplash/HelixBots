using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitsController : MonoBehaviour {
    [SerializeField]
    private float _machTime = 60f;
    [SerializeField]
    private float journeyTime = 30f;
    private float _startTime;
    private bool _moving = false;
    private Vector3 _initialPos;

    [SerializeField]
    private List<ParticleSystem> _backgroundParticles;

    [SerializeField]
    private int _calmEmission = 50;
   
    [SerializeField]
    private int _intenseEmission = 500;

    // Use this for initialization
    void Start () {
        _initialPos = transform.position;
        
        StartCoroutine("StartMatchTime");
    }

    private IEnumerator StartMatchTime()
    {
        _moving = false;
        yield return new WaitForSeconds(_machTime);
        AudioManager.Instance.StartSuddenDeathMusic();
        IntenseParticleEffect();
        _startTime = Time.time;
        _moving = true;
    }

    // Update is called once per frame
    void Update () {
        if (_moving)
        {
            float fracComplete = (Time.time - _startTime) / journeyTime;
            transform.position = Vector3.Lerp(_initialPos, Vector3.zero, fracComplete);
        }
    }

    public void StopMovement()
    {
        StopCoroutine("StartMatchTime");
        _moving = false;
    }

    public void ResetLimit()
    {
        CalmParticleEffect();
        StopMovement();
        transform.position = _initialPos;
        StartCoroutine("StartMatchTime");
    }

    private void IntenseParticleEffect()
    {
        foreach (ParticleSystem particle in _backgroundParticles)
        {
            ParticleSystem.EmissionModule module = particle.emission;
            ParticleSystem.MinMaxCurve rate = new ParticleSystem.MinMaxCurve();
            rate.constantMax = _intenseEmission;
            module.rateOverTime = rate;
        }
    }

    private void CalmParticleEffect()
    {
        foreach (ParticleSystem particle in _backgroundParticles)
        {
            ParticleSystem.EmissionModule module = particle.emission;
            ParticleSystem.MinMaxCurve rate = new ParticleSystem.MinMaxCurve();
            rate.constantMax = _calmEmission;
            module.rateOverTime = rate;
        }
    }
}
