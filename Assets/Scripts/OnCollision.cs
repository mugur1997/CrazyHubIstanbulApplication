using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
    public CharacterControllers characterController;
    [SerializeField] private Transform target = null;
    public CapsuleCollider col;
   
    public void Start()
    {
        col = GetComponent<CapsuleCollider>();
    }
    public void Update()
    {
        transform.position = target.transform.position;
        col.height = characterController.characterController.height;
        col.center = characterController.characterController.center;

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            return;
        }
        characterController.OnCharacterColliderHit(collision.collider);
    }
}
