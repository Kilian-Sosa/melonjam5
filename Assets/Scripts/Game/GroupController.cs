using System.Collections.Generic;
using UnityEngine;

public class GroupController : MonoBehaviour {
    public List<GameObject> groupMembers = new List<GameObject>();

    void Start() {
        // groupMembers = new List<GameObject> { gameObject };
    }

    public void AddToGroup(GameObject member) {
        if (groupMembers != null && !groupMembers.Contains(member)) groupMembers.Add(member);
    }

    public int GetGroupSize() => groupMembers.Count;
}