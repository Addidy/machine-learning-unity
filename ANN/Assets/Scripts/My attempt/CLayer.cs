using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLayer {

    public int numNeurons;
    public List<CNeruon> neurons = new List<CNeruon>();

    public CLayer(int nNeurons, int numNeuronsInputs) {
        numNeurons = nNeurons;
        for(int i = 0; i < nNeurons; i++) 
            neurons.Add(new CNeruon(numNeuronsInputs));
    }
}
