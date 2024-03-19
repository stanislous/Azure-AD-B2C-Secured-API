﻿using Newtonsoft.Json;

namespace TodoListService.Models;

public class InputClaimsModel
{
    // Demo: User's object id in Azure AD B2C
    public string signInName { get; set; }
    public string password { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static InputClaimsModel Parse(string JSON)
    {
        return JsonConvert.DeserializeObject(JSON, typeof(InputClaimsModel)) as InputClaimsModel;
    }
}