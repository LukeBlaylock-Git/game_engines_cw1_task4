using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    
    public enum State { Patrol, Chase } //These are the states our AI will alternate between.
    public State CurrentState; //This is just to save the State that is presently being used, eg patrol, chase, etc
    [Header("References")]
    public NavMeshAgent Agent; //Navmesh agent refers to the "Navmesh agent" that is on the actual enemy itself.
    public Transform PlayerTarget; //The Player which the "guard" is looking for.
    [Header("Patrol Settings")]
    public Transform[] Waypoints; // A list of points to visit
    private int CurrentWaypointIndex = 0; // Which point are we going to?
    public float WaitTimer = 0f; //How long our guard is going to wait at each waypoint.
    public float LostPlayerDelay = 2f;
    public float LostPlayerTimer = 0f;
    [Header("Detection")]
    public float DetectionRange = 10f; // A "Dome" around the enemy, which says how close they need to be to be detected.
    [Range(0, 360)]
    public float ViewAngle = 90f; // The angle of the "Cone" which the player has to be within to be "detected"
    //Below are the layer masks used to prevent the guard cone from just spotting our player through a wall.
    public LayerMask ObstacleMask; // Layers that block vision, basically "walls"
    public LayerMask PlayerMask; // Layers that are for the "player"
    void Start()
    {
        if (Agent == null) Agent = GetComponent<NavMeshAgent>();
        if (Waypoints.Length > 0) //Starts the "guard" moving towards the first waypoint.
        {
            Agent.SetDestination(Waypoints[0].position);
        }
    }
    void Update()
    {
       // The "Brain" of our Guard, switches between the two current states, this is checked very tick since its in "Update"
        switch (CurrentState)
        {
            case State.Patrol:
                PatrolLogic(); //Function call for moving between points.
                CheckForPlayer();  //Function call for looking for the player.
                break;
            case State.Chase: 
                ChaseLogic(); //Function call for chasing the player.
                CheckIfLostPlayer(); //Function call for checking the player is out of range.
                break;
        }
    }
    void PatrolLogic()
    {
        if (Waypoints.Length == 0) return; //No waypoints, no patrol.

        //Checking if the guard has reached their current waypoint.
        if (!Agent.pathPending && Agent.remainingDistance < 0.5f)
        {
           
            WaitTimer += Time.deltaTime; //Wait at the waypoint for the WaitTimer duration.

            if (WaitTimer > 2f)
            {
                CurrentWaypointIndex = (CurrentWaypointIndex + 1) % Waypoints.Length;
                Agent.SetDestination(Waypoints[CurrentWaypointIndex].position);
                WaitTimer = 0f;
            }
        }
    }

    void ChaseLogic()
    {
        // Simply move to the player
        if (PlayerTarget != null) //When player is NOT outside detection
        {
            Agent.SetDestination(PlayerTarget.position);
        }
    }
    void CheckForPlayer()
    {
        if (CanSeePlayer())
        {
            CurrentState = State.Chase;
        }
    }
    void CheckIfLostPlayer()
    {
        float Dist = Vector3.Distance(transform.position, PlayerTarget.position);


        if (Dist > DetectionRange || !CanSeePlayer()) // Checking if the player is out of range or we can no longer see them.
        {
            LostPlayerTimer += Time.deltaTime; //Starting the count when the player is out of view
            if (LostPlayerTimer >= LostPlayerDelay) //If the timer is less than or equal to LostPlayerDelay, return to patrol.
            {
                CurrentState = State.Patrol;
                LostPlayerTimer = 0f;
                Agent.SetDestination(Waypoints[CurrentWaypointIndex].position);
            }
        }
        else
        {
            LostPlayerTimer = 0f; //If the player is still visible, the timer is reset to 0.
        }
    }
    // Debug Visualization (Draws ranges in the Editor)
    void OnDrawGizmosSelected()
    {
        // Draw Detection Range Circle for the PlayerDetection.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
        // Draw Vision Cone Lines for the View
        Vector3 ViewAngleA = DirFromAngle(-ViewAngle / 2, false);
        Vector3 ViewAngleB = DirFromAngle(ViewAngle / 2, false);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + ViewAngleA *
        DetectionRange);
        Gizmos.DrawLine(transform.position, transform.position + ViewAngleB *
        DetectionRange);
    }
    Vector3 DirFromAngle(float AngleInDegrees, bool AngleIsGlobal) //Inbuilt function to convert a anglet o a direction vector.
    {
        if (!AngleIsGlobal)
        {
            AngleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(AngleInDegrees * Mathf.Deg2Rad), 0,
        Mathf.Cos(AngleInDegrees * Mathf.Deg2Rad));
    }
        bool CanSeePlayer()
    {
        if (PlayerTarget == null) return false;
        // Checking Distance
        Vector3 DirToPlayer = (PlayerTarget.position - transform.position);
        float DstToPlayer = DirToPlayer.magnitude;
        if (DstToPlayer < DetectionRange)
        {
           
            // Vector3.Angle calculates the angle between our forward direction and the player
        if (Vector3.Angle(transform.forward, DirToPlayer) < ViewAngle / 2)
            {
                // 3. Obstruction Check (Raycast)
                // We shoot a laser to the player. If it hits an obstacle, we can't see.
        if (!Physics.Raycast(transform.position, DirToPlayer, DstToPlayer, ObstacleMask))
                {
                    return true; // Player seen.
                }
            }
        }
        return false; // Player is not seen.
    }
    public void ResetGuard() //Whole point of this function is to reset the guard.
    {
        CurrentState = State.Patrol;
        LostPlayerTimer = 0f;
        WaitTimer = 0f; 

        CurrentWaypointIndex = 0;

        if(Waypoints.Length > 0)
        {
            transform.position = Waypoints[0].position;
            Agent.SetDestination(Waypoints[0].position);
        }
    }
}