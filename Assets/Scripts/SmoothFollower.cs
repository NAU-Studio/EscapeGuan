using UnityEngine;

namespace EscapeGuan
{
    public class SmoothFollower : MonoBehaviour
    {
        public float FollowSpeed;

        public Transform Target;

        public Vector3 Offset;

        public float ShakeDrag;
        private float ShakeRange;

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, Target.position + Offset, FollowSpeed * Time.deltaTime) + new Vector3(Random.Range(-ShakeRange, ShakeRange), Random.Range(-ShakeRange, ShakeRange));
        }

        public void Shake(float range)
        {
            ShakeRange += range;
        }

        private void FixedUpdate()
        {
            if (ShakeRange > 0)
                ShakeRange -= ShakeDrag * Time.fixedDeltaTime;
            else
                ShakeRange = 0;
        }
    }
}
