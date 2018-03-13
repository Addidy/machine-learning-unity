using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBrain : MonoBehaviour {

    int DNALength = 2;
    public float distanceRecord;
    public CDNA dna;
    public GameObject eyes;
    bool alive = true;
    bool seeWall = true;



    void OnCollisionEnter(Collision obj) {  //die when you fall off
        if (obj.gameObject.tag == "dead") {
            alive = false;
            distanceRecord = 0;            
        }
    }

    public void Init() {    //set up
        //initialise DNA decisions
        //0 forward
        //1 left
        //2 right
        //initialise DNA properties
        //0 turnrate        

        dna = new CDNA(DNALength, 3, new float[,] { {20, 160} });
        distanceRecord = 0;
        alive = true;
    }

    void Update() {                                         //Always be...
        if (!alive) return;                                     //if flagged as dead do nothing

        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 1, Color.red, 0.05f);     //show me where the bot is looking
        seeWall = false;
        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 1, out hit))     //if the bot sees something...
            if (hit.collider.gameObject.tag == "wall")                                          //if that something is the floor...
                seeWall = true;                                                                       //flag that we can see the ground (how anticlimactic)        

        // read DNA
        float turn = 0;
        float move = 0;
        if (seeWall) {                                                                        //...            
            if (dna.GetGene(0) == 0) { move = 1; }                                                      //perform actions based on the first gene
            else if (dna.GetGene(0) == 1) turn = -1*dna.GetPropertyValue(0);
            else if (dna.GetGene(0) == 2) turn = 1*dna.GetPropertyValue(0);
        } else {                                                                                //otherwise if we cannot see walkable ground...
            if (dna.GetGene(1) == 0) { move = 1; }                                                      //perform actions based on the second gene
            else if (dna.GetGene(1) == 1) turn = -1*dna.GetPropertyValue(0);
            else if (dna.GetGene(1) == 2) turn =  1*dna.GetPropertyValue(0);
        }

        this.transform.Translate(0, 0, move * Time.deltaTime * 3);
        this.transform.Rotate(0, turn, 0);

        float distanceFromStart = Vector3.Distance(FindObjectOfType<CPopulationManager>().transform.position, this.transform.position);
        distanceRecord = Math.Max(distanceFromStart, distanceRecord);//update how far the bot has gotten
    }
}