using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    private float _deltaTime = 0.0f;
    private GUIStyle _guiStyle;
    private Rect _rect;
    private int _width;
    private int _height;

    void Start()
    {
        _guiStyle = new GUIStyle();
        _width = Screen.width;
        _height = Screen.height;

        float rate = (float)_width / (float)_height;
        if (rate > 1f)
        {
            _rect = new Rect(_width * 0.82f, 0f, _width, _height * 2f / 100);
            _guiStyle.fontSize = _height * 2 / 50;
        }
        else
        {
            _rect = new Rect(_width * 0.675f, 0f, _width, _height * 2f / 100);
            _guiStyle.fontSize = _width * 2 / 50;
        }

        _guiStyle.alignment = TextAnchor.UpperLeft;
        _guiStyle.normal.textColor = Color.green;
    }

    void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        float msec = _deltaTime * 1000.0f;
        float fps = 1.0f / _deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(_rect, text, _guiStyle);
    }
}