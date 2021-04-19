using UnityEngine;
using UnityEngine.Rendering;

// 专门用于渲染单个摄像机的类
// 因为RP中的Render每帧都会调用,通过这个类对每个单独的摄像机去分发,让其做自己的独立渲染
namespace CustomRP.Runtime
{
    public partial class CameraRenderer
    {
        private ScriptableRenderContext _context;

        private Camera _camera;

        private const string BufferName = "Render Camera";

        private CommandBuffer _buffer = new CommandBuffer
        {
            name = BufferName
        };

        private CullingResults _cullingResults;

        private Lighting _lighting = new Lighting();

        public void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing, ShadowSettings shadowSettings)
        {
            this._context = context;
            this._camera = camera;
            PrepareBuffer();
            PrepareForSceneWindow();
            if (!Cull(shadowSettings.maxDistance)) return;
            _buffer.BeginSample(SampleName);
            ExecuteBuffer();
            _lighting.Setup(context, _cullingResults, shadowSettings);
            _buffer.EndSample(SampleName);
            Setup(); 
            // DrawVisibleGeometry方法的工作是绘制相机所能看到的所有几何图形
            DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
            DrawUnsupportedShaders();
            DrawGizmos();
            _lighting.CleanUp(); 
            Submit();
        }

        bool Cull(float maxShadowDistance)
        {
            if (_camera.TryGetCullingParameters(out ScriptableCullingParameters p))
            {
                p.shadowDistance = Mathf.Min(maxShadowDistance, _camera.farClipPlane);
                _cullingResults = _context.Cull(ref p);
                return true;
            }

            return false;
        }

        private void Setup()
        {
            // DrawSkybox只是用于确定是否应该绘制天空盒
            // 为了正确渲染,需要设置视图投影矩阵,即Shader中Unity_MatrixVP矩阵
            // 通过SetupCameraProperties将摄像机的属性应用于上下文,这回设置VP矩阵以及其他一些属性
            _context.SetupCameraProperties(_camera);
            CameraClearFlags flags = _camera.clearFlags;
            // 用于清除上一帧的旧的渲染内容,以进行下一帧的新的渲染内容的渲染
            _buffer.ClearRenderTarget(
                flags <= CameraClearFlags.Depth,
                flags == CameraClearFlags.Color,
                flags == CameraClearFlags.Color ? _camera.backgroundColor.linear : Color.clear
            );
            // 用于在Profiler和FrameDebugger中进行注入
            _buffer.BeginSample(SampleName);
            ExecuteBuffer();
        }
    
        private void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing)
        {
            var sortingSettings = new SortingSettings(_camera)
            {
                criteria = SortingCriteria.CommonOpaque
            };
            var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings)
            {
                enableDynamicBatching = useDynamicBatching,
                enableInstancing = useGPUInstancing
            };
            drawingSettings.SetShaderPassName(1, litShaderTagId);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            
            _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
            // 仅仅这样 并没有使天空盒渲染出来,这是因为我们向上下文发出的命令都是缓冲的,
            // 必须通过在上下文调用Submit来提交进渲染队列的工作才能去执行
        
            // FrameDebugger中,其为Camera.RenderSkybox条目,
            // 它下面有一个DrawMesh项,代表实际的DrawCall
            _context.DrawSkybox(_camera);

            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;
            _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
        }

        private void Submit()
        {
            // 用于在Profiler和FrameDebugger中进行注入
            _buffer.EndSample(SampleName);
            ExecuteBuffer();
            _context.Submit();
        }

        private void ExecuteBuffer()
        {
            // 执行缓冲区 ExecuteCommandBuffer会从缓冲区复制命令但不会清除它.
            // 如果要重用就必须让清除和执行一起完成.
            _context.ExecuteCommandBuffer(_buffer);
            _buffer.Clear();
        }
    }
}
