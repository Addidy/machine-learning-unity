using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CDNA {
    public struct GeneticProperty {
        public float value;
        public float minValue;
        public float maxValue;

        public GeneticProperty(float min, float max) {
            minValue = min;
            maxValue = max;
            value = Random.Range(min, max);
        }

        public float GetValue() {
            return value;
        }
    }

    List<int> genes = new List<int>();
    List<GeneticProperty> geneticProperties = new List<GeneticProperty>();

    int dnaLengthDecision = 0;
    int maxValues = 0;

    public CDNA(int l, int v, float[,] gPropertyLimits) {
        dnaLengthDecision = l;
        maxValues = v;
        SetRandom();

        //for(int i = 0; i < gPropertyLimits.Length; i++) 
        //    geneticProperties.Add(new GeneticProperty(gPropertyLimits[i, 0],gPropertyLimits[i, 1]));
        geneticProperties.Add(new GeneticProperty(gPropertyLimits[0, 0],gPropertyLimits[0, 1]));
    }

    private void SetRandom() {
        genes.Clear();
        for(int i = 0; i < dnaLengthDecision; i++) 
            genes.Add(Random.Range(0, maxValues));
    }

    public int GetGene(int pos) { return genes[pos];  }
    public void SetInt(int pos, int val) { genes[pos] = val; }

    public float GetPropertyValue(int pos) { return geneticProperties[pos].GetValue(); }    

    public void Combine(CDNA d1, CDNA d2) {
        for(int i = 0; i < dnaLengthDecision; i++) 
            genes[i] = (i < dnaLengthDecision / 2.0) ? d1.GetGene(i) : d2.GetGene(i);

        for (int i = 0; i < geneticProperties.Count; i++) 
            geneticProperties[i] = (i < geneticProperties.Count / 2.0) ? d1.geneticProperties[i] : d2.geneticProperties[i];
    }

    public void Mutate() {
        genes[Random.Range(0, dnaLengthDecision)] = Random.Range(0, maxValues);

        int randomGeneticPropIndex = Random.Range(0, geneticProperties.Count);
        geneticProperties[randomGeneticPropIndex] = new GeneticProperty(geneticProperties[randomGeneticPropIndex].minValue, geneticProperties[randomGeneticPropIndex].maxValue);
    }
}