using UnityEngine;
using UnityEngine.Rendering;

public class DrawOutlines: MonoBehaviour
{
    [Range(1,16)]
    [SerializeField]
    private int blurIterations;    
    [SerializeField]
    private Renderer[] targets;
    [SerializeField]
    private Color color;
    [Range(0, 1)]
    [SerializeField]
    private float addBlend;
    [SerializeField]
    private CompareFunction depthMode;


    private CommandBuffer commandBuffer;
    private Camera cam;
    private int _prePassID;
    private int _blurredID;
    private int _tempID;
    private int _screenID;
    private Material blurMaterial;
    private Material outlineMaterial;
    private Material composeMaterial;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        blurMaterial = new Material(Shader.Find("Hidden/Blur"));
        outlineMaterial = new Material(Shader.Find("Hidden/Outline"));
        composeMaterial = new Material(Shader.Find("Hidden/Compose"));
        commandBuffer = new CommandBuffer();
        commandBuffer.name = "OUTLINES";
        cam.AddCommandBuffer(CameraEvent.BeforeImageEffects, commandBuffer);

        _prePassID = Shader.PropertyToID("_OutlinePrepass");
        _blurredID = Shader.PropertyToID("_Blurred");
        _tempID = Shader.PropertyToID("_Temp");
        _screenID = Shader.PropertyToID("_Screen");

        outlineMaterial.SetColor("_Color", color);
        outlineMaterial.SetInt("_ZTestMode", (int)depthMode);
        composeMaterial.SetFloat("_AlphaMultiplier", addBlend);

        UpdateCommandBuffer();
    }
    
    private void UpdateCommandBuffer()
    {
        if (commandBuffer == null) return;
        commandBuffer.Clear();
        commandBuffer.GetTemporaryRT(_screenID, -1, -1, 16, FilterMode.Bilinear);
        commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, _screenID);
        commandBuffer.GetTemporaryRT(_prePassID, -1, -1, 16, FilterMode.Bilinear);
        commandBuffer.SetRenderTarget(_prePassID, BuiltinRenderTextureType.CameraTarget);
        commandBuffer.ClearRenderTarget(false, true, Color.clear);


        for (int i = 0; i < targets.Length; i++)
        {
            commandBuffer.DrawRenderer(targets[i], outlineMaterial);
        }

        commandBuffer.GetTemporaryRT(_blurredID, -2, -2, 0, FilterMode.Bilinear);
        commandBuffer.GetTemporaryRT(_tempID, -2, -2, 0, FilterMode.Bilinear);
        commandBuffer.Blit(_prePassID, _blurredID);

        for (int i = 0; i < blurIterations; i++)
        {
            commandBuffer.Blit(_blurredID, _tempID, blurMaterial, 0);
            commandBuffer.Blit(_tempID, _blurredID, blurMaterial, 1);
        }
        commandBuffer.ReleaseTemporaryRT(_tempID);

        commandBuffer.Blit(_screenID, BuiltinRenderTextureType.CameraTarget, composeMaterial);
        commandBuffer.ReleaseTemporaryRT(_prePassID);
        commandBuffer.ReleaseTemporaryRT(_blurredID);
        commandBuffer.ReleaseTemporaryRT(_screenID);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateCommandBuffer();
        if (outlineMaterial == null) return;
        outlineMaterial.SetColor("_Color", color);
        outlineMaterial.SetInt("_ZTestMode", (int)depthMode);

        if (composeMaterial == null) return;
        composeMaterial.SetFloat("_AlphaMultiplier", addBlend);

    }
#endif
}