using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Vehicles.Utility
{
    public class CheckpointCircuit : MonoBehaviour
    {

        #region Fields

        [SerializeField] private Color m_PointColor = Color.green;

        #endregion


        #region Members

        private void OnDrawGizmos() => DrawCircuitGizmos(false);

        private void OnDrawGizmosSelected() => DrawCircuitGizmos(true);

        #endregion


        #region Methods

        private void DrawCircuitGizmos(bool selected)
        {
            Color basicColor = Gizmos.color;
            Color pointColor = m_PointColor;
            if (!selected)
            {
                pointColor.a *= 0.5f;
            }
            Gizmos.color = pointColor;
            var matrix = Gizmos.matrix;
            foreach (Transform child in transform)
            {
                BoxCollider collider = child.GetComponent<BoxCollider>();
                if (collider != null)
                {
                    Gizmos.matrix = child.localToWorldMatrix;
                    Gizmos.DrawWireCube(collider.center, collider.size);
                }
            }
            Gizmos.matrix = matrix;
            Gizmos.color = basicColor;
        }

        public void RenameCheckpoints()
        {
            int i = 0;
            foreach (Transform child in transform)
            {
                child.name = "Checkpoint " + i.ToString("000");
                i++;
            }
        }

        #endregion


    }
}


namespace Vehicles.Utility.Inspector
{
#if UNITY_EDITOR
    [CustomEditor(typeof(CheckpointCircuit))]
    public class CheckpointCircuitEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var circuit = target as CheckpointCircuit;
            EditorGUILayout.Space();
            if (GUILayout.Button("Rename Checkpoints"))
            {
                circuit.RenameCheckpoints();
            }
            var level = EditorGUI.indentLevel;

        }
    }
#endif
}