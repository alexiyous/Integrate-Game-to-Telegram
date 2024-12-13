using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SecretKeyGenerator
{
    public static string GenerateSecretKey(int length = 32) {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=";
        StringBuilder res = new StringBuilder();
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider()) {
            byte[] uintBuffer = new byte[sizeof(uint)];

            while (length-- > 0) {
                rng.GetBytes(uintBuffer);
                uint num = BitConverter.ToUInt32(uintBuffer, 0);
                res.Append(validChars[(int)(num % (uint)validChars.Length)]);
            }
        }
        return res.ToString();
    }
}

