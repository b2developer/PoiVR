using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/*
* class PoiOptions 
* 
* singleton holds all of the PoiProfiles in memory
* loads them from a directory with all of the data stored in text files
* 
* @author: Bradley Booth, Daniel Witt, Academy of Interactive Entertainment, 2018
*/
public class PoiOptions : MonoBehaviour
{

    //singleton instance
    public static PoiOptions instance;

    public static string folderPath = "";

    //immutable poi profile
    public PoiProfile defaultProfile;

    //the currently active profile
    public PoiProfile activeProfile = null;

    //list of poi profiles stored
    public List<PoiProfile> profiles;

    //sliders that have been mapped to the profile member variable
    public UISlider[] mappedSliders;

    public float G_ELASTICITY_GS
    {
        get
        {
            return activeProfile.elasticity;
        }

        set
        {
            //immutable default profile
            if (activeProfile != defaultProfile)
            {
                activeProfile.elasticity = value;
                activeProfile.Apply();
            }
        }
    }

    public float G_DRAG_GS
    {
        get
        {
            return activeProfile.drag;
        }

        set
        {
            //immutable default profile
            if (activeProfile != defaultProfile)
            {
                activeProfile.drag = value;
                activeProfile.Apply();
            }
        }
    }

    public float G_TIMESCALE_GS
    {
        get
        {
            return activeProfile.timeScale;
        }

        set
        {
            //immutable default profile
            if (activeProfile != defaultProfile)
            {
                activeProfile.timeScale = value;
                activeProfile.Apply();
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        profiles = new List<PoiProfile>();

        defaultProfile = new PoiProfile("0.5,0.5,0.5");
        profiles.Add(defaultProfile);

        GetInternalDataPath();

        instance = this;

        LoadAll();
    }

    /*
    * GetInternalDataPath
    * 
    * gets the folder directory of the application
    * 
    * @returns void
    */
    public static void GetInternalDataPath()
    {
        folderPath = Application.dataPath + "//Profiles//";
    }


    // Update is called once per frame
    void Update()
    {

    }

    /*
    * Select
    * 
    * sets the profile with the specified id
    * 
    * @param int ID - the id of the profile to set 
    * @returns void
    */
    public void Select(int ID)
    {
        activeProfile = profiles[ID];
        activeProfile.Apply();
        SetSliders();
    }

    /*
    * Save 
    * 
    * stores all of the poi profiles into external memory
    * 
    * @returns void
    */
    public void Save()
    {
        string[] allFiles = new string[profiles.Count - 1];
        int afLen = allFiles.GetLength(0);

        for (int i = 0; i < afLen; i++)
        {
            string path = folderPath + "profile" + (i + 1).ToString() + ".txt";

            StreamWriter sw = new StreamWriter(path);
            string serial = profiles[i+1].Serialise();

            sw.Write(serial);

            sw.Close();
        }
    }

    /*
    * LoadAll 
    * 
    * loads all of the poi profiles in from the profile folder
    * stored in either the build and the editor
    * 
    * @returns void
    */
    public void LoadAll()
    {
        //get all sub-directories
        string[] allFiles = Directory.GetFiles(folderPath);
        int afLen = allFiles.GetLength(0);

        for (int i = 0; i < afLen; i++)
        {
            string path = allFiles[i];

            //file type check
            string extension = path.Substring(path.Length - 4);

            if (extension != ".txt")
            {
                continue;
            }

            //read the data at the directory
            StreamReader sr = new StreamReader(path);
            string serial = sr.ReadToEnd();

            sr.Close();

            //deserialise the data
            PoiProfile np = new PoiProfile(serial);

            profiles.Add(np);
        }

        Select(0);
    }

    /*
    * SetSliders 
    * 
    * automatically maps the unscaled slider values to the active poi-profile
    * 
    * @returns void
    */
    public void SetSliders()
    {
        mappedSliders[0].OverrideValue(G_ELASTICITY_GS);
        mappedSliders[1].OverrideValue(G_DRAG_GS);
        mappedSliders[2].OverrideValue(G_TIMESCALE_GS);
    }
}

