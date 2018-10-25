using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeColor : MonoBehaviour {

    //private UnityEngine.

    private Color _colourStart;
    private Color _colourEnd;
    private float lerpProgress = 0;
    private float rate = 1;

    [SerializeField]
    private UnityEngine.UI.Image _image;


	// Use this for initialization
	void Start () {
        _colourStart = new Color(Random.value, Random.value, Random.value);
        _colourEnd = new Color(Random.value, Random.value, Random.value);

        _image.color = _colourStart;
	}
	
	// Update is called once per frame
	void Update () {
        // Blend towards the current target colour
        lerpProgress += Time.deltaTime*rate;
        _image.color = Color.Lerp (_colourStart, _colourEnd, lerpProgress);

        // If we've got to the current target colour, choose a new one
        if(lerpProgress >= 1f) {
            lerpProgress = 0f;
            _colourStart = _image.color;
            _colourEnd = new Color(Random.value, Random.value, Random.value);
        }
	}
}
