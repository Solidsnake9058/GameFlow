using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor;

namespace UnityGame.App.Editor
{
    using UnityGame.Data;

    public class ReorderableListEx
    {
        private enum SelectMode
        {
            Default,
            Ctrl,
            Shift,
        }

        private ReorderableList _List;
        private IList _SortingObjs;
        private SerializedObject _SerializedObject;
        private SelectMode _SelectMode = 0;
        private int _SelectIndex = -1;
        private bool _IsSelectedForMove;
        private Rect? _ScrollRect;

        public ReorderableList ReorderableList { get { return _List; } }
        public bool IsValid { get { return _List != null; } }

        public void Init()
        {
            _List = null;
            _SortingObjs = null;
            _SerializedObject = null;
        }

        public void Setup(SerializedObject serializedObject, SerializedProperty elements, IList objs)
        {
            _List = new ReorderableList(serializedObject, elements);
            _SortingObjs = objs;
            _SerializedObject = serializedObject;
            _List.onAddCallback += _ => _List.serializedProperty.arraySize++;
            _List.onSelectCallback += list =>
            {
                if (_SortingObjs == null) return;
                var asset = _SortingObjs[list.index] as SortingObject;
                if (_SelectMode == SelectMode.Ctrl)
                    asset.IsSelectedForMove = !asset.IsSelectedForMove;
                else if (_SelectMode == SelectMode.Shift)
                {
                    if (_SelectIndex >= 0)
                    {
                        var len = Mathf.Abs(list.index - _SelectIndex) + 1;
                        var start = Mathf.Min(_SelectIndex, list.index);
                        Enumerable.Range(start, len).ToList().ForEach(i =>
                        {
                            var obj = _SortingObjs[i] as SortingObject;
                            obj.IsSelectedForMove = !obj.IsSelectedForMove;
                        });
                    }
                    else
                        asset.IsSelectedForMove = !asset.IsSelectedForMove;
                }
                else
                {
                    for (var i = 0; i < _SortingObjs.Count; i++)
                    {
                        var obj = _SortingObjs[i] as SortingObject;
                        if (obj.IsSelectedForMove)
                            obj.IsSelectedForMove = false;
                    }
                }
                _SelectIndex = list.index;
                _IsSelectedForMove = false;
                for (var i = 0; i < _SortingObjs.Count; i++)
                {
                    _IsSelectedForMove = (_SortingObjs[i] as SortingObject).IsSelectedForMove;
                    if (_IsSelectedForMove) break;
                }
            };
        }

        public void drawElementCallback(ElementCallbackDelegate param)
        {
            _List.drawElementCallback = (rect, i, isActive, isFocused) =>
            {
                if (i >= _SortingObjs.Count) return;
                if (_ScrollRect.HasValue && !_ScrollRect.Value.Overlaps(rect)) return;

                var item = _SortingObjs[i] as SortingObject;
                var defaultBgColor = GUI.backgroundColor;
                if (item.IsSelectedForMove)
                    GUI.backgroundColor = Color.green;

                // content
                param(ref rect, i);

                // move button
                var width = 70;
                rect.x += 20;
                rect.size = new Vector2(width, 17);
                if (_IsSelectedForMove)
                {
                    if (!item.IsSelectedForMove && GUI.Button(rect, "move"))
                    {
                        var ctrlList = new List<SortingObject>();
                        // search
                        for (var j = 0; j < _SortingObjs.Count; j++)
                            if ((_SortingObjs[j] as SortingObject).IsSelectedForMove)
                                ctrlList.Add(_SortingObjs[j] as SortingObject);
                        // remove
                        ctrlList.ForEach(x => _SortingObjs.Remove(x));
                        // insert
                        ctrlList.Reverse();
                        ctrlList.ForEach(x => _SortingObjs.Insert(_SortingObjs.IndexOf(item) + 1, x));
                    }
                    else if (item.IsSelectedForMove && GUI.Button(rect, "remove"))
                    {
                        var ctrlList = new List<SortingObject>();
                        // search
                        for (var j = 0; j < _SortingObjs.Count; j++)
                            if ((_SortingObjs[j] as SortingObject).IsSelectedForMove)
                                ctrlList.Add(_SortingObjs[j] as SortingObject);
                        // remove
                        ctrlList.ForEach(x => _SortingObjs.Remove(x));
                    }
                }
                rect.x += width;
                GUI.backgroundColor = defaultBgColor;
            };
        }

        public void Update()
        {
            if (UnityEngine.Event.current.control)
                _SelectMode = SelectMode.Ctrl;
            else if (UnityEngine.Event.current.shift)
                _SelectMode = SelectMode.Shift;
            else
                _SelectMode = SelectMode.Default;
        }

        public void Show()
        {
            _SerializedObject.Update();
            _List?.DoLayoutList();
            _SerializedObject.ApplyModifiedProperties();
        }

        public void SetScrollRect(float x, float y, float width, float height)
        {
            _ScrollRect = new Rect(x, y, width, height);
        }

        public delegate void ElementCallbackDelegate(ref Rect rect, int index);
        public delegate void AddCallbackDelegate(ReorderableList list);
    }
}
