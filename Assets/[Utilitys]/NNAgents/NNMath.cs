using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNMath
{
    /// <summary>
    /// Generate random weight value
    /// </summary>
    /// <returns>The calculated output.</returns>
    public static float RandomWeightValue()
    {
        return UnityEngine.Random.Range(-0.5f, 0.5f);
    }

    /// <summary>
    /// Neuron activation function
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <returns>The calculated output.</returns>
    public static float ActivateFunction(float value)
    {
        return TanHFunction(value);
    }

    #region Sub

    /// <summary>
    /// The standard sigmoid function.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <returns>The calculated output.</returns>
    public static float SigmoidFunction(float value)
    {
        return value > 10f ? 1.0f : value < -10 ? 0.0f : 1.0f / (1.0f + Mathf.Exp(-value));
    }

    /// <summary>
    /// The standard TanH function.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <returns>The calculated output.</returns>
    public static float TanHFunction(float value)
    {
        return value > 10f ? 1.0f : value < -10 ? 0.0f : (float)Math.Tanh(value);
    }

    /// <summary>
    /// The SoftSign function as proposed by Xavier Glorot and Yoshua Bengio (2010): 
    /// "Understanding the difficulty of training deep feedforward neural networks".
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <returns>The calculated output.</returns>
    public static float SoftSignFunction(float value)
    {
        return value / (1 + Mathf.Abs(value));
    }

    #endregion

}
