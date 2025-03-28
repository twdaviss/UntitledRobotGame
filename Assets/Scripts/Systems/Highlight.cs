using UnityEngine;
using static GameManager;

public class Highlight : MonoBehaviour
{
    [SerializeField] private Material shaderMaterial;

    private Material defaultMaterial;
    private void OnEnable()
    {
        defaultMaterial = GetComponent<SpriteRenderer>().material;
        GameManager.onHighlight += Hightlight;
        GameManager.onUnHighlight += UnHightlight;
    }

    private void OnDisable()
    {
        GameManager.onHighlight -= Hightlight;
        GameManager.onUnHighlight -= UnHightlight;
    }

    private void Hightlight()
    {
        GetComponentInParent<SpriteRenderer>().material = shaderMaterial;
    }
    private void UnHightlight()
    {
        GetComponentInParent<SpriteRenderer>().material = defaultMaterial;
    }
}
