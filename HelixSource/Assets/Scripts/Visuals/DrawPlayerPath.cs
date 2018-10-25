using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vectrosity;

public class DrawPlayerPath : MonoBehaviour {

    private const float TIME_BETWEEN_POINTS = 0.01f;

    [SerializeField]
    private float _lineWidth = 12.0f;

    [SerializeField]
    private int _maxPathPoints = 500;

    [SerializeField]
    private int _maxAttackPoints = 500;

    [SerializeField]
    private Transform _transformToTrack;

    private List<VectorLine> _pathLines;
    private List<VectorLine> _attackLines;

    private VectorLine _currentPathLine;
    private VectorLine _currentAttackPathLine;
    private bool _showingLine = false;
    private bool _continuousUpdate = true;
    private int pathIndex = 0;

    private bool _isAttacking = false;

    private float _timePassed = 0f;

    public Color _lineColor = Color.clear;

    public bool IsAttacking
    {
        get
        {
            return _isAttacking;
        }
        set
        {
            _isAttacking = value;
        }
    }

	// Use this for initialization
	void Awake () {
        _pathLines = new List<VectorLine>();
        _attackLines = new List<VectorLine>();

        _showingLine = true;
        //StartCoroutine("SamplePoints");
	}
	
//    private IEnumerator SamplePoints () {
//        if(_showingLine)
//        {
//            _pathLine.points3.Add (_transformToTrack.position);
//            if (++pathIndex == _maxPoints) {
//                _showingLine = false;
//            }
//            yield return new WaitForSeconds(0.05f);
//
//            if (_continuousUpdate) {
//                _pathLine.Draw();
//            }
//        }
//	}

    void Update()
    {
        if(_showingLine)
        {
            _timePassed += Time.deltaTime;

            if (_timePassed > TIME_BETWEEN_POINTS)
            {
                _timePassed = 0f;

                _currentPathLine.points3.Add(_transformToTrack.position);
                CheckAndRemovePathPoints();

                if (_isAttacking)
                {
                    if (_currentAttackPathLine == null)
                    {
                        SplitAttackLine();
                    }


                    AddAttackPoints();
                    //CheckAndRemoveAttackPoints();
                }


                if (_continuousUpdate)
                {
                    DrawPathLines();
                    DrawAttackLines();
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            ClearAttackLines();
        }
    }

    private void CheckAndRemovePathPoints()
    {
        int totalPathPoints = 0;

        foreach (VectorLine line in _pathLines)
        {
            totalPathPoints += line.points3.Count;
        }

        while(totalPathPoints > _maxPathPoints)
        {
            if(_pathLines[0].points3.Count > 0)
            {
                _pathLines[0].points3.RemoveAt(0);
                totalPathPoints--;
            }
            else if(_pathLines[0].points3.Count == 0)
            {
                //Debug.Log("Deleted line");
                _pathLines[0].Draw();
                VectorLine line = _pathLines[0];
                VectorLine.Destroy(ref line);
                _pathLines.RemoveAt(0);
            }
        }
    }

    private void AddAttackPoints()
    {
        int totalAttackPoints = 0;

        foreach (VectorLine line in _attackLines)
        {
            totalAttackPoints += line.points3.Count;
        }

        if (totalAttackPoints < _maxAttackPoints && _currentPathLine.points3.Count > 5)
        {
            _currentAttackPathLine.points3.Add (_currentPathLine.points3[_currentPathLine.points3.Count - 5]);
        }
    }

    private void CheckAndRemoveAttackPoints()
    {
        int totalAttackPoints = 0;

        foreach (VectorLine line in _attackLines)
        {
            totalAttackPoints += line.points3.Count;

        }

        while(totalAttackPoints > _maxAttackPoints)
        {
            if(_attackLines[0].points3.Count > 0)
            {
                _attackLines[0].points3.RemoveAt(0);
                totalAttackPoints--;
            }
            else if(_attackLines[0].points3.Count == 0)
            {
                _attackLines.RemoveAt(0);
            }
        }
    }

    public void SplitBothLines()
    {
        SplitPathLine();
        SplitAttackLine();
    }

    private void SplitPathLine()
    {
        _currentPathLine = new VectorLine("PlayerPath", new List<Vector3>(), _lineWidth, LineType.Continuous);
        _currentPathLine.color = _lineColor;

        _pathLines.Add(_currentPathLine);
    }

    private void SplitAttackLine()
    {
        if(_isAttacking)
        {
            _currentAttackPathLine = new VectorLine("AttackPath", new List<Vector3>(), _lineWidth * 5, LineType.Continuous);
            _currentAttackPathLine.color = _lineColor;

            _attackLines.Add(_currentAttackPathLine);
        }
    }

    private void DrawPathLines()
    {
        foreach (VectorLine line in _pathLines)
        {
            line.Draw();
            //line.collider = true;
        }
    }

    private void DrawAttackLines()
    {
        foreach (VectorLine line in _attackLines)
        {
            line.Draw();
            //line.collider = true;
        }
    }

    private void UpdateWidths()
    {
        /*int numberWidths = 0;
        foreach (VectorLine line in _pathLines)
        {
            numberWidths += line.points3.Count - 1;
            line.smoothWidth;
        }

        VectorLine currentLine = _pathLines[0];
        int currentWidth = 0;
        int currentWidthInLine = 0;
        while(currentWidth < numberWidths)
        {
            if(currentWidthInLine >
        }*/
    }

    public void StartShowingLine()
    {
        _showingLine = true;
        SplitPathLine();
    }

    public void ClearPathLines()
    {
        int numberPathLines = _pathLines.Count;
        for (int i = 0; i < numberPathLines; i++)
        {
            VectorLine line = _pathLines[0];
            VectorLine.Destroy(ref line);
            _pathLines.RemoveAt(0);
            _currentPathLine = null;
            _showingLine = false;
        }
    }

    public void ClearAttackLines()
    {
        int numberAttackLines = _attackLines.Count;
        for (int i = 0; i < numberAttackLines; i++)
        {
            VectorLine line = _attackLines[0];
            VectorLine.Destroy(ref line);
            _attackLines.RemoveAt(0);
            _currentAttackPathLine = null;
        }
    }

    public void SetColor(Color pColor)
    {
        _lineColor = pColor;

        foreach (VectorLine line in _pathLines)
        {
            line.color = _lineColor;
        }

        foreach (VectorLine line in _attackLines)
        {
            line.color = _lineColor;
        }
    }
}
