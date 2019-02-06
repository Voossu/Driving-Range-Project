using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extend;

public class UIChart : MonoBehaviour
{
    public RectTransform chart;

    public UILineRenderer[] lines;


    float maxValue = 0;

    List<float[]> points = new List<float[]>() { new float[] { 0, 0, 0 } };

    private void Awake()
    {

    }

    public void AddPoints(float[] newPoints)
    {

        for (int i = 0; i < newPoints.Length; i++)
        {
            if (newPoints[i] > maxValue)
            {
                maxValue = newPoints[i];
            }
        }

        this.points.Add(newPoints);

        Show();
    }

    public void Clear()
    {
        maxValue = 0;
        points = new List<float[]>() { new float[] { 0, 0, 0 } };
        Show();
    }


    public void Show()
    {

        Vector2 size = chart.sizeDelta;


        size.x = chart.rect.width / (this.points.Count - 1);
        size.y = chart.rect.height / maxValue;

        for (int j = 0; j < lines.Length; j++)
        {
            lines[j].Points = new Vector2[this.points.Count];
        }

        for (int i = 0; i < this.points.Count; i++)
        {
            for (int j = 0; j < lines.Length; j++)
            {
                lines[j].Points[i] = new Vector2(i * size.x, this.points[i][j] * size.y);
            }
        }

    }


}
