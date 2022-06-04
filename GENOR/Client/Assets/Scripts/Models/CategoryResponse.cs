using System;
using System.Collections.Generic;


[Serializable]
public class Category
{
    public string name;
    public List<string> indicators;
}

[Serializable]
public class CategoryResponse
{
    public List<Category> categories;
}