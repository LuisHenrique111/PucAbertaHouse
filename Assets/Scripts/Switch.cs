using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable once InconsistentNaming
namespace PUC.House
{
    public class Switch : MonoBehaviour
    {
        [Header("References")]
        public HouseController controller;
        public string key;
        
        [Header("Events")]
        public UnityEvent onSwitchOn;
        public UnityEvent onSwitchOff;

        public bool IsOn
        {
            get => isOn;
            set => SetValue(value);
        }
        
        [SerializeField, HideInInspector]
        private bool isOn;
        
        public void SetValue(bool state, bool updateOnDatabase = true)
        {
            if (!updateOnDatabase)
            {
                isOn = state;
                Debug.Log($"Switch {key} is now {(isOn ? "on" : "off")}");
                    
                if (isOn)
                {
                    onSwitchOn.Invoke();
                }
                else
                {
                    onSwitchOff.Invoke();
                }
                return;
            }
            
            controller.SetValue(key, state, (_, _) => {});
        }

        public void Toggle()
        {
            SetValue(!isOn);
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(Switch))]
    public class SwitchEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var @switch = (target as Switch)!;
            
            var newValue = EditorGUILayout.Toggle("Is On", @switch.IsOn);
            
            if (newValue != @switch.IsOn)
            {
                @switch.IsOn = newValue;
            }
        }
    }
    #endif
}