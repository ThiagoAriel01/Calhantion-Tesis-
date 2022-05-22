using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileStreamer
{
    static private FileStreamer _instance;

    static public string appdata
    {
        get => Application.persistentDataPath;
    }

    static public FileStreamer instance
    {
        get
        {
            if (_instance == null)
                _instance = new FileStreamer();
            return _instance;
        }
    }

    public void WriteText (string txt, string path)
    {
        File.WriteAllText(path, txt);
    }

    public string LoadText (string fullpath)
    {
        if (!File.Exists(fullpath))
            return string.Empty;
        return File.ReadAllText(fullpath);
    }
}
