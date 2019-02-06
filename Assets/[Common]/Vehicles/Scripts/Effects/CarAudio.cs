using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Vehicles.Car
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(CarControlSystem))]
    public class CarAudio : MonoBehaviour
    {
        // This script reads some of the car's current properties and plays sounds accordingly.
        // The engine sound can be a simple single clip which is looped and pitched, or it
        // can be a crossfaded blend of four clips which represent the timbre of the engine
        // at different RPM and Throttle state.

        // the engine clips should all be a steady pitch, not rising or falling.

        // when using four channel engine crossfading, the four clips should be:
        // lowAccelClip : The engine at low revs, with throttle open (i.e. begining acceleration at very low speed)
        // highAccelClip : Thenengine at high revs, with throttle open (i.e. accelerating, but almost at max speed)
        // lowDecelClip : The engine at low revs, with throttle at minimum (i.e. idling or engine-braking at very low speed)
        // highDecelClip : Thenengine at high revs, with throttle at minimum (i.e. engine-braking at very high speed)

        // For proper crossfading, the clips pitches should all match, with an octave offset between low and high.


        public enum EngineAudioOptions // Options for the engine audio
        {
            Simple, // Simple style audio
            FourChannel // four Channel audio
        }


        #region Members

        [SerializeField] private EngineAudioOptions m_EngineSoundStyle = EngineAudioOptions.FourChannel; // Set the default audio options to be four channel
        [SerializeField] private AudioClip m_LowAccelClip;                                              // Audio clip for low acceleration
        [SerializeField] private AudioClip m_LowDecelClip;                                              // Audio clip for low deceleration
        [SerializeField] private AudioClip m_HighAccelClip;                                             // Audio clip for high acceleration
        [SerializeField] private AudioClip m_HighDecelClip;                                             // Audio clip for high deceleration
        [SerializeField] private float m_PitchMultiplier = 1f;                                          // Used for altering the pitch of audio clips
        [SerializeField] private float m_LowPitchMin = 1f;                                              // The lowest possible pitch for the low sounds
        [SerializeField] private float m_LowPitchMax = 6f;                                              // The highest possible pitch for the low sounds
        [SerializeField] private float m_HighPitchMultiplier = 0.25f;                                   // Used for altering the pitch of high sounds
        [SerializeField] private float m_MaxRolloffDistance = 500;                                      // The maximum distance where rollof starts to take place
        [SerializeField] private float m_DopplerLevel = 1;                                              // The mount of doppler effect used in the audio
        [SerializeField] private bool m_UseDoppler = true;                                              // Toggle for using doppler

        private AudioSource m_LowAccel; // Source for the low acceleration sounds
        private AudioSource m_LowDecel; // Source for the low deceleration sounds
        private AudioSource m_HighAccel; // Source for the high acceleration sounds
        private AudioSource m_HighDecel; // Source for the high deceleration sounds
        private bool m_StartedSound; // flag for knowing if we have started sounds
        private CarControlSystem m_CarController; // Reference to car we are controlling

        #endregion

        #region Actions

        // Update is called once per frame
        private void Update()
        {
            // get the distance to main camera
            float camDist = (Camera.main.transform.position - transform.position).sqrMagnitude;

            // stop sound if the object is beyond the maximum roll off distance
            if (m_StartedSound && camDist > m_MaxRolloffDistance * m_MaxRolloffDistance)
            {
                StopSound();
            }

            // start the sound if not playing and it is nearer than the maximum distance
            if (!m_StartedSound && camDist < m_MaxRolloffDistance * m_MaxRolloffDistance)
            {
                StartSound();
            }

            if (m_StartedSound)
            {
                // The pitch is interpolated between the min and max values, according to the car's revs.
                float pitch = ULerp(m_LowPitchMin, m_LowPitchMax, m_CarController.Revs);

                // clamp to minimum pitch (note, not clamped to max for high revs while burning out)
                pitch = Mathf.Min(m_LowPitchMax, pitch);

                if (m_EngineSoundStyle == EngineAudioOptions.Simple)
                {
                    // for 1 channel engine sound, it's oh so simple:
                    m_HighAccel.pitch = pitch * m_PitchMultiplier * m_HighPitchMultiplier;
                    m_HighAccel.dopplerLevel = m_UseDoppler ? m_DopplerLevel : 0;
                    m_HighAccel.volume = 1;
                }
                else
                {
                    // for 4 channel engine sound, it's a little more complex:

                    // adjust the pitches based on the multipliers
                    m_LowAccel.pitch = pitch * m_PitchMultiplier;
                    m_LowDecel.pitch = pitch * m_PitchMultiplier;
                    m_HighAccel.pitch = pitch * m_HighPitchMultiplier * m_PitchMultiplier;
                    m_HighDecel.pitch = pitch * m_HighPitchMultiplier * m_PitchMultiplier;

                    // get values for fading the sounds based on the acceleration
                    float accFade = Mathf.Abs(m_CarController.AccelInput);
                    float decFade = 1 - accFade;

                    // get the high fade value based on the cars revs
                    float highFade = Mathf.InverseLerp(0.2f, 0.8f, m_CarController.Revs);
                    float lowFade = 1 - highFade;

                    // adjust the values to be more realistic
                    highFade = 1 - ((1 - highFade) * (1 - highFade));
                    lowFade = 1 - ((1 - lowFade) * (1 - lowFade));
                    accFade = 1 - ((1 - accFade) * (1 - accFade));
                    decFade = 1 - ((1 - decFade) * (1 - decFade));

                    // adjust the source volumes based on the fade values
                    m_LowAccel.volume = lowFade * accFade;
                    m_LowDecel.volume = lowFade * decFade;
                    m_HighAccel.volume = highFade * accFade;
                    m_HighDecel.volume = highFade * decFade;

                    // adjust the doppler levels
                    m_HighAccel.dopplerLevel = m_UseDoppler ? m_DopplerLevel : 0;
                    m_LowAccel.dopplerLevel = m_UseDoppler ? m_DopplerLevel : 0;
                    m_HighDecel.dopplerLevel = m_UseDoppler ? m_DopplerLevel : 0;
                    m_LowDecel.dopplerLevel = m_UseDoppler ? m_DopplerLevel : 0;
                }
            }
        }

        #endregion

        #region Methods

        private void StartSound()
        {
            // get the carcontroller ( this will not be null as we have require component)
            m_CarController = GetComponent<CarControlSystem>();

            // setup the simple audio source
            m_HighAccel = SetUpEngineAudioSource(m_HighAccelClip);

            // if we have four channel audio setup the four audio sources
            if (m_EngineSoundStyle == EngineAudioOptions.FourChannel)
            {
                m_LowAccel = SetUpEngineAudioSource(m_LowAccelClip);
                m_LowDecel = SetUpEngineAudioSource(m_LowDecelClip);
                m_HighDecel = SetUpEngineAudioSource(m_HighDecelClip);
            }

            // flag that we have started the sounds playing
            m_StartedSound = true;
        }

        private void StopSound()
        {
            //Destroy all audio sources on this object:
            foreach (var source in GetComponents<AudioSource>())
            {
                Destroy(source);
            }

            m_StartedSound = false;
        }

        // sets up and adds new audio source to the gane object
        private AudioSource SetUpEngineAudioSource(AudioClip clip)
        {
            // create the new audio source component on the game object and set up its properties
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = 0;
            source.loop = true;

            // start the clip from a random point
            source.time = Random.Range(0f, clip.length);
            source.Play();
            source.minDistance = 5;
            source.maxDistance = m_MaxRolloffDistance;
            source.dopplerLevel = 0;
            return source;
        }

        // unclamped versions of Lerp and Inverse Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value) * from + value * to;
        }

        #endregion
    }
}
