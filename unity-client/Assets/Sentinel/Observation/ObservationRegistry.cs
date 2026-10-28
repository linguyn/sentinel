using System.Collections.Generic;

public static class ObservationRegistry {
    private static readonly HashSet<ObservableEntity> observableEntities
        = new HashSet<ObservableEntity>();

    public static void Register(ObservableEntity entity) {
        observableEntities.Add(entity);
    }

    public static void Unregister(ObservableEntity entity) {
        observableEntities.Remove(entity);
    }

    public static IEnumerable<ObservableEntity> GetEntities() {
        return observableEntities;
    }

    public static int Count {
        get { return observableEntities.Count; }
    }
}