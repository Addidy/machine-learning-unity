using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CNeruon {

    public int numInputs;           //how many inputs
    public double bias;             //
    public double output;
    public double errorGradient;    //
    public List<double> weights = new List<double>();
    public List<double> inputs = new List<double>();

    public CNeruon(int nInputs) {
        bias = Random.Range(-1.0f, 1.0f);
        numInputs = nInputs;
        for (int i = 0; i < nInputs; i++)
            weights.Add(Random.Range(-1.0f, 1.0f));
    }
}