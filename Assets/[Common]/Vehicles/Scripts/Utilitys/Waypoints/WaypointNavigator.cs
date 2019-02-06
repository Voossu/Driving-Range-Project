using System;
using Vehicles.Car;
using UnityEngine;

namespace Vehicles.Utility
{
    [RequireComponent(typeof(CarFollowControl))]
    [DisallowMultipleComponent]
    public class WaypointNavigator : MonoBehaviour
    {

        #region Members

        [Header("Display settings")]
        [SerializeField] private Color m_DirectionColor = Color.green;
        [SerializeField] private Color m_ForwardColor = Color.red;

        // This script can be used with any object that is supposed to follow a
        // route marked out by waypoints.

        // This script manages the amount to look ahead along the route,
        // and keeps track of progress and laps.

        [Header("Navigator settings")]
        [SerializeField] private WaypointCircuit m_Circuit; // A reference to the waypoint-based route we should follow
        public WaypointCircuit Circuit
        {
            get => m_Circuit;
            set => m_Circuit = value;
        }
        [SerializeField] private float m_LookAheadForSpeedOffset = 10;
        // The offset ahead only the route for speed adjustments (applied as the rotation of the waypoint target transform)
        [SerializeField] private float m_LookAheadForSpeedFactor = .2f;
        // A multiplier adding distance ahead along the route for speed adjustments
        [SerializeField] private ProgressStyle m_ProgressStyle = ProgressStyle.SmoothAlongRoute;
        // whether to update the position smoothly along the route (good for curved paths) or just when we reach each waypoint.
        [SerializeField] private float m_PointToPointThreshold = 4;
        // proximity to waypoint which must be reached to switch target to next waypoint : only used in PointToPoint mode.


        public enum ProgressStyle { SmoothAlongRoute, PointToPoint }


        // these are public, readable by other objects - i.e. for an AI to know where to head!
        public WaypointCircuit.RoutePointer TargetPoint { get; private set; }
        public WaypointCircuit.RoutePointer SpeedPoint { get; private set; }
        public WaypointCircuit.RoutePointer ProgressPoint { get; private set; }


        private CarFollowControl m_FollowControl;
        private Transform m_Target => m_FollowControl.target;


        private float m_ProgressDistance; // The progress round the route, used in smooth mode.
        private int m_ProgressNum; // the current waypoint number, used in point-to-point mode.
        private Vector3 m_LastPosition; // Used to calculate current speed (since we may not have a rigidbody component)
        private float m_Speed; // current speed of this object (calculated from delta since last frame)

        #endregion


        #region Actions

        // setup script properties
        private void Start()
        {
            // we use a transform to represent the point to aim for, and the point which
            // is considered for upcoming changes-of-speed. This allows this component
            // to communicate this information to the AI without requiring further dependencies.

            // You can manually create a transform and assign it to this component *and* the AI,
            // then this component will update it, and the AI can read it.
            m_FollowControl = transform.GetComponent<CarFollowControl>();
            Reset();
            if (m_Circuit == null)
            {
                throw new ArgumentNullException("WaypointCircuit");
            }
        }

        // reset the object to sensible values
        public void Reset()
        {
            m_ProgressDistance = 0;
            m_ProgressNum = 0;
            if (m_ProgressStyle == ProgressStyle.PointToPoint)
            {
                m_Target.position = m_Circuit.Positions[m_ProgressNum];
                m_Target.rotation = m_Circuit.Rotations[m_ProgressNum];
            }
        }

        private void Update()
        {
            if (m_ProgressStyle == ProgressStyle.SmoothAlongRoute)
            {
                // determine the position we should currently be aiming for
                // (this is different to the current progress position, it is a a certain amount ahead along the route)
                // we use lerp as a simple way of smoothing out the speed over time.
                if (Time.deltaTime > 0)
                {
                    m_Speed = Mathf.Lerp(m_Speed, (m_LastPosition - transform.position).magnitude / Time.deltaTime, Time.deltaTime);
                }
                var routePointer = m_Circuit.GetRoutePointer(m_ProgressDistance + m_LookAheadForSpeedOffset + m_LookAheadForSpeedFactor * m_Speed);
                m_Target.position = routePointer.position;
                m_Target.rotation = Quaternion.LookRotation(routePointer.direction);
            }
            else
            {
                // point to point mode. Just increase the waypoint if we're close enough:
                Vector3 targetDelta = m_Target.position - transform.position;
                if (targetDelta.magnitude < m_PointToPointThreshold)
                {
                    m_ProgressNum = (m_ProgressNum + 1) % m_Circuit.Length;
                }
                m_Target.position = m_Circuit.Positions[m_ProgressNum];
                m_Target.rotation = m_Circuit.Rotations[m_ProgressNum];
            }

            // get our current progress along the route
            ProgressPoint = m_Circuit.GetRoutePointer(m_ProgressDistance);
            Vector3 progressDelta = ProgressPoint.position - transform.position;
            if (Vector3.Dot(progressDelta, ProgressPoint.direction) < 0)
            {
                m_ProgressDistance += progressDelta.magnitude * 0.5f;
            }
            m_LastPosition = transform.position;
        }

        private void OnDrawGizmos()
        {
            if (m_Circuit != null && Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, m_Target.position);
                Gizmos.DrawWireSphere(m_Circuit.GetRoutePosition(m_ProgressDistance), 1);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(m_Target.position, m_Target.position + m_Target.forward);
            }
        }

        #endregion

    }
}
