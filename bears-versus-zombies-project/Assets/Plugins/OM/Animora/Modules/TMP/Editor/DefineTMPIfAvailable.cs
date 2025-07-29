using UnityEditor;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace OM.Animora.Modules.TMP.Editor
{
    [InitializeOnLoad]
    public static class DefineTMPIfAvailable
    {
        const string TMP_DEFINE = "Animora_TMP";
        static ListRequest _listRequest;

        static DefineTMPIfAvailable()
        {
#if UNITY_6000_0_OR_NEWER

            var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTarget);
            var defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget).Split(';').ToList();
            bool hasDefine = defines.Contains(TMP_DEFINE);

            if (!hasDefine)
            {
                defines.Add(TMP_DEFINE);
                SetDefines(defines, namedBuildTarget);
            }
#else
            // Safe async call
            _listRequest = Client.List(true);
            EditorApplication.update += CheckPackageList;

            // Also listen to package changes later
            Events.registeredPackages += _ => StartListRequest();
#endif
        }

        static void StartListRequest()
        {
            _listRequest = Client.List(true);
            EditorApplication.update += CheckPackageList;
        }

        static void CheckPackageList()
        {
            if (!_listRequest.IsCompleted) return;

            EditorApplication.update -= CheckPackageList;

            if (_listRequest.Status == StatusCode.Success)
            {
                bool hasTMP = _listRequest.Result.Any(p => p.name == "com.unity.textmeshpro");

                var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
                var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTarget);
                var defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget).Split(';').ToList();
                bool hasDefine = defines.Contains(TMP_DEFINE);

                if (hasTMP && !hasDefine)
                {
                    defines.Add(TMP_DEFINE);
                    SetDefines(defines, namedBuildTarget);
                }
                else if (!hasTMP && hasDefine)
                {
                    defines.Remove(TMP_DEFINE);
                    SetDefines(defines, namedBuildTarget);
                }
            }
            else
            {
                Debug.LogError("Failed to list Unity packages: " + _listRequest.Error.message);
            }
        }

        static void SetDefines(System.Collections.Generic.List<string> defines, NamedBuildTarget group)
        {
            string newDefines = string.Join(";", defines);
            PlayerSettings.SetScriptingDefineSymbols(group, newDefines);
        }
    }

}