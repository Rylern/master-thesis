using System;
using System.Collections.Generic;



[Serializable]
public class AddCategoryRequest
{
    public string name;
    public List<string> indicators;

    public AddCategoryRequest(string name, List<string> indicators)
    {
        this.name = name;
        this.indicators = indicators;
    }
}