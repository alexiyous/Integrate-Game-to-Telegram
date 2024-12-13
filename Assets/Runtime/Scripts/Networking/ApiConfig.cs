using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ApiConfig
{
    public string SECRET_KEY = "your_secret_key_here";
    [SerializeField]
    private string apiEndpoint = "your_api_endpoint_here";
    [SerializeField]
    private GameConfig gameConfig;
    public event Action OnLoading;
    public event Action OnFailSendingToServer;
    public event Action OnSuccesSendingToServer;

    public void Init() {
        SECRET_KEY = gameConfig.SecretKey;
    }

    // Send score with multiple layers of security
    public IEnumerator SendScore(int score, string playerId) {
        // Get timestamp
        long timestamp = GetTimestamp();

        // Encrypt score
        string encryptedScore = EncryptScore(score);
        if (encryptedScore == null) {
            OnLoading?.Invoke();
            yield break;
        }

        // Create payload
        var payload = new ScoreData {
            playerId = playerId,
            score = encryptedScore,
            timestamp = timestamp
        };

        // Generate HMAC of the payload
        string payloadJson = JsonUtility.ToJson(payload);
        string hmac = GenerateHMAC(payloadJson);

        // Create final request data
        var requestData = new RequestData {
            payload = payload,
            signature = hmac
        };

        string jsonData = JsonUtility.ToJson(requestData);

        // Send request
        using (UnityWebRequest request = UnityWebRequest.Put(apiEndpoint + playerId, jsonData)) {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-Timestamp", timestamp.ToString());
            request.SetRequestHeader("X-Signature", hmac);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                Debug.Log("Score sent successfully");
                OnSuccesSendingToServer?.Invoke();
            } else {
                Debug.LogError($"Error sending score: {request.error}");
                OnFailSendingToServer?.Invoke();
                yield break;
            }
        }
    }


    #region Utils
    // Modern encryption using AES
    private string EncryptScore(int score) {
        try {
            using (Aes aes = Aes.Create()) {
                // Create key and IV
                byte[] key = Encoding.UTF8.GetBytes(SECRET_KEY.PadRight(32));
                aes.Key = key;
                aes.GenerateIV();

                // Create encryptor
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Convert score to bytes
                byte[] scoreBytes = BitConverter.GetBytes(score);

                // Encrypt
                byte[] encrypted;
                using (var msEncrypt = new System.IO.MemoryStream()) {
                    // Write IV first (needed for decryption)
                    msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var bwEncrypt = new System.IO.BinaryWriter(csEncrypt)) {
                        bwEncrypt.Write(scoreBytes);
                    }
                    encrypted = msEncrypt.ToArray();
                }

                // Convert to Base64 for transmission
                return Convert.ToBase64String(encrypted);
            }
        } catch (Exception e) {
            Debug.LogError($"Encryption failed: {e.Message}");
            return null;
        }
    }

    // Additional security with HMAC
    private string GenerateHMAC(string data) {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SECRET_KEY))) {
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hashBytes);
        }
    }

    // Timestamp to prevent replay attacks
    private long GetTimestamp() {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    } 
    #endregion


}
