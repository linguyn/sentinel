using System;
using UnityEngine;

[Flags]
public enum PlayerAction {
    None = 0,
    Sprint = 1 << 0,
    Jump = 1 << 1,
    Crouch = 1 << 2,
    Attack = 1 << 3,
    Interact = 1 << 4
}

public class CommonIntent {

    private static Vector3 moveDirection;
    private static PlayerAction actions; 

    public static Vector3 MoveDirection {
        get { return moveDirection; }
        set { moveDirection = value; }
    }

    public static PlayerAction HumanAction {
        get { return actions;  }
        set { actions = value; }
    }
}
