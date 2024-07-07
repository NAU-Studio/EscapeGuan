using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BloodAmountEffect : MonoBehaviour
{
    public float Amount;
    public float AmountDecreasement;

    public Image Image => GetComponent<Image>();

    private void Update()
    {
        Image.color = new(Image.color.r, Image.color.g, Image.color.b, Amount);
    }

    private void FixedUpdate()
    {
        if (Amount > 0)
            Amount -= AmountDecreasement * Time.fixedDeltaTime;
        else
            Amount = 0;
    }

    public void SetAmount(float amount)
    {
        Amount = amount;
    }
}
