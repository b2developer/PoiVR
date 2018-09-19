using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class PoiGraphics 
* 
* interchanges the particle systems of the poi
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class PoiGraphics : MonoBehaviour
{
    public enum EGraphicType
    {
        PARTICLE,
        TRAIL,
    }

    public GameObject particle;
    public GameObject trail;

    public GameObject particle2;
    public GameObject trail2;

    //positions of the poi
    public Transform leftPoi;
    public Transform rightPoi;

    //there needs to be two libraries for each poi
    public Transform libraryTransform;

    //transforms containing all of the particles
    public Transform particleFolder;
    public Transform trailFolder;

    private List<GameObject> particles;
    private List<GameObject> trails;

    //transforms containing all of the particles
    public Transform particleFolder2;
    public Transform trailFolder2;

    private List<GameObject> particles2;
    private List<GameObject> trails2;

    // Use this for initialization
    void Start ()
    {
        particles = new List<GameObject>();
        trails = new List<GameObject>();
        particles2 = new List<GameObject>();
        trails2 = new List<GameObject>();

        Transform[] allChildren = particleFolder.transform.GetComponentsInChildren<Transform>();

        //get all of the immediate children from a list of all children
        foreach (Transform child in allChildren)
        {
            if (child.transform.parent == particleFolder)
            {
                particles.Add(child.gameObject);
                Disable(child.gameObject);
            }
        }

        allChildren = particleFolder2.transform.GetComponentsInChildren<Transform>();

        //get all of the immediate children from a list of all children
        foreach (Transform child in allChildren)
        {
            if (child.transform.parent == particleFolder2)
            {
                particles2.Add(child.gameObject);
                Disable(child.gameObject);
            }
        }

        allChildren = trailFolder.transform.GetComponentsInChildren<Transform>();

        //get all of the immediate children from a list of all children
        foreach (Transform child in allChildren)
        {
            if (child.transform.parent == trailFolder)
            {
                trails.Add(child.gameObject);
                Disable(child.gameObject);
            }
        }

        allChildren = trailFolder2.transform.GetComponentsInChildren<Transform>();

        //get all of the immediate children from a list of all children
        foreach (Transform child in allChildren)
        {
            if (child.transform.parent == trailFolder2)
            {
                trails2.Add(child.gameObject);
                Disable(child.gameObject);
            }
        }

        SetParticle(0);
        SetTrail(0);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}


    /*
    * SetParticle
    * 
    * enables the selected particle system and disables the previous one
    * 
    * @param int activeID - the id of the asset in library to change
    */
    public void SetParticle(int activeID)
    {
        //only change if the particle effect is available
        if (activeID >= particles.Count)
        {
            return;
        }

        Disable(particle);
        Disable(particle2);
        Enable(particles[activeID], leftPoi);
        Enable(particles2[activeID], rightPoi);

        particle = particles[activeID];
        particle2 = particles2[activeID];
    }


    /*
    * SetTrail
    * 
    * enables the selected particle system and disables the previous one
    * 
    * @param int activeID - the id of the asset in library to change
    */
    public void SetTrail(int activeID)
    {
        //only change if the particle effect is available
        if (activeID >= trails.Count)
        {
            return;
        }

        Disable(trail);
        Disable(trail2);
        Enable(trails[activeID], leftPoi);
        Enable(trails2[activeID], rightPoi);

        trail = trails[activeID];
        trail2 = trails2[activeID];
    }


    /*
    * Enable 
    * 
    * enables the particle systems under a specific game-object
    * 
    * @param GameObject system - parent object that contains all particle systems
    * @param Transform root - the system needs to be moved after being enabled
    * @returns void
    */
    public void Enable(GameObject system, Transform root)
    {
    
        system.SetActive(true);

        ParticleSystem[] ps = system.GetComponentsInChildren<ParticleSystem>();

        //enable all of the particle system
        for (int i = 0; i < ps.GetLength(0); i++)
        {
            ps[i].Play();
        }

        system.transform.SetParent(root);
        system.transform.localPosition = Vector3.zero;
        system.transform.localRotation = Quaternion.identity;
        system.transform.localScale = Vector3.one;
    }


    /*
    * Disable
    * 
    * disables the particle systems under a specific game-object
    * 
    * @param GameObject system - parent object that contains all particle systems
    * @returns void
    */
    public void Disable(GameObject system)
    {
        if (system == null)
        {
            return;
        }

        ParticleSystem[] ps = system.GetComponentsInChildren<ParticleSystem>();

        //enable all of the particle system
        for (int i = 0; i < ps.GetLength(0); i++)
        {
            ps[i].Stop();
        }

        system.transform.SetParent(libraryTransform);
        system.transform.localPosition = Vector3.zero;
        system.transform.localRotation = Quaternion.identity;
        system.transform.localScale = Vector3.one;

        system.SetActive(false);
    }


}
