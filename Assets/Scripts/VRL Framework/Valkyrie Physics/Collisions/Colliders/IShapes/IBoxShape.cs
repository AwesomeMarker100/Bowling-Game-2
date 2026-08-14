using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.VRL_Framework.Valkyrie_Physics.Collisions.Colliders.IShapes
{
    public class IBoxShape : IColliderShape
    {

        private Vector3 topRightBack;
        private Vector3 topRightFront;
        private Vector3 topLeftBack;
        private Vector3 topLeftFront;

        private Vector3 bottomRightBack;
        private Vector3 bottomRightFront;
        private Vector3 bottomLeftBack;
        private Vector3 bottomLeftFront;

        private Vector3[] verts;

        public IBoxShape(Vector3 globalCenter, Vector3 size, Quaternion rotation)
        {
            float xMin = globalCenter.x - (size.x / 2);
            float xMax = globalCenter.x + (size.x / 2);
            float yMin = globalCenter.y - (size.y / 2);
            float yMax = globalCenter.y + (size.y / 2);
            float zMin = globalCenter.z - (size.z / 2);
            float zMax = globalCenter.z + (size.z / 2);

            topLeftFront = rotation * (new Vector3(xMin, yMax, zMin) - globalCenter);
            topLeftFront += globalCenter;

            topRightFront = rotation * (new Vector3(xMax, yMax, zMin) - globalCenter);
            topRightFront += globalCenter;

            bottomLeftFront = rotation * (new Vector3(xMin, yMin, zMin) - globalCenter);
            bottomLeftFront += globalCenter;

            bottomRightFront = rotation * (new Vector3(xMax, yMin, zMin) - globalCenter);
            bottomRightFront += globalCenter;

            topLeftBack = rotation * (new Vector3(xMin, yMax, zMax) - globalCenter);
            topLeftBack += globalCenter;

            topRightBack = rotation * (new Vector3(xMax, yMax, zMax) - globalCenter);
            topRightBack += globalCenter;


            bottomLeftBack = rotation * (new Vector3(xMin, yMin, zMax) - globalCenter);
            bottomLeftBack += globalCenter;


            bottomRightBack = rotation * (new Vector3(xMax, yMin, zMax) - globalCenter);
            bottomRightBack += globalCenter;

            verts = new Vector3[] {topLeftFront,
            topRightFront,
            bottomLeftFront,
            bottomRightFront,
            topLeftBack,
            topRightBack,
            bottomLeftBack,
            bottomRightBack,};
        }

        public Vector3 GetFurthestPoint(Vector3 dir)
        {
            Vector3 furthestPoint = topLeftFront;

            for (int i = 1; verts.Length > i; i++)
            {
                //if the furthest point isn't as aligned as well to the given direction as another point then change the furthest point
                if (Vector3.Dot(furthestPoint, dir) < Vector3.Dot(verts[i], dir)) furthestPoint = verts[i];

            }

            return furthestPoint;
        }
    }
}
