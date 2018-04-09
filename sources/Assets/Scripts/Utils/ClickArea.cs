using UnityEngine.UI;

namespace Assets.Scripts.Utils
{
    public class ClickArea : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}