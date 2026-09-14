using System; 

/**ApiModels(DTO): blueprints that responsible for creating data shapes/classes for backend communication**/

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
