using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decision : MonoBehaviour
{
    public CharacterControllers character;
    public AiController Ai;
    private CameraFollow cam;
    public GameObject red;
    public Canvas Completed;
    public Canvas[] clues;
    public GameObject green;
    public GameObject reds;
    public GameObject greens;
    public Transform door;
    public GameObject holder;
    public Transform Pos;
    public Canvas Fail;
    public Canvas Panel;
    public int indexs=-1;
    public AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<CameraFollow>();
        Ai = GameObject.FindWithTag("AI").GetComponent<AiController>();
        character = GameObject.FindWithTag("Player").GetComponent<CharacterControllers>();

        audio = GameObject.FindWithTag("Player").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Panel.gameObject == true){
            Clues();
        }
       
        if (door.transform.position.x > 0)
        {
            
            door.transform.position = Vector3.Lerp(door.transform.position, Pos.transform.position, Time.deltaTime);
        }
        cam.index = indexs;

        if (character.runSpeed > 1)
        {
            indexs = -1;
        }
    }

   public void Right()
    {
        if (indexs < 3)
        {
            indexs++;
        }
        else
        {
            indexs = 0;
        }
        
    }
    public void Left()
    {
        if (indexs > 0)
        {
            indexs--;
        }
        else
        {
            indexs = 3;
        }
    }

    public void Clues()
    {
        if (indexs == 0)
        {
            clues[1].gameObject.SetActive(true);
            clues[0].gameObject.SetActive(false);
            clues[2].gameObject.SetActive(false);
            clues[3].gameObject.SetActive(false);
        }
        else if(indexs == 1)
        {
            clues[1].gameObject.SetActive(false);
            clues[0].gameObject.SetActive(false);
            clues[2].gameObject.SetActive(true);
            clues[3].gameObject.SetActive(false);
        }

        else if (indexs == 2)
        {
            clues[1].gameObject.SetActive(false);
            clues[0].gameObject.SetActive(true);
            clues[2].gameObject.SetActive(false);
            clues[3].gameObject.SetActive(false);
        }
        else
        {
            clues[1].gameObject.SetActive(false);
            clues[0].gameObject.SetActive(false);
            clues[2].gameObject.SetActive(false);
            clues[3].gameObject.SetActive(true);
        }
    }
    public void Chasing()
    {
        if (indexs == 2)
        {
            Ai.runSpeed = 29.8f;
            character.runSpeed = 30;
            Panel.gameObject.SetActive(false);
            audio.volume = 1;

        }
        else
        {
            Panel.gameObject.SetActive(false);
            Fail.gameObject.SetActive(true);
        }
    }

    public void Fire()
    {
        if(character.RightFire == true)
        {
            character.PlayAnimation("FiringR");
            character.runSpeed = 10f;
            door.transform.position = Vector3.Lerp(door.transform.position,Pos.transform.position, Time.deltaTime);
            FindObjectOfType<AudioManager>().Play("Rifle");
            red.gameObject.SetActive(false);
            green.gameObject.SetActive(true);


        }
        if (character.LeftFire == true)
        {
            FindObjectOfType<AudioManager>().Play("Rifle");
            character.PlayAnimation("FiringL");
            character.runSpeed = 10f;
            holder.gameObject.SetActive(false);
            reds.gameObject.SetActive(false);
            greens.gameObject.SetActive(true);
        }
    }

 
}
