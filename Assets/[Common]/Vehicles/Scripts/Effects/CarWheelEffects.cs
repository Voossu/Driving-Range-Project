using System.Collections;
using UnityEngine;

namespace Vehicles.Car
{
    [RequireComponent(typeof(AudioSource))]
    public class CarWheelEffects : MonoBehaviour
    {

        #region Members

        public Transform m_SkidTrailPrefab;
        public static Transform m_SkidTrailsDetachedParent;
        public ParticleSystem m_SkidParticles;
        public bool m_Skidding { get; private set; }
        public bool m_PlayingAudio { get; private set; }


        private AudioSource m_AudioSource;
        private Transform m_SkidTrail;
        private WheelCollider m_WheelCollider;

        #endregion


        #region Actions

        private void Start()
        {
            m_SkidParticles = transform.root.GetComponentInChildren<ParticleSystem>();

            if (m_SkidParticles == null)
            {
                Debug.LogWarning(" no particle system found on car to generate smoke particles", gameObject);
            }
            else
            {
                m_SkidParticles.Stop();
            }

            m_WheelCollider = GetComponent<WheelCollider>();
            m_AudioSource = GetComponent<AudioSource>();
            m_PlayingAudio = false;

            if (m_SkidTrailsDetachedParent == null)
            {
                m_SkidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
            }
        }


        public void EmitTyreSmoke()
        {
            m_SkidParticles.transform.position = transform.position - transform.up * m_WheelCollider.radius;
            m_SkidParticles.Emit(1);
            if (!m_Skidding)
            {
                StartCoroutine(StartSkidTrail());
            }
        }


        public void PlayAudio()
        {
            m_AudioSource.Play();
            m_PlayingAudio = true;
        }


        public void StopAudio()
        {
            m_AudioSource.Stop();
            m_PlayingAudio = false;
        }


        public IEnumerator StartSkidTrail()
        {
            m_Skidding = true;
            m_SkidTrail = Instantiate(m_SkidTrailPrefab);
            while (m_SkidTrail == null)
            {
                yield return null;
            }
            m_SkidTrail.parent = transform;
            m_SkidTrail.localPosition = -Vector3.up * m_WheelCollider.radius;
        }


        public void EndSkidTrail()
        {
            if (!m_Skidding)
            {
                return;
            }
            m_Skidding = false;
            m_SkidTrail.parent = m_SkidTrailsDetachedParent;
            Destroy(m_SkidTrail.gameObject, 10);
        }

        #endregion
    }
}
