using TMPro;

using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class Version : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TMP_Text>().text = $"Escape Guan {Application.version}";
    }
}
