using System;
using UnityEngine;

namespace YxModDll.Mod.Features
{
    public class AI : MonoBehaviour
    {
    	public enum AIState
    	{
    		Walk,
    		WalkClose,
    		Jump,
    		Climb,
    		ClimbTop
    	}

    	public static int layerMask = 1945697333;

    	public Human human;

    	public Vector3 target;

    	public Vector3 curTarget;

    	public int walkForward;

    	public int walkRight;

    	public float cameraPitch;

    	public bool jump;

    	public bool reach;

    	public AIState state;

    	public void Start()
    	{
    		state = AIState.Walk;
    		jump = true;
    		walkForward = 1;
    		cameraPitch = 20f;
    	}

    	public void GetPath()
    	{
    		MonoBehaviour.print($"State: {state}");
    		switch (state)
    		{
    		case AIState.Walk:
    		{
    			Vector3 direction2 = curTarget - human.transform.position;
    			human.controls.cameraYawAngle = Mathf.Atan2(direction2.x, direction2.z) * 57.29578f;
    			walkRight = (((int)(Time.time * 4f) % 6 >= 3) ? 1 : (-1));
    			if (Physics.Raycast(human.transform.position, direction2, out var hitInfo7, direction2.magnitude + 1f, layerMask, QueryTriggerInteraction.Ignore))
    			{
    				Vector3 vector2 = Vector3.Project(hitInfo7.point - human.transform.position, hitInfo7.normal);
    				if (vector2.sqrMagnitude < 36f)
    				{
    					state = AIState.WalkClose;
    					jump = false;
    					walkRight = 0;
    					human.controls.cameraYawAngle = Mathf.Atan2(0f - hitInfo7.normal.x, 0f - hitInfo7.normal.z) * 57.29578f;
    					cameraPitch = -80f;
    					reach = true;
    					curTarget = human.transform.position + vector2;
    				}
    			}
    			break;
    		}
    		case AIState.WalkClose:
    		{
    			Vector3 direction = curTarget - human.transform.position;
    			if (Physics.Raycast(human.transform.position, direction, out var hitInfo4, direction.magnitude + 1f, layerMask, QueryTriggerInteraction.Ignore) && hitInfo4.distance < 2f)
    			{
    				state = AIState.Jump;
    				jump = true;
    			}
    			if (Physics.Raycast(human.ragdoll.partLeftHand.transform.position, YawToVector3(human.controls.cameraYawAngle), out var hitInfo5, direction.magnitude + 1f, layerMask, QueryTriggerInteraction.Ignore) && hitInfo5.distance < 0.1f)
    			{
    				this.Schedule(3, delegate
    				{
    					state = AIState.Climb;
    					jump = false;
    					FeatureManager.autoClimb = true;
    					FeatureManager.climbState = 0;
    				});
    			}
    			break;
    		}
    		case AIState.Jump:
    		{
    			Vector3 vector = curTarget - human.transform.position;
    			if (Physics.Raycast(human.ragdoll.partLeftHand.transform.position, YawToVector3(human.controls.cameraYawAngle), out var hitInfo2, vector.magnitude + 1f, layerMask, QueryTriggerInteraction.Ignore) && hitInfo2.distance < 0.1f)
    			{
    				this.Schedule(3, delegate
    				{
    					state = AIState.Climb;
    					jump = false;
    					FeatureManager.autoClimb = true;
    					FeatureManager.climbState = 0;
    				});
    			}
    			if (Physics.Raycast(human.transform.position, YawToVector3(human.controls.cameraYawAngle), out var hitInfo3, vector.magnitude + 1f, layerMask, QueryTriggerInteraction.Ignore) && hitInfo3.distance < 0.4f)
    			{
    				this.Schedule(3, delegate
    				{
    					state = AIState.Climb;
    					jump = false;
    					FeatureManager.autoClimb = true;
    					FeatureManager.climbState = 0;
    				});
    			}
    			break;
    		}
    		case AIState.Climb:
    		{
    			if (!Physics.Raycast(human.transform.position, YawToVector3(human.controls.cameraYawAngle), out var _, 1f, layerMask, QueryTriggerInteraction.Ignore))
    			{
    				state = AIState.ClimbTop;
    				jump = false;
    				cameraPitch = 80f;
    				FeatureManager.autoClimb = false;
    			}
    			break;
    		}
    		case AIState.ClimbTop:
    		{
    			if (Physics.Raycast(human.transform.position, Vector3.down, out var hitInfo, 1f, layerMask, QueryTriggerInteraction.Ignore) && Mathf.Abs(hitInfo.distance - 0.3f) < 0.01f)
    			{
    				state = AIState.Walk;
    				reach = false;
    				cameraPitch = 20f;
    				curTarget = target;
    				this.Schedule(10, delegate
    				{
    					jump = true;
    				});
    			}
    			break;
    		}
    		}
    	}

    	public void FixedUpdate()
    	{
    		GetPath();
    	}

    	public static Vector3 YawToVector3(float yaw)
    	{
    		float f = yaw * ((float)Math.PI / 180f);
    		return new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
    	}
    }
}
