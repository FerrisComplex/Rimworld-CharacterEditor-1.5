// Decompiled with JetBrains decompiler
// Type: CharacterEditor.FileIO
// Assembly: CharacterEditor, Version=1.4.1242.0, Culture=neutral, PublicKeyToken=null
// MVID: 31AEEDD2-5E67-4752-86A4-C61702D6EBC1
// Assembly location: O:\SteamLibrary\steamapps\common\RimWorld\Mods\CharacterEditor\v1.5\Assemblies\CharacterEditor.dll

using System;
using System.IO;
using Verse;

namespace CharacterEditor;

internal static class FileIO
{
    internal static string PATH_DESKTOP => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    internal static string PATH_PAWNEX => PATH_DESKTOP + "\\pawnslots.txt";

    internal static bool ExistsDir(string path)
    {
        return Directory.Exists(path);
    }

    internal static void CheckOrCreateDir(string path)
    {
        if (Directory.Exists(path))
            return;
        Directory.CreateDirectory(path);
    }

    internal static string[] GetDirFolderList(string path, string searchPattern, bool rekursiv = true)
    {
        if (!Directory.Exists(path))
            return null;
        try
        {
            return Directory.GetDirectories(path, searchPattern, rekursiv ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }
        catch (UnauthorizedAccessException ex)
        {
            return null;
        }
        catch
        {
            return null;
        }
    }

    internal static string[] GetDirFileList(string path, string searchPattern, bool rekursiv = true)
    {
        if (!Directory.Exists(path))
            return null;
        try
        {
            return Directory.GetFiles(path, searchPattern, rekursiv ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }
        catch (UnauthorizedAccessException ex)
        {
            return null;
        }
        catch
        {
            return null;
        }
    }

    internal static bool WriteFile(string filepath, byte[] bytes)
    {
        try
        {
            using (var fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }

        return false;
    }

    internal static bool Exists(string filepath)
    {
        return File.Exists(filepath);
    }

    internal static byte[] ReadFile(string filepath)
    {
        FileStream fileStream = null;
        try
        {
            fileStream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var buffer = new byte[(int)fileStream.Length];
            fileStream.Read(buffer, 0, (int)fileStream.Length);
            fileStream.Close();
            return buffer;
        }
        catch (Exception ex)
        {
            fileStream?.Close();
            Log.Error(ex.Message);
        }

        return new byte[0];
    }
}