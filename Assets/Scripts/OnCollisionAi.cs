using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionAi : MonoBehaviour
{
    public AiController characterControllerAi;
    [SerializeField] private Transform target = null;
    public CapsuleCollider col;
  
    public void Start()
    {
        col = GetComponent<CapsuleCollider>();
    }
    public void Update()
    {
        transform.position = target.transform.position;
        col.height = characterControllerAi.characterControllerAi.height;
        col.center = characterControllerAi.characterControllerAi.center;

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "AI")
        {
            return;
        }
        characterControllerAi.OnCharacterColliderHit(collision.collider);
    }
}
