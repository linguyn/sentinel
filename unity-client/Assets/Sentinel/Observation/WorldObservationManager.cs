using System.Collections.Generic;
using UnityEngine; 


public static class WorldObservationManager {
    private static List<EntityObservation> worldObservation = new List<EntityObservation>(); 

    public static void SetWorldObservation() {
        worldObservation.Clear();
        var observableEntities = ObservationRegistry.GetEntities(); 
        foreach (ObservableEntity entities in observableEntities) {
            worldObservation.Add(entities.GetObservation()); 
        }
    }
    
    public static List<EntityObservation> GetWorldObservation() {
        return worldObservation; 
    }

}
