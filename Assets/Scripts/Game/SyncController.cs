using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SyncController : MonoBehaviour {
    [SerializeField] GameObject[] _balls;
    [SerializeField] BoxCollider[] _colliders;
    PlayerController _controller;
    EnemyController _enemyController;
    NavMeshAgent _navMeshAgent;
    GroupController _groupController;

    void Start() {
        _controller = GetComponent<PlayerController>();
        _enemyController = GetComponent<EnemyController>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _groupController = GetComponent<GroupController>();
    }

    void Update() {
        if (!GameManager.Instance.isActive) FreezePosition();
        else UnFreezePositon();
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            HandlePlayerCollision(collision.gameObject);
        } else if (collision.gameObject.CompareTag("Enemy")) {
            HandleEnemyCollision(collision.gameObject);
        }
    }

    void HandlePlayerCollision(GameObject player) {
        if (_controller.enabled) return;
        HandleEnemyConversion(player);
    }

    void HandleEnemyCollision(GameObject enemy) {
        GroupController enemyGroup = enemy.GetComponent<GroupController>();

        // Convert enemy group to players
        foreach (GameObject member in new List<GameObject>(enemyGroup.groupMembers)) {
            member.GetComponent<SyncController>().HandlePlayerCollision(gameObject);
        }

        // TODO: Fix this

        //// Merge groups if both are enemies
        // if (_groupController.groupMembers[0].CompareTag("Enemy") && enemy.CompareTag("Enemy")) {
        //    foreach (GameObject member in enemyGroup.groupMembers) {
        //        _groupController.AddToGroup(member);
        //    }
        //    enemyGroup.groupMembers.Clear();

        //    // Set the first member as the leader
        //    SetGroupLeader(_groupController.groupMembers[0]);
        // } else {
        //    // Compare group sizes
        //    if (_groupController.GetGroupSize() >= enemyGroup.GetGroupSize()) {
        //        // Convert enemy group to players
        //        foreach (GameObject member in enemyGroup.groupMembers) {
        //            member.GetComponent<SyncController>().HandlePlayerCollision(gameObject);
        //        }
        //    } else {
        //        // Convert player group to enemies
        //        foreach (GameObject member in _groupController.groupMembers) {
        //            member.GetComponent<SyncController>().HandleEnemyConversion();
        //        }
        //        GameManager.Instance.GameOver();
        //    }
        // }
    }

    public void HandlePlayerConversion() {
        if (_controller.enabled) {
            _controller.enabled = false;
            _enemyController.enabled = true;
            _navMeshAgent.enabled = true;
            ChangeBalls(0); // TODO: Show the correct ball
            gameObject.tag = "Enemy";

            // Add this object to its own group
            _groupController.groupMembers.Clear();
            _groupController.groupMembers.Add(gameObject);

            // Set the first member as the leader
            SetGroupLeader(_groupController.groupMembers[0]);

            // Update GameManager
            GameManager.Instance.ChangePlayerCount();
        }
    }

    public void HandleEnemyConversion(GameObject player = null) {
        if (_enemyController.enabled) {
            player = player != null ? player : GameObject.Find("Player");
            if (!AudioManager.Instance.IsPlayingCountDown()) AudioManager.Instance.PlaySFX("enemyConversion");

            if (_colliders.Length == 2) {
                _colliders[0].enabled = false;
                _colliders[1].enabled = true;
            }
            _controller.enabled = true;
            _enemyController.StopAllCoroutines();
            _enemyController.enabled = false;
            _navMeshAgent.enabled = false;
            ChangeBalls(1);
            gameObject.tag = "Player";

            // Add this object to its own group
            _groupController.groupMembers.Clear();

            GroupController playerGroupController = player.GetComponent<GroupController>();
            playerGroupController.AddToGroup(gameObject);
            _groupController.groupMembers = playerGroupController.groupMembers;
            _controller.SetAttributes(player.GetComponent<PlayerController>().GetSpeed(), player.GetComponent<PlayerController>().GetJumpForce());

            // Update GameManager
            GameManager.Instance.ChangePlayerCount();
        }
    }

    void SetGroupLeader(GameObject leader) {
        foreach (GameObject member in _groupController.groupMembers) {
            EnemyController enemyController = member.GetComponent<EnemyController>();
            enemyController.SetLeader(member == leader);
        }
    }

    public void ChangeBalls(int i) {
        for (int j = 0; j < _balls.Length; j++) _balls[j].SetActive(j == i);
    }

    void FreezePosition() {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
    }

    void UnFreezePositon() {
        if (GetComponent<Rigidbody>().constraints != RigidbodyConstraints.FreezePosition) return;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }
}