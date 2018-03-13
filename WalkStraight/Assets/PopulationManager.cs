using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager : MonoBehaviour {

    public GameObject botPrefab;                            //subject to spawn
    public int populationSize = 50;                         //amount to spawn per generation
    List<GameObject> population = new List<GameObject>();   //object pool for subjects
    public static float elapsed = 0;                        //timer
    public float trialTime = 5;                             //time for each generation
    int generation = 1;                                     //generation tracker

    GUIStyle guiStyle = new GUIStyle();
    void OnGUI()                        //tell me relevant information
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
        GUI.EndGroup();
    }

    void Start()                                                                        // at the start of the game...
    {
        for(int i = 0; i < populationSize; i++)                                             // do <populationSize> times...
        {
            Vector3 startingPos = new Vector3(  this.transform.position.x + Random.Range(-2, 2),
                                                this.transform.position.y,
                                                this.transform.position.z + Random.Range(-2, 2));// Assign a starting vector within a square boundary where ever this game object is

            GameObject b = Instantiate(botPrefab, startingPos, this.transform.rotation);        // spawn the test subject
            b.GetComponent<Brain>().Init();                                                     // boot up test subject's brain
            population.Add(b);                                                                  // add test subject to object pool
        }
    }

    GameObject Breed(GameObject parent1, GameObject parent2)//
    {
        Vector3 startingPos = new Vector3(  this.transform.position.x + Random.Range(-2, 2),
                                            this.transform.position.y,
                                            this.transform.position.z + Random.Range(-2, 2));   //random starting position within bounds to assign
        GameObject offspring = Instantiate(botPrefab, startingPos, this.transform.rotation);    //spawn test subject
        Brain b = offspring.GetComponent<Brain>();                                              //get brain handle
        if (Random.Range(0, 100) == 1)  //roll a "100 sided dice" and if we get "1" mutate the subject...
        {
            b.Init();                       //test subjects genes will be randomised by this
            b.dna.Mutate();                 //randomise value of a single gene (isn't this kind of redundant?)
        }
        else                            //else if we roll any other number
        {       
            b.Init();               
            b.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);    //use parents dna
        }
        return offspring;
    }

    void BreedNewPopulation()                                                   // ...
    {
        List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<Brain>().timeAlive + o.GetComponent<Brain>().distanceTravelled).ToList();  //create a list based on the strength of each test subjects performance

        population.Clear();                                                         //clear the current object pool
        
        for (int i = (int)(sortedList.Count / 2.0f); i < sortedList.Count - 1; i++) //breed upper half of sorted list
        {           
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));                    //
        }


        
        for(int i = 0; i < sortedList.Count; i++)                                   //destroy all parents and previous population
        {
            Destroy(sortedList[i]); 
        }
        generation++;                                                               //increment the generation count
    }

    void Update()               //always be...
    {
        elapsed += Time.deltaTime;  //tracking how much time has passed
        if(elapsed >= trialTime)    //if the time passed has passed the amount assigned for each trial...
        {
            BreedNewPopulation();       //spawn a new test generation based off the current one.
            elapsed = 0;                //reset the timer
        }
    }
}
