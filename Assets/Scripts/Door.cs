using UnityEngine;

// ReSharper disable ParameterHidesMember
// ReSharper disable once InconsistentNaming
namespace PUC.House
{
    public class Door : MonoBehaviour
    {
        public Vector3 axis = new(0, 1, 0);
        public float closed;
        public float opened = 90f;
        public float angularSpeed = 90f;
        
        public bool IsOpen
        {
            get => isOpen;
            set => isOpen = value;
        }
        
        [SerializeField, HideInInspector]
        private bool isOpen;
        
        public void Update()
        {
            var dt = angularSpeed * Time.deltaTime;
            transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, isOpen ? axis * opened : axis * closed, dt, dt);
        }
    }
}