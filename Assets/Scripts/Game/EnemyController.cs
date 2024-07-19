using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
    [SerializeField] float _playerDetectionDistance, _playerAttackDistance;
    Vector3 _destination;
    Transform _player;
    List<GameObject> _groupMembers;
    bool _isLeader = false;

    void Start() {
        //transform.Rotate(-90, 0, 0); // Fix rotation
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _groupMembers = new List<GameObject> { gameObject };
        _isLeader = true;
    }

    void Update() {
        if (!GameManager.Instance.isActive) {
            StopAllCoroutines();
            return;
        }
        if (!_isLeader && _groupMembers.Count > 1) FollowLeader();
    }

    void SetRandomDestination() {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position + Random.insideUnitSphere * 10f, out hit, 10f, NavMesh.AllAreas)) {
            _destination = hit.position;
            SetDestinationForGroup(_destination);
        }
    }

    void SetDestinationForGroup(Vector3 destination) {
        if (_isLeader && _groupMembers.Count > 0) {
            foreach (GameObject member in _groupMembers) {
                NavMeshAgent agent = member.GetComponent<NavMeshAgent>();
                if (agent != null && agent.enabled) {
                    agent.SetDestination(destination);
                    // member.GetComponent<Animator>().SetFloat("velocity", 2);
                }
            }
        }
    }

    public void StartGroupPatrol() {
        if (_isLeader) StartCoroutine(Patrol());
    }

    public void StartGroupAlert() {
        if (_isLeader) StartCoroutine(Alert());
    }

    public void StartGroupCrazy() {
        if (_isLeader) StartCoroutine(GoCrazy());
    }

    IEnumerator Patrol() {
        SetRandomDestination();
        while (true) {
            if (Vector3.Distance(transform.position, _destination) < 1.5f) {
                // foreach (GameObject member in _groupMembers)
                //    member.GetComponent<Animator>().SetFloat("velocity", 0);
                yield return new WaitForSeconds(Random.Range(1f, 5f));
                SetRandomDestination();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Alert() {
        while (true) {
            if (Vector3.Distance(transform.position, _player.position) < _playerDetectionDistance) {
                yield return new WaitForSeconds(Random.Range(1f, 3f));
                SetRandomDestination();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator GoCrazy() {
        //while (true) {
        //    if (Random.Range(0, 1) < 0.05f) GetComponent<SyncController>().HandleEnemyConversion();
        //    yield return new WaitForSeconds(3f);
        //}
        yield return null;
    }

    public void SetLeader(bool isLeader) {
        this._isLeader = isLeader;
        if (isLeader) {
            StopAllCoroutines();
            StartGroupPatrol();
        }
    }

    void FollowLeader() {
        if (_groupMembers.Count > 0) {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            GameObject leader = _groupMembers[0];
            if (agent != null && leader != null) agent.SetDestination(leader.transform.position);
        }
    }

    public List<GameObject> GetGroupMembers() => _groupMembers;
}