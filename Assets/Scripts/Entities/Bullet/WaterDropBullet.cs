using UnityEngine;

namespace EscapeGuan.Entities.Bullet
{
    public class WaterDropBullet : Bullet
    {
        public void Init(Entity thrower, float initvel, float ang, float cr, float cm, float aa)
        {
            base.Start();
            Init(thrower, initvel, ang);
            CriticalRate = cr;
            CriticalMultiplier = cm;
            AttackValue = aa;
            Highest = Random.Range(1f, 2);
            transform.eulerAngles = new(0, 0, 180 - ang);
            Rigidbody.angularVelocity = 0;
        }

        public override void Drop()
        {
            Instantiate(GameManager.Templates["water_drop_particle"], transform.position, Quaternion.identity);
            base.Drop();
        }
    }
}
