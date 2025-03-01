using UnityEngine;
using UnityEngine.Rendering;

public class ExampleRenderPipelineInstance2 : RenderPipeline
{
    // Use this variable to a reference to the Render Pipeline Asset that was passed to the constructor

    // The constructor has an instance of the ExampleRenderPipelineAsset class as its parameter.
    public ExampleRenderPipelineInstance2() {
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        var cmd = new CommandBuffer();
        cmd.ClearRenderTarget(true, true, Color.red);
        context.ExecuteCommandBuffer(cmd);
        cmd.Release();

        context.Submit();
    }
}
