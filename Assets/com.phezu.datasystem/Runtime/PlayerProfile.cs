using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerProfile : ISerializationCallbackReceiver {

    public string Version;

    [NonSerialized] public Dictionary<string, string> Data;

    #region Serialization

    //Unity does not serialize dictionaries by default

    [SerializeField] private List<string> m_Keys;
    [SerializeField] private List<string> m_Values;

    public void OnAfterDeserialize() {
        Data = new();

        for (int i = 0; i != Math.Min(m_Keys.Count, m_Values.Count); i++)
            Data.Add(m_Keys[i], m_Values[i]);
    }

    public void OnBeforeSerialize() {
        m_Keys = new();
        m_Values = new();

        if (Data == null)
            return;

        foreach (var kvp in Data) {
            m_Keys.Add(kvp.Key);
            m_Values.Add(kvp.Value);
        }
    }

    #endregion
}