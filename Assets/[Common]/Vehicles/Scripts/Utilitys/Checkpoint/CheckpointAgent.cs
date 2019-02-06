using UnityEngine;
using UnityEngine.Events;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Vehicles.Utility
{
    public class CheckpointAgent : MonoBehaviour
    {

        #region Members

        public CheckpointCircuit Circuit = null;
        public bool InOrder = true;
        public bool CheckImprove = true;
        public float ImproveTime = 10;

        [Header("Actions")]
        public UnityEvent OnCheckpointEnter = null;
        public UnityEvent OnFinishlineEnter = null;
        public UnityEvent OnNotImprove = null;


        private int lastCheckpointIndex = -1;
        public int Enter => lastCheckpointIndex + 1;

        #endregion

        #region Actions

        private void OnEnable()
        {
            if (CheckImprove)
            {
                StartCoroutine(ImprovingCheack());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Circuit != null && other.transform.parent != null && Circuit.transform == other.transform.parent)
            {
                int currentIndex = other.transform.GetSiblingIndex();
                if (!InOrder || lastCheckpointIndex + 1 == currentIndex)
                {
                    OnCheckpointEnter.Invoke();
                    lastCheckpointIndex = currentIndex;
                }
                if (currentIndex == other.transform.parent.childCount)
                {
                    OnFinishlineEnter.Invoke();
                }
            }
        }

        #endregion

        #region Corutine

        IEnumerator ImprovingCheack()
        {
            while (true)
            {
                float OldIndex = lastCheckpointIndex; // Save the initial cheack point
                yield return new WaitForSeconds(ImproveTime); // Wait for some time
                if (OldIndex == lastCheckpointIndex) // Check if the last cheack didn't change yet 
                {
                    OnNotImprove.Invoke(); // Make not improve actions
                }
            }
        }

        #endregion

    }
}