using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA {

    List<int> genes = new List<int>();              //list of genes
    int dnaLength = 0;                              //length of dna
    int maxValues = 0;                              //maximum values

    public DNA(int l, int v)                        //initialise with length and maxvalues parameters
    {
        dnaLength = l;
        maxValues = v;
        SetRandom();
    }

    public void SetRandom()                         //reset genes with random values from 0 to "maxValues"
    {
        genes.Clear();
        for(int i = 0; i < dnaLength; i++)
        {
            genes.Add(Random.Range(0, maxValues));
        }
    }

    public void SetInt(int pos, int value)          //set a specific gene to a specified value
    {
        genes[pos] = value;
    }

    public void Combine(DNA d1, DNA d2)             //set this dna pool to be half each params(parents) genes
    {
        for(int i = 0; i < dnaLength; i++)              //for every gene...
        {
            if(i < dnaLength / 2.0)                         //if the gene position is less than half way...
            {
                int c = d1.genes[i];                            
                genes[i] = c;                                   //assign genes from first parent
            }
            else                                            //else if the position is greater than halfway...
            {
                int c = d2.genes[i];
                genes[i] = c;                                   //assign genes from second parent
            }
        }
    }

    public void Mutate()                            //Randomise the value of a single gene
    {
        genes[Random.Range(0, dnaLength)] = Random.Range(0, maxValues);
    }

    public int GetGene(int pos)                     //Get a specific gene value from this DNA
    {
        return genes[pos];
    }
}
