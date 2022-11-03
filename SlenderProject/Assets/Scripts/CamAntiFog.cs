using UnityEngine;

public class CamAntiFog : MonoBehaviour
{
    private bool prevFogState;

    private void OnPreRender()
    {
        prevFogState = RenderSettings.fog;
        RenderSettings.fog = false;
    }

    private void OnPostRender()
    {
        RenderSettings.fog = prevFogState;
    }
}
