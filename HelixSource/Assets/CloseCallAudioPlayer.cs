using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCallAudioPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other) {
        AudioManager.Instance.PlayAudio(AudioManager.AudioTracks.CloseCall);
    }
}
