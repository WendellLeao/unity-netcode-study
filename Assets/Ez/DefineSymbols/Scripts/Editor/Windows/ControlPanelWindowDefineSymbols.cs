// Copyright (c) 2016 - 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using QuickEditor;
using QuickEngine.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;

namespace Ez
{
    public partial class ControlPanelWindow : QWindow
    {
#if EZ_DEFINE_SYMBOLS
        public const string DEFAULT_PRESET_NAME = "-- New Preset";
        public static string newPresetName = "";

        private List<string> symbols, presetSymbols;
        private ReorderableList presetSymbolsReordableList;
        private BuildTargetGroup selectedBuildTargetGroup, previouslySelectedBuildTargetGroup;

        private string[] presets;
        private string selectedPresetName = DEFAULT_PRESET_NAME;
        private int selectedPresetIndex = 0;
        private string loadedPresetName = DEFAULT_PRESET_NAME;

        private const int LIST_ITEMS_VERTICAL_PADDING = 0;

        private AnimBool selectedBuildTargetGroupIsTheActivePlatform;
        private AnimBool defineSymbolsNewPreset;

        private static string[] GetDefineSymbolsPresetsFileNames
        {
            get
            {
                List<string> list = new List<string> { DEFAULT_PRESET_NAME };
                string[] presetNames = QuickEngine.IO.File.GetFilesNames(EZT.RELATIVE_PATH_DEFINE_SYMBOLS_PRESETS, "asset");
                if(presetNames != null && presetNames.Length > 0) { list.AddRange(presetNames); }
                return list.ToArray();
            }
        }

        void DrawDefineSymbols()
        {
            DrawPageHeader("DEFINE SYMBOLS", QColors.Green, "Preprocessor Directives Manager", QUI.IsProSkin ? QColors.UnityLight : QColors.UnityMild, EZResources.IconDefineSymbols);
            QUI.Space(SPACE_16);

            if(EditorApplication.isCompiling) { QUI.GhostTitle("Editor is compiling...", QColors.Color.Gray); return; }
            selectedBuildTargetGroupIsTheActivePlatform.target = selectedBuildTargetGroup == QUtils.GetActiveBuildTargetGroup();
            QUI.BeginHorizontal(WindowSettings.CurrentPageContentWidth);
            {
                DrawDefineSymbolsBuildTargetGroup(WindowSettings.CurrentPageContentWidth / 2);
                DrawDefineSymbolsSymbolsPreset(WindowSettings.CurrentPageContentWidth / 2);
            }
            QUI.EndHorizontal();
        }

        void DrawDefineSymbolsBuildTargetGroup(float width)
        {
            QUI.BeginVertical(width);
            {
                QUI.GhostTitle("BUILD TARGET GROUP", QColors.Color.Blue, width);
                QUI.Space(SPACE_4);
                QUI.BeginHorizontal(width);
                {
                    QUI.Space(SPACE_8);
                    if(QUI.GhostButton("Copy to Symbols Preset   >>>", QColors.Color.Blue, width - SPACE_8, 24))
                    {
                        presetSymbols.Clear();
                        presetSymbols.AddRange(symbols);
                    }
                    QUI.FlexibleSpace();
                }
                QUI.EndHorizontal();
                QUI.Space(SPACE_4);
                QUI.BeginHorizontal(width);
                {
                    QUI.Space(SPACE_8);
                    QUI.SetGUIBackgroundColor(EditorGUIUtility.isProSkin ? QColors.Blue.Color : QColors.BlueLight.Color);
                    selectedBuildTargetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup(selectedBuildTargetGroup, GUILayout.Width(width - SPACE_8 - (106 * selectedBuildTargetGroupIsTheActivePlatform.faded)));
                    QUI.ResetColors();
                    if(selectedBuildTargetGroupIsTheActivePlatform.faded > 0.05f)
                    {
                        QUI.Label("is the Active Platform", Style.Text.Small, 106 * selectedBuildTargetGroupIsTheActivePlatform.faded);
                    }
                    QUI.FlexibleSpace();
                }
                QUI.EndHorizontal();

                if(selectedBuildTargetGroup != previouslySelectedBuildTargetGroup)
                {
                    symbols = QUtils.GetScriptingDefineSymbolsForGroup(selectedBuildTargetGroup);
                    previouslySelectedBuildTargetGroup = selectedBuildTargetGroup;
                    Repaint();
                }

                QUI.Space(SPACE_8 + SPACE_16);

                QUI.BeginHorizontal(width);
                {
                    QUI.Space(SPACE_8);
                    DrawActiveSymbolsList(width - 26);
                    QUI.FlexibleSpace();
                }
                QUI.EndHorizontal();
            }
            QUI.EndVertical();
        }

        void DrawDefineSymbolsSymbolsPreset(float width)
        {
            QUI.BeginVertical(width);
            {
                QUI.GhostTitle("SYMBOLS PRESET", QColors.Color.Green, width);
                QUI.Space(SPACE_4);
                QUI.BeginHorizontal(width);
                {
                    QUI.Space(SPACE_8);
                    if(QUI.GhostButton("<<<   Copy to Build Target Group", QColors.Color.Green, width - SPACE_8, 24))
                    {
                        List<string> tempList = presetSymbols;
                        tempList = QUtils.CleanList(tempList);
                        presetSymbols.Clear();
                        presetSymbols.AddRange(tempList);
                        symbols.Clear();
                        symbols.AddRange(presetSymbols);
                        QUtils.SetScriptingDefineSymbolsForGroup(selectedBuildTargetGroup, symbols);
                        if(presetSymbols.Count == 0) { presetSymbols.Add(""); }
                    }
                    QUI.FlexibleSpace();
                }
                QUI.EndHorizontal();
                QUI.Space(SPACE_4);
                if(defineSymbolsNewPreset.value) //NEW PRESET
                {
                    QUI.BeginHorizontal(width);
                    {
                        QUI.Space(SPACE_8);
                        QUI.Label("Enter a new preset name...", Style.Text.Small, width - SPACE_8);
                    }
                    QUI.EndHorizontal();

                    QUI.Space(-SPACE_4);

                    QUI.BeginHorizontal(width);
                    {
                        QUI.Space(SPACE_8);
                        GUI.SetNextControlName("newPresetName");
                        newPresetName = QUI.TextField(newPresetName, EditorGUIUtility.isProSkin ? QColors.Green.Color : QColors.GreenLight.Color, (width - SPACE_8 - 16 - 2 - 16) * defineSymbolsNewPreset.faded);

                        //if the user hits Enter while either the key or value fields were being edited
                        bool keyboardReturnPressed = Event.current.isKey &&
                                                  Event.current.keyCode == KeyCode.Return &&
                                                  Event.current.type == EventType.KeyUp &&
                                                  (GUI.GetNameOfFocusedControl() == "newPresetName");

                        QUI.Space(-2);
                        if(QUI.ButtonOk() || keyboardReturnPressed)
                        {
                            newPresetName = newPresetName.Trim();
                            if(newPresetName.Equals(DEFAULT_PRESET_NAME))
                            {
                                QUI.DisplayDialog("Enter a new preset name", "You are trying to save a preset with the defaut preset name '" + DEFAULT_PRESET_NAME + "'. Please enter another preset name.", "Ok");
                            }
                            else if(string.IsNullOrEmpty(newPresetName))
                            {
                                QUI.DisplayDialog("Enter a new preset name", "You are trying to save a preset with no name. Please enter a name.", "Ok");
                            }
                            else if(Q.GetResource<DefineSymbols.DefineSymbolsPreset>(EZT.RESOURCES_PATH_DEFINE_SYMBOLS_PRESETS, newPresetName) != null)
                            {
                                if(QUI.DisplayDialog("Overwrite preset?", "There is a preset with the same name '" + newPresetName + "' in the presets list. Are you sure you want to overwrite it?", "Overwrite", "Cancel"))
                                {
                                    AssetDatabase.MoveAssetToTrash(EZT.RELATIVE_PATH_DEFINE_SYMBOLS_PRESETS + newPresetName + ".asset");
                                    SavePreset(presetSymbols, newPresetName);
                                    selectedPresetName = newPresetName;
                                    RefreshPresetNames();
                                    defineSymbolsNewPreset.target = false;
                                    newPresetName = "";
                                }
                            }
                            else
                            {
                                SavePreset(presetSymbols, newPresetName);
                                selectedPresetName = newPresetName;
                                RefreshPresetNames();
                                defineSymbolsNewPreset.target = false;
                                newPresetName = "";
                            }
                        }
                        QUI.Space(2);

                        //if the user hits Escape while either the key or value fields were being edited
                        bool keyboardEscapePressed = Event.current.isKey &&
                                                     Event.current.keyCode == KeyCode.Escape &&
                                                     Event.current.type == EventType.KeyUp &&
                                                     (GUI.GetNameOfFocusedControl() == "newPresetName");

                        if(QUI.ButtonCancel() || keyboardEscapePressed)
                        {
                            defineSymbolsNewPreset.target = false;
                            newPresetName = "";
                        }
                        QUI.FlexibleSpace();
                    }
                    QUI.EndHorizontal();
                    QUI.Space(5);
                }
                else //NORMAL VIEW
                {
                    QUI.BeginHorizontal(width);
                    {
                        QUI.Space(SPACE_8);
                        QUI.BeginChangeCheck();
                        QUI.SetGUIBackgroundColor(EditorGUIUtility.isProSkin ? QColors.Green.Color : QColors.GreenLight.Color);
                        selectedPresetIndex = EditorGUILayout.Popup(selectedPresetIndex, presets, GUILayout.Width(width - SPACE_8));
                        QUI.ResetColors();
                        if(QUI.EndChangeCheck())
                        {
                            Undo.RecordObject(this, "Select Preset");
                            selectedPresetName = presets[selectedPresetIndex];
                        }
                        QUI.FlexibleSpace();
                    }
                    QUI.EndHorizontal();

                    QUI.Space(SPACE_2);

                    if(loadedPresetName.Equals(selectedPresetName) || selectedPresetName.Equals(DEFAULT_PRESET_NAME))
                    {
                        QUI.BeginHorizontal(width);
                        {
                            QUI.Space(SPACE_8);
                            if(selectedPresetName.Equals(DEFAULT_PRESET_NAME))
                            {
                                if(QUI.GhostButton("Create a New Preset with current symbols", QColors.Color.Green, width - SPACE_8))
                                {
                                    defineSymbolsNewPreset.target = true;
                                    newPresetName = "";
                                }
                            }
                            else
                            {
                                if(QUI.GhostButton("NEW", QColors.Color.Green, ((width - SPACE_8) / 4) - SPACE_2))
                                {
                                    defineSymbolsNewPreset.target = true;
                                    newPresetName = "";
                                }
                                QUI.Space(SPACE_2);
                                if(QUI.GhostButton("RELOAD", QColors.Color.Green, ((width - SPACE_8) / 4) - SPACE_2))
                                {
                                    LoadPreset(selectedPresetName);
                                }
                                QUI.Space(SPACE_2);
                                if(QUI.GhostButton("SAVE", QColors.Color.Green, ((width - SPACE_8) / 4) - SPACE_2))
                                {
                                    SavePreset(presetSymbols, selectedPresetName);
                                }
                                QUI.Space(SPACE_2);
                                if(QUI.GhostButton("DELETE", QColors.Color.Red, ((width - SPACE_8) / 4) - SPACE_2))
                                {
                                    if(QUI.DisplayDialog("Delete preset?", "Are you sure you want to delete the '" + selectedPresetName + "' preset?", "Yes", "No"))
                                    {
                                        DeletePreset(selectedPresetName);
                                    }
                                }
                            }
                            QUI.FlexibleSpace();
                        }
                        QUI.EndHorizontal();
                    }
                    else
                    {
                        QUI.BeginHorizontal(width);
                        {
                            QUI.Space(SPACE_8);
                            if(QUI.GhostButton("Load Preset", QColors.Color.Green, width - SPACE_8))
                            {
                                LoadPreset(selectedPresetName);
                            }
                            QUI.FlexibleSpace();
                        }
                        QUI.EndHorizontal();
                    }
                }

                QUI.Space(-SPACE_8 - SPACE_4);
                DrawSelectedPresetsList(width);
            }
            QUI.EndVertical();
        }

        void DrawActiveSymbolsList(float width)
        {
            if(symbols == null || symbols.Count == 0)
            {
                symbols = new List<string> { "" };
            }
            QUI.BeginVertical(width);
            {
                for(int i = 0; i < symbols.Count; i++)
                {
                    DrawListItem(symbols, i, true, width, EditorGUIUtility.singleLineHeight);
                    QUI.Space(3);
                }
            }
            QUI.EndVertical();
        }

        void DrawSelectedPresetsList(float width)
        {
            if(presetSymbols == null || presetSymbols.Count == 0) { presetSymbols = new List<string> { "" }; }
            QUI.BeginVertical(width);
            {
                presetSymbolsReordableList.DoLayoutList();
                QUI.Space(-EditorGUIUtility.singleLineHeight - 3);
                QUI.BeginHorizontal(width);
                {
                    QUI.FlexibleSpace();
                    if(QUI.ButtonPlus())
                    {
                        presetSymbols.Add("");
                    }
                    QUI.Space(-SPACE_2);
                }
                QUI.EndHorizontal();
            }
            QUI.EndVertical();
        }

        /// <summary>
        /// Draws a list's line.
        /// </summary>
        /// <param name="list">Target list</param>
        /// <param name="index">Line index</param>
        /// <param name="readOnly">Should the line be editable? If FALSE it will draw a Label. If TRUE it will draw a TextField with - and + buttons</param>
        /// <param name="width">Width of the line</param>
        /// <returns></returns>
        string DrawListItem(List<string> list, int index, bool readOnly, float width, float height)
        {
            if(readOnly)
            {
                QUI.Label(list[index], Style.Text.Normal, width, height);
            }
            else
            {
                QUI.BeginHorizontal(width);
                {
                    list[index] = EditorGUILayout.TextField(list[index], GUILayout.Width(width - 41), GUILayout.Height(height));
                    if(index == 0 && string.IsNullOrEmpty(list[index]))
                    {

                    }
                    else
                    {
                        QUI.SetGUIBackgroundColor(QColors.RedLight.Color);
                        if(QUI.ButtonMinus())
                        {
                            list.Remove(list[index]);
                            QUI.ResetColors();
                            return "";
                        }
                        QUI.ResetColors();
                        QUI.Space(1);
                        QUI.SetGUIBackgroundColor(QColors.GreenLight.Color);
                        if(QUI.ButtonPlus())
                        {
                            list.Insert(index, "");
                        }
                        QUI.ResetColors();
                    }
                }
                QUI.EndHorizontal();
            }
            return list[index];
        }

        void InitDefineSymbols()
        {
            defineSymbolsNewPreset = new AnimBool(false, Repaint);

            selectedBuildTargetGroup = QUtils.GetActiveBuildTargetGroup();
            previouslySelectedBuildTargetGroup = selectedBuildTargetGroup;

            selectedBuildTargetGroupIsTheActivePlatform = new AnimBool(selectedBuildTargetGroup == QUtils.GetActiveBuildTargetGroup(), Repaint);

            if(symbols == null) { symbols = new List<string>(); } else { symbols.Clear(); }
            symbols = QUtils.GetScriptingDefineSymbolsForGroup(selectedBuildTargetGroup);
            if(presetSymbols == null) { presetSymbols = new List<string>(); } else { presetSymbols.Clear(); }
            presetSymbols.AddRange(symbols);

            presetSymbolsReordableList = new ReorderableList(presetSymbols, typeof(string), true, false, false, false)
            {
                showDefaultBackground = false,
                drawElementBackgroundCallback = (rect, index, active, focused) => { }
            };
            presetSymbolsReordableList.drawElementCallback = (rect, index, active, focused) =>
            {
                if(index == presetSymbolsReordableList.list.Count) { return; }
                float width = WindowSettings.CurrentPageContentWidth / 2;
                int dragBumbWidth = 0;
                int buttonWidth = 16;
                int textFieldWidth = (int)width - 20 - buttonWidth - 2 - buttonWidth;
                rect.x += dragBumbWidth;
                QUI.SetGUIBackgroundColor(EditorGUIUtility.isProSkin ? QColors.Green.Color : QColors.GreenLight.Color);
                presetSymbolsReordableList.list[index] = EditorGUI.TextField(new Rect(rect.x, rect.y, textFieldWidth, EditorGUIUtility.singleLineHeight), (string)presetSymbolsReordableList.list[index]);
                QUI.ResetColors();
                rect.x += textFieldWidth;
                rect.x += 2;
                rect.y -= 1;
                if(QUI.ButtonMinus(rect))
                {
                    if(index == 0 && presetSymbolsReordableList.list.Count == 1) //is this the last list entry?
                    {
                        presetSymbolsReordableList.list[index] = ""; //yes --> add an empty entry
                    }
                    else
                    {
                        presetSymbolsReordableList.list.Remove(presetSymbolsReordableList.list[index]); //no --> remove the entry
                    }
                }
                rect.x += buttonWidth;
                rect.x += 2;
                if(QUI.ButtonPlus(rect))
                {
                    presetSymbolsReordableList.list.Insert(index, ""); //add a new empty entry
                }
            };

            selectedPresetName = DEFAULT_PRESET_NAME;
            loadedPresetName = selectedPresetName;
            RefreshPresetNames();
        }

        void RefreshPresetNames()
        {
            presets = GetDefineSymbolsPresetsFileNames;
            selectedPresetIndex = ArrayUtility.IndexOf(presets, selectedPresetName);
        }

        /// <summary>
        /// Saves a new preset as an asset file in the DefineSymbolsPresets folder. The new preset's filename will be the preset name.
        /// </summary>
        void SavePreset(List<string> presetValues, string presetName)
        {
            presetValues = QUtils.CleanList(presetValues);
            DefineSymbols.DefineSymbolsPreset asset = Q.CreateAsset<DefineSymbols.DefineSymbolsPreset>(EZT.RELATIVE_PATH_DEFINE_SYMBOLS_PRESETS, presetName);
            asset.presetValues = new List<string>(presetValues);
            QUI.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RefreshPresetNames();
            LoadPreset(presetName);
        }

        /// <summary>
        /// Loads a preset with the given presetName. If the preset file does not exist, nothing will happen.
        /// </summary>
        void LoadPreset(string presetName)
        {
            DefineSymbols.DefineSymbolsPreset preset = Q.GetResource<DefineSymbols.DefineSymbolsPreset>(EZT.RESOURCES_PATH_DEFINE_SYMBOLS_PRESETS, presetName);
            if(preset == null) { return; }
            if(presetSymbols == null)
            {
                presetSymbols = new List<string>();
            }
            else
            {
                presetSymbols.Clear();
            }
            if(preset.presetValues == null || preset.presetValues.Count == 0)
            {
                preset.presetValues = new List<string> { "" };
            }
            presetSymbols.AddRange(preset.presetValues);
            loadedPresetName = presetName;
            selectedPresetName = loadedPresetName;
        }

        /// <summary>
        /// Deletes a preset with the given presetName. If the preset file does not exist, nothing will happen.
        /// </summary>
        void DeletePreset(string presetName)
        {
            if(presetName.Equals(DEFAULT_PRESET_NAME))
            {
                Debug.Log("[EZ][DefineSymbols] You cannot delete the '" + DEFAULT_PRESET_NAME + "' preset, as it does not exist." +
                          "Before you try to delete a preset, make you you loaded it first.");
                return;
            }

            if(AssetDatabase.MoveAssetToTrash(EZT.RELATIVE_PATH_DEFINE_SYMBOLS_PRESETS + presetName + ".asset"))
            {
                Debug.Log("[EZ][DefineSymbols] The '" + presetName + "' preset asset file has been moved to trash.");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                selectedPresetName = DEFAULT_PRESET_NAME;
                selectedPresetIndex = 0;
                loadedPresetName = DEFAULT_PRESET_NAME;
                RefreshPresetNames();
            }
        }
#endif
    }
}
