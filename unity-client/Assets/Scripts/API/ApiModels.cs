using System; 

[Serializable]
public class Observation
{
    public bool has_key;
    public bool door_locked; 
}

[Serializable]
public class ActionResponse {

    public string action;  
}
