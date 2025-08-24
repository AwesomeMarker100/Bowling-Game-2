using UnityEngine;

[CreateAssetMenu(fileName = "ValkPhysMat", menuName = "ValkPhysMat")]
public class ValkPhysMat : ScriptableObject
{

    private enum CombineMode
    {
        Average,
        Minimum,
        Maximum,
        Multiply
    }

    [SerializeField] float staticFrictionCoefficient = 0.1f;
    [SerializeField] float kineticFrictionCoefficient = 0.1f;
    [SerializeField] float coefficientOfRestitution = 1;
    [SerializeField] CombineMode frictionCombineMode = CombineMode.Average;
    [SerializeField] CombineMode bounceCombineMode = CombineMode.Average; 
}
