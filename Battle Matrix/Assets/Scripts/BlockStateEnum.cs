using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockStateEnum : MonoBehaviour {
    public enum BlockState { Falling, Locked, Matched, Attack, Ticking }
    // falling (not due to attack), locked in place, matched, falling due to attack, locked but ticking down until it can match
}
