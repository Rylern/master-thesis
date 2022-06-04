using System.Collections;
using System.Collections.Generic;

public class Indicator
{
    public string name { get; private set; }
    public float weight { get; private set; }

    public Indicator()
    {}
    
    public Indicator(string name, float weight): this()
    {
        this.name = name;
        this.weight = weight;
    }
}
