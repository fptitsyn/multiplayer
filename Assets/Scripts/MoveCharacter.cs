using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class MoveCharacter : NetworkBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintMultiplier;
    [SerializeField] private float jumpForce;
    [SerializeField] private float distanceToGround;

    private InputAction _walkAction;
    private InputAction _sprintAction;
    private InputAction _jumpAction;

    private const float Gravity = -9.81f;

    private CharacterController _controller;
    private bool _grounded;
    private Vector3 _velocity;
    private float _moveSpeed;

    private void Start()
    {
        _walkAction = InputSystem.actions.FindAction("Move");
        _sprintAction = InputSystem.actions.FindAction("Sprint");
        _jumpAction = InputSystem.actions.FindAction("Jump");
    
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Walk();
    }

    private void Walk()
    {
        if (!IsOwner) return;
        
        Vector2 moveValue = _walkAction.ReadValue<Vector2>();
        if (_sprintAction.IsPressed() && moveValue is { y: > 0, x: 0 })
        {
            _moveSpeed = sprintMultiplier * walkSpeed;
        }
        else
        {
            _moveSpeed = walkSpeed;
        }
    
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
    
        Vector3 move = transform.right * moveValue.x + transform.forward * moveValue.y;
        
        _controller.Move(move * (_moveSpeed * Time.deltaTime));
        
        _grounded = Physics.Raycast(transform.position, Vector3.down, distanceToGround);
        
        if (_jumpAction.IsPressed() && _grounded)
        {
            _grounded = false;
            _velocity.y = jumpForce;
        }
        
        _velocity.y += Gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}
