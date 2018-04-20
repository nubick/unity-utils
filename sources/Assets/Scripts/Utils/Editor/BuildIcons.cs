using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils.Editor
{
    [CreateAssetMenu]
    public class BuildIcons : ScriptableObject
    {
        public BuildTargetGroup TargetGroup;
        public IconKind IconKind;
        public Texture2D[] Icons;

        public void ApplyToPlayerSettings()
        {
            PlayerSettings.SetIconsForTargetGroup(TargetGroup, Icons, IconKind);
        }
    }

    [CustomEditor(typeof(BuildIcons))]
    public class PlatformIconsEditor : UnityEditor.Editor
    {
        private BuildIcons BuildIcons { get { return target as BuildIcons; } }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            //GUILayout.BeginHorizontal();

            if (GUILayout.Button("Fill from Player Settings"))
            {
                BuildIcons.Icons = PlayerSettings.GetIconsForTargetGroup(BuildIcons.TargetGroup, BuildIcons.IconKind);
                EditorUtility.SetDirty(BuildIcons);
            }

            if (GUILayout.Button("Validate"))
            {
                Validate();
            }

            if (GUILayout.Button("Apply To Player Settings"))
            {
                BuildIcons.ApplyToPlayerSettings();
            }

            //GUILayout.EndHorizontal();
        }

        private void Validate()
        {
            int[] sizes = PlayerSettings.GetIconSizesForTargetGroup(BuildIcons.TargetGroup, BuildIcons.IconKind);

            if (BuildIcons.Icons.Length != sizes.Length)
            {
                Debug.LogError(string.Format("Icons length '{0}' is not equal target group sizes length '{1}'.", BuildIcons.Icons.Length, sizes.Length));
                return;
            }

            for (int i = 0; i < BuildIcons.Icons.Length; i++)
            {
                Texture2D icon = BuildIcons.Icons[i];
                if (icon != null)
                {
                    if (icon.width != icon.height)
                        Debug.LogError(string.Format("Icon '{0}' width ({1}) and height ({2}) is not equal", icon.name, icon.width, icon.height));

                    if (sizes[i] != icon.width)
                        Debug.LogError(string.Format("Icon '{0}' size ({1}) is not equal to required size ({2})", icon.name, icon.width, sizes[i]));
                }
            }
        }
    }
}