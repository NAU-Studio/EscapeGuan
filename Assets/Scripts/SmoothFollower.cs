using UnityEngine;

namespace EscapeGuan
{
    public class SmoothFollower : MonoBehaviour
    {
        public float FollowSpeed;

        public Transform Target;

        public Vector3 Offset;

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, Target.position + Offset, FollowSpeed * Time.deltaTime);
        }
    }
}
