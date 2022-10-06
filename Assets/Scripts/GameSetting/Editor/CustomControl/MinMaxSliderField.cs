using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System.Collections.Generic;

namespace UnityGame.Data
{
    public class MinMaxSliderField : BaseField<Vector2>
    {
        private const string UXMLPATH = "Assets/Scripts/GameSetting/Editor/CustomControl/MinMaxSliderField.uxml";

        private MinMaxSlider m_MinMaxSliderBar;
        public MinMaxSlider minMaxSliderBar => m_MinMaxSliderBar;
        private FloatField m_MinInputTextField;
        public FloatField minInputTextField => m_MinInputTextField;
        private FloatField m_MaxInputTextField;
        public FloatField maxInputTextField => m_MaxInputTextField;

        private string _bindingPath;
        public string customBindingPath
        {
            get { return _bindingPath; }
            set
            {
                _bindingPath = value;
                minMaxSliderBar.bindingPath = _bindingPath;
                minInputTextField.bindingPath = $"{_bindingPath}.x";
                maxInputTextField.bindingPath = $"{_bindingPath}.y";
            }
        }

        public float minValue
        {
            get
            {
                return minMaxSliderBar.minValue;
            }
            set
            {
                minMaxSliderBar.minValue = value;
                minInputTextField.value = value;
            }
        }

        public float maxValue
        {
            get
            {
                return minMaxSliderBar.maxValue;
            }
            set
            {
                minMaxSliderBar.maxValue = value;
                maxInputTextField.value = value;
            }
        }

        public float lowLimit
        {
            get
            {
                return minMaxSliderBar.lowLimit;
            }
            set
            {
                minMaxSliderBar.lowLimit = value;
            }
        }

        public float highLimit
        {
            get
            {
                return minMaxSliderBar.highLimit;
            }
            set
            {
                minMaxSliderBar.highLimit = value;
            }
        }

        private VisualElement visualInput;

        public static readonly string ussCustomClassName = "unity-min-max-slider-custom";

        public static readonly string labelCustomUssClassName = ussCustomClassName + "__label";

        public static readonly string inputCustomUssClassName = ussCustomClassName + "__input";

        public static readonly string dragCustomContainerUssClassName = ussCustomClassName + "__drag-container";

        public static readonly string textCustomFieldClassName = ussCustomClassName + "__text-field";

        public MinMaxSliderField() : this(null)
        {
        }


        public MinMaxSliderField(float minValue, float maxValue, float minLimit, float maxLimit)
            : this(null, minValue, maxValue, minLimit, maxLimit)
        {
        }

        public MinMaxSliderField(string label, float minValue = 0f, float maxValue = 10f, float minLimit = float.MinValue, float maxLimit = float.MaxValue)
            : base(label, (VisualElement)null)
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXMLPATH);
            visualTree.CloneTree(this);


            //取得控制項
            m_MinMaxSliderBar = this.Q<MinMaxSlider>("min-max-slider");
            m_MinInputTextField = this.Q<FloatField>("min-value");
            m_MaxInputTextField = this.Q<FloatField>("max-value");

            m_MinMaxSliderBar.AddToClassList(dragCustomContainerUssClassName);
            m_MinInputTextField.AddToClassList(textCustomFieldClassName);
            m_MaxInputTextField.AddToClassList(textCustomFieldClassName);

            m_MinMaxSliderBar.RegisterValueChangedCallback(OnMinMaxSliderValueChange);
            m_MinInputTextField.RegisterValueChangedCallback(OnMinTextFieldValueChange);
            m_MaxInputTextField.RegisterValueChangedCallback(OnMaxTextFieldValueChange);
            //m_MinInputTextField.RegisterCallback<FocusOutEvent>(OnTextFieldFocusOut);

            base.labelElement.AddToClassList(labelCustomUssClassName);
            //取得BaseField產生的VisualElement
            var elements = this.Children();
            visualInput = elements.First();

            //將控制項設定到VisualElement底下
            visualInput.Add(m_MinMaxSliderBar);
            visualInput.AddToClassList(inputCustomUssClassName);
            VisualElement inputCustom = new VisualElement();
            visualInput.Add(inputCustom);
            inputCustom.Add(m_MinInputTextField);
            inputCustom.Add(m_MaxInputTextField);
        }

        public MinMaxSliderField(string label, string bindingPath, SerializedObject serializedObject, float minLimit = float.MinValue, float maxLimit = float.MaxValue)
            : this(null, 0, 0, minLimit, maxLimit)
        {
            
            this.label = label;
            this.lowLimit = minLimit;
            this.highLimit = maxLimit;
            this.customBindingPath = bindingPath;
            Bind(serializedObject);
        }

        private float GetClampedValue(float newValue, float low, float high)
        {
            float val = low;
            float val2 = high;
            if (val.CompareTo(val2) > 0)
            {
                float val3 = val;
                val = val2;
                val2 = val3;
            }
            return Mathf.Clamp(newValue, val, val2);
        }

        public void Bind(SerializedObject serializedObject)
        {
            m_MinMaxSliderBar.Bind(serializedObject);
            m_MinInputTextField.Bind(serializedObject);
            m_MaxInputTextField.Bind(serializedObject);
        }

        private void OnMinMaxSliderValueChange(ChangeEvent<Vector2> evt)
        {
            Vector2 clampedValue = evt.newValue;
            if (!EqualityComparer<float>.Default.Equals(clampedValue.x, minInputTextField.value))
            {
                minValue = clampedValue.x;
                evt.StopPropagation();
            }
            else if (!EqualityComparer<float>.Default.Equals(clampedValue.y, maxInputTextField.value))
            {
                maxValue = clampedValue.y;
                evt.StopPropagation();
            }
        }

        private void OnMinTextFieldValueChange(ChangeEvent<float> evt)
        {
            float clampedValue = GetClampedValue(evt.newValue, lowLimit, maxValue);
            if (!EqualityComparer<float>.Default.Equals(clampedValue, minValue))
            {
                minValue = clampedValue;
                evt.StopPropagation();
            }
        }

        private void OnMaxTextFieldValueChange(ChangeEvent<float> evt)
        {
            float clampedValue = GetClampedValue(evt.newValue, minValue, highLimit);
            if (!EqualityComparer<float>.Default.Equals(clampedValue, maxValue))
            {
                maxValue = clampedValue;
                evt.StopPropagation();
            }
        }

        public class Factory : UxmlFactory<MinMaxSliderField, Traits> { }

        public class Traits : UxmlTraits
        {
            private UxmlFloatAttributeDescription m_MinValue = new UxmlFloatAttributeDescription
            {
                name = "min-value",
                defaultValue = 0f
            };

            private UxmlFloatAttributeDescription m_MaxValue = new UxmlFloatAttributeDescription
            {
                name = "max-value",
                defaultValue = 10f
            };

            private UxmlFloatAttributeDescription m_LowLimit = new UxmlFloatAttributeDescription
            {
                name = "low-limit",
                defaultValue = -10
            };

            private UxmlFloatAttributeDescription m_HighLimit = new UxmlFloatAttributeDescription
            {
                name = "high-limit",
                defaultValue = 40
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ((BaseField<Vector2>)ve).label = "Min/Max Slider";
                ve.pickingMode = PickingMode.Ignore;
                MinMaxSliderField minMaxSlider = (MinMaxSliderField)ve;
                minMaxSlider.minValue = m_MinValue.GetValueFromBag(bag, cc);
                minMaxSlider.maxValue = m_MaxValue.GetValueFromBag(bag, cc);
                minMaxSlider.lowLimit = m_LowLimit.GetValueFromBag(bag, cc);
                minMaxSlider.highLimit = m_HighLimit.GetValueFromBag(bag, cc);
            }
        }
    }
}