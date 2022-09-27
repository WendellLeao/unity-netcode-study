// Copyright (c) 2016 - 2018 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using QuickEditor;
using UnityEditor;

#pragma warning disable 0162
namespace Ez.Internal
{
    [InitializeOnLoad]
    public class DefineSymbolsSymbolLoader
    {
        static DefineSymbolsSymbolLoader()
        {
            EditorApplication.update += RunOnce;
        }

        static void RunOnce()
        {
            EditorApplication.update -= RunOnce;
            CreateMissingFolders();
            LoadSymbol();
        }

        static void CreateMissingFolders()
        {
            if(!AssetDatabase.IsValidFolder(EZT.PATH + "/DefineSymbols/Editor")) { AssetDatabase.CreateFolder(EZT.PATH + "/DefineSymbols", "Editor"); }
            if(!AssetDatabase.IsValidFolder(EZT.PATH + "/DefineSymbols/Editor/Resources")) { AssetDatabase.CreateFolder(EZT.PATH + "/DefineSymbols/Editor", "Resources"); }
            if(!AssetDatabase.IsValidFolder(EZT.PATH + "/DefineSymbols/Editor/Resources/EZT")) { AssetDatabase.CreateFolder(EZT.PATH + "/DefineSymbols/Editor/Resources", "EZT"); }
            if(!AssetDatabase.IsValidFolder(EZT.PATH + "/DefineSymbols/Editor/Resources/EZT/DefineSymbols")) { AssetDatabase.CreateFolder(EZT.PATH + "/DefineSymbols/Editor/Resources/EZT", "DefineSymbols"); }
            if(!AssetDatabase.IsValidFolder(EZT.PATH + "/DefineSymbols/Editor/Resources/EZT/DefineSymbols/Presets")) { AssetDatabase.CreateFolder(EZT.PATH + "/DefineSymbols/Editor/Resources/EZT/DefineSymbols", "Presets"); }
            if(!AssetDatabase.IsValidFolder(EZT.PATH + "/DefineSymbols/Editor/Resources/EZT/DefineSymbols/Version")) { AssetDatabase.CreateFolder(EZT.PATH + "/DefineSymbols/Editor/Resources/EZT/DefineSymbols", "Version"); }
        }

        static void LoadSymbol()
        {
#if EZ_SOURCE
            return;
#endif
            QUtils.AddScriptingDefineSymbol(EZT.SYMBOL_EZ_DEFINE_SYMBOLS);
        }
    }
}
#pragma warning restore 0162
