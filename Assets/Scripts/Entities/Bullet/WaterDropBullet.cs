using UnityEngine;

namespace EscapeGuan.Entities.Bullet
{
    public class WaterDropBullet : Bullet
    {
        public override void Init(Entity thrower, float initvel, float ang)
        {
            base.Init(thrower, initvel, ang);
            Highest = Random.Range(1f, 2);
            transform.eulerAngles = new(0, 0, 180 - ang);
            Rigidbody.angularVelocity = 0;
            Rigidbody.mass = 0.005f;
        }

        public override void Drop()
        {
            Instantiate(GameManager.Templates["water_drop_particle"], transform.position, Quaternion.identity);
            GameManager.Main.PlayAudio(AudioSources.Prop, "se.water.drip");
            base.Drop();
        }
    }
}
