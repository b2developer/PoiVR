#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System.IO;


/*
* class PoiProfileBuildProcessor
* child class of IPreprocessBuild
* 
* defines actions that take place when 
* the game is built to a specified location
* 
* @author: Bradley Booth, Daniel Witt, Academy of Interactive Entertainment, 2018
*/
class PoiProfileBuildProcessor : IPostprocessBuild
{
    public int callbackOrder
    {
        get
        {
            return 0;
        }
    }

    /*
    * OnPreprocessBuild
    * 
    * runs when the application is built
    * defines customised actions that are executed during the building process
    * 
    * @param BuildTarget target - name of the executable target
    * @param string path - the target of the executable
    * @returns void
    */
    public void OnPostprocessBuild(BuildTarget target, string path)
    {
        string[] directoryPieces = path.Split('/');
        foreach (string s in directoryPieces)
        {
            //Debug.Log(s);
        }

        //this gives 'gameName'.exe, the .exe will be removed by getting a substring
        string exeFile = directoryPieces[directoryPieces.GetLength(0) - 1];

        //directory of the root folder
        string rootFolder = path.Substring(0, path.Length - exeFile.Length);

        string dataFolder = exeFile.Substring(0, exeFile.Length - 4);
        dataFolder += "_Data/";

        dataFolder = rootFolder + dataFolder;

        Debug.Log("PoiProfileBuildProcessor.OnPreprocessBuild for target " + target + " at path " + path);

        PoiOptions.GetInternalDataPath();
        RecordingManager.GetInternalDataPath();

        //copy the folders from the editor to the application
        DirectoryInfo editorProfiles = new DirectoryInfo(PoiOptions.folderPath);
        DirectoryInfo editorRigAnimations = new DirectoryInfo(RecordingManager.folderPath);
        DirectoryInfo application = new DirectoryInfo(dataFolder);

        //create each folder
        DirectoryInfo profiles = application.CreateSubdirectory(application.FullName + "/Profiles/");
        CopyFilesRecursively(editorProfiles, profiles);

        DirectoryInfo animations = application.CreateSubdirectory(application.FullName + "/RigAnimations/");
        CopyFilesRecursively(editorRigAnimations, animations);
    }

    public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
    {
        //call the file copier for each sub-directory
        foreach (DirectoryInfo dir in source.GetDirectories())
        {
            CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
        }

        //copy the files from the directory to the destination
        foreach (FileInfo file in source.GetFiles())
        {
            file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
    }
}
#endif
