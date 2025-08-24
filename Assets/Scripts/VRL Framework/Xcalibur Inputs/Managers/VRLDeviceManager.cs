using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[ExecuteAlways]
public class VRLDeviceManager : MonoBehaviour
{

    public static InputDevice GetHeadDevice()
    {

        return InputDevices.GetDeviceAtXRNode(XRNode.Head);

    }

    public static InputDevice GetLeftHandController()
    {

        return InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

    }

    public static InputDevice GetRightHandController()
    {

        return InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

    }

}
