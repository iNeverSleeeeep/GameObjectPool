using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Fusion
{
    [CustomEditor(typeof(PoolPresets))]
    public class PoolPresetsEditor : UnityEditor.Editor
    {
        private SerializedProperty m_PoolPresets;

        private ReorderableList m_PoolPresetsList;

        private int m_SelectIndex;

        private void OnEnable()
        {
            if (target == null)
                return;

            m_SelectIndex = 0;

            m_PoolPresets = serializedObject.FindProperty("poolPresets");

            m_PoolPresetsList = new ReorderableList(serializedObject, m_PoolPresets);
            m_PoolPresetsList.drawHeaderCallback = DrawHeaderCallback;
            m_PoolPresetsList.elementHeightCallback = ElementHeightCallback;
            m_PoolPresetsList.drawElementCallback = DrawElementCallback;
            m_PoolPresetsList.onAddCallback = OnAddCallback;
        }

        private void DrawHeaderCallback(Rect rect)
        {
            GUI.Label(rect, "数量：" + m_PoolPresets.arraySize);
        }

        private float ElementHeightCallback(int index)
        {
            if (m_SelectIndex ==  index)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            Debug.LogErrorFormat("DrawElementCallback {0} {1} {2}", index, isActive, isFocused);
            var asset = m_PoolPresets.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, asset);
        }

        private void OnAddCallback(ReorderableList list)
        {
            m_PoolPresets.arraySize++;
            EditorUtility.SetDirty(target);
        }
    }
}

