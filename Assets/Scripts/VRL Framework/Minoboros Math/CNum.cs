using org.mariuszgromada.math.mxparser.parsertokens;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public struct CNum
{
    public float Re { get; private set; }
    public float Im { get; private set; }

    public float modulus { get; private set; }

    public float modulusSqr { get; private set; }

    public float argument { get; private set; }


    //Operator Overloads
    #region

    //Equality Operators
    #region
    public static bool operator ==(CNum z1, CNum z2)
    {
        return z1.Re == z2.Re && z1.Im == z2.Im;
    }

    public static bool operator !=(CNum z1, CNum z2)
    {
        return !(z1 == z2);
    }
    #endregion

    //Casting to float, int, Vec2
    #region
    public static explicit operator float(CNum z)
    {
        return z.Re;
    }

    #endregion

    //Addition and Subtraction 
    #region
    public static CNum operator +(CNum z1, CNum z2)
    {
        return new CNum(z1.Re + z2.Re, z1.Im + z2.Im);
    }

    public static CNum operator +(CNum z, float a)
    {
        return new CNum(z.Re + a, z.Im);
    }

    public static CNum operator +(float a, CNum z)
    {
        return z + a;
    }

    public static CNum operator -(CNum z)
    {
        return new CNum(-z.Re, -z.Im);
    }

   

    public static CNum operator -(CNum z1, CNum z2)
    {
        return new CNum(z1.Re - z2.Re, z1.Im - z2.Im);
    }

    public static CNum operator -(CNum z, float a)
    {
        return new CNum(z.Re - a, z.Im);
    }

    public static CNum operator -(float a, CNum z)
    {
        return -(z - a);
    }

    #endregion

    //Multiplication and Division
    #region
    public static CNum operator *(CNum z1, CNum z2)
    {
        return new CNum(z1.Re * z2.Re - z1.Im * z2.Im, z1.Re * z2.Im + z1.Im * z2.Re);
    }

    public static CNum operator *(CNum z, float a) { 

        return new CNum(z.Re* a, z.Im * a);

    }

    public static CNum operator *(float a, CNum z)
    {
        return z * a;
    }

    public static CNum operator /(CNum z, float a)
    {
        return new CNum(z.Re / a, z.Im / a);
    }

    public static CNum operator /(float a, CNum z1)
    {
        return a * z1.GetConjugate() / z1.modulusSqr;
    }

    public static CNum operator /(CNum z1, CNum z2)
    {
        return (z1 * z2.GetConjugate()) / z2.modulusSqr;
    }

    #endregion
    #endregion

    //Common CNums
    public static CNum zero = new CNum(0, 0);
    public static CNum one = new CNum(1, 0);
    public static CNum i = new CNum(0, 1);


    public CNum(float Re, float Im)
    {
        this.Re = Re;
        this.Im = Im;

        this.modulusSqr = Mathf.Pow(Re, 2) + Mathf.Pow(Im, 2);
        this.modulus = Mathf.Sqrt(modulusSqr);
        this.argument = Mathf.Atan2(Im, Re);

    }

    public CNum(float Re, float Im, float modulusSqr, float argument)
    {
        this.Re = Re;
        this.Im = Im;
        this.modulusSqr = modulusSqr;

        modulus = Mathf.Sqrt(modulusSqr);
        this.argument = argument;
    }

    public CNum GetConjugate()
    {
        return new CNum(Re, -Im);
    }


    public CNum GetNormalized()
    {
        if (modulus == 0) return CNum.zero;
        else if (modulus == 1) return this;
        return this / modulus;
    }

    //using Euler's formula
    public static CNum FromPolar(float modulus, float argument)
    {
        return modulus * new CNum(Mathf.Cos(argument), Mathf.Sin(argument));
    }

    public static CNum FromVector2(Vector2 v) => new CNum(v.x, v.y);
    public override string ToString() => $"{Re} + j{Im}";
    public CNum GetReciprocal() => 1 / this;
 
    //raising e to a complex power
    public static CNum Exp(CNum z)
    {
        //e^z = e^(Re{z}) * e^(jIm{z}) = e^(Re{z}) * (cos(Im{z}) + jsin(Im{z}))
        float modulus = Mathf.Exp(z.Re);
        return FromPolar(modulus, z.Im);
    }

    public CNum Pow(float a)
    {
        return FromPolar(Mathf.Pow(modulus, a), argument * a);
    }

   public CNum Pow(CNum z2)
    {
        float natLogR = Mathf.Log(MinoMath.e, modulus);

        float alpha = natLogR * z2.Re - argument * z2.Im;
        float beta = natLogR * z2.Im + argument * z2.Re;

        return FromPolar(Mathf.Exp(alpha), beta);
    }

}
