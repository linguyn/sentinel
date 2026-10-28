using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObservableEntity : MonoBehaviour {
    [SerializeField] private string entityType;

    private string entityId;
    private IObservableStateProvider[] stateProviders;

    private void Awake() {
        entityId = entityType + "_" + gameObject.GetEntityId().ToString();

        stateProviders = GetComponents<MonoBehaviour>()
            .OfType<IObservableStateProvider>()
            .ToArray();
    }

    public EntityObservation GetObservation() {
        List<StateEntry> states = new List<StateEntry>();

        foreach (IObservableStateProvider provider in stateProviders) {
            List<StateEntry> providerStates = provider.GetState();

            if (providerStates != null) {
                states.AddRange(providerStates);
            }
        }

        EntityObservation observation = new EntityObservation {
            id = entityId,

            type = entityType,

            position = new Position {
                x = transform.position.x,
                y = transform.position.y,
                z = transform.position.z
            },

            state = states
        };

        return observation;
    }

    private void OnEnable() {
        ObservationRegistry.Register(this);
    }

    private void OnDisable() {
        ObservationRegistry.Unregister(this);
    }
}