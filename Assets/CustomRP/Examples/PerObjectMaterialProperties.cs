using System;
using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{
    private static int baseColorId = Shader.PropertyToID("_BaseColor"),
        cutOffId = Shader.PropertyToID("_CutOff"),
        metallicId = Shader.PropertyToID("_Metallic"),
        smoothnessId = Shader.PropertyToID("_Smoothness");
    [SerializeField]
    Color baseColor = Color.white;

    private static MaterialPropertyBlock _block;

    [SerializeField, Range(0f, 1f)] private float cutoff = 0.5f, metallic = 0f, smoothness = 0.5f;
    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        if (_block == null)
        {
            _block = new MaterialPropertyBlock();
        }
        _block.SetColor(baseColorId, baseColor);
        _block.SetFloat(cutOffId, cutoff);
        _block.SetFloat(metallicId, metallic);
        _block.SetFloat(smoothnessId, smoothness);
        GetComponent<Renderer>().SetPropertyBlock(_block);
    }
}
