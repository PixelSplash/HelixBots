using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour {
    [SerializeField]
    public int _bombId;
    [SerializeField]
    private float _lifeTime;
    [SerializeField]
    public GameObject _player;


    [SerializeField]
    public GameObject _explosionParticles;

    // Use this for initialization
    void Start () {
        StartCoroutine(AliveTimerStart());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private IEnumerator AliveTimerStart()
    {
        //Debug.Log("Sdas");
        yield return new WaitForSeconds(_lifeTime);
        //PlayerController temporalPlayer = _player.GetComponent<PlayerController>();
        
        GameObject particles = (GameObject)Instantiate(_explosionParticles, transform.position, Quaternion.identity);
        particles.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", _player.GetComponent<PlayerController>().PlayerColor);
        AudioManager.Instance.PlayAudio(AudioManager.AudioTracks.MineExplosion);
        BombDestroy(false);

    }
    public void BombDestroy(bool kill)
    {
        //PlayerController temporalPlayer = _player.GetComponent<PlayerController>();
        if (_player != null && _player.GetComponent<PlayerController>() != null)
        {
            _player.GetComponent<PlayerController>()._bombAmount++;

            if (kill)
            {
                _player.GetComponent<PlayerController>().NewKill();
            }
        }
        Destroy(gameObject);

    }
    void OnDestroy()
    {

    }
}
