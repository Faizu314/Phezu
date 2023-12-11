using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GradientGen : MonoBehaviour
{
    [Serializable]
    private struct GradientData {
        public Color Color;
        [Range(0f, 1f)] public float From;
        [Range(0f, 1f)] public float To;
        [Header("Opacity")]
        [Range(0f, 1f)] public float _From;
        [Range(0f, 1f)] public float _To;
    }

    private enum GradientAxis { X, Y, Z };
    [SerializeField] private GradientAxis m_GradientAxis;
    [SerializeField] private bool m_ReverseGradient;
    [SerializeField] private List<GradientData> m_GradientData;

    private Gradient m_Gradient;
    private Renderer m_Renderer;
    private MaterialPropertyBlock m_MPB;
    private int m_AxisXID;
    private int m_AxisYID;
    private int m_AxisZID;
    private int m_GradientTexID;

    private void OnEnable() {
        Init();
    }

    private void LateUpdate() {
        if (!m_MPB.HasTexture(m_GradientTexID))
            Init();

        Apply();
    }

    private void Init() {
        m_Renderer = GetComponent<Renderer>();
        m_MPB = new();
        m_Renderer.GetPropertyBlock(m_MPB);
        m_AxisXID = Shader.PropertyToID("_AxisX");
        m_AxisYID = Shader.PropertyToID("_AxisY");
        m_AxisZID = Shader.PropertyToID("_AxisZ");
        m_GradientTexID = Shader.PropertyToID("_GradientTex");
        if (m_Gradient == null)
            m_Gradient = new();
    }

    private void Apply() {
        GradientColorKey[] colorKeys = new GradientColorKey[m_GradientData.Count];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[m_GradientData.Count * 2];

        for (int i = 0; i < m_GradientData.Count; i++) {
            colorKeys[i].color = m_GradientData[i].Color;
            colorKeys[i].time = (m_GradientData[i].To + m_GradientData[i].From) / 2f;
        }

        for (int i = 0; i < m_GradientData.Count; i++) {
            int j = i * 2;

            alphaKeys[j].alpha = m_GradientData[i]._From;
            alphaKeys[j].time = m_GradientData[i].From;

            alphaKeys[j + 1].alpha = m_GradientData[i]._To;
            alphaKeys[j + 1].time = m_GradientData[i].To;
        }


        m_Gradient.SetKeys(colorKeys, alphaKeys);
        var tex = m_MPB.GetTexture(m_GradientTexID) as Texture2D;

        if (tex == null || tex.height != 4 || tex.width != 256) {
            tex = new Texture2D(256, 4);
            tex.alphaIsTransparency = true;
        }

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 256; j++) {
                var color = m_Gradient.Evaluate((float)j / 256f);
                if (m_ReverseGradient)
                    tex.SetPixel(j, i, color);
                else
                    tex.SetPixel(256 - j, i, color);
            }
        }
        tex.Apply();

        m_MPB.SetTexture(m_GradientTexID, tex);
        m_MPB.SetFloat(m_AxisXID, (m_GradientAxis == GradientAxis.X) ? 1f : 0f);
        m_MPB.SetFloat(m_AxisYID, (m_GradientAxis == GradientAxis.Y) ? 1f : 0f);
        m_MPB.SetFloat(m_AxisZID, (m_GradientAxis == GradientAxis.Z) ? 1f : 0f);
        m_MPB.SetVector("_ObjectBounds", m_Renderer.bounds.size);

        m_Renderer.SetPropertyBlock(m_MPB);
    }
}
