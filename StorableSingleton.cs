using System.IO;
using UnityEngine;
using AesEverywhere;

/// <summary>
/// A generic singleton class that provides functionality to save and load its state to/from a file.
/// The state is serialized as JSON and stored in the application's persistent data path.
/// Support encrypt and decrypt using AES Everywhere(https://github.com/mervick/aes-everywhere).
/// </summary>
public abstract class StorableSingleton<T> where T : class, new()
{
    private static bool debugEnabled = false;
    private static bool encryptionEnabled = true;
    private static string encryptionKey = "vC4S(]~*U^`HQ15"; // Generate a New Key: https://randomkeygen.com/
    private static string fileName => typeof(T).Name;
    private static string fileExtension = ".ss";

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Load();
                if (instance == null)
                {
                    if (debugEnabled) Debug.Log($"Creating new instance of {typeof(T).Name}");
                    instance = new T();
                }
            }
            return instance;
        }
    }

    public static void Save() => Save(null);

    public static void Save(string name)
    {
        try
        {
            string filePath = Application.persistentDataPath + "/" + fileName + name + fileExtension;
            string json = JsonUtility.ToJson(Instance, true);
            File.WriteAllText(filePath, encryptionEnabled ? Encrypt(json) : json);
            if (debugEnabled) Debug.Log($"Saved instance of {typeof(T).Name} to {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving instance of {typeof(T).Name}: {ex.Message}");
        }
    }

    private static void Load() => Load(null);

    public static void Load(string name)
    {
        try
        {
            string filePath = Application.persistentDataPath + "/" + fileName + name + fileExtension;
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                instance = JsonUtility.FromJson<T>(encryptionEnabled ? Decrypt(json) : json);
                if (debugEnabled) Debug.Log($"Loaded instance of {typeof(T).Name} from {filePath}");
            }
            else if (debugEnabled)
            {
                Debug.Log($"No saved file found for {typeof(T).Name} at {filePath}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error loading instance of {typeof(T).Name}: {ex.Message}");
        }
    }

    public static void Delete() => Delete(fileName);

    public static void Delete(string name)
    {
        try
        {
            string filePath = Application.persistentDataPath + "/" + fileName + name + fileExtension;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                if (debugEnabled) Debug.Log($"Cleared saved data for {typeof(T).Name} at {filePath}");
            }
            else if (debugEnabled)
            {
                Debug.Log($"No file to delete for {typeof(T).Name} at {filePath}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error deleting file for {typeof(T).Name}: {ex.Message}");
        }
    }

    public static string Encrypt(string clearText)
    {
        try
        {
            AES256 aes = new AES256();
            string cipherText = aes.Encrypt(clearText, encryptionKey);
            if (debugEnabled)
            {
                Debug.Log("Encrypt: " + clearText);
                Debug.Log("CipherText: " + cipherText);
            }
            return cipherText;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error encrypting text: {ex.Message}");
            return null;
        }
    }

    public static string Decrypt(string cipherText)
    {
        try
        {
            AES256 aes = new AES256();
            string clearText = aes.Decrypt(cipherText, encryptionKey);
            if (debugEnabled)
            {
                Debug.Log("Decrypt: " + cipherText);
                Debug.Log("ClearText: " + clearText);
            }
            return clearText;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error decrypting text: {ex.Message}");
            return null;
        }
    }

    public static void DebugEnabled(bool value)
    {
        debugEnabled = value;
    }

    public static void EncryptionEnabled(bool value)
    {
        encryptionEnabled = value;
    }
}
