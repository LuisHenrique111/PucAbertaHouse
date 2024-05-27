using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace PUC.House
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 5f;
        public float lookSpeed = 2f;
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private CharacterController _controller;
        private PlayerInput _playerInput;
        public XRIDefaultInputActions inputActions;
        private Vector3 _velocity;
        private const float _GRAVITY = -9.81f;
        public float interactionDistance = 3f;
        public Camera playerCamera;
        private Ray _ray;
        private RaycastHit _hit;
        public XRRayInteractor rayInteractor;
        
        public Vector2 _currentRotation;
        public float maxYAngle = 80f;

        private void OnEnable()
        {
            _controller = GetComponent<CharacterController>();
            _playerInput = new PlayerInput();
            //inputActions = new XRIDefaultInputActions();
            _playerInput.PlayerControl.Enable();
            _playerInput.PlayerControl.Interact.performed += Interact;
            //inputActions.XRILeftHandInteraction.Activate.performed += Interact;
            Cursor.lockState = CursorLockMode.Locked; 
            Cursor.visible = false; 
        }

        private void Update()
        {
            _ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            
            Debug.DrawRay(_ray.origin, _ray.direction * interactionDistance, Color.red);
            Move();
            Look();
            ApplyGravity();
        }

        private void Move()
        {
            //moveInput = inputActions.XRILeftHandLocomotion.Move.ReadValue<Vector2>();
            _moveInput = _playerInput.PlayerControl.Move.ReadValue<Vector2>();
            var transform1 = transform;
            var move = transform1.right * _moveInput.x + transform1.forward * _moveInput.y;
            move = Quaternion.Euler(new Vector3 (0, _currentRotation.x, 0)) * move;
            _controller.Move(move * (speed * Time.deltaTime));
        }
        
        private void Look()
        {
            //lookInput = inputActions.XRILeftHandLocomotion.Move.ReadValue<Vector2>();
            _lookInput = _playerInput.PlayerControl.Look.ReadValue<Vector2>();
            _currentRotation.x += _lookInput.x * lookSpeed;
            _currentRotation.y -= _lookInput.y * lookSpeed;
            _currentRotation.x = Mathf.Repeat(_currentRotation.x, 360);
            _currentRotation.y = Mathf.Clamp(_currentRotation.y, -maxYAngle, maxYAngle);
            Camera.main!.transform.rotation = Quaternion.Euler(_currentRotation.y,_currentRotation.x,0);
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            _velocity.y += _GRAVITY * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        void Interact(InputAction.CallbackContext context){

            if(context.performed){
                InteractWithObject();
            }
        }
        
        void InteractWithObject()
        {
            _ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_ray, out _hit, interactionDistance))
            {
                Debug.Log("Hit: " + _hit.collider.name);
                
                var @switch = _hit.collider.GetComponentInParent<Switch>();
                
                Debug.Log(@switch);
                
                if (@switch != null)
                {
                    @switch.Toggle();
                }
            }
        }
    }
}