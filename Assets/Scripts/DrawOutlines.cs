using UnityEngine;
using UnityEngine.Rendering;

public class DrawOutlines: MonoBehaviour
{
    [Range(0,8)]
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
    [Range(0,4)]
    [SerializeField]
    private int downscaleFactor;


    private CommandBuffer commandBuffer;
    private Camera cam;
    private int _prePassID;
    private int _blurredID;
    private int _tempID;
    private Material blurMaterial;
    private Material outlineMaterial;
    private Material composeMaterial;

    private int screenWidth;
    private int screenHeight;


    private void OnEnable()
    {
        blurMaterial = new Material(Shader.Find("Hidden/Blur"));
        outlineMaterial = new Material(Shader.Find("Hidden/Outline"));
        composeMaterial = new Material(Shader.Find("Hidden/Compose"));

        outlineMaterial.SetColor("_Color", color);
        outlineMaterial.SetInt("_ZTestMode", (int)depthMode);
        composeMaterial.SetFloat("_AlphaMultiplier", addBlend);

        _prePassID = Shader.PropertyToID("_OutlinePrepass");
        _blurredID = Shader.PropertyToID("_Blurred");
        _tempID = Shader.PropertyToID("_Temp");        

        commandBuffer = new CommandBuffer();
        commandBuffer.name = "OUTLINES";
        cam = GetComponent<Camera>();
        cam.AddCommandBuffer(CameraEvent.BeforeImageEffects, commandBuffer);

        screenWidth = Screen.width;
        screenHeight = Screen.height;

        SetupCommandBuffer();
    }
    
    private void SetupCommandBuffer()
    {
        commandBuffer.Clear();
        RenderToTexture();
        Blur();
        ReleaseRTs();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, composeMaterial);
    }

    private void ReleaseRTs()
    {
        commandBuffer.ReleaseTemporaryRT(_prePassID);
        commandBuffer.ReleaseTemporaryRT(_blurredID);
        commandBuffer.ReleaseTemporaryRT(_tempID);
    }

    private void Blur()
    {
        commandBuffer.GetTemporaryRT(_blurredID, screenWidth >> downscaleFactor, screenHeight >> downscaleFactor, 0, FilterMode.Bilinear);
        commandBuffer.GetTemporaryRT(_tempID, screenWidth >> downscaleFactor, screenHeight >> downscaleFactor, 0, FilterMode.Bilinear);
        commandBuffer.Blit(_prePassID, _blurredID);

        for (int i = 0; i < blurIterations; i++)
        {
            commandBuffer.Blit(_blurredID, _tempID, blurMaterial, 0);
            commandBuffer.Blit(_tempID, _blurredID, blurMaterial, 1);
        }        
    }

    private void RenderToTexture()
    {
        commandBuffer.GetTemporaryRT(_prePassID, -1, -1, 16, FilterMode.Bilinear);
        commandBuffer.SetRenderTarget(_prePassID, BuiltinRenderTextureType.CameraTarget);
        commandBuffer.ClearRenderTarget(false, true, Color.clear);
        for (int i = 0; i < targets.Length; i++)
        {
            commandBuffer.DrawRenderer(targets[i], outlineMaterial);
        }
    }

    

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(commandBuffer != null) SetupCommandBuffer();

        if (outlineMaterial == null) return;
        outlineMaterial.SetColor("_Color", color);
        outlineMaterial.SetInt("_ZTestMode", (int)depthMode);

        if (composeMaterial) composeMaterial.SetFloat("_AlphaMultiplier", addBlend);

    }
#endif
}