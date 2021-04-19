#ifndef CUSTOM_BRDF_INCLUDED
#define CUSTOM_BRDF_INCLUDED
#include "Surface.hlsl"
#define MIN_REFLECTIVITY 0.04
struct BRDF 
{
    float3 diffuse; // 漫反射
    float3 specular; // 高光/镜面
    float roughness; // 粗糙度
};

float SpecularStrength(Surface surface, BRDF brdf, Light light)
{
    float3 h = SafeNormalize(light.direction + surface.viewDirection);
    float nh2 = Square(saturate(dot(surface.normal, h)));
    float lh2 = Square(saturate(dot(light.direction, h)));
    float r2 = Square(brdf.roughness);
    float d2 = Square(nh2 * (r2 - 1.0) + 1.00001);
    float normalization = brdf.roughness * 4.0 + 2.0;
    return r2 / (d2 * max(0.1, lh2) * normalization);
}

float3 DirectBRDF(Surface surface, BRDF brdf, Light light)
{
    return SpecularStrength(surface, brdf, light) * brdf.specular + brdf.diffuse;
}

// 没有它 非金属将不会获得镜面反射高光
float OneMinusReflectivity(float metallic)
{
    float range = 1.0 - MIN_REFLECTIVITY;
    return range - metallic * range;
}

BRDF GetBRDF(inout Surface surface, bool applyAlphaToDiffuse = false)
{
    BRDF brdf;
    brdf.diffuse = surface.color * OneMinusReflectivity(surface.metallic);
    if (applyAlphaToDiffuse)
    {
        brdf.diffuse *= surface.alpha;
    }
    // brdf.specular = surface.color - brdf.diffuse; // 出射光的量不能超过入射光的量,表明镜面反射颜色应等于表面颜色减去漫反射颜色
    brdf.specular = lerp(MIN_REFLECTIVITY, surface.color, surface.metallic);
    float perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(surface.smoothness);
    brdf.roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
    return brdf;
}

#endif
