using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextureSaveLoadHandler : MonoBehaviour
{
    static public TextureSaveLoadHandler instance;

    private string pathOfTextures;

    private void Awake()
    {
        instance = this;

        pathOfTextures =  Application.persistentDataPath+"/WebTexturesFolder";

        
    }


    /// <summary>
    /// Load Image from disk into game
    /// </summary>
    /// <param name="fileName"> file name of the image got from the json data </param>
    /// <returns></returns>

    public Texture2D LoadImageFromDisk(string fileName)
    {


        //string path = pathOfTextures + "/" + fileName;

        string path = Path.Combine(pathOfTextures, fileName);

       // Logging.Log($"Path of Texture = {path}");

        if (!File.Exists(path))
        {
           // Logging.Log("Error, File Not Found");
            return null;
        }

        byte[] textureBytes = File.ReadAllBytes(path);

        Texture2D loadedTexture = new Texture2D(0, 0);
        loadedTexture.LoadImage(textureBytes);


        return loadedTexture;

    }

    /// <summary>
    /// Save / write Texture into local storage using the filename given
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="fileName"></param>


    public bool CheckIfTextureExists(string fileName)
    {

        string path = pathOfTextures + "/" + fileName;

        if (File.Exists(path))
        {
            return true;

        }
        return false;
    }

    public void SaveImageOnDisk(Texture2D texture, string fileName)
    {

        //string path = pathOfTextures + "/" + fileName;

        string path = Path.Combine(pathOfTextures, fileName);

        if(!Directory.Exists(pathOfTextures))
        {
            var folder = Directory.CreateDirectory(pathOfTextures); // returns a DirectoryInfo object
        }

       

        if (texture is null)
        {
        //    Logging.Log("No Image To save");
            return;
        }

        byte[] textureBytes = texture.EncodeToPNG();

        File.WriteAllBytes(path, textureBytes);

      // Logging.Log(" File Written On Disk ");

    }

    public void DeleteImagesFolder()
    {
        if (Directory.Exists(pathOfTextures))
        {
            Directory.Delete(pathOfTextures,true);

        //    Logging.Log(" Texture Folder Deleted ");
        }
    }







}

