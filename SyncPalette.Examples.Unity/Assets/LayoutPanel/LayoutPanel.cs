﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using zUI;
    // [RequireComponent(typeof(LayoutElement))]
   
    [SelectionBase]
    public class LayoutPanel : MonoBehaviour
    {
        public DrawInspectorBg draw;
        [HideInInspector] public bool hasSiblingTop;
        [HideInInspector] public bool hasSiblingBottom;
        [HideInInspector] public bool isAlignedBottom;
        public static string spacerName { get { return "---"; } }
        public Text labelText;
        public LayoutElement resizableElement;
        public bool freeMode;
        public string panelName
        {
            get { return _panelName; }
            set
            {
                if (labelText == null)
                {
                    var top = GetComponentInChildren<LayoutTopControl>();
                    if (top != null)
                    {
                        labelText = top.GetComponentInChildren<Text>();
                        labelText.SetText(value);
                        if (labelText == null) Debug.Log("no label", gameObject);
                    }
                    else Debug.Log("no top control?", gameObject);
                }
                if (labelText != null)

                    labelText.SetText(value);
                _panelName = value;
                name = value;
            }
        }
        [SerializeField] string _panelName;
        public void PlaceDropTarget(Transform target, int newSiblingIndex = -1)
        {
            transform.SetParent(target);
            if (newSiblingIndex != -1)
                transform.SetSiblingIndex(newSiblingIndex);
            else transform.SetAsLastSibling();
        }
        void Start()
        {   
           
            OnValidate();
        }
        public static int borderSpacing
        {
            get { return __borderSpacing; }
            set
            {
                __borderSpacing = value;
                if (onBorderSizeChange != null) onBorderSizeChange();
            }
        }



#if UNITY_ANDROID && !UNITY_EDITOR
        static int __borderSpacing = 2;
#else
        static int __borderSpacing = 1;
#endif

        public static int verticalSpacing
        {
            get { return _verticalSpacing; }
            set
            {
                _verticalSpacing = value;
                if (onBorderSizeChange != null) onBorderSizeChange();
            }
        }
        static int _verticalSpacing = 1;
        public static int borderSize
        {
            get { return _borderSize; }
            set
            {
                _borderSize = value;
                if (onBorderSizeChange != null) onBorderSizeChange();
            }
        }
        static int _topHeight = 25;
        //static int _topHeight = 9;
        public static int topHeight { get { return _topHeight; } set { _topHeight = value; if (onBorderSizeChange != null) onBorderSizeChange.Invoke(); } }

#if UNITY_ANDROID && !UNITY_EDITOR
        static int _borderSize = 4;
#else
        static int _borderSize = 4;
#endif
        public static System.Action
        onBorderSizeChange;
        void OnValidate()
        {
     //      gameObject.AddOrGetComponent<PanelSaverHelper>();
            if (string.IsNullOrEmpty(_panelName)) _panelName = name;
            panelName = _panelName;
            if (resizableElement == null)
            {
                var les = this.GetComponentsInDirectChildren<LayoutElement>();
                for (int i = 0; i < les.Length; i++)
                {
                    if (!les[i].ignoreLayout && les[i].flexibleHeight > 0)
                        resizableElement = les[i];
                }
            }
        }
        void SetGroupPadding()
        {
            var group = gameObject.AddOrGetComponent<VerticalLayoutGroup>();
            if (group != null)
            {
                var padding = group.padding;
                if (padding.top < LayoutPanel.topHeight)
                {
                    padding.top = LayoutPanel.topHeight;
                    group.padding = padding;
                }
            }

        }
    
        public Transform GetTargetTransformForSide(LayoutBorderDragger.Side side)
        {
            if (transform.parent == null) return transform;
            return transform.parent;
        }
        public LayoutElement GetTargetElementForSide(LayoutBorderDragger.Side side)
        {
            // if (side == LayoutBorderDragger.Side.Left || side == LayoutBorderDragger.Side.Right)
            //     return transform.parent.GetComponent<LayoutElement>();
            // else
            return resizableElement;
        }

        void Reset()
        {
            string _panelName = name;
        }

        void OnTransformParentChanged()
        {
            var c = transform.GetComponentInParent<LayoutColumn>();
            if (c != null)
                c.NotifyOfChange(this);
        }
    }