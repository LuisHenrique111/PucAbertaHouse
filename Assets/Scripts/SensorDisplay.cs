using TMPro;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable ParameterHidesMember
// ReSharper disable once InconsistentNaming
namespace PUC.House
{
    public class SensorDisplay : MonoBehaviour
    {
        [Header("References")]
        public string key;
        public TMP_Text text;
        
        [Header("Settings")]
        public string format = "{0}";

        [Header("Events")]
        public UnityEvent<double> onValueChanged;

        public double Value
        {
            get => value;
            set => SetValue(value);
        }
        
        [SerializeField, HideInInspector]
        private double value;

        public void SetValue(double value)
        {
            this.value = value;
            
            onValueChanged.Invoke(value);
            
            if (text == null)
                return;
            
            text.text = string.Format(format, value/100);
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(SensorDisplay))]
    public class SensorDisplayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var sensorDisplay = (target as SensorDisplay)!;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Value", string.Format(sensorDisplay.format, sensorDisplay.Value));
            EditorGUI.EndDisabledGroup();
        }
    }
    #endif
}