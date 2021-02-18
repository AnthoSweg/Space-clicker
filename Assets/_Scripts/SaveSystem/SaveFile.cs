using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security;

//The accessible part of the class -- You wan't to access things in here
public partial class SaveFile
{
    private static SecureString _appKey = null;
    public static SecureString AppKey
    {
        get
        {
            if (SaveFile._appKey == null)
            {
                string key = "SP4C3CLICK3R_4PP_K3Y";
                SaveFile._appKey = SaveFile.GetSecureString(key);
                key = "";
            }
            return SaveFile._appKey;
        }
    }

    public const string fileName = "space";
    public const string fileExtension = "click";

    private static Content _fileContent = null;
    public static Content FileContent
    {
        get
        {
            if (SaveFile._fileContent == null)
            {
                SaveFile._fileContent = SaveFile.ReadFromFile();
            }
            return SaveFile._fileContent;
        }
    }

    public static SaveFile.Content Clear()
    {
        SaveFile._fileContent = new SaveFile.Content();
        SaveFile._fileContent.WriteFile();
        return SaveFile._fileContent;
    }

    //Returns the most recent save
    public static GameState CurrentState
    {
        get
        {
            return SaveFile.FileContent.GameStates[0];
        }
    }

    #region Storage

    public static string Directory
    {
        get
        {
            return Application.persistentDataPath;
        }
    }

    public static string FullPath
    {
        get
        {
            return string.Format("{0}/{1}.{2}", SaveFile.Directory, SaveFile.fileName, SaveFile.fileExtension);
        }
    }

    public static void Write()
    {
        SaveFile.FileContent.WriteFile();
    }

    public static void ReloadfromFile()
    {
        SaveFile._fileContent = SaveFile.ReadFromFile();
    }

    public static SaveFile.Content ReadFromFile()
    {
        return SaveFile.ReadFromFile(SaveFile.fileName);
    }

    public static SaveFile.Content ReadFromFile(string name)
    {
        string file = string.Format("{0}/{1}.{2}", Application.persistentDataPath, name, SaveFile.fileExtension);

        SaveFile.Content result = null;

        if (File.Exists(file))
        {
            string content = File.ReadAllText(file);
            if (string.IsNullOrEmpty(content))
            {
                Debug.LogError("The Save File is invalid, the file is empty.");
                return SaveFile.GetNewFileContent();
            }

            try
            {
                content = SaveFile.DecryptIfNeeded(content);

                result = JsonUtility.FromJson<SaveFile.Content>(content);
                Debug.LogFormat("Save File file found. Path: {0}", file);
            }
            catch (Exception)
            {
                Debug.LogError("The Save File is invalid, unable to extract data, a new one will be created.");
                result = SaveFile.GetNewFileContent();
            }

            try
            {
                File.WriteAllText(string.Format("{0}/{1}.{2}.bak", Application.persistentDataPath, name, SaveFile.fileExtension), result.ToJson(true));
                //Debug.Log("A backup has been created.");
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Unable to create a backup.", ex.ToString());
            }
        }
        else
        {
            Debug.LogError("The Save File doesn't exist, a new one will be created.");
            result = SaveFile.GetNewFileContent();
        }

        return result;
    }

    public static SaveFile.Content GetNewFileContent()
    {
        return new SaveFile.Content();
    }

    private static string DecryptIfNeeded(string text)
    {
        string result = "";
        if (SaveFile.IsEncrypted(text, out result))
        {
            Debug.Log("File was encrypted");
            return result;
        }
        else
        {
            //Debug.Log("File was not encrypted");
            return text;
        }
    }

    private static bool IsEncrypted(string text, out string clearText)
    {
        clearText = "";
        try
        {
            clearText = SaveFile.Decrypt(text);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    #endregion

    #region Security
    //"=" are not allowed in Mailto (the string is truncated), so they will be replaced by this value.
    private const string equals = "ikwalz";

    public static string Encrypt(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }

        string secureKey64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(SaveFile.AppKey.ToString()));
        string result = Convert.ToBase64String(Encoding.UTF8.GetBytes(text)) + secureKey64;
        return result.Replace("=", SaveFile.equals);
    }

    public static string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
        {
            return "";
        }

        encryptedText = encryptedText.Replace(SaveFile.equals, "=");

        string secureKey64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(SaveFile.AppKey.ToString()));
        encryptedText = encryptedText.Replace(secureKey64, "");

        return Encoding.UTF8.GetString(Convert.FromBase64String(encryptedText));
    }

    private static SecureString GetSecureString(string key)
    {
        key = key.Trim();

        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("Key is invalid.");
            return null;
        }

        SecureString secureString = new SecureString();
        if (key.Length > 0)
        {
            foreach (var c in key.ToCharArray())
            {
                secureString.AppendChar(c);
            }
        }

        key = "";

        return secureString;
    }

    #endregion
}

//The data writing / reading part of the class -- Don't mind this
[Serializable]
public partial class SaveFile
{
    public static readonly int maxGameState = 2;
    public class Content
    {
        private const bool encryptSave = false;

        [SerializeField]
        private List<GameState> gameStates = null;
        public List<GameState> GameStates
        {
            get
            {
                if (this.gameStates == null)
                {
                    this.gameStates = new List<GameState>();
                }

                if (this.gameStates.Count == 0)
                {
                    this.gameStates.Add(GameState.CreateNewGameState());
                    SaveFile.Write();
                }

                this.gameStates = this.gameStates.KeepFirstValues(maxGameState);

                return this.gameStates;
            }
            set
            {
                this.gameStates = value;
            }
        }
        public GameState AddGameState(GameState gameState)
        {
            GameState newGameState = gameState.GetClone();

            if (this.GameStates.Count > 0)
            {
                DateTime oldTimeStamp = this.GameStates[0].TimeStampUTC;
                DateTime newTimeStamp = newGameState.TimeStampUTC;
                if (oldTimeStamp == newTimeStamp)
                {
                    Debug.LogFormat("Will erase save because old ({0}) is the same as new ({1})", oldTimeStamp, newTimeStamp);
                    this.GameStates[0] = newGameState;
                }
                else
                {
                    Debug.LogFormat("Will insert save because standard save. Timestamp: {0}", newGameState.TimeStampUTC);
                    this.GameStates.Insert(0, newGameState);
                }
            }
            else
            {
                Debug.LogFormat("Will insert save because no save exists. Timestamp: {0}", newGameState.TimeStampUTC);
                this.GameStates.Insert(0, newGameState);
            }

            return this.GameStates[0];
        }

        public string ToJson(bool encrypt = false)
        {
            string json = JsonUtility.ToJson(this, true);
            if (encrypt)
            {
                json = SaveFile.Encrypt(json);
            }
            return json;
        }

        #region STORAGE

        public void WriteFile()
        {
            this.WriteFile(this.GameStates[0]);
        }

        public void WriteFile(GameState state)
        {
            GameState newState = state.GetClone();
            newState.UpdateTimeStampUTC();

            this.AddGameState(newState);

            string json = this.ToJson();

            if (encryptSave)
            {
                json = SaveFile.Encrypt(json);
            }

            string path = string.Format("{0}/{1}.{2}", Application.persistentDataPath, SaveFile.fileName, SaveFile.fileExtension);

            try
            {
                File.WriteAllText(path, json);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("[PROFILE] Successfully saved profile (path: {0}).", path));

                Debug.Log(sb.ToString());
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("[PROFILE] Unable to save GamSave (path: {0}), {1}", path, ex.ToString());
            }
        }

        #endregion

        
    }
}

public static class ListExtended
{
    public static List<T> KeepFirstValues<T>(this List<T> list, int count)
    {
        if (list.Count > count)
        {
            list.RemoveRange(count, list.Count - count);
        }

        return list;
    }
}
