﻿using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AssetDanshari
{
    public class AssetDanshariWindow : EditorWindow
    {
        [MenuItem("Tool/Asset Danshari")]
        static void ShowWindow()
        {
            GetWindow<AssetDanshariWindow>();
        }

        private AssetDanshariSetting m_AssetDanshariSetting;
        private Vector2 m_ScrollViewVector2;
        private ReorderableList m_ReorderableList;

        private void Awake()
        {
            titleContent.text = "Asset Danshari";
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnGUI()
        {
            var style = AssetDanshariStyle.Get();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            Rect toolBtnRect = GUILayoutUtility.GetRect(style.help, EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false));
            if (GUI.Button(toolBtnRect, style.help, EditorStyles.toolbarDropDown))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(style.about, false, OnContextAbout);
                menu.DropDown(toolBtnRect);
            }
            EditorGUILayout.EndHorizontal();

            if (m_ReorderableList != null)
            {
                m_ScrollViewVector2 = GUILayout.BeginScrollView(m_ScrollViewVector2);
                m_ReorderableList.DoLayoutList();
                GUILayout.EndScrollView();
            }
        }

        private void Init()
        {
            if (m_AssetDanshariSetting == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:" + typeof(AssetDanshariSetting).Name);
                if (guids.Length == 0)
                {
                    Debug.LogError("Missing AssetDanshariSetting File");
                    return;
                }

                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                m_AssetDanshariSetting = AssetDatabase.LoadAssetAtPath<AssetDanshariSetting>(path);
            }

            if (m_ReorderableList == null)
            {
                m_ReorderableList = new ReorderableList(m_AssetDanshariSetting.assetReferenceInfos, null, true, true, true, true);
                m_ReorderableList.drawHeaderCallback = OnDrawHeaderCallback;
                m_ReorderableList.drawElementCallback = OnDrawElementCallback;
                m_ReorderableList.elementHeight += 55;
            }
        }

        private void OnDrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, AssetDanshariStyle.Get().assetReferenceTitle);
        }

        private void OnDrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (m_AssetDanshariSetting == null || m_AssetDanshariSetting.assetReferenceInfos.Count < index)
            {
                return;
            }

            var info = m_AssetDanshariSetting.assetReferenceInfos[index];
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 2;

            EditorGUI.BeginChangeCheck();
            info.title = EditorGUI.TextField(rect, info.title);
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            Rect rect2 = new Rect(rect) { width = 50f };
            Rect rect3 = new Rect(rect) { x = rect2.x + rect2.width, width = rect.width - rect2.width - 75f };
            Rect rect4 = new Rect(rect) { x = rect3.x + rect3.width + 5f, width = 70f };
            EditorGUI.LabelField(rect2, AssetDanshariStyle.Get().assetReferenceReference);
            info.referenceFolder = EditorGUI.TextField(rect3, info.referenceFolder);
            info.referenceFolder = OnDrawElementAcceptDrop(rect3, info.referenceFolder);
            bool valueChanged = EditorGUI.EndChangeCheck();
            if (GUI.Button(rect4, AssetDanshariStyle.Get().assetReferenceCheckRef))
            {

            }
            
            rect2.y += EditorGUIUtility.singleLineHeight + 2;
            rect3.y += EditorGUIUtility.singleLineHeight + 2;
            rect4.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.LabelField(rect2, AssetDanshariStyle.Get().assetReferenceAsset);
            EditorGUI.BeginChangeCheck();
            info.assetFolder = EditorGUI.TextField(rect3, info.assetFolder);
            info.assetFolder = OnDrawElementAcceptDrop(rect3, info.assetFolder);
            valueChanged |= EditorGUI.EndChangeCheck();
            if (GUI.Button(rect4, AssetDanshariStyle.Get().assetReferenceCheckDup))
            {
                AssetDuplicateWindow.CheckPaths(info.referenceFolder,
                    info.assetFolder, info.assetCommonFolder);
            }

            rect2.width += 25f;
            rect2.y += EditorGUIUtility.singleLineHeight + 2;
            rect3.y += EditorGUIUtility.singleLineHeight + 2;
            rect3.x += 25f;
            rect3.width = rect.width - rect2.width;
            EditorGUI.LabelField(rect2, AssetDanshariStyle.Get().assetReferenceAssetCommon);
            EditorGUI.BeginChangeCheck();
            info.assetCommonFolder = EditorGUI.TextField(rect3, info.assetCommonFolder);
            info.assetCommonFolder = OnDrawElementAcceptDrop(rect3, info.assetCommonFolder);
            valueChanged |= EditorGUI.EndChangeCheck();

            if (valueChanged)
            {
                EditorUtility.SetDirty(m_AssetDanshariSetting);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }

        private string OnDrawElementAcceptDrop(Rect rect, string label)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0 && !string.IsNullOrEmpty(DragAndDrop.paths[0]))
                {
                    if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        GUI.changed = true;

                        return AssetDanshariUtility.PathArrayToStr(DragAndDrop.paths);
                    }
                }
            }

            return label;
        }

        private void OnContextAbout()
        {
            Application.OpenURL("https://github.com/akof1314/UnityAssetDanshari");
        }
    }
}