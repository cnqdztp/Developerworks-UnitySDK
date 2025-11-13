using UnityEditor;
using UnityEngine;

namespace PlayKit_SDK.Auth
{
    /// <summary>
    /// Adds menu items to the Unity Editor for PlayKit SDK tools and utilities.
    /// </summary>
    public static class PlayKit_AuthMenu
    {
        /// <summary>
        /// Opens the example folder in the Project window.
        /// </summary>
        [MenuItem("PlayKit SDK/Open Examples Folder", priority = 51)]
        private static void OpenExamplesFolder()
        {
            string examplePath = "Assets/Developerworks_SDK/Example";
            Object exampleFolder = AssetDatabase.LoadAssetAtPath<Object>(examplePath);
            if (exampleFolder != null)
            {
                EditorGUIUtility.PingObject(exampleFolder);
                Selection.activeObject = exampleFolder;
            }
            else
            {
                Debug.LogWarning("[PlayKit SDK] Examples folder not found.");
            }
        }

        /// <summary>
        /// Opens the PlayKit documentation in the default browser.
        /// </summary>
        [MenuItem("PlayKit SDK/Documentation", priority = 52)]
        private static void OpenDocumentation()
        {
            Application.OpenURL("https://docs.playkit.dev");
        }

        /// <summary>
        /// Opens the GitHub repository for bug reports and issues.
        /// </summary>
        [MenuItem("PlayKit SDK/Report Issue on GitHub", priority = 53)]
        private static void ReportIssue()
        {
            Application.OpenURL("https://github.com/playkit/unity-sdk/issues");
        }

        /// <summary>
        /// Clears the locally stored Player Token using PlayerPrefs.
        /// </summary>
        [MenuItem("PlayKit SDK/Clear Local Player Token", priority = 100)]
        private static void ClearLocalPlayerToken()
        {
            // Call the static method from your existing AuthManager
            PlayKit_AuthManager.ClearPlayerToken();

            // Log a confirmation message to the Unity Console
            Debug.Log("[PlayKit SDK] Local player token and expiry have been cleared from PlayerPrefs.");
        }
    }
}