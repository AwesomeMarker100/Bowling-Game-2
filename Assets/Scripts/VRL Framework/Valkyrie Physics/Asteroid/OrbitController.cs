
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{

    /*
     * ALL OF THIS SHIT IS BASED FROM THIS WEBSITE http://www.physicsbootcamp.org/The-Orbit-Equation.html
     * THAT WAS A LIE-- THESE WEBSITES WERE ALSO HOMIES
     * http://dslavsk.sites.luc.edu/courses/phys314/classnotes/celestialmechanics.pdf 
     * https://www.vanderbilt.edu/AnS/physics/astrocourses/ast201/angular_momentum.html
     * https://www.mathsisfun.com/polar-cartesian-coordinates.html
     * https://www.lehman.edu/faculty/anchordoqui/chapter25.pdf
     * https://galileo.phys.virginia.edu/classes/152.mf1i.spring02/EllipticOrbits.htm
     * https://en.wikipedia.org/wiki/Standard_gravitational_parameter
     * 
     * L- angular momentum
     * e- eccentricity
     * 
     * 
     * ORBIT IS IN POLAR COORDINATES WHERE RADIUS IS THE X AND ANGLE IS THE Y
     * 
     */

    [SerializeField] public ValkyrieRigidbody center;
    [SerializeField] private bool useVRB = false;

    [SerializeField] public float orbitalPeriod = 2;
    [SerializeField][Range(0, tau)] private float angleDelta = Mathf.PI / 12;

    //Eccentricity is how much a conic curve deviates from a circular curve since orbits are not perfectly circular
    //if this is zero, it will orbit like a circle
    //if this is one, it becomes a parabola and it will never complete an orbit you donkey

    [SerializeField][Range(0, 1)] public float eccentricity = 0.02f;

    //initial radius -- starting radius

    private const float tau = 2 * Mathf.PI;
    private float radius = 3f;

    public bool orbit = true;


    //ANOMALIES
    private float meanAnomaly = 0f;
    private float trueAnomaly = 0f;

    //x is radius, y is angle
    private Vector3 semiMinorAxis;
    private Vector3 semiMajorAxis;

    Vector3 initCenterPos = Vector3.zero;
    Vector3 initThisPos = Vector3.zero;


    private float xCoord = 0.0f;
    private float zCoord = 0.0f;

    private ValkyrieRigidbody vrb;

    //https://stjarnhimlen.se/comp/ppcomp.html#3 carried the team

    // Start is called before the first frame update
    void Awake()
    {

        //initCenterPos = center.transform.position;
        initThisPos = this.transform.position;
        initCenterPos = center.transform.position;

        //periapsis is the closest length of the ellipse
        semiMinorAxis = initCenterPos - initThisPos;
        semiMajorAxis = (Quaternion.AngleAxis(90f, Vector3.up) * semiMinorAxis).normalized * (semiMinorAxis.magnitude * Mathf.Sqrt(1 - Mathf.Pow(eccentricity, 2)));
        

        vrb = GetComponent<ValkyrieRigidbody>();
        meanAnomaly = Random.Range(0, tau - Mathf.Epsilon);


    }

    /*private void Start()
    {
        meanAnomaly += (tau / orbitalPeriod) * Time.deltaTime;
        if (meanAnomaly >= tau) meanAnomaly -= tau;

        ComputeTrueAnomaly();
        SetPosition(false);
    }*/

    //KEPLER's ORIGINAL EQUATION IS M = E - esinE where M is mean anomaly, E is eccentric anomaly, and e is eccentricity

    // Update is called once per frame
    void Update()
    {
        if (orbit)
        {
            //mean anomaly is an angle that tracks the change in angle of the orbiting body from periapsis 
            

            ComputeCoordinates();
            SetPosition(useVRB);

            meanAnomaly += (tau / orbitalPeriod) * Time.deltaTime;
            if (meanAnomaly >= tau) meanAnomaly -= tau;
        }
    }




    //true anomaly is the actual angle the body is from periapsis
    private void ComputeCoordinates()
    {
        //need eccentric anomaly to compute true anomaly
        float eccentricAnomaly = GetEccentricAnomaly();
        
        //computing x and y components of true anomaly to get the angle
        xCoord = (semiMajorAxis.magnitude * (Mathf.Cos(eccentricAnomaly) - eccentricity));
        zCoord = (semiMinorAxis.magnitude * Mathf.Sin(eccentricAnomaly));

        //using inverse tangent 2(which will get correct angle no matter what quadrant) passing in our x and y components
        
        //trueAnomaly = Mathf.Atan2(yv, xv);
       // radius = Mathf.Sqrt(Mathf.Pow(xv, 2) + Mathf.Pow(yv, 2));

        //make sure its not negative or above tau
        if (trueAnomaly < 0) trueAnomaly += tau;
        if (trueAnomaly >= tau) trueAnomaly -= tau;

        //didn't convert to cartesian coords but rather just used polar coords
        

    }

    private void SetPosition(bool useVRB)
    {
        // Vector3 newPos = radius * (Quaternion.AngleAxis(trueAnomaly * Mathf.Rad2Deg, Vector3.up) * periapsisVector).normalized;

        Vector3 newPos = new Vector3(xCoord, transform.position.y, zCoord);
        if (useVRB)
        {
            if (vrb != null) vrb.SetVelocity((newPos + center.transform.position - transform.position));
            else transform.position = newPos + center.transform.position;
        }
        else
        {
            transform.position = newPos + center.transform.position ;
        }
    }

    //another angle that is used in calculating position in orbit, derived from mean and true anomaly

    private float GetEccentricAnomaly()
    {
        float approximation = meanAnomaly;

        int i = 0;
        int maxIter = 200;

        float delta = meanAnomaly;

        while (maxIter > i && delta > 0.8f) //Newton's Method for making a function converge
        {   
            
            float newApproximation = approximation - (GetApproximateAnomaly(approximation) / (1 - (eccentricity * Mathf.Cos(approximation))));
            delta = Mathf.Abs(newApproximation - approximation);

            approximation = newApproximation;
            i++;
        }

        return approximation;

    }

    public void FollowOrbit()
    {

        OrbitController[] controllers = FindObjectsOfType<OrbitController>();

        float minDist = Vector3.Distance(controllers[0].transform.position, transform.position);
        OrbitController theOne = controllers[0];

        for(int i = 1; i < controllers.Length; i++)
        {

            if(Vector3.Distance(controllers[i].transform.position, transform.position) < minDist)
            {

                theOne = controllers[i];

            }


        }

        this.semiMinorAxis = theOne.semiMinorAxis;
        this.orbitalPeriod = theOne.orbitalPeriod;
        this.eccentricity = theOne.eccentricity;
        this.angleDelta = theOne.angleDelta;
        this.center = theOne.center;

        this.transform.position = theOne.transform.position;

        this.orbit = true;


    }

    public void StopOrbit()
    {

        orbit = false;

    }

    private float GetApproximateAnomaly(float approximate)
    {

        return approximate - (eccentricity * Mathf.Sin(approximate)) - meanAnomaly;

    }

    private float GetDerivativeOfApproximateAnomaly(float approximate)
    {

        return 1 + eccentricity * Mathf.Cos(approximate);

    }


}
