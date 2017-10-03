using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Materials")]
    public Material OriginalMaterial;
    public Material HighlightMaterial;

    private Renderer rend;

    void Start ()
    {
        rend = GetComponent<Renderer>();
    }

    public void Highlight()
    {
        rend.material = HighlightMaterial;
    }

    public void UnHighlight()
    {
        rend.material = OriginalMaterial;
    }
}
