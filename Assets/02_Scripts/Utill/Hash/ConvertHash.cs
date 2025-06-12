using System;
using System.Security.Cryptography;
using System.Text;

public class ConvertHash
{
    public static string StringToHash(string input)
    {
        using var sha = SHA256.Create();
        byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}