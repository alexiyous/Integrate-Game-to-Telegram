using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : ScriptableObject
{
    private string secretKey;

    public string SecretKey => secretKey;

    // Create the config file
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/Game Config")]
    public static void CreateConfig() {
        var config = ScriptableObject.CreateInstance<GameConfig>();
        config.secretKey = SecretKeyGenerator.GenerateSecretKey();
        UnityEditor.AssetDatabase.CreateAsset(config, "Assets/GameConfig.asset");
    }
#endif
}
