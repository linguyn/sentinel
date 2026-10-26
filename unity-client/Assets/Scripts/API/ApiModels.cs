using System;
using System.Collections.Generic;

/**ApiModels(DTO): blueprints that responsible for creating data shapes/classes for backend communication**/

[System.Serializable]
public class StateEntry {
    public string key;
    public string value;
}

[System.Serializable]
public class EntityObservation
{
    public string id;
    public string type;
    public Position position;
    public List<StateEntry> state;
}

[System.Serializable]
public class Position {
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class ActionResponse {

    public string action;
    public Position position;
    public string target_id;
}


[System.Serializable]
public class ActionHistory {
    public string action;
    public string target_id;
    public ActionStatus action_status;
    public string reason; 
}