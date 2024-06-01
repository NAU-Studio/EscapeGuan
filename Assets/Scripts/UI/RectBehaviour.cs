using UnityEngine;

namespace EscapeGuan.UI
{
    public abstract class RectBehaviour : MonoBehaviour
    {
        public new RectTransform transform => GetComponent<RectTransform>();
    }
}