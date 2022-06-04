using System;
using System.Collections.Generic;


[Serializable]
public class Geometry
{
    public string type;
    public List<List<List<double>>> coordinates;
}

[Serializable]
public class Properties
{
    public double globalIndicator;
    public bool valueAvailable;
}

[Serializable]
public class Feature
{
    public string id;
    public string type;
    public Properties properties;
    public Geometry geometry;
}

[Serializable]
public class ComputeIndicatorResponse
{
    public string type;
    public List<Feature> features;
}