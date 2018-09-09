﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetDanshari
{
    public class AssetDuplicateWindow : EditorWindow
    {
        public static void CheckPaths(string refPaths, string paths, string commonPaths)
        {
            var window = GetWindow<AssetDuplicateWindow>();
            window.Focus();
            window.SetCheckPaths(refPaths, paths, commonPaths);
        }

        private SearchField m_SearchField;
        private AssetDuplicateTreeModel m_DuplicateTreeModel;
        private AssetDuplicateTreeView m_DuplicateTreeView;

        [SerializeField]
        private TreeViewState m_TreeViewState;
        [SerializeField]
        private MultiColumnHeaderState m_MultiColumnHeaderState;

        private void Awake()
        {
            titleContent = AssetDanshariStyle.Get().duplicateTitle;
            minSize = new Vector2(727f, 331f);
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnGUI()
        {
            var style = AssetDanshariStyle.Get();
            style.InitGUI();

            if (m_DuplicateTreeModel.assetPaths != null)
            {
                if (m_DuplicateTreeModel.dataCount == 0)
                {
                    ShowNotification(style.duplicateNothing);
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                    if (GUILayout.Button(style.expandAll, EditorStyles.toolbarButton, GUILayout.Width(50f)))
                    {
                        m_DuplicateTreeView.ExpandAll();
                    }
                    if (GUILayout.Button(style.collapseAll, EditorStyles.toolbarButton, GUILayout.Width(50f)))
                    {
                        m_DuplicateTreeView.CollapseAll();
                    }
                    m_DuplicateTreeView.searchString = m_SearchField.OnToolbarGUI(m_DuplicateTreeView.searchString);
                    if (GUILayout.Button(style.exportCsv, EditorStyles.toolbarButton, GUILayout.Width(70f)))
                    {
                        m_DuplicateTreeModel.ExportCsv();
                    }
                    EditorGUILayout.EndHorizontal();
                    m_DuplicateTreeView.OnGUI(GUILayoutUtility.GetRect(0, 100000, 0, 100000));
                }
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(m_DuplicateTreeModel.assetPaths, style.labelStyle);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                ShowNotification(style.duplicateWaiting);
            }
        }

        private void Init()
        {
            if (m_SearchField != null)
            {
                return;
            }

            if (m_TreeViewState == null)
            {
                m_TreeViewState = new TreeViewState();
            }

            bool firstInit = m_MultiColumnHeaderState == null;
            var headerState = CreateMultiColumnHeader();
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
            m_MultiColumnHeaderState = headerState;

            var multiColumnHeader = new AssetMultiColumnHeader(headerState);
            if (firstInit)
            {
                multiColumnHeader.ResizeToFit();
            }

            m_DuplicateTreeModel = new AssetDuplicateTreeModel();
            m_DuplicateTreeView = new AssetDuplicateTreeView(m_TreeViewState, multiColumnHeader, m_DuplicateTreeModel);

            m_SearchField = new SearchField();
        }

        private MultiColumnHeaderState CreateMultiColumnHeader()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = AssetDanshariStyle.Get().duplicateHeaderContent,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 200,
                    minWidth = 150,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = AssetDanshariStyle.Get().duplicateHeaderContent2,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 300,
                    minWidth = 100,
                    autoResize = false,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = AssetDanshariStyle.Get().duplicateHeaderContent3,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 60,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = AssetDanshariStyle.Get().duplicateHeaderContent4,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 110,
                    minWidth = 60,
                    autoResize = true
                }
            };

            return new MultiColumnHeaderState(columns);
        }

        private void SetCheckPaths(string refPaths, string paths, string commonPaths)
        {
            RemoveNotification();
            m_DuplicateTreeModel.SetDataPaths(refPaths, paths, commonPaths);
            if (m_DuplicateTreeModel.dataCount != 0)
            {
                m_DuplicateTreeView.Reload();
                m_DuplicateTreeView.ExpandAll();
            }
        }
    }
}
