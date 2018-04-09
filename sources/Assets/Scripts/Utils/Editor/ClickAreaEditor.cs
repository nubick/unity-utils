using UnityEditor;

namespace Assets.Scripts.Utils.Editor
{
    [CustomEditor(typeof(ClickArea))]
    public class ClickAreaEditor : UnityEditor.Editor
    {
        //Hide any additional settings in ClickArea component. I don't need color or raycast target. They don't have sense here.
        public override void OnInspectorGUI()
        {
        }
    }
}