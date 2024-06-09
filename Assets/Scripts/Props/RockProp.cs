using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RockProp : PropEntity
{
    public GameObject ParticleTemplate;

    public override void Kill()
    {
        GameObject g = Instantiate(ParticleTemplate, transform.position, Quaternion.identity);
        g.GetComponent<ParticleSystem>().Play();
        base.Kill();
    }
}
