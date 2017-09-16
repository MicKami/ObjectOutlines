using UnityEngine;
using UnityEngine.Rendering;

public class DrawOutlines: MonoBehaviour
{
    [Range(0,16)]
    [SerializeField]
    private int blurIterations;
    [SerializeField]
    private Material outlineMaterial;
    [SerializeField]
    private Renderer[] targets;


    private CommandBuffer commandBuffer;
    private Camera cam;
    private int _prePassID;
    private int _blurredID;
    private int _tempID;
    private Material blurMaterial;


    private void Awake()
    {
        cam = GetComponent<Camera>();
        blurMaterial = new Material(Shader.Find("Hidden/Blur"));
        commandBuffer = new CommandBuffer();
        commandBuffer.name = "Outlines";
        cam.AddCommandBuffer(CameraEvent.BeforeImageEffects, commandBuffer);

        _prePassID = Shader.PropertyToID("_OutlinePrepass");
        _blurredID = Shader.PropertyToID("_Blurred");
        _tempID = Shader.PropertyToID("_Temp");
        UpdateCommandBuffer();
    }

    private void OnValidate()
    {
        UpdateCommandBuffer();
    }

    private void UpdateCommandBuffer()
    {
        if (commandBuffer == null) return;
        commandBuffer.Clear();
        commandBuffer.GetTemporaryRT(_prePassID, -1, -1, 0, FilterMode.Bilinear);
        commandBuffer.SetRenderTarget(_prePassID, BuiltinRenderTextureType.CurrentActive);
        commandBuffer.ClearRenderTarget(false, true, Color.black);

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

        commandBuffer.Blit(_blurredID, BuiltinRenderTextureType.CameraTarget);
        commandBuffer.ReleaseTemporaryRT(_prePassID);
        commandBuffer.ReleaseTemporaryRT(_blurredID);
        commandBuffer.ReleaseTemporaryRT(_tempID);
    }
}