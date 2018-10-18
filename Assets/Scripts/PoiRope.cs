using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class PoiRope 
* 
* controls the spawning of nodes in a rope made of 
* rigidbodies connected with spring joints
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class PoiRope : MonoBehaviour
{
    public const float SPHERE_RADIUS = 0.25f;

    /*
    * class RopeProperties
    * 
    * defines all rope characteristics
    * 
    * @author: Bradley Booth, Academy of Interactive Entertainment, 2018
    */
    public class RopeProperties
    {
        public float m_radius = 0.0f;
        public float m_distance = 0.0f;
        public float m_elasticity = 0.0f;
        public float m_weight = 0.0f;
        public float m_dampening = 0.0f;
        public float m_drag = 0.0f;
    }

    /*
    * class PoiBezier
    * 
    * forms and calculates the final position of a recursive bezier
    * 
    * @author: Bradley Booth, Academy of Interactive Entertainment, 2018
    */
    public class PoiBezier
    {
        //positions to calculate the beizer with
        public Transform[] bezierPositions;

        //the rig joint to set the position to
        public Transform rigJoint;

        //local interpolation of the rig accross the beizer
        public float interpolationValue = 0.0f;

        /*
        * CalculateBezier 
        * 
        * interpolates the rig joint accross the beizer positions using
        * a recursive lerping algorithm
        * 
        * @returns void
        */
        public void CalculateBezier()
        {
            Vector3 mid1 = Vector3.Lerp(bezierPositions[0].position, bezierPositions[1].position, interpolationValue);
            Vector3 mid2 = Vector3.Lerp(bezierPositions[1].position, bezierPositions[2].position, interpolationValue);
            rigJoint.position = Vector3.Lerp(mid1, mid2, interpolationValue);

            rigJoint.up = (mid1 - mid2).normalized;
        }
    }

    public GameObject poiEnd;

    public Rigidbody ropeStart = null;
    public int ropeLength = 0;

    public RopeProperties m_properties;
    public List<GameObject> m_spawnedNodes;

    public Mesh nodeMesh;
    public Material nodeMaterial;
    public Material lineMaterial;

    //visual model of the poi
    public GameObject poiModel;
    public Transform[] poiJoints;
    public PoiBezier[] poiBeziers;

    public float stretchScalar = 1.75f;
    private float m_maxStretchValue = 0.0f;

    private bool pausedSimulation = false;

    void Start()
    {
        m_spawnedNodes = new List<GameObject>();

        m_properties = new RopeProperties();

        m_properties.m_elasticity = 17500.0f;
        m_properties.m_distance = 0.03f;
        m_properties.m_radius = 0.05f;
        m_properties.m_weight = 1.0f;
        m_properties.m_dampening = 3.5f;
        m_properties.m_drag = 1.0f;

    
        poiJoints = poiModel.GetComponentsInChildren<Transform>();

        CreateNodes(m_properties, 20);
    }

    /*
    * CreateNodes
    * 
    * generates new nodes for the rope
    * 
    * @param RopeProperties properties - the properties for the new nodes
    * @param int amount - the amount of new nodes to create
    * @returns void
    */
    void CreateNodes(RopeProperties properties, int amount)
    {
        //repeat the function for every node
        for (int i = 0; i < amount; i++)
        {
            int snc = m_spawnedNodes.Count;

            GameObject node = new GameObject();

            node.name = "RopeNode";

            node.transform.localScale = Vector3.one * properties.m_radius;

            MeshFilter meshF = node.AddComponent<MeshFilter>();
            meshF.mesh = nodeMesh;

            MeshRenderer meshR = node.AddComponent<MeshRenderer>();
            meshR.material = nodeMaterial;

            meshR.enabled = false;

            SpringJoint sj = node.AddComponent<SpringJoint>();

            sj.enablePreprocessing = false;
            sj.enableCollision = true;

            Rigidbody rb = node.GetComponent<Rigidbody>();

            rb.mass = properties.m_weight;
            rb.drag = properties.m_drag;

            sj.spring = properties.m_elasticity;
            sj.damper = properties.m_dampening;

            sj.minDistance = properties.m_distance;
            sj.maxDistance = properties.m_distance;

            sj.autoConfigureConnectedAnchor = false;

            sj.anchor = Vector3.up * SPHERE_RADIUS;
            sj.connectedAnchor = Vector3.up * -SPHERE_RADIUS;

            SphereCollider sc = node.AddComponent<SphereCollider>();
            sc.radius = SPHERE_RADIUS;

            LineRenderer lr = node.AddComponent<LineRenderer>();

            lr.enabled = false;

            lr.startWidth = properties.m_radius * 0.5f;
            lr.endWidth = properties.m_radius * 0.5f;

            Vector3[] positions = new Vector3[] { node.transform.position, ropeStart.transform.position };

            if (snc == 0)
            {
                //attach to the remote
                sj.connectedBody = ropeStart;
                node.transform.position = ropeStart.transform.position + ropeStart.transform.rotation * Vector3.forward * properties.m_distance;
            }
            else if (snc == 1)
            {
                //attach to the furthest rope node
                sj.connectedBody = m_spawnedNodes[snc - 1].GetComponent<Rigidbody>();
                node.transform.position = m_spawnedNodes[snc - 1].transform.position + (m_spawnedNodes[snc - 1].transform.position - ropeStart.transform.position).normalized * properties.m_distance;

                positions = new Vector3[] { node.transform.position, m_spawnedNodes[snc - 1].transform.position };
            }
            else
            {
                //attach to the furthest rope node
                sj.connectedBody = m_spawnedNodes[snc - 1].GetComponent<Rigidbody>();
                node.transform.position = m_spawnedNodes[snc - 1].transform.position + (m_spawnedNodes[snc - 2].transform.position - m_spawnedNodes[snc - 1].transform.position).normalized * properties.m_distance;

                positions = new Vector3[] { node.transform.position, m_spawnedNodes[snc - 1].transform.position };
            }

            lr.material = lineMaterial;
            lr.startColor = Color.white;
            lr.endColor = Color.white;
            lr.SetPositions(positions);

            m_spawnedNodes.Add(node);
        }

        UpdateTether();

        UpdateRope();
    }

    /*
    * DeleteNodes
    * 
    * deletes nodes from the rope
    * 
    * @param int amount - the amount of new nodes to create
    * @returns void
    */
    void DeleteNodes(int amount)
    {
        int snc = m_spawnedNodes.Count;

        //cannot remove anymore
        if (snc == 0)
        {
            return;
        }

        //repeat the function the specified amount of times
        for (int i = 0; i < amount; i++)
        {
            snc = m_spawnedNodes.Count;

            Destroy(m_spawnedNodes[snc - 1]);
            m_spawnedNodes.RemoveAt(snc - 1);
        }

        UpdateRope();
    }

    /*
    * SetLength 
    * 
    * dynamically spawns and deletes nodes to obtain the specified length
    * 
    * @param int nodeCount - the target amount of nodes
    * @returns void
    */
    public void SetLength(int nodeCount)
    {
        int snc = m_spawnedNodes.Count;

        if (snc > nodeCount)
        {
            DeleteNodes(snc - nodeCount);
            UpdateTether();
        }
        else if (snc < nodeCount)
        {
            CreateNodes(m_properties, nodeCount - snc);
            UpdateTether();
        }
    }

    /*
    * SetRopeProperties 
    * 
    * sets the new properties for nodes spawned after this is called
    * 
    * @param RopeProperties properties - the properties for the new nodes
    * @returns void
    */
    void SetRopeProperties(RopeProperties properties)
    {
        m_properties = properties;

        int snc = m_spawnedNodes.Count;

        for (int i = 0; i < snc; i++)
        {
            SpringJoint sj = m_spawnedNodes[i].GetComponent<SpringJoint>();

            sj.spring = m_properties.m_elasticity;

            Rigidbody rb = m_spawnedNodes[i].GetComponent<Rigidbody>();

            rb.drag = m_properties.m_drag;

        }
    }

    /*
    * UpdateTether 
    * 
    * calculates where each joint of the poi model should
    * be relative to each node in the 
    * 
    * @returns void 
    */
    public void UpdateTether()
    {
        List<GameObject> nodes = new List<GameObject>();
        nodes.Add(ropeStart.gameObject);
        nodes.AddRange(m_spawnedNodes);

        int jl = poiJoints.GetLength(0);
        int snc = nodes.Count;

        if (snc > 2)
        {

            poiBeziers = new PoiBezier[jl];

            //iterate through all joints and bezier curves and map all of the joints to the best fitting bezier curve
            for (int i = 0; i < jl; i++)
            {

                //distance to the best bezier curves
                float bestMap = 0.75f;
                float trueMap = 0.0f;
                int bj = -1;

                for (int j = 0; j < snc - 2; j++)
                {
                    float jointLerp = (float)i / (jl - 1);

                    float minLerp = j / (float)(snc - 1);
                    float maxLerp = (j + 2) / (float)(snc - 1);

                    float map = (jointLerp - minLerp) / (maxLerp - minLerp);

                    //compatible beizer curve
                    if (map >= 0.0f && map <= 1.0f)
                    {
                        float distance = Mathf.Abs(0.5f - map);

                        //maximum check
                        if (bestMap > distance)
                        {
                            bestMap = distance;
                            trueMap = map;
                            bj = j;
                        }
                    }

                    //all compatible bezier curves have been passed
                    if (map < 0.0f)
                    {
                        break;
                    }
                }

                PoiBezier bezier = new PoiBezier();

                bezier.rigJoint = poiJoints[i];
                bezier.bezierPositions = new Transform[3] { nodes[bj].transform, nodes[bj + 1].transform, nodes[bj + 2].transform };
                bezier.interpolationValue = trueMap;

                poiBeziers[i] = bezier;
            }
        }
    }

    /*
    * UpdateRope 
    * 
    * controls the line renderers that are attached to the rope nodes
    * 
    * @returns void
    */
    void UpdateRope()
    {
      
        int snc = m_spawnedNodes.Count;

        m_maxStretchValue = m_properties.m_distance * snc * stretchScalar;

        for (int i = 0; i < snc; i++)
        {
            LineRenderer lr = m_spawnedNodes[i].GetComponent<LineRenderer>();

            if (i == 0)
            {
                Vector3[] positions = new Vector3[] { m_spawnedNodes[i].transform.position, ropeStart.transform.position };
                lr.SetPositions(positions);
            }
            else
            {
                Vector3[] positions = new Vector3[] { m_spawnedNodes[i].transform.position, m_spawnedNodes[i - 1].transform.position };
                lr.SetPositions(positions);
            }
        }

     
        //assign the fixed joint
        if (snc > 1)
        {
            FixedJoint fj = poiEnd.GetComponent<FixedJoint>();

            fj.connectedBody = m_spawnedNodes[snc - 1].GetComponent<Rigidbody>();
            fj.autoConfigureConnectedAnchor = false;
            fj.connectedAnchor = Vector3.zero;
        }
        else
        {
            FixedJoint fj = poiEnd.GetComponent<FixedJoint>();

            fj.connectedBody = ropeStart.GetComponent<Rigidbody>();
            fj.autoConfigureConnectedAnchor = false;
            fj.connectedAnchor = Vector3.zero;

        }

        //scale the velocities and positions down if the string becomes too stretched
        if (snc >= 1)
        {
            Vector3 rootPosition = ropeStart.transform.position;
            Vector3 endPosition = m_spawnedNodes[snc - 1].transform.position;

            float magnitude = (endPosition - rootPosition).magnitude;

            if (magnitude > m_maxStretchValue)
            { 
                Vector3[] previousPositions = new Vector3[snc];

                //remember the unaltered previous positions
                for (int i = 0; i < snc; i++)
                {
                    Vector3 previousPosition = ropeStart.transform.position;

                    if (i > 0)
                    {
                        previousPosition = m_spawnedNodes[i - 1].transform.position;
                    }

                    previousPositions[i] = previousPosition;
                }

                float ratio = m_maxStretchValue / magnitude;

                //scale the relative positions and the velocities of each node in the string down
                for (int i = 0; i < snc; i++)
                {
                    Vector3 relative = (m_spawnedNodes[i].transform.position - previousPositions[i]);

                    m_spawnedNodes[i].transform.position = previousPositions[i] + relative * ratio;
                    m_spawnedNodes[i].GetComponent<Rigidbody>().velocity *= ratio;
                }
            }
        }

        foreach (PoiBezier pb in poiBeziers)
        {
            pb.CalculateBezier();
        }

    }

    /*
    * PauseSimulation 
    * 
    * enables/disables the simulation of the poi rope
    * 
    * @param bool paused - the flag that indicates which state to switch to
    * @returns void
    */
    public void PauseSimulation(bool paused)
    {
        //remote hasn't been turned on, there are no spawned nodes
        if (m_spawnedNodes == null)
        {
            return;
        }

        int snc = m_spawnedNodes.Count;

        for (int i = 0; i < snc; i++)
        {
            m_spawnedNodes[i].GetComponent<Rigidbody>().isKinematic = paused;
        }

        pausedSimulation = paused;
    }

    /*
    * ResetJoints 
    * 
    * due to a bug with the joint system when the steam overlay is displayed
    * the joints need to be reset whenever steam is unpaused
    * 
    * @returns void
    */
    public void ResetJoints()
    {
        int prevLength = ropeLength;

        SetLength(0);
        SetLength(prevLength);
    }

    /*
    * CalculateTension 
    * 
    * calculates the average tension accross all nodes in the rope
    * according to the spring equation (kx + b)
    * 
    * @returns float - the average tension
    */
    public float CalculateTension()
    {
        if (m_spawnedNodes == null || pausedSimulation)
        {
            return 0.0f;
        }

        int snc = m_spawnedNodes.Count;

        float ratioSum = 0.0f;

        for (int i = 1; i < snc; i++)
        {
            //calculate the relative distance of each node in the chain
            Vector3 relative = m_spawnedNodes[i].transform.position - m_spawnedNodes[i - 1].transform.position;

            float relativeMagnitude = relative.magnitude;

            //calculate how hard the spring is pulling back
            float ratio = relativeMagnitude / m_properties.m_distance;

            ratioSum += ratio;
        }

        ratioSum /= snc;

        return ratioSum;
    }
	void Update ()
    {
        //only update the poi rope if it isn't paused
        if (!pausedSimulation)
        {
            SetLength(ropeLength);
            UpdateRope();
        }

        m_properties.m_elasticity = GameProperties.G_ELASTICITY;
        m_properties.m_drag = GameProperties.G_DRAG;

        SetRopeProperties(m_properties);
    }
}
