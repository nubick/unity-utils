using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Utils.Editor
{
    public class ButtonSoundsEditor : EditorWindow
    {
        private AudioSource _audioSource;
        private AudioClip _clickSound;
        private Vector2 _scrollPosition;
        private Button _selectedButton;

        [MenuItem("Window/Utils/Button sounds")]
        public static void OpenEditor()
        {
            ButtonSoundsEditor window = GetWindow<ButtonSoundsEditor>();
            window.titleContent = new GUIContent("Button sounds");
            window.Show();
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();

            Button[] buttons = Resources.FindObjectsOfTypeAll<Button>().Where(_ => PrefabUtility.GetPrefabType(_) != PrefabType.Prefab).ToArray();
            ButtonClickSound[] clickSounds = buttons.Select(_ => _.GetComponent<ButtonClickSound>()).Where(_ => _ != null).ToArray();

            DrawTopPanel(clickSounds);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            foreach (Button button in buttons)
                DrawButton(button);
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            DrawRightPanel();

            GUILayout.EndHorizontal();


            DrawBottomPanel(buttons);

            GUILayout.EndVertical();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        private void DrawTopPanel(ButtonClickSound[] clickSounds)
        {
            GUILayout.BeginVertical("Box");
            GUILayout.Space(5);

            if (_audioSource == null)
                _audioSource = GetFirstAudioSource(clickSounds);

            if (_clickSound == null)
                _clickSound = GetFirstClickSound(clickSounds);

            _audioSource = EditorGUILayout.ObjectField("Audio Source:", _audioSource, typeof(AudioSource), true) as AudioSource;
            _clickSound = EditorGUILayout.ObjectField("Click sound:", _clickSound, typeof(AudioClip), false) as AudioClip;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply to all"))
            {
                foreach (ButtonClickSound clickSound in clickSounds)
                {
                    clickSound.AudioSource = _audioSource;
                    clickSound.ClickSound = _clickSound;
                    EditorUtility.SetDirty(clickSound);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        private void DrawButton(Button button)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("#", GUILayout.Width(20)))
            {
                Selection.activeObject = button;
                _selectedButton = button;
            }

            GUILayout.Label(button.name, GUILayout.Width(100));

            ButtonClickSound clickSound = button.GetComponent<ButtonClickSound>();
            if (clickSound == null)
            {
                if (GUILayout.Button("Add click sound", GUILayout.Width(100)))
                {
                    AddClickSoundToButton(button);
                }
            }
            else
            {
                clickSound.ClickSound = EditorGUILayout.ObjectField(clickSound.ClickSound, typeof(AudioClip), false, GUILayout.Width(200)) as AudioClip;
                if (GUILayout.Button("Remove", GUILayout.Width(75)))
                {
                    DestroyImmediate(clickSound);
                }
            }

            GUILayout.EndHorizontal();
        }

        private void AddClickSoundToButton(Button button)
        {
            ButtonClickSound buttonClickSound = button.gameObject.AddComponent<ButtonClickSound>();
            buttonClickSound.AudioSource = _audioSource;
            buttonClickSound.ClickSound = _clickSound;
            EditorUtility.SetDirty(button.gameObject);
            EditorUtility.SetDirty(buttonClickSound);
        }

        private AudioSource GetFirstAudioSource(ButtonClickSound[] clickSounds)
        {
            ButtonClickSound buttonClickSound = clickSounds.FirstOrDefault(_ => _.AudioSource != null);
            return buttonClickSound == null ? null : buttonClickSound.AudioSource;
        }

        private AudioClip GetFirstClickSound(ButtonClickSound[] clickSounds)
        {
            ButtonClickSound buttonClickSound = clickSounds.FirstOrDefault(_ => _.ClickSound != null);
            return buttonClickSound == null ? null : buttonClickSound.ClickSound;
        }

        private void DrawBottomPanel(Button[] buttons)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add click sound to all"))
            {
                foreach (Button button in buttons.Where(_ => _.GetComponent<ButtonClickSound>() == null))
                    AddClickSoundToButton(button);
            }
            if (GUILayout.Button("Clear all buttons"))
            {
                foreach (Button button in buttons)
                {
                    ButtonClickSound buttonClickSound = button.GetComponent<ButtonClickSound>();
                    if (buttonClickSound != null)
                    {
                        DestroyImmediate(buttonClickSound);
                        EditorUtility.SetDirty(button);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawRightPanel()
        {
            if (_selectedButton != null)
            {
                GUILayout.BeginVertical();

                Image image = _selectedButton.GetComponent<Image>();
                if (image != null)
                {
                    GUILayout.Box(image.sprite.texture);
                }

                GUILayout.Label(GetTransformPath(_selectedButton.transform));

                GUILayout.EndVertical();
            }
        }

        private string GetTransformPath(Transform tr)
        {
            string path = tr.root.name;
            if (tr != tr.root)
                path += "/" + AnimationUtility.CalculateTransformPath(tr, tr.root);
            return path;
        }
    }
}
