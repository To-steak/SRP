/* 
This is a simplified example of a custom Scriptable Render Pipeline.
It demonstrates how a basic render loop works.
It shows the clearest workflow, rather than the most efficient runtime performance.
*/

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;

public class ExampleRenderPipeline : RenderPipeline
{
    public ExampleRenderPipeline()
    {
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        // Create and schedule a command to clear the current render target
        // var cmd = new CommandBuffer();
        // cmd.ClearRenderTarget(true, true, Color.black);
        // context.ExecuteCommandBuffer(cmd);
        // cmd.Release();

        // obsolete 되었음, context를 통한 render는 이제 지양되고 command buffer를 통한 render를 지향한다.
        // Iterate over all Cameras
        // foreach (Camera camera in cameras)
        // {
        //     Get the culling parameters from the current Camera
        //     camera.TryGetCullingParameters(out var cullingParameters);

        //     Use the culling parameters to perform a cull operation, and store the results
        //     var cullingResults = context.Cull(ref cullingParameters);

        //     Update the value of built-in shader variables, based on the current Camera
        //     context.SetupCameraProperties(camera);

        //     Tell Unity which geometry to draw, based on its LightMode Pass tag value
        //     ShaderTagId shaderTagId = new ShaderTagId("ExampleLightModeTag");

        //     Tell Unity how to sort the geometry, based on the current Camera
        //     var sortingSettings = new SortingSettings(camera);

        //     Create a DrawingSettings struct that describes which geometry to draw and how to draw it
        //     DrawingSettings drawingSettings = new DrawingSettings(shaderTagId, sortingSettings);

        //     Tell Unity how to filter the culling results, to further specify which geometry to draw
        //     Use FilteringSettings.defaultValue to specify no filtering
        //     FilteringSettings filteringSettings = FilteringSettings.defaultValue;

        //     Schedule a command to draw the geometry, based on the settings you have defined
        //     context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

        //     Schedule a command to draw the Skybox if required
        //     if (camera.clearFlags == CameraClearFlags.Skybox && RenderSettings.skybox != null)
        //     {
        //         context.DrawSkybox(camera);
        //     }

        //     Instruct the graphics API to perform all scheduled commands
        //     context.Submit();
        // }

        // new 버전, context는 이제 command buffer를 실행하는 역할만을 하게된다.
        RendererList rendererList;
        CommandBuffer commandBuffer = new CommandBuffer();
        commandBuffer.ClearRenderTarget(true, true, Color.black);
        context.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Release();

        foreach (Camera camera in cameras)
        {
            camera.TryGetCullingParameters(out var cullingParameters);

            var cullingResults = context.Cull(ref cullingParameters);

            context.SetupCameraProperties(camera);

            ShaderTagId shaderTagId = new ShaderTagId("ExampleLightModeTag");

            var sortingSettings = new SortingSettings(camera);

            DrawingSettings drawingSettings = new DrawingSettings(shaderTagId, sortingSettings);

            FilteringSettings filteringSettings = FilteringSettings.defaultValue;

            RendererListParams rendererListParams =
              new RendererListParams(cullingResults, drawingSettings, filteringSettings);

            rendererList = context.CreateRendererList(ref rendererListParams);
            commandBuffer = new CommandBuffer();
            commandBuffer.DrawRendererList(rendererList);
            context.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Release();

            if (camera.clearFlags == CameraClearFlags.Skybox && RenderSettings.skybox != null)
            {
                rendererList = context.CreateSkyboxRendererList(camera);
                commandBuffer = new CommandBuffer();
                commandBuffer.DrawRendererList(rendererList);
                context.ExecuteCommandBuffer(commandBuffer);
                commandBuffer.Release();
            }

            context.Submit();

        }
    }
}