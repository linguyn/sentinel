using System.Collections.Generic;
using UnityEngine;

public static class ActionRegistry {
    private static readonly Dictionary<string, IActionAdapter> registeredActions
            = new Dictionary<string, IActionAdapter>();

    public static void Register(IActionAdapter adapter) {
        if (registeredActions.TryGetValue(adapter.ActionName, out IActionAdapter existing)) {
            // Throw error if there's duplicated actions 
            Debug.LogError(
                $"Duplicate action '{adapter.ActionName}': already registered by " +
                $"'{(existing as Component)?.name}', ignoring the one on '{(adapter as Component)?.name}'."
            );
            return;
        }

        registeredActions.Add(adapter.ActionName, adapter);
    }

    public static void Unregister(IActionAdapter adapter) {
        if (registeredActions.TryGetValue(adapter.ActionName, out IActionAdapter existing)  
            && existing == adapter) {
            registeredActions.Remove(adapter.ActionName);
        }
    }


    // Function for EXECUTOR to find an adapter by name 
    public static bool TryGet(string actionName, out IActionAdapter adapter) {
        return registeredActions.TryGetValue(actionName, out adapter);
    }

    public static IEnumerable<IActionAdapter> GetAll() {
        return registeredActions.Values;
    }

    public static int Count {
        get { return registeredActions.Count; }
    }
}
