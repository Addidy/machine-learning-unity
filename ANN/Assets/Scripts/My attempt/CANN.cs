using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CANN {

    public int numInputs;
    public int numOutputs;
    public int numHidden;       //number of hidden layers
    public int numNPerHidden;   //number of neurons per hidden layer
    public double alpha;        //learn rate (scale how much impact error rate has across network)
    List<CLayer> layers = new List<CLayer>();

    public CANN(int nI, int nO, int nH, int nPH, double a) {
        numInputs = nI;
        numOutputs = nO;
        numHidden = nH;
        numNPerHidden = nPH;
        alpha = a;

        if (numHidden > 0) {                                //if number of hidden layers specified is > 0...
            layers.Add(new CLayer(numNPerHidden, numInputs));    //add the input layer
            for (int i = 0; i < numHidden - 1; i++)             //for the amount of hidden layers do...
                layers.Add(new CLayer(numNPerHidden, numNPerHidden));//add a hidden layer with specified parameters
            layers.Add(new CLayer(numOutputs, numNPerHidden));   //add the output layer
        } else                                             //otherwise...
            layers.Add(new CLayer(numOutputs, numInputs));       //just make a single layer with neurons matching the outputs
    }

    public List<double> Go(List<double> inputValues, List<double> desiredOutput) {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        if (inputValues.Count != numInputs) {
            Debug.Log("ERROR: Number of Inputs must be " + numInputs);
            return outputs;
        }

        inputs = new List<double>(inputValues);                             //inputs for the input layer
        for (int i = 0; i < numHidden + 1; i++) {                            //for every LAYER including input and output layers...
            if (i > 0)                                                          //if we not working with the input layer...
                inputs = new List<double>(outputs);                                 //make the inputs the outputs from the previous layer(we have no inputs for input layer)
            outputs.Clear();                                                    //clear the outputs so we can set this layers outputs

            for (int j = 0; j < layers[i].numNeurons; j++) {                     //for every NEURON in the LAYER...
                double N = 0;                                                       //declare N (will gather all the output values multiplied by input weights for this neurons activation function
                layers[i].neurons[j].inputs.Clear();                                //clear the neurons last inputs so we can update them

                for (int k = 0; k < layers[i].neurons[j].numInputs; k++) {           //for each INPUT in the NEURON...
                    layers[i].neurons[j].inputs.Add(inputs[k]);                         //add the last output
                    N += layers[i].neurons[j].weights[k] * inputs[k];                   //accumulatively add the weights times the inputs N
                }

                N -= layers[i].neurons[j].bias;                                     //accumulatively add the bias to N
                layers[i].neurons[j].output = ActivationFunction(N);                //get the ouput of this neuron based on N
                outputs.Add(layers[i].neurons[j].output);                           //add this neurons output to the output list, will be passed as inputs for next layer
            }//T.T
        }

        UpdateWeights(outputs, desiredOutput);

        return outputs;
    }

    void UpdateWeights(List<double> outputs, List<double> desiredOutput) {                                  //back propagate through neurons and update weights based on results and desired outputs
        double error;                                                                                       //error to later calculate
        for (int i = numHidden; i >= 0; i--) {                                                                  //for every LAYER (going backwards from the output to the input layers)...
            for (int j = 0; j < layers[i].numNeurons; j++) {                                                        //for every NEURON in each LAYER being looped through....................
                if (i == numHidden) {                                                                                   //if we are currently looping through the output LAYER...................
                    error = desiredOutput[j] - outputs[j];                                                                  //calculate the error with the output layers result (this is also part of the reason we have to go through layer loop backwards)
                    layers[i].neurons[j].errorGradient = outputs[j] * (1 - outputs[j]) * error;                             //determine how responsible each neuron is for the error
                    //errorGradient calculated with Delta Rule: en.wikipedia.org/wiki/Delta_rule
                } else {                                                                                                //otherwise if we are looping through a layer that is not the output layer...
                    layers[i].neurons[j].errorGradient = layers[i].neurons[j].output * (1 - layers[i].neurons[j].output);   //determine how responsible each neuron is for the error
                    double errorGradSum = 0;
                    for (int p = 0; p < layers[i + 1].numNeurons; p++)                                                     //for every neuron in the hidden layer after the hidden layer we are currently looping through... (O.o)
                        errorGradSum += layers[i + 1].neurons[p].errorGradient * layers[i + 1].neurons[p].weights[j];           //add together the sum of error gradients from the proceeding layer
                    layers[i].neurons[j].errorGradient *= errorGradSum;                                                     //multiply each neurons responsibility error gradient by the accumulative responsibility of the next layer... or something
                }
                for (int k = 0; k < layers[i].neurons[j].numInputs; k++) {                                              //for every INPUT in the current NEURON we are looping through...
                    if (i == numHidden) {                                                                                   //if we are on the output layer...
                        error = desiredOutput[j] - outputs[j];                                                                  //calculate the error with the output layers result (for some reason we have decided to do it twice)
                        layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * error;                      //update the weight (remember if the error was 0 the weight won't update as it would be correct) multiply it by alpha to get the learning rate
                    } else                                                                                                 //else if it is not the output layer
                        layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * layers[i].neurons[j].output;//update the weight based on.... if it "fired" or had an input as that means it contributed to the resulting value
                }
                layers[i].neurons[j].bias += alpha * -1 * layers[i].neurons[j].errorGradient;                           //update the bias for the NEURON based on the resulting error gradient (not sure why it is multiplied by -1?)
            }
        }
    }

    //for full list of activation functions
    //see en.wikipedia.org/wiki/Activation_function
    double ActivationFunction(double value) {           //activation function (penny has made it so we can swap between sigmoid (continous value 0 to 1) or binary step value (0 or 1)
        return Sigmoid(value);
    }

    double Step(double value) { //(aka binary step)
        if (value < 0) return 0;
        else return 1;
    }

    double Sigmoid(double value) {//(aka logistic softstep)
        double k = (double)System.Math.Exp(value);
        return k / (1.0f + k);
    }
}