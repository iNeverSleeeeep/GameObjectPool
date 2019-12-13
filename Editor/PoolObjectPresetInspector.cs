using Nirvana.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Fusion.Editor
{
    [CustomPropertyDrawer(typeof(PoolPreset))]
    sealed class PoolPresetDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var preloadCount = property.FindPropertyRelative("preloadCount");
            return preloadCount.intValue > 0 ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var preloadCount = property.FindPropertyRelative("preloadCount");
            var preloadPerFrame = property.FindPropertyRelative("preloadPerFrame");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("预加载数量：");
            preloadCount.intValue = EditorGUILayout.IntField(preloadCount.intValue);
            EditorGUILayout.EndHorizontal();
            if (preloadCount.intValue > 0)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("每帧预加载数量：");
                preloadPerFrame.intValue = EditorGUILayout.IntField(preloadPerFrame.intValue);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            var autoRelease = property.FindPropertyRelative("autoRelease");
            EditorGUILayout.LabelField("自动释放状态：" + (autoRelease.intValue > 0 ? "开启" : "关闭"));
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("自动释放剩余数量：");
            autoRelease.intValue = EditorGUILayout.IntField(autoRelease.intValue);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();

            var maxActiveObjects = property.FindPropertyRelative("maxActiveObjects");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("最大活跃对象数量：");
            maxActiveObjects.intValue = EditorGUILayout.IntField(maxActiveObjects.intValue);
            EditorGUILayout.EndHorizontal();
        }
    }

    [CustomEditor(typeof(PoolObjectPreset))]
    internal sealed class PoolObjectPresetInspector : UnityEditor.Editor
    {
        private SerializedProperty m_Preset;
        private SerializedProperty m_Assets;
        private ReorderableList m_AssetsList;

        private void OnEnable()
        {
            if (target == null)
                return;
            m_Preset = serializedObject.FindProperty("preset");
            m_Assets = serializedObject.FindProperty("assets");

            m_AssetsList = new ReorderableList(serializedObject, m_Assets);
            m_AssetsList.drawHeaderCallback = DrawHeaderCallback;
            m_AssetsList.elementHeightCallback = ElementHeightCallback;
            m_AssetsList.drawElementCallback = DrawElementCallback;
            m_AssetsList.onAddCallback = OnAddCallback;
        }

        private void DrawHeaderCallback(Rect rect)
        {
            GUI.Label(rect, "数量：" + m_Assets.arraySize);
        }

        private float ElementHeightCallback(int index)
        {
            return AssetIDDrawer.Height;
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var asset = m_Assets.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, asset);
        }

        private void OnAddCallback(ReorderableList list)
        {
            m_Assets.arraySize++;
            EditorUtility.SetDirty(target);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(m_Preset);

            EditorGUILayout.Space();

            m_Assets.isExpanded = EditorGUILayout.Foldout(this.m_Assets.isExpanded, "资源列表");
            if (m_Assets.isExpanded)
                m_AssetsList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
