using UnityEngine;
using System.Collections;

[System.Serializable]
public class HitPoints : System.Object {

    public int max = 1;
    private float current = 1f;

    public void Increment(int value) {
        Current += value;
    }

    public void Decrement(int value) {
        Current -= value;
    }

    public int Current {
        get { return (int)current*max; }
        set { current = value/max; }
    }
}
