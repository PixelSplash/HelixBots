using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vectrosity;

public class DrawCenterLine : MonoBehaviour {

    [SerializeField]
    private float _lineWidth = 2.0f;

    [SerializeField]
    private float _lineTextureScale = 4.0f;

    [SerializeField]
    private Texture _lineTexture;

    private VectorLine _centerLine;
    private Color _colourStart;
    private Color _colourEnd;
    private float lerpProgress = 0;
    private float rate = 1;
    public Color _lineColor = Color.clear;
    // Use this for initialization
    void Start () {
        List<Vector2> linePoints = new List<Vector2>();
        linePoints.Add (new Vector2(0f, Screen.height/2f));                // ...one on the left side of the screen somewhere
        linePoints.Add (new Vector2(Screen.width-1f, Screen.height/2f));   // ...and one on the right

        // Make a VectorLine object using the above points, with a width of 2 pixels
        _centerLine = new VectorLine("Line", linePoints, _lineTexture,  _lineWidth, LineType.Continuous);
        _centerLine.textureScale = _lineTextureScale;

        _colourStart = new Color(Random.value, Random.value, Random.value);
        _colourEnd = new Color(Random.value, Random.value, Random.value);

        _centerLine.color = _colourStart;
	}

    void Update ()
    {
        // Draw the line
        _centerLine.Draw();
        _centerLine.textureOffset = -Time.time * 2.0f % 1f;

        // Blend towards the current target colour
        lerpProgress += Time.deltaTime*rate;
        _centerLine.color  = Color.Lerp (_colourStart, _colourEnd, lerpProgress);

        // If we've got to the current target colour, choose a new one
        if(lerpProgress >= 1f) {
            lerpProgress = 0f;
            _colourStart = _centerLine.color;
            _lineColor = new Color(Random.value, Random.value, Random.value);
            _colourEnd = _lineColor;
        }
    }

}
