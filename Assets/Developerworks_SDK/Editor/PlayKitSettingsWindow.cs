using PlayKit_SDK.Editor;
using UnityEngine;
using UnityEditor;

namespace Developerworks.SDK
{
    /// <summary>
    /// Editor window for configuring PlayKit SDK settings.
    /// Access via PlayKit SDK > Settings
    /// PlayKit SDK 配置窗口
    /// 通过 PlayKit SDK > Settings 访问
    /// </summary>
    public class PlayKitSettingsWindow : EditorWindow
    {
        private PlayKitSettings settings;
        private SerializedObject serializedSettings;
        private Vector2 scrollPosition;

        // Tab navigation
        private enum Tab
        {
            Configuration,  // 配置
            Development,    // 开发
            About          // 关于
        }
        private Tab currentTab = Tab.Configuration;

        // Developer token visibility toggle
        private bool showDeveloperToken = false;

        // Auto validation state
        private string lastValidatedGameId = "";
        private string lastValidatedToken = "";
        private bool isValidating = false;
        private ValidationResult validationResult = null;

        [System.Serializable]
        private class ValidationResult
        {
            public bool success;
            public bool tokenValid;
            public string tokenError;
            public GameInfo game;
            public TokenInfo token;
            public string error;
        }

        [System.Serializable]
        private class GameInfo
        {
            public string id;
            public string name;
            public string description;
            public bool is_suspended;
            public bool is_hosted;
            public bool enable_steam_auth;
            public string steam_app_id;
        }

        [System.Serializable]
        private class TokenInfo
        {
            public string id;
            public string name;
            public string created_at;
        }

        [MenuItem("PlayKit SDK/Settings", priority = 0)]
        public static void ShowWindow()
        {
            PlayKitSettingsWindow window = GetWindow<PlayKitSettingsWindow>("PlayKit SDK Settings");
            window.minSize = new Vector2(500, 550);
            window.Show();
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            settings = PlayKitSettings.Instance;
            if (settings != null)
            {
                serializedSettings = new SerializedObject(settings);
            }
        }

        private void OnGUI()
        {
            if (settings == null || serializedSettings == null)
            {
                LoadSettings();
                if (settings == null)
                {
                    EditorGUILayout.HelpBox(
                        "Failed to load PlayKit settings. Please check console for errors.\n" +
                        "无法加载 PlayKit 设置。请检查控制台错误。",
                        MessageType.Error
                    );
                    return;
                }
            }

            // Update serialized object at the start of OnGUI
            serializedSettings.Update();

            // Header with logo and title
            DrawHeader();

            EditorGUILayout.Space(5);

            // Tab navigation
            DrawTabNavigation();

            EditorGUILayout.Space(5);

            // Content area with scroll
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            switch (currentTab)
            {
                case Tab.Configuration:
                    DrawConfigurationTab();
                    break;
                case Tab.Development:
                    DrawDevelopmentTab();
                    break;
                case Tab.About:
                    DrawAboutTab();
                    break;
            }

            EditorGUILayout.EndScrollView();

            // Apply changes at the end of OnGUI
            if (serializedSettings.hasModifiedProperties)
            {
                serializedSettings.ApplyModifiedProperties();
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("PlayKit SDK", new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter
            });

            GUILayout.Label("Unity游戏AI开发套件 Unity Game AI Development Kit", new GUIStyle(EditorStyles.label)
            {
                fontSize = 10,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic
            });

            EditorGUILayout.EndVertical();
        }

        private void DrawTabNavigation()
        {
            EditorGUILayout.BeginHorizontal();

            GUIStyle tabStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fixedHeight = 30
            };

            if (GUILayout.Toggle(currentTab == Tab.Configuration, "配置 Configuration", tabStyle))
            {
                currentTab = Tab.Configuration;
            }

            if (GUILayout.Toggle(currentTab == Tab.Development, "开发 Development", tabStyle))
            {
                currentTab = Tab.Development;
            }

            if (GUILayout.Toggle(currentTab == Tab.About, "关于 About", tabStyle))
            {
                currentTab = Tab.About;
            }

            EditorGUILayout.EndHorizontal();
        }

        #region Configuration Tab

        private void DrawConfigurationTab()
        {
            EditorGUILayout.Space(10);

            // Game Configuration
            DrawGameConfiguration();

            EditorGUILayout.Space(10);

            // Validation Status
            DrawValidationStatus();

            EditorGUILayout.Space(10);

            // AI Model Defaults
            DrawModelDefaults();
        }

        private void DrawGameConfiguration()
        {
            GUILayout.Label("游戏配置 | Game Configuration", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Game ID
            SerializedProperty gameIdProp = serializedSettings.FindProperty("gameId");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(gameIdProp, new GUIContent(
                "游戏 ID | Game ID",
                "从 PlayKit 控制台获取的游戏ID\nYour Game ID from the PlayKit dashboard"
            ));

            // Auto-validate when Game ID changes
            if (EditorGUI.EndChangeCheck() && !string.IsNullOrWhiteSpace(gameIdProp.stringValue))
            {
                ValidateConfiguration();
            }

            if (string.IsNullOrWhiteSpace(gameIdProp.stringValue))
            {
                EditorGUILayout.HelpBox(
                    "⚠ 游戏 ID 是必填项！请从 PlayKit 控制台获取。\n" +
                    "⚠ Game ID is required! Get your Game ID from the PlayKit dashboard.",
                    MessageType.Warning
                );
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawValidationStatus()
        {
            GUILayout.Label("配置验证状态 | Configuration Validation Status", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (isValidating)
            {
                EditorGUILayout.HelpBox(
                    "🔄 正在验证配置...\n" +
                    "🔄 Validating configuration...",
                    MessageType.Info
                );
            }
            else if (validationResult != null)
            {
                DrawValidationResult();
            }
            else if (!string.IsNullOrWhiteSpace(settings.GameId))
            {
                EditorGUILayout.HelpBox(
                    "ℹ️ 配置已更改，将在下次保存时自动验证。\n" +
                    "ℹ️ Configuration changed, will auto-validate on next save.",
                    MessageType.Info
                );
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "ℹ️ 请先配置游戏 ID。\n" +
                    "ℹ️ Please configure Game ID first.",
                    MessageType.Info
                );
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawValidationResult()
        {
            if (!validationResult.success)
            {
                // Game not found or API error
                EditorGUILayout.HelpBox(
                    $"❌ 验证失败 | Validation Failed\n\n{validationResult.error}",
                    MessageType.Error
                );
                return;
            }

            // Game found
            if (validationResult.game != null)
            {
                string gameName = validationResult.game.name ?? "Unknown";
                string gameDesc = validationResult.game.description ?? "";

                EditorGUILayout.LabelField("游戏信息 | Game Information", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField("名称 | Name:", gameName);
                if (!string.IsNullOrEmpty(gameDesc))
                {
                    EditorGUILayout.LabelField("描述 | Description:", gameDesc, EditorStyles.wordWrappedLabel);
                }

                // Game status warnings
                if (validationResult.game.is_suspended)
                {
                    EditorGUILayout.HelpBox(
                        "⚠️ 游戏已被暂停 | Game is suspended",
                        MessageType.Warning
                    );
                }

                EditorGUILayout.Space(5);
            }

            // Token validation
            if (validationResult.tokenValid && validationResult.token != null)
            {
                EditorGUILayout.HelpBox(
                    $"✅ 开发者令牌有效 | Developer Token Valid\n\n" +
                    $"令牌名称 | Token Name: {validationResult.token.name}\n" +
                    $"创建时间 | Created: {validationResult.token.created_at}",
                    MessageType.Info
                );
            }
            else if (!string.IsNullOrEmpty(validationResult.tokenError))
            {
                EditorGUILayout.HelpBox(
                    $"⚠️ 开发者令牌无效 | Developer Token Invalid\n\n{validationResult.tokenError}",
                    MessageType.Warning
                );
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "ℹ️ 未提供开发者令牌，将使用玩家认证。\n" +
                    "ℹ️ No developer token provided, will use player authentication.",
                    MessageType.Info
                );
            }
        }

        private void DrawModelDefaults()
        {
            GUILayout.Label("AI 模型默认值 | AI Model Defaults", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.HelpBox(
                "配置默认使用的 AI 模型。留空则使用服务器默认值。\n" +
                "Configure default AI models. Leave empty to use server defaults.",
                MessageType.Info
            );

            // Default Chat Model
            SerializedProperty chatModelProp = serializedSettings.FindProperty("defaultChatModel");
            EditorGUILayout.PropertyField(chatModelProp, new GUIContent(
                "默认对话模型 | Default Chat Model",
                "例如：gpt-4o-mini\nExample: gpt-4o-mini"
            ));

            EditorGUILayout.Space(5);

            // Default Image Model
            SerializedProperty imageModelProp = serializedSettings.FindProperty("defaultImageModel");
            EditorGUILayout.PropertyField(imageModelProp, new GUIContent(
                "默认图像模型 | Default Image Model",
                "例如：dall-e-3\nExample: dall-e-3"
            ));

            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Development Tab

        private void DrawDevelopmentTab()
        {
            EditorGUILayout.Space(10);

            GUILayout.Label("开发者工具 | Developer Tools", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Storage Mode Toggle
            EditorGUILayout.LabelField("开发者令牌存储方式 | Developer Token Storage", EditorStyles.miniBoldLabel);

            SerializedProperty useLocalProp = serializedSettings.FindProperty("useLocalDeveloperToken");
            EditorGUILayout.PropertyField(useLocalProp, new GUIContent(
                "使用本地存储 | Use Local Storage",
                "启用：令牌存储在 EditorPrefs（本地，不会提交到版本控制）\n" +
                "禁用：令牌存储在项目设置（可提交到版本控制，适合团队共享）\n\n" +
                "Enabled: Token stored in EditorPrefs (local, not tracked by version control)\n" +
                "Disabled: Token stored in project settings (can be committed, suitable for team sharing)"
            ));

            EditorGUILayout.Space(5);

            // Display appropriate help message based on storage mode
            if (useLocalProp.boolValue)
            {
                EditorGUILayout.HelpBox(
                    "🔒 本地模式：令牌存储在本地 EditorPrefs，不会被 Git 追踪。适合个人开发。\n" +
                    "🔒 Local Mode: Token stored in local EditorPrefs, not tracked by Git. Suitable for personal development.",
                    MessageType.Info
                );
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "📦 项目模式：令牌存储在项目配置中，可以提交到版本控制。适合团队共享（私有仓库）。\n" +
                    "📦 Project Mode: Token stored in project settings, can be committed to version control. Suitable for team sharing (private repos).",
                    MessageType.Info
                );
            }

            EditorGUILayout.Space(8);

            // Developer Token Input
            EditorGUILayout.LabelField("开发者令牌（可选）| Developer Token (Optional)", EditorStyles.miniBoldLabel);

            if (useLocalProp.boolValue)
            {
                // Local storage mode - use EditorPrefs
                string localToken = PlayKitSettings.LocalDeveloperToken;

                EditorGUI.BeginChangeCheck();
                if (showDeveloperToken)
                {
                    string newToken = EditorGUILayout.TextField("令牌 | Token", localToken);
                    if (newToken != localToken)
                    {
                        PlayKitSettings.LocalDeveloperToken = newToken;
                        // Auto-validate when token changes
                        if (EditorGUI.EndChangeCheck())
                        {
                            ValidateConfiguration();
                        }
                    }
                }
                else
                {
                    string maskedToken = string.IsNullOrEmpty(localToken) ?
                        "(未设置 Not Set)" : new string('●', 20);
                    EditorGUILayout.LabelField("令牌 | Token", maskedToken);
                }
            }
            else
            {
                // Project storage mode - use ScriptableObject
                SerializedProperty tokenProp = serializedSettings.FindProperty("developerToken");

                EditorGUI.BeginChangeCheck();
                if (showDeveloperToken)
                {
                    EditorGUILayout.PropertyField(tokenProp, new GUIContent("令牌 | Token"));
                }
                else
                {
                    string maskedToken = string.IsNullOrEmpty(tokenProp.stringValue) ?
                        "(未设置 Not Set)" : new string('●', 20);
                    EditorGUILayout.LabelField("令牌 | Token", maskedToken);
                }

                // Auto-validate when token changes
                if (EditorGUI.EndChangeCheck() && !string.IsNullOrWhiteSpace(tokenProp.stringValue))
                {
                    ValidateConfiguration();
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(
                showDeveloperToken ? "👁 隐藏令牌 | Hide Token" : "👁 显示令牌 | Show Token",
                GUILayout.Height(25),
                GUILayout.Width(200)))
            {
                showDeveloperToken = !showDeveloperToken;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Ignore Developer Token option
            SerializedProperty ignoreProp = serializedSettings.FindProperty("ignoreDeveloperToken");
            EditorGUILayout.PropertyField(ignoreProp, new GUIContent(
                "忽略开发者令牌 | Ignore Developer Token",
                "强制使用玩家认证流程进行测试\nForce player authentication flow for testing"
            ));

            EditorGUILayout.Space(10);

            // Clear Player Token Button
            if (GUILayout.Button("清除本地玩家令牌 Clear Local Player Token", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog(
                    "清除玩家令牌 Clear Player Token",
                    "确定要清除本地存储的玩家令牌吗？下次运行时需要重新登录。\n" +
                    "Are you sure you want to clear the local player token? You'll need to login again on next run.",
                    "确定 Yes",
                    "取消 Cancel"))
                {
                    PlayKit_SDK.Auth.PlayKit_AuthManager.ClearPlayerToken();
                    EditorUtility.DisplayDialog(
                        "成功 Success",
                        "玩家令牌已清除。\nPlayer token has been cleared.",
                        "确定 OK"
                    );
                }
            }

            EditorGUILayout.EndVertical();
        }

        #endregion

        #region About Tab

        private void DrawAboutTab()
        {
            EditorGUILayout.Space(10);

            // Version Info
            DrawVersionInfo();

            EditorGUILayout.Space(10);

            // Quick Links
            DrawQuickLinks();

            EditorGUILayout.Space(10);

            // Resources
            DrawResources();
        }

        private void DrawVersionInfo()
        {
            GUILayout.Label("版本信息 Version Information", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("SDK 版本 SDK Version:", PlayKit_SDK.PlayKit_SDK.VERSION);
            EditorGUILayout.LabelField("Unity 版本 Unity Version:", Application.unityVersion);

            EditorGUILayout.Space(5);

            if (GUILayout.Button("检查更新 Check for Updates", GUILayout.Height(30)))
            {
                PlayKit_UpdateChecker.CheckForUpdates(true);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawQuickLinks()
        {
            GUILayout.Label("快速链接 Quick Links", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("📖 文档 Documentation", GUILayout.Height(30)))
            {
                Application.OpenURL("https://docs.playkit.dev");
            }
            if (GUILayout.Button("💡 示例 Examples", GUILayout.Height(30)))
            {
                OpenExampleScenes();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🐛 报告问题 Report Issue", GUILayout.Height(30)))
            {
                Application.OpenURL("https://github.com/playkit/unity-sdk/issues");
            }
            if (GUILayout.Button("🌐 官网 Website", GUILayout.Height(30)))
            {
                Application.OpenURL("https://playkit.dev");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawResources()
        {
            GUILayout.Label("资源与支持 Resources & Support", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.HelpBox(
                "📧 Email: support@agentlandlab.com",
                MessageType.Info
            );

            // if (GUILayout.Button("加入 Discord 社区 Join Discord Community", GUILayout.Height(30)))
            // {
            //     Application.OpenURL("https://discord.gg/playkit");
            // }

            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Helper Methods

        private async void ValidateConfiguration()
        {
            string currentGameId = settings.GameId;
            string currentToken = settings.DeveloperToken;

            // Skip if already validating same configuration
            if (isValidating ||
                (currentGameId == lastValidatedGameId && currentToken == lastValidatedToken))
            {
                return;
            }

            lastValidatedGameId = currentGameId;
            lastValidatedToken = currentToken;
            isValidating = true;
            validationResult = null;
            Repaint();

            try
            {
                string apiUrl = $"https://playkit.agentlandlab.com/api/external/validate-editor-config?gameId={UnityEngine.Networking.UnityWebRequest.EscapeURL(currentGameId)}";

                using (var webRequest = UnityEngine.Networking.UnityWebRequest.Get(apiUrl))
                {
                    // Add developer token if provided
                    if (!string.IsNullOrWhiteSpace(currentToken))
                    {
                        webRequest.SetRequestHeader("Authorization", $"Bearer {currentToken}");
                    }

                    var operation = webRequest.SendWebRequest();

                    // Wait for completion
                    while (!operation.isDone)
                    {
                        await System.Threading.Tasks.Task.Delay(100);
                    }

                    if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        string jsonResponse = webRequest.downloadHandler.text;
                        validationResult = JsonUtility.FromJson<ValidationResult>(jsonResponse);
                    }
                    else
                    {
                        validationResult = new ValidationResult
                        {
                            success = false,
                            error = $"API Error: {webRequest.error}"
                        };
                    }
                }
            }
            catch (System.Exception ex)
            {
                validationResult = new ValidationResult
                {
                    success = false,
                    error = $"Exception: {ex.Message}"
                };
            }
            finally
            {
                isValidating = false;
                Repaint();
            }
        }

        private void OpenExampleScenes()
        {
            // Find example scenes in the SDK
            string examplePath = "Assets/Developerworks_SDK/Example";
            Object exampleFolder = AssetDatabase.LoadAssetAtPath<Object>(examplePath);
            if (exampleFolder != null)
            {
                EditorGUIUtility.PingObject(exampleFolder);
                Selection.activeObject = exampleFolder;
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "示例 Examples",
                    "未在 SDK 中找到示例场景。\nNo example scenes found in the SDK.",
                    "确定 OK"
                );
            }
        }

        #endregion
    }
}
