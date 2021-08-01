using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{    
    [SerializeField] private float speed; // set speed in inspector
    [SerializeField] private float rotateSpeed;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform shootPoint;
    private Vector3 moveDirection;

    private Rigidbody rb;
    

    // private int hullPoints;
    // private int shieldPoints;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {        
        // ABSTRACTION
        MoveShip();
        RotateShip();        
    }

    private void MoveShip()
    {
        rb.AddRelativeForce(moveDirection * speed);
    }

    private void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        moveDirection = new Vector3(inputVec.x, 0, inputVec.y);
    }

    private void OnShootLaser()
    {
        GameObject laser = ObjectPool.SharedInstance.GetPooledObject();
        if (laser != null)
        {
            laser.transform.SetPositionAndRotation(shootPoint.position, transform.rotation);
            laser.SetActive(true);
            laser.GetComponent<MoveLaser>().StartLaserTimer();
        }

    }
    private void OnShootMissile()
    {
        if (GetClosestEnemy() != null)
        {
            GameObject tempMissile;
            tempMissile = Instantiate(missilePrefab, transform.position, Quaternion.identity);
            tempMissile.GetComponent<HomingMissile>().Fire(GetClosestEnemy());
        }
        
    }
    private void OnShield()
    {
        Debug.Log("Shield Activated!");
    }
    private void RotateShip()
    {
        Vector3 lookDirection = (MousePosition2D.MouseWorldPosition - transform.position).normalized;
        Quaternion rotateShip = Quaternion.LookRotation(lookDirection);

        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotateShip, rotateSpeed));
    }
    
    private Transform GetClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }

        return bestTarget;
    }
}
