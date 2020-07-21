using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BGC.Annotation.Basic
{
    [Serializable]
    public class Position
    {
        public float x;
        public float y;
        public float z;
    }
    [Serializable]
    public class Rotation
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }
    [Serializable]
    public class Coordinates
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    public class AnnotationEntity
    {
        public string type;  // Mandatory : Spot, Polyline, Polygon, Circle, Sphere, Cube, Free, Pointer
        public int instanceID;  // Mandatory : Spot, Polyline, Polygon, Circle, Sphere, Cube, Free, Pointer
        public string coordinate_system; // utm, tm, Cartesian
        public string label; // any text
        public string line_color; // red, blue, yellow, or #4542 - color code
        public string thickness; // float
        public string line_style; // solid, dotted, loosly dotted
        public string fill_color; // red, blue, yellow, or #4542 - color code BUT, less than 50% transparency 
        public string symbol; // 
        public Position position;
        public Rotation rotation;
        public Coordinates coordinates;
        public string tag; // any data for the annotation
        public List<AnnotationEntity> children = new List<AnnotationEntity>();
    }
    [Serializable]
    public class BgcAnnotation
    {
        public string datetime; // updated date time
        public string coordinate_system; // utm, tm, Cartesian
        public string group; // group name which created the annotation
        public string tag; // any data for the annotation
        public List<AnnotationEntity> annotationEntities = new List<AnnotationEntity>();

        public BgcAnnotation()
        {
        }

        public static BgcAnnotation instance = null;

        public static BgcAnnotation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BgcAnnotation();
                }
                return instance;
            }
        }

    }
/*
    [Serializable]
    public class RootObject
    {
        public BgcAnnotation bgc_annotation;
    }
    */
}