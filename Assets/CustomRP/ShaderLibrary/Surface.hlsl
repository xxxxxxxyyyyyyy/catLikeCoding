#ifndef CUSTOM_SURFACE_INCLUDED
#define CUSTOM_SURFACE_INCLUDED

struct Surface   
{
    float3 position;
    float3 normal; // 表面法线
    float3 viewDirection;
    float depth;
    float3 color; // 基础色
    float alpha; // alpha
    float metallic; // 金属度
    float smoothness; // 光滑度
    float dither;
};

#endif
