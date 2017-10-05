using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Materials")]
    public Material OriginalMaterial;
    public Material HighlightMaterial;
    public Material KillMaterial;

    private Renderer rend;

    void Start ()
    {
        rend = GetComponent<Renderer>();
    }

    public void Highlight(bool isKill = false)
    {
        rend.material = isKill ? KillMaterial : HighlightMaterial;
    }

    public void UnHighlight()
    {
        rend.material = OriginalMaterial;
    }
}
