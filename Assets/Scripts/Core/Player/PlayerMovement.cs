using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;
    [Header("Settings")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float turningRate = 30f;

    private Vector2 previousMovementInput;

    public override void OnNetworkSpawn(){
        if(!IsOwner) {return;}
        inputReader.MoveEvent += HandleMove;
    }
    public override void OnNetworkDespawn(){
        if(!IsOwner) {return;}
        inputReader.MoveEvent -= HandleMove;
    }
    // Update is called once per frame
    private void Update()
    {
        if(!IsOwner) {return;}
        float zRotation = previousMovementInput.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);
        rb.velocity = (Vector2)bodyTransform.up * movementSpeed * previousMovementInput.y;
    }

    private void HandleMove(Vector2 movementInput){
        previousMovementInput = movementInput;
    }
}
