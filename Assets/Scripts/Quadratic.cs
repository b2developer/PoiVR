using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* class Quadratic 
* 
* stores and solves quadratic equations for their roots
* 
* @author: Bradley Booth, Academy of Interactive Entertainment, 2018
*/
public class Quadratic
{
    public const float QUADRATIC_EPSILON = 1e-5f;

    //coefficients
    public float a = 0;
    public float b = 0;
    public float c = 0;

    /*
    * Quadratic() 
    * default constructor
    */
    public Quadratic()
    {

    }


    /*
    * Quadratic(float _a, float _b, float _c) 
    * 
    * constructor, passes in parameters
    * 
    * @param float _a - x^2 coefficient
    * @param float _b - x^1 coefficient
    * @param float _c - x^0 coefficient
    */
    public Quadratic(float _a, float _b, float _c)
    {
        a = _a;
        b = _b;
        c = _c;
    }

    /*
    * Solve 
    * 
    * uses the quadratic equation to solve for roots
    * 
    * @returns float[] - a list of the roots (0-2 in length)
    */
    public float[] Solve()
    {
        //discriminant determines how many roots there are
        float discrim = b * b - 4 * a * c;

        if (discrim < QUADRATIC_EPSILON)
        {
            //no roots
            return new float[] { };
        }
        else if (discrim > QUADRATIC_EPSILON)
        {
            //2 roots
            float nRoot = (-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
            float pRoot = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

            return new float[2] { nRoot, pRoot };
        }
        else
        {
            //1 root
            return new float[1] { -b / (2 * a) };
        }
    }
}
