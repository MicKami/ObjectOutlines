using UnityEngine;
using UnityEngine.Rendering;

public class DrawOutlines: MonoBehaviour
{
    [SerializeField]
    private Renderer[] targets;
    [SerializeField]
    private Material outlineMaterial;

    private Camera cam;
    private int _prePassID;
    private int _blurredID;


    private void Awake()
    {
        cam = GetComponent<Camera>();
        CommandBuffer commandBuffer = new CommandBuffer();
        commandBuffer.name = "Outlines";
        cam.AddCommandBuffer(CameraEvent.BeforeImageEffects, commandBuffer);

        _prePassID = Shader.PropertyToID("_OutlinePrepass");
        _blurredID = Shader.PropertyToID("_Blurred");

        commandBuffer.GetTemporaryRT(_prePassID, -1, -1, 16, FilterMode.Bilinear);
        commandBuffer.SetRenderTarget(_prePassID, BuiltinRenderTextureType.CurrentActive);
        commandBuffer.ClearRenderTarget(false, true, Color.black);

        for (int i = 0; i < targets.Length; i++)
        {
            commandBuffer.DrawRenderer(targets[i], outlineMaterial);
        }

        commandBuffer.GetTemporaryRT(_blurredID, -2, -2, 16, FilterMode.Bilinear);
        commandBuffer.Blit(_prePassID, _blurredID);

        commandBuffer.Blit(_blurredID, BuiltinRenderTextureType.CameraTarget);
    }
}