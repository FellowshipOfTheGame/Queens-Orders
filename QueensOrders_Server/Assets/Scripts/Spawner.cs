using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Spawner myScript = (Spawner)target;
        if (GUILayout.Button("Warrior!"))
        {
            myScript.SpawnWarrior();
        }
    }
}
#endif

public class Spawner : MonoBehaviour {

    public GameObject warrior;

	public void SpawnWarrior()
    {
        Instantiate(warrior, GetComponent<Transform>().position, GetComponent<Transform>().rotation);
    }
}
