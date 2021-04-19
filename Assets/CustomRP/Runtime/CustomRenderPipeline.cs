using UnityEngine;
using UnityEngine.Rendering;

namespace CustomRP.Runtime
{
    // 自定义渲染管线
    public class CustomRenderPipeline : RenderPipeline
    {
        private CameraRenderer renderer = new CameraRenderer();
        private bool useDynamicBatching, useGPUInstancing;
        private ShadowSettings _shadowSettings;
        public CustomRenderPipeline(bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher, ShadowSettings shaodwSettings)
        {
            this.useDynamicBatching = useDynamicBatching;
            this.useGPUInstancing = useGPUInstancing;
            this._shadowSettings = shaodwSettings;
            GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
            GraphicsSettings.lightsUseLinearIntensity = true;
        }
        // context为上下文结构,会提供到当前引擎的连接
        // 一个相机的数组,按照提供的摄像机顺序进行渲染是RP的主要责任
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            // 对相机的渲染任务进行分发
            foreach (Camera camera in cameras)
            {
                renderer.Render(context, camera, useDynamicBatching, useGPUInstancing, _shadowSettings);
            }
        }
    }
}
