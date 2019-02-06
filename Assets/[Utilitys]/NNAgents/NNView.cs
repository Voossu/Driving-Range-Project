using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extend;
using Vehicles.Car;

public class NNView : MonoBehaviour
{

    #region Members

    [SerializeField] private RectTransform m_UIViewPane;
    [SerializeField] private Color NegColor = Color.red;
    [SerializeField] private Color PosColor = Color.green;


    private NNAgent m_Agent;
    public NNAgent Agent { set => SetAgent(value); get => m_Agent; }


    [Header("Prefabs")]
    [SerializeField] private GameObject m_LayerPrefab;
    [SerializeField] private GameObject m_NeuronPrefab;
    [SerializeField] private GameObject m_WeightPrefab;


    private GameObject[] Layers = new GameObject[0];
    private GameObject[][] Neurons = new GameObject[0][];
    private GameObject[][][] Weights = new GameObject[0][][];

    #endregion

    private void Awake()
    {
        CarNeuralControl cnc = GameObject.FindWithTag("NPC")?.GetComponent<CarNeuralControl>();
        SetAgent(cnc?.agent);
    }


    #region Initialize

    Coroutine view;

    private void SetAgent(NNAgent agent)
    {
        if (view != null) StopCoroutine(view);
        Clear();
        m_Agent = agent;
        if (Agent != null)
        {
            m_UIViewPane.gameObject.SetActive(true);
            view = StartCoroutine(InitView());
        }
        else {
            m_UIViewPane.gameObject.SetActive(false);
        }
    }

    IEnumerator InitView()
    {
        InitLayers();
        InitNeurons();
        yield return null;
        InitWeights();

        while (Agent != null)
        {
            ShowNeurons();
            ShowWeights();
            yield return new WaitForFixedUpdate();
        }
    }


    private void Clear()
    {
        for (int i = 0; i < Layers.Length; i++)
        {
            Destroy(Layers[i]);
        }
        StopCoroutine("InitWeights");
        Layers = new GameObject[0];
        Neurons = new GameObject[0][];
        Weights = new GameObject[0][][];
    }

    private void InitLayers()
    {
        Layers = new GameObject[Agent.layers.Length];
        for (int i = 0; i < Agent.layers.Length; i++)
        {
            Layers[i] = Instantiate(m_LayerPrefab, m_UIViewPane, false);
            Layers[i].name = m_LayerPrefab.name + i.ToString("000");
        }
    }

    private void InitNeurons()
    {
        Neurons = new GameObject[Agent.layers.Length][];
        for (int i = 0; i < Agent.layers.Length; i++)
        {
            int neuronsInLayer = (Agent.bias && i != Agent.layers.Length - 1 ? 1 : 0) + Agent.layers[i];
            Neurons[i] = new GameObject[neuronsInLayer];

            for (int j = 0; j < neuronsInLayer; j++)
            {
                Neurons[i][j] = Instantiate(m_NeuronPrefab, Layers[i].transform, false);
                Neurons[i][j].name = m_NeuronPrefab.name + i.ToString("000") + "." + j.ToString("000");
                
            }
        }
    }

    private void InitWeights()
    {
        Weights = new GameObject[Agent.layers.Length - 1][][];
        for (int i = 1; i < Agent.layers.Length; i++)
        {
            int neuronsInBackLayer = (Agent.bias ? 1 : 0) + Agent.layers[i - 1];
            int neuronsInNextLayer = Agent.layers[i];
            Weights[i - 1] = new GameObject[neuronsInBackLayer][];

            for (int j = 0; j < neuronsInBackLayer; j++)
            {
                Weights[i - 1][j] = new GameObject[neuronsInNextLayer];

                for (int k = 0; k < neuronsInNextLayer; k++)
                {
                    Weights[i - 1][j][k] = Instantiate(m_WeightPrefab, Neurons[i - 1][j].transform, false);
                    Weights[i - 1][j][k].name = m_WeightPrefab.name + (i - 1).ToString("000") + "." + j.ToString("000") + "-" + i.ToString("000") + "." + k.ToString("000");

                    Vector2[] positions = new Vector2[2];
                    positions[0] = Vector2.zero;
                    positions[1] = (Neurons[i][k].transform.position - Neurons[i - 1][j].transform.position);

                    positions[1].x /= m_UIViewPane.localScale.x * m_UIViewPane.lossyScale.x;
                    positions[1].y /= m_UIViewPane.localScale.y * m_UIViewPane.lossyScale.y;

                    var lr = Weights[i - 1][j][k].GetComponent<UILineRenderer>();
                    lr.Points = positions;

                }
            }
        }
    }

    private IEnumerator InitWeightsPos()
    {
        while (true)
        {
            // for (int i = 1; i < Agent.Layers.Length; i++)
            // {
            //     int neuronsInBackLayer = (Agent.Bias ? 1 : 0) + Agent.Layers[i - 1];
            //     int neuronsInNextLayer = Agent.Layers[i];

            //     for (int j = 0; j < neuronsInBackLayer; j++)
            //     {
            //         for (int k = 0; k < neuronsInNextLayer; k++)
            //         {
            //             Vector2[] positions = new Vector2[2];
            //             positions[0] = Vector2.zero;
            //             positions[1] = Neurons[i][k].transform.position - Neurons[i - 1][j].transform.position;

            //             var lr = Weights[i - 1][j][k].GetComponent<UILineRenderer>();
            //             //lr.color = color;
            //             lr.Points = positions;
            //         }
            //     }
            // }
            yield return null;
        }
    }

    #endregion

    #region 

    private void FixedUpdate()
    {
        // if (Agent != null)
        // {
        //     ShowNeurons();
        //     ShowWeights();
        // }
    }

    private void ShowNeurons()
    {
        for (int i = 0; i < Agent.layers.Length; i++)
        {
            for (int j = 0; j < Agent.layers[i]; j++)
            {
                Neurons[i][j].GetComponentInChildren<Image>().color = Color.Lerp(NegColor, PosColor, Agent.neurons[i][j] / (i == 0 ? Agent.normalize : 1));
            }
            if (Agent.bias && i != Agent.layers.Length - 1)
            {
                Neurons[i][Agent.layers[i]].GetComponentInChildren<Image>().color = Color.Lerp(NegColor, PosColor, 1);
            }
        }
    }

    private void ShowWeights()
    {
        for (int i = 1; i < Agent.layers.Length; i++)
        {
            int neuronsInBackLayer = (Agent.bias ? 1 : 0) + Agent.layers[i - 1];
            int neuronsInNextLayer = Agent.layers[i];

            for (int j = 0; j < neuronsInBackLayer; j++)
            {
                for (int k = 0; k < neuronsInNextLayer; k++)
                {
                    float value = Agent.bias && j == neuronsInBackLayer - 1 ? 1.0f : (Agent.weights[i - 1][k][j] + 0.5f) * Agent.neurons[i - 1][j];
                    Color color = Color.Lerp(NegColor, PosColor, value / (i == 1 && !(Agent.bias && j == neuronsInBackLayer - 1) ? Agent.normalize : 1));
                    var lr = Weights[i - 1][j][k].GetComponent<UILineRenderer>();
                    lr.color = color;
                }
            }
        }
    }

    #endregion

}



// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using Vehicles.Car;


// public class NNView : MonoBehaviour
// {

//     #region Members

//     [SerializeField] private Color NegColor = Color.red;
//     [SerializeField] private Color PosColor = Color.green;

//     [SerializeField] private Transform m_NNViewPane;
//     [SerializeField] private NNAgent m_NNAgent;
//     public NNAgent NNAgent
//     {
//         get => m_NNAgent;
//         set => m_NNAgent = value;
//     }

//     [Header("Prefabs")]
//     [SerializeField] private GameObject m_LayerPrefab;
//     [SerializeField] private GameObject m_NeuronPrefab;
//     [SerializeField] private GameObject m_WeightPrefab;

//     private GameObject[] Layers;
//     private GameObject[][] Neurons;
//     private GameObject[][][] Weights;

//     #endregion

//     #region Actions

//     private void Awake()
//     {

//         CarNeuralControl cnc = GameObject.FindWithTag("NPC").GetComponent<CarNeuralControl>();
//         m_NNAgent = cnc.NNAgent;

//         InitStruct();
//     }

//     private void Update()
//     {
//         for (int i = 0; i < NNAgent.Layers.Length; i++)
//         {
//             for (int j = 0; j < NNAgent.Layers[i]; j++)
//             {
//                 Neurons[i][j].GetComponent<Image>().color = Color.Lerp(NegColor, PosColor, NNAgent.Neurons[i][j]);
//                 if (i < Layers.Length - 1)
//                 {
//                     for (int k = 0; k < NNAgent.Layers[i + 1]; k++)
//                     {
//                         var cl = Color.Lerp(NegColor, PosColor, NNAgent.Neurons[i][j] /* NNAgent.Weights[i][j][k]*/);
//                         var lr = Weights[i][j][k].GetComponent<LineRenderer>();
//                         var v3 = new Vector3[] { Neurons[i][j].transform.position, Neurons[i + 1][k].transform.position };
//                         lr.SetPositions(v3);
//                         lr.startColor = cl;
//                         lr.endColor = cl;
//                         lr.widthMultiplier = 0.1f;
//                     }
//                 }
//             }
//         }

//     }


//     #endregion

//     #region Methods

//     private void InitView()
//     {

//     }

//     private void InitStruct()
//     {

//         Layers = new GameObject[NNAgent.Layers.Length];
//         Neurons = new GameObject[NNAgent.Layers.Length][];
//         Weights = new GameObject[NNAgent.Layers.Length - 1][][];

//         for (int i = 0; i < NNAgent.Layers.Length; i++)
//         {
//             Layers[i] = Instantiate(m_LayerPrefab, m_NNViewPane);
//             Layers[i].name = m_LayerPrefab.name + " " + i.ToString("000");

//             Neurons[i] = new GameObject[NNAgent.Layers[i]];
//             if (i < Layers.Length - 1)
//             {
//                 Weights[i] = new GameObject[NNAgent.Layers[i]][];
//             }

//             for (int j = 0; j < NNAgent.Layers[i]; j++)
//             {
//                 Neurons[i][j] = Instantiate(m_NeuronPrefab, Layers[i].transform);
//                 Neurons[i][j].name = m_NeuronPrefab.name + " " + i.ToString("000") + "." + j.ToString("000");

//                 if (i < Layers.Length - 1)
//                 {
//                     Weights[i][j] = new GameObject[NNAgent.Layers[i + 1]];
//                     for (int k = 0; k < NNAgent.Layers[i + 1]; k++)
//                     {
//                         Weights[i][j][k] = Instantiate(m_WeightPrefab, Neurons[i][j].transform, false);
//                         Weights[i][j][k].name = m_WeightPrefab.name + " " + i.ToString("000") + "." + j.ToString("000") + "-" + (i + 1).ToString("000") + "." + k.ToString("000");
//                     }
//                 }
//             }
//         }
//     }

//     #endregion


//     // private void Display()
//     // {
//     //     Debug.Log(m_NNAgent.Layers.Length);

//     //     Layers = new GameObject[m_NNAgent.Layers.Length];
//     //     Neurons = new GameObject[m_NNAgent.Layers.Length][];

//     //     for (int i = 0; i < Layers.Length; i++)
//     //     {
//     //         Layers[i] = Instantiate(m_LayerPrefab, m_NNViewPane);
//     //         Layers[i].name = m_LayerPrefab.name + " " + i.ToString("000");
//     //         Neurons[i] = new GameObject[m_NNAgent.Layers[i]];

//     //         for (int j = 0; j < m_NNAgent.Layers[i]; j++)
//     //         {
//     //             Neurons[i][j] = Instantiate(m_NeuronPrefab, Layers[i].transform);
//     //             Neurons[i][j].name = m_NeuronPrefab.name + " " + i.ToString("000") + "." + j.ToString("000");


//     //         }
//     //     }

//     //     Weights = new GameObject[m_NNAgent.Layers.Length][][];

//     //     Transform ct = GetComponentInParent<Canvas>().worldCamera?.transform;

//     //     ct = GameObject.FindWithTag("MainCamera").transform;

//     //     for (int i = 0; i < Weights.Length - 1; i++)
//     //     {
//     //         Weights[i] = new GameObject[m_NNAgent.Layers[i]][];
//     //         for (int j = 0; j < Weights[i].Length; j++)
//     //         {
//     //             Weights[i][j] = new GameObject[m_NNAgent.Layers[i + 1]];

//     //             for (int k = 0; k < Neurons[i + 1].Length; k++)
//     //             {
//     //                 Weights[i][j][k] = Instantiate(m_WeightPrefab, Neurons[i][j].transform, false);
//     //                 Weights[i][j][k].name = m_WeightPrefab.name + " " + i.ToString("000") + "." + j.ToString("000") + "-" + (i + 1).ToString("000") + "." + k.ToString("000");

//     //                 // Weights[i][j][k] = Instantiate(WeightPrefab, Neurons[i][j].transform, false);
//     //                 // Weights[i][j][k].name = WeightPrefab.name + " " + i.ToString("000") + "." + j.ToString("000") + "-" + (i + 1).ToString("000") + "." + k.ToString("000");

//     //                 // Vector2 connect = Neurons[i][j].transform.position - Neurons[i + 1][k].transform.position;

//     //                 // Image im = Weights[i][j][k].GetComponent<Image>();

//     //                 // im.rectTransform.sizeDelta = new Vector2(1, connect.magnitude);

//     //                 // Weights[i][j][k].transform.rotation = Quaternion.AngleAxis(Vector2.Angle(Vector2.up, connect), new Vector3(0, 0, 1));


//     //                 LineRenderer lr = Weights[i][j][k].GetComponent<LineRenderer>();

//     //                 Vector3[] Pos = new Vector3[] {
//     //                  Neurons[i][j].transform.position, //+ ct.forward * 0.1f,
//     //                  Neurons[i+1][k].transform.position //+ ct.forward * 0.1f
//     //                 };

//     //                 Debug.Log(Pos[0].ToString() + " " + Pos[1].ToString());

//     //                 lr.SetPositions(Pos);
//     //                 lr.widthMultiplier = 0.5f;

//     //             }

//     //         }
//     //     }



//     // }

//     // private void Update()
//     // {
//     //     Transform ct = GetComponentInParent<Canvas>().worldCamera?.transform;

//     //     ct = GameObject.FindWithTag("MainCamera").transform;

//     //     for (int i = 0; i < Weights.Length; i++)
//     //     {
//     //         for (int j = 0; j < m_NNAgent.Layers[i]; j++)
//     //         {
//     //             Neurons[i][j].GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, m_NNAgent.Neurons[i][j]);

//     //             if (i < Weights.Length - 1)
//     //             {
//     //                 for (int k = 0; k < Neurons[i + 1].Length; k++)
//     //                 {
//     //                     // Image im = Weights[i][j][k].GetComponent<Image>();
//     //                     // im.transform.localPosition = Vector3.zero;

//     //                     // Vector2 connect = Neurons[i][j].transform.position - Neurons[i + 1][k].transform.position;

//     //                     // im.rectTransform.sizeDelta = new Vector2(1, connect.magnitude);

//     //                     // Weights[i][j][k].transform.rotation = Quaternion.AngleAxis(Vector2.Angle(Vector2.up, connect), new Vector3(0, 0, 1));

//     //                     // var v = Vector3.Lerp(Neurons[i][j].transform.position, Neurons[i + 1][k].transform.position, 0.5f);

//     //                     // v.z = -1f;

//     //                     // Weights[i][j][k].transform.position = v;

//     //                     //                        Debug.Log(agent.Weights[i][j].Length);


//     //                     LineRenderer lr = Weights[i][j][k].GetComponent<LineRenderer>();

//     //                     Vector3[] Pos = new Vector3[] {
//     //                         Neurons[i][j].transform.position ,//+ ct.forward * 0.1f,
//     //                         Neurons[i+1][k].transform.position //+ ct.forward * 0.1f
//     //                     };

//     //                     lr.SetPositions(Pos);
//     //                     lr.widthMultiplier = 0.5f;
//     //                     Color c = Color.Lerp(Color.red, Color.green, m_NNAgent.Neurons[i][j] /* agent.Weights[i][j][k]*/);
//     //                     lr.startColor = c;
//     //                     lr.endColor = c;
//     //                 }
//     //             }
//     //         }
//     //     }

//     // }





// }
