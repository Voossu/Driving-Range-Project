using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Vehicles.Utility
{
    public class WaypointCircuit : MonoBehaviour
    {

        #region Members

        [SerializeField] private bool m_AutoUpdate = false;
        [SerializeField] private bool m_SmoothRoute = true;
        [SerializeField] private bool m_DelinkRoute = true;
        [SerializeField] private int m_SmoothSteps = 100;


        [HideInInspector] [SerializeField] private int cash_Length = 0;
        public int Length { get => cash_Length; private set => cash_Length = value; }

        [HideInInspector] [SerializeField] private Vector3[] cash_Position = new Vector3[0];
        public Vector3[] Positions { get => cash_Position; private set => cash_Position = value; }

        [HideInInspector] [SerializeField] private Quaternion[] cash_Rotations = new Quaternion[0];
        public Quaternion[] Rotations { get => cash_Rotations; private set => cash_Rotations = value; }

        [HideInInspector] [SerializeField] private float[] cash_Distances = new float[0];
        public float TotalDistance => cash_Distances[m_DelinkRoute && Length != 0 ? Length - 1 : Length];


        [Header("Display settings")]
        [SerializeField] private Color m_PointColor = Color.yellow;
        [SerializeField] private Color m_ChildColor = Color.cyan;
        [SerializeField] private Color m_RouteColor = Color.yellow;
        [Min(0.2f)] [SerializeField] private float m_PointRadius = 0.5f;

        #endregion


        #region Actions

        private void OnDrawGizmos() => DrawCircuitGizmos(false);
        private void OnDrawGizmosSelected() => DrawCircuitGizmos(true);
        private void Reset() => ResetWaipointRoute();

        private void Update()
        {
            if (m_AutoUpdate) ResetWaipointRoute();
        }
        #endregion


        #region Methods

        public void ResetWaipointRoute()
        {
            Length = transform.childCount;
            Positions = new Vector3[Length];
            Rotations = new Quaternion[Length];
            cash_Distances = new float[Length + 1];

            if (Length > 0)
            {
                int i = 0;
                float accumulateDistance = 0;
                foreach (Transform child in transform)
                {
                    child.name = "Waypoint " + i.ToString("000");
                    if (i > 0)
                    {
                        accumulateDistance += (Positions[i - 1] - Positions[i]).magnitude;
                    }
                    Positions[i] = child.position;
                    Rotations[i] = child.rotation;
                    cash_Distances[i] = accumulateDistance;
                    i++;
                }
                accumulateDistance += (Positions[i - 1] - Positions[0]).magnitude;
                cash_Distances[i] = accumulateDistance;
            }
        }

        private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
        {
            // comments are no use here... it's the catmull-rom equation.
            // Un-magic this, lord vector!
            return 0.5f *
                   ((2 * p1) + (-p0 + p2) * i + (2 * p0 - 5 * p1 + 4 * p2 - p3) * i * i +
                    (-p0 + 3 * p1 - 3 * p2 + p3) * i * i * i);
        }

        //this being here will save GC allocs
        private int p0n, p1n, p2n, p3n;
        private float inverseLerp;
        private Vector3 P0, P1, P2, P3;

        public Vector3 GetRoutePosition(float dist)
        {
            dist = Mathf.Repeat(dist, TotalDistance);
            int point = 0;
            while (cash_Distances[point] < dist) point++;

            // get nearest two points, ensuring points wrap-around start & end of circuit
            p1n = ((point - 1) + Length) % Length;
            p2n = point;

            // found point numbers, now find interpolation value between the two middle points

            inverseLerp = Mathf.InverseLerp(cash_Distances[p1n], cash_Distances[p2n], dist);

            if (!m_SmoothRoute)
            {
                return Vector3.Lerp(Positions[p1n], Positions[p2n], inverseLerp);
            }
            else
            {
                // smooth catmull-rom calculation between the two relevant points

                // get indices for the surrounding 2 points, because
                // four points are required by the catmull-rom function
                p0n = ((point - 2) + Length) % Length;
                p3n = (point + 1) % Length;

                // сorrection smooth point if delink route
                if (m_DelinkRoute)
                {
                    if (p0n == Length - 1) p0n = p1n;
                    if (p3n == 0) p3n = p2n;
                }

                // 2nd point may have been the 'last' point - a dupe of the first,
                // (to give a value of max track distance instead of zero)
                // but now it must be wrapped back to zero if that was the case.
                p2n = p2n % Length;

                P0 = Positions[p0n];
                P1 = Positions[p1n];
                P2 = Positions[p2n];
                P3 = Positions[p3n];

                return CatmullRom(P0, P1, P2, P3, inverseLerp);
            }
        }

        public RoutePointer GetRoutePointer(float dist)
        {
            // position and direction
            Vector3 p1 = GetRoutePosition(dist);
            Vector3 p2 = GetRoutePosition(dist + 0.1f);
            Vector3 delta = p2 - p1;
            if (dist > TotalDistance) return new RoutePointer(Positions[Length - 1], Vector3.zero);
            return new RoutePointer(p1, delta.normalized);
        }

        private void DrawCircuitGizmos(bool selected)
        {
            Color basicColor = Gizmos.color;
            Color pointColor = m_PointColor;
            Color childColor = m_ChildColor;
            Color routeColor = m_RouteColor;
            if (!selected)
            {
                pointColor.a *= 0.5f;
                childColor.a *= 0.5f;
                routeColor.a *= 0.5f;
            }

            Gizmos.color = pointColor;
            for (int i = 0; i < Length; i++)
            {
                Gizmos.DrawWireSphere(Positions[i], m_PointRadius);
            }

            Gizmos.color = childColor;
            foreach (Transform child in transform)
            {
                Gizmos.DrawWireSphere(child.position, m_PointRadius);
            }

            Gizmos.color = routeColor;
            Vector3 back = Positions[0];
            if (m_SmoothRoute)
            {
                float totalDistance = TotalDistance;

                for (float dist = 0; dist < totalDistance; dist += totalDistance / m_SmoothSteps)
                {
                    Vector3 next = GetRoutePosition(dist);
                    Gizmos.DrawLine(back, next);
                    back = next;
                }
                Gizmos.DrawLine(back, Positions[m_DelinkRoute ? Length - 1 : 0]);
            }
            else
            {
                int l = m_DelinkRoute ? Length - 1 : Length;
                for (int n = 0; n < l; ++n)
                {
                    Vector3 next = Positions[(n + 1) % Length];
                    Gizmos.DrawLine(back, next);
                    back = next;
                }
            }

            Gizmos.color = basicColor;
        }

        #endregion


        #region Structs

        public struct RoutePointer
        {
            public Vector3 position;
            public Vector3 direction;

            public RoutePointer(Vector3 position, Vector3 direction)
            {
                this.position = position;
                this.direction = direction;
            }
        }

        #endregion

    }
}


namespace Vehicles.Utility.Inspector
{
#if UNITY_EDITOR
    [CustomEditor(typeof(WaypointCircuit))]
    public class WaypointCircuitEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var circuit = target as WaypointCircuit;
            var level = EditorGUI.indentLevel;
            EditorGUILayout.Space();
            if (GUILayout.Button("Reset Waipoint Route"))
            {
                circuit.ResetWaipointRoute();
            }
            EditorGUI.indentLevel = level;
        }
    }
#endif
}



