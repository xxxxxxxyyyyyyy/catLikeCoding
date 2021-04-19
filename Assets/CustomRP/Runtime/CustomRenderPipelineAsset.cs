using UnityEngine;
using UnityEngine.Rendering;

namespace CustomRP.Runtime
{
    // 自定义管线的资产文件
    // CreateAssetMenu来在编辑器下拉菜单中添加创建资产选项
    // 返回一个CustomRenderPipeline自定义渲染管线的新实例
    // 这会提供一个有效的,附带功能的管线实例,尽管它现在还没有提供任何功能 
    [CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
    public class CustomRenderPipelineAsset : RenderPipelineAsset
    {
        [SerializeField] private bool useDynamicBatching, useGPUInstancing, useSRPBatcher;
        [SerializeField] private ShadowSettings shadows = default;
        protected override RenderPipeline CreatePipeline()
        {
            return new CustomRenderPipeline(useDynamicBatching, useGPUInstancing, useSRPBatcher, shadows);
        }
    }
}
