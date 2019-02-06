using System;
using System.Collections.Generic;
using UnityEngine;

public class NNAgent : ICloneable
{

    #region Members

    public int[] layers;
    public bool bias { get; private set; }

    public float[][] neurons { get; private set; }
    public float[][][] weights { get; private set; }

    public float normalize;

    #endregion

    #region Initialize

    /// <summary>
    /// Initilizes and NN with random Weights
    /// </summary>
    /// <param name="layers">NN Layers</param>
    public NNAgent(int[] layers, bool bias = true)
    {
        // DeepCopy of Layers of this NN
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; ++i)
        {
            this.layers[i] = layers[i];
        }
        this.bias = bias;

        // Generate Matrix
        InitNeurons();
        InitWeights();
    }

    /// <summary>
    /// DeepCopy Constructor 
    /// </summary>
    /// <param name="agent">NN to DeepCopy</param>
    public NNAgent(NNAgent agent)
    {
        // DeepCopy of Layers of this NN
        layers = new int[agent.layers.Length];
        for (int i = 0; i < agent.layers.Length; i++)
        {
            layers[i] = agent.layers[i];
        }
        bias = agent.bias;

        // Generate Matrix
        InitNeurons();
        InitWeights();

        // Set Weights
        CopyWeights(agent.weights);
    }

    /// <summary>
    /// Generate Neutron Matrix
    /// </summary>
    private void InitNeurons()
    {
        // Neuron Init
        List<float[]> neuronsList = new List<float[]>();

        // Run through all Layers
        for (int i = 0; i < layers.Length; ++i)
        {
            // Add Layer to Neuron List
            neuronsList.Add(new float[layers[i]]);
        }

        // Convert List to Array
        neurons = neuronsList.ToArray();

    }

    /// <summary>
    /// Generate Weight Matrix
    /// </summary>
    private void InitWeights()
    {
        // Weight Init (will be converted to 3D Array)
        List<float[][]> weightsList = new List<float[][]>();

        // Itterate over all Neurons that have a Weight connection
        for (int i = 1; i < layers.Length; i++)
        {
            // Layer Weight List for this current Layer (will be converted to 2D Array)
            List<float[]> layerWeightsList = new List<float[]>();

            int neuronsInPreviousLayer = layers[i - 1];

            // Itterate over all Neurons in this current Layer
            for (int j = 0; j < layers[i]; j++)
            {
                // Neurons Weights
                float[] neuronWeights = new float[neuronsInPreviousLayer];

                // Itterate over all Neurons in the previous Layer and set the Weights randomly between 0.5f and -0.5
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    // Give random Weights to Neuron Weights
                    neuronWeights[k] = NNMath.RandomWeightValue();
                }

                // Add Neuron Weights of this current Layer to Layer Weights
                layerWeightsList.Add(neuronWeights);
            }

            // Add this Layers Weights converted into 2D Array into Weights List
            weightsList.Add(layerWeightsList.ToArray());
        }

        // Convert to 3D Array
        weights = weightsList.ToArray();
    }

    /// <summary>
    /// Copy Weights
    /// </summary>
    /// <param name="copyWeights">Weights to Copy</param>
    private void CopyWeights(float[][][] copyWeights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }

    #endregion

    #region Calculate

    /// <summary>
    /// Feed Forward (FF) this NN with a given input Array
    /// </summary>
    /// <param name="inputs">NN Inputs</param>
    /// <returns></returns>
    public float[] FeedForward(float[] inputs)
    {
        // Add inputs to the Neuron Matrix
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        // Itterate over all Neurons and compute FF values 
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < layers[i]; j++)
            {
                // Init 
                float value = bias ? 1.0f : 0.0f;

                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    // Sum off all Weights connections of this Neuron Weight values in their previous Layer
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }

                // Hyperbolic Tangent activation
                neurons[i][j] = NNMath.ActivateFunction(value);
            }
        }

        // Return output Layer
        return neurons[neurons.Length - 1];
    }

    #endregion

    #region Evalution

    public static NNAgent Mutate(NNAgent agent, float probability, float amount)
    {
        NNAgent mutate = new NNAgent(agent);
        for (int i = 1; i < mutate.layers.Length; i++)
        {
            for (int j = 0; j < mutate.layers[i]; j++)
            {
                for (int k = 0; k < mutate.layers[i - 1]; k++)
                {
                    if (UnityEngine.Random.Range(0, 1) < probability)
                    {
                        mutate.weights[i - 1][j][k] = UnityEngine.Random.Range(0f, 1f) * (amount * 2.0f) - amount;
                    }
                }
            }
        }
        return mutate;
    }

    public static NNAgent Crossover(NNAgent agentL, NNAgent agentR, float uniformRate)
    {
        NNAgent agent = new NNAgent(agentL);
        for (int i = 1; i < agent.layers.Length; i++)
        {
            for (int j = 0; j < agent.layers[i]; j++)
            {
                for (int k = 0; k < agent.neurons[i - 1].Length; k++)
                {
                    if (UnityEngine.Random.Range(0, 1) > uniformRate)
                    {
                        agent.weights[i - 1][j][k] = agentR.weights[i - 1][j][k];
                    }
                }
            }
        }
        return agent;
    }

    public void Mutator(float neuronProbability = 1.00f, float weightProbability = 0.08f)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                // Mutate Input Weight to Node 
                if (UnityEngine.Random.Range(0f, 1) < neuronProbability)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        float weight = weights[i][j][k];

                        // Mutate Weight value 
                        float randomNumber = UnityEngine.Random.Range(0f, 1f);

                        // Flip sign of Weight
                        if (randomNumber <= weightProbability * 0.25f) // 1
                        {
                            weight *= -1f;
                        }
                        // Random Weight between -0.5 and 0.5
                        else if (randomNumber <= weightProbability * 0.50f) // 2
                        {
                            weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                        }
                        // Randomly increase Weight by 0% to 100%
                        else if (randomNumber <= weightProbability * 0.75f)  // 3
                        {
                            float factor = UnityEngine.Random.Range(1f, 2f);
                            weight *= factor;
                        }
                        // Randomly decrease Weight by 0% to 100%
                        else if (randomNumber <= weightProbability * 1.0f) // 4
                        {
                            float factor = UnityEngine.Random.Range(0f, 1f);
                            weight *= factor;
                        }

                        // Set mutated Weight
                        weights[i][j][k] = weight;
                    }
                }
            }
        }
    }

    public object Clone()
    {
        return new NNAgent(this);
    }

    #endregion

}