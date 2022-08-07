using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public GameObject playerMesh;
    public float horizontal = 0;
    public float vertical = 0;
    public Vector3 direction = new Vector3();
    public float camAngle = 0;

    public float speed = 6f;
    public float normSpeed = 6f;
    public float fastSpeed = 30f;

    // Killian method
    /*
    public Vector3 GetMoveDirection(float h, float v)
    {
        if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f))
        {
            return Vector3.zero;
        }
    
        Camera viewCamera = Camera.main;
        // Do the move in screenspace.
        Vector3 screenOrigin = viewCamera.WorldToScreenPoint(transform.position);
        float screenRight = screenOrigin.x + h * 100f;
        float screenUp = screenOrigin.y + v * 100f;
    
        Ray ray = viewCamera.ScreenPointToRay(new Vector3(screenRight, screenUp));
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        bool success = groundPlane.Raycast(ray, out float d);
        Debug.Assert(success);
        Debug.DrawRay(ray.origin + ray.direction * d, Vector3.up * 10f);
        // Construct worldspace move direction from orthagonal unit vectors.
        Vector3 moveVector = (ray.origin + ray.direction * d) - transform.position;
    
        return moveVector.normalized;
    }
    //*/

    // Worldspace Diag
    //*
    public Vector3 GetMoveDirection(float h, float v)
    {
        if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f))
        {
            return Vector3.zero;
        }

        Camera viewCamera = Camera.main;
        // Do the move in screenspace.
        Vector3 screenOrigin = viewCamera.WorldToScreenPoint(transform.position);
        float screenH = screenOrigin.x + 100f;
        float screenV = screenOrigin.y + 100f;

        Ray rayH = viewCamera.ScreenPointToRay(new Vector3(screenH, screenOrigin.y, screenOrigin.z));
        Ray rayV = viewCamera.ScreenPointToRay(new Vector3(screenOrigin.x, screenV, screenOrigin.z));

        Plane groundPlane = new Plane(Vector3.up, transform.position);
        bool success = groundPlane.Raycast(rayH, out float hT);
        success &= groundPlane.Raycast(rayV, out float vT);
        if (!success)
        {
            Debug.LogWarning("PlayerMovement could not cast a ray from the camera " + (viewCamera?.name).ToString() + " to the ground plane of the player.");
            return Vector3.zero;
        }

        #region Draw Raycasts
        //*
        void DrawX(Vector3 pos)
        {
            Debug.DrawLine(pos + new Vector3(-0.5f, 0f, 0.5f), pos + new Vector3(0.5f, 0f, -0.5f));
            Debug.DrawLine(pos + new Vector3(0.5f, 0f, 0.5f), pos + new Vector3(-0.5f, 0f, -0.5f));
        }

        Debug.DrawLine(transform.position, rayH.origin + rayH.direction * hT);
        DrawX(rayH.origin + rayH.direction * hT);
        Debug.DrawLine(transform.position, rayV.origin + rayV.direction * vT);
        DrawX(rayV.origin + rayV.direction * vT);
        //*/
        #endregion

        Vector3 hPos = ((rayH.origin + rayH.direction * hT) - transform.position).normalized;
        Vector3 vPos = ((rayV.origin + rayV.direction * vT) - transform.position).normalized;
        vPos *= (Vector3.Dot(viewCamera.transform.forward, vPos) > 0f ? 1f : -1f);
        // Construct worldspace move direction from orthagonal unit vectors.
        Vector3 moveVector = hPos * h + vPos * v;

        return moveVector.normalized;
    }
    //*/

    // Update is called once per frame
    void Update()
    {
        // Raycast to a vertical plane defined by the player's current position to get the worldspace equivalent point.
        // Get input.
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Vector3 moveVector = GetMoveDirection(horizontal, vertical);
        if (moveVector.sqrMagnitude >= 0.1f)
        {
            controller.Move(moveVector * speed * Time.deltaTime);
            Debug.DrawRay(transform.position, moveVector * 5, Color.yellow, .01f); // draw the debug ray
        }
        DebugLoop();
    }

    private void DebugLoop()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = fastSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = normSpeed;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + GetMoveDirection(1f, 0f) * speed, transform.position + GetMoveDirection(-1f, 0f) * speed);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + GetMoveDirection(0f, -1f) * speed, transform.position + GetMoveDirection(0f, 1f) * speed);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + GetMoveDirection(-1f, 1f) * speed, transform.position + GetMoveDirection(1f, -1f) * speed);
        Gizmos.DrawLine(transform.position + GetMoveDirection(1f, 1f) * speed, transform.position + GetMoveDirection(-1f, -1f) * speed);
    }

}
