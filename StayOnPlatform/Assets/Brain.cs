using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour {

    int DNALength = 2;
    public float timeAlive;
    public float timeWalking;
    public DNA dna;
    public GameObject eyes;
    bool alive = true;
    bool seeGround = true;

    public GameObject ethanPrefab;
    GameObject ethan;

    void OnDestroy() {
        Destroy(ethan);    
    }

    void OnCollisionEnter(Collision obj) {  //die when you fall off
        if(obj.gameObject.tag == "dead") {
            alive = false;
            timeAlive = 0;
            timeWalking = 0;
        }
    }

    public void Init() {    //set up
        //initialise DNA
        //0 forward
        //1 left
        //2 right
        dna = new DNA(DNALength, 3);
        timeAlive = 0;
        alive = true;
        ethan = Instantiate(ethanPrefab, this.transform.position, this.transform.rotation);
        ethan.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().target = this.transform;
    }

    void Update() {                                         //Always be...
        if (!alive) return;                                     //if flagged as dead do nothing

        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 10, Color.red, 0.05f);     //show me where the bot is looking
        seeGround = false;
        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out hit)) {   //if the bot sees something...
            if(hit.collider.gameObject.tag == "platform") {                                         //if that something is the floor...
                seeGround = true;                                                                       //flag that we can see the ground (how anticlimactic)
            }
        }
        timeAlive = PopulationManager.elapsed;                                                  //update how long the bot has been alive

        // read DNA
        float turn = 0;
        float move = 0;
        if (seeGround) {                                                                        //if we saw the ground
            //make 'move' relative to character and always move forward
            if (dna.GetGene(0) == 0) {move = 1; timeWalking += 1;}                                                      //perform actions based on the first gene
            else if (dna.GetGene(0) == 1) turn = -90;
            else if (dna.GetGene(0) == 2) turn = 90;
        } else {                                                                                //otherwise if we cannot see walkable ground...
            if (dna.GetGene(1) == 0) {move = 1; timeWalking += 1;}                                                      //perform actions based on the second gene
            else if (dna.GetGene(1) == 1) turn = -90;
            else if (dna.GetGene(1) == 2) turn = 90;
        }

        this.transform.Translate(0, 0, move * 0.1f);
        this.transform.Rotate(0, turn, 0);
    }
}
