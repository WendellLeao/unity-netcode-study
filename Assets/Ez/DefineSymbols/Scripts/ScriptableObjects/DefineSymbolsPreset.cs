// Copyright (c) 2016 - 2017 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ez.DefineSymbols
{
    [Serializable]
    public class DefineSymbolsPreset : ScriptableObject
    {
        public List<string> presetValues;
    }
}
