using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimatedDoor))]
public class DoorStateProvider : MonoBehaviour, IObservableStateProvider {

    private AnimatedDoor door;


    public List<StateEntry> GetState() {
        return new List<StateEntry>
        {
        new StateEntry
        {
            key = "locked",
            value = door.IsLocked.ToString().ToLower()
        },

        new StateEntry
        {
            key = "open",
            value = (door.CurrentDoorState == DoorState.Open)
                .ToString()
                .ToLower()
        }
    };
    }

    private void Awake() {
        door = GetComponent<AnimatedDoor>();
    }
}
