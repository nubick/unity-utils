using System;
using Assets.Scripts.Data;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    [CustomEditor(typeof(SoundEffect))]    
    public class SoundEffectEditor : UnityEditor.Editor
    {
        private SoundEffect SoundEffect { get { return target as SoundEffect; } }

        public void OnEnable()
        {
            Play();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();

            SoundEffect.Type = (SoundEffectType)EditorGUILayout.EnumPopup("Type", SoundEffect.Type);

            switch(SoundEffect.Type)
            {
                case SoundEffectType.Single:
                    SoundEffect.AudioClip = EditorGUILayout.ObjectField("Audio Clip", SoundEffect.AudioClip, typeof(AudioClip), false) as AudioClip;
                    break;
                case SoundEffectType.Multiple:
                    SoundEffect.SelectionType = (SoundEffectSelectionType)EditorGUILayout.EnumPopup("Selection", SoundEffect.SelectionType);
                    SerializedProperty serializedProperty = serializedObject.FindProperty("AudioClips");
                    EditorGUILayout.PropertyField(serializedProperty, true);
                    serializedObject.ApplyModifiedProperties();
                    break;
                default:
                    throw new Exception("Not supported SoundEffectType: " + SoundEffect.Type);
            }

            SoundEffect.Volume = EditorGUILayout.Slider("Volume", SoundEffect.Volume, 0f, 1f);

            if (GUILayout.Button("Play"))
                Play();

            GUILayout.EndVertical();

            EditorUtility.SetDirty(SoundEffect);
        }

        private void Play()
        {
            AudioSource audioSource = FindObjectOfType<AudioSource>();
            if (audioSource == null)
                Debug.Log("Can't find AudioSource in the opened scene to play Sound Effect.");
            else
                SoundEffect.Play(audioSource);            
        }
    }
}