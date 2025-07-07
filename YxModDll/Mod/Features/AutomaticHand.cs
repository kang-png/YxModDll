using System;
using System.Collections;
using System.Collections.Generic;
using HumanAPI;
using Multiplayer;
using UnityEngine;


namespace YxModDll.Mod.Features
{
#if false
    // Token: 0x0200087D RID: 2173
    public class AutomaticHand : MonoBehaviour
    {
        // Token: 0x04002F82 RID: 12162
        public static AutomaticHand ins;
        // Token: 0x04002F86 RID: 12166
        private List<Human> autoHuman;
        // Token: 0x04002F87 RID: 12167
        private HandMuscles.TargetingMode targetingMode = HandMuscles.TargetingMode.Chest;
        // Token: 0x04002F88 RID: 12168
        private HandMuscles.TargetingMode grabTargetingMode = HandMuscles.TargetingMode.Ball;
        // Token: 0x04002F89 RID: 12169
        private AutomaticHand.ScanMem leftMem = new AutomaticHand.ScanMem();
        // Token: 0x04002F8A RID: 12170
        private AutomaticHand.ScanMem rightMem = new AutomaticHand.ScanMem();
        // Token: 0x04002F8B RID: 12171
        private Collider[] colliders = new Collider[10];
        // Token: 0x04002F8C RID: 12172
        public float maxLiftForce;
        // Token: 0x04002F8D RID: 12173
        public float maxPushForce;
        // Token: 0x04002F8E RID: 12174
        public float liftDampSqr;
        // Token: 0x04002F8F RID: 12175
        public float liftDamp;
        // Token: 0x04002F90 RID: 12176
        public float armMass;
        // Token: 0x04002F91 RID: 12177
        public float bodyMass;
        // Token: 0x04002F92 RID: 12178
        public float maxForce;
        // Token: 0x04002F93 RID: 12179
        public float grabMaxForce;
        // Token: 0x04002F94 RID: 12180
        public float climbMaxForce;
        // Token: 0x04002F95 RID: 12181
        public float gravityForce;
    }
}
// Token: 0x0200087E RID: 2174
public class AutomaticHand : MonoBehaviour
{
    // Token: 0x06003032 RID: 12338 RVA: 0x00020F53 File Offset: 0x0001F153
    public void OnDestory()
    {
        global::UnityEngine.Object.Destroy(this);
        AutomaticHand.ins = null;
    }

    // Token: 0x06003033 RID: 12339 RVA: 0x00020F61 File Offset: 0x0001F161
    public static void AutomaticHandMod()
    {
        if (AutomaticHand.ins == null)
        {
            NetGame.instance.gameObject.AddComponent<AutomaticHand>();
            Debug.Log("自动伸手模式已开启");
            return;
        }
        Debug.Log("自动伸手模式已关闭");
        AutomaticHand.ins.OnDestory();
    }

    // Token: 0x06003034 RID: 12340 RVA: 0x00146508 File Offset: 0x00144708
    public void Update()
    {
        if (NetGame.isClient)
        {
            AutomaticHand.AutomaticHandMod();
            return;
        }
        if (this.autoHuman == null)
        {
            this.autoHuman = new List<Human>();
        }
        foreach (Human human in this.autoHuman)
        {
            if (!(human == null))
            {
                try
                {
                    float num = (human.function_Attribute.autohandup ? 1f : 0f);
                    float num2 = (human.function_Attribute.autohandup ? 1f : 0f);
                    human.controls.leftGrab = human.function_Attribute.autohandup;
                    human.controls.rightGrab = human.function_Attribute.autohandup;
                    bool autohandup = human.function_Attribute.autohandup;
                    if (!NetGame.isClient)
                    {
                        this.OnFixedUpdate(human, num, num2, autohandup, autohandup);
                    }
                    else
                    {
                        float z = human.controls.walkDirection.z;
                        float x = human.controls.walkDirection.x;
                        float cameraPitchAngle = human.controls.cameraPitchAngle;
                        float cameraYawAngle = human.controls.cameraYawAngle;
                        bool jump = human.controls.jump;
                        bool unconscious = human.controls.unconscious;
                        bool shootingFirework = human.controls.shootingFirework;
                        human.player.SendMove(z, x, cameraPitchAngle, cameraYawAngle, num, num2, jump, unconscious, shootingFirework);
                    }
                    this.JurgeColloder(human);
                    continue;
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                    continue;
                }
            }
            this.autoHuman.Remove(human);
            if (this.autoHuman.Count == 0)
            {
                AutomaticHand.AutomaticHandMod();
                break;
            }
            break;
        }
    }

    // Token: 0x06003035 RID: 12341 RVA: 0x001466E8 File Offset: 0x001448E8
    private float AverageVelcityy(Human human)
    {
        if (human.ragdoll.partBall.rigidbody.velocity.y < 0f)
        {
            return human.ragdoll.partBall.rigidbody.velocity.y;
        }
        if (human.ragdoll.partChest.rigidbody.velocity.y < 0f)
        {
            return human.ragdoll.partChest.rigidbody.velocity.y;
        }
        if (human.ragdoll.partWaist.rigidbody.velocity.y < 0f)
        {
            return human.ragdoll.partWaist.rigidbody.velocity.y;
        }
        return 0f;
    }

    // Token: 0x06003036 RID: 12342 RVA: 0x001467B0 File Offset: 0x001449B0
    private bool isLocalCollider(Collider collider)
    {
        string name = collider.name;
        string[] array = new string[]
        {
            "Head", "Chest", "Waist", "Hips", "LeftArm", "LeftForearm", "LeftHand", "LeftThigh", "LeftLeg", "LeftFoot",
            "RightArm", "RightForearm", "RightHand", "RightThigh", "RightLeg", "RightFoot", "Ball"
        };
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(name))
            {
                return true;
            }
            string text = name.Clone() as string;
            if (text.Length > array[i].Length)
            {
                text = text.Substring(0, array[i].Length);
                if (text.Equals(array[i]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Token: 0x06003037 RID: 12343 RVA: 0x00020F9F File Offset: 0x0001F19F
    private IEnumerator enLargeYaw(Human human)
    {
        yield return null;
        yield break;
    }

    // Token: 0x06003038 RID: 12344 RVA: 0x001468B4 File Offset: 0x00144AB4
    private void JurgeColloder(Human human)
    {
        Rigidbody rigidbody = human.rigidbodies[0];
        foreach (Rigidbody rigidbody2 in human.rigidbodies)
        {
            if (rigidbody2.transform.position.y < rigidbody.transform.position.y)
            {
                rigidbody = rigidbody2;
            }
        }
        if (!NetGame.isClient && AutomaticHand.GetGroundManager(human).onGround && !human.function_Attribute.oldlargeyaw)
        {
            base.StartCoroutine(this.handdownGround2(human));
            return;
        }
        if (NetGame.isClient && !human.function_Attribute.oldlargeyaw && human.onGround)
        {
            base.StartCoroutine(this.handdownGround2(human));
            return;
        }
        if (!NetGame.isClient)
        {
            float num = this.AverageVelcityy(human);
            if (num < 0f && num > -0.5f)
            {
                Collider[] array = Physics.OverlapSphere(rigidbody.transform.position, 0.5f);
                int num2 = 0;
                for (int j = 0; j < array.Length; j++)
                {
                    if (!this.isLocalCollider(array[j]))
                    {
                        num2++;
                    }
                }
                if (num2 != 0)
                {
                    human.function_Attribute.autohandup = true;
                }
            }
        }
        if (!AutomaticHand.AutoHandCamera)
        {
            return;
        }
        float num3 = Time.time - human.function_Attribute.oldautotime;
        if (Math.Abs(human.controls.cameraYawAngle - human.function_Attribute.oldyawangle) / num3 > 140f)
        {
            human.function_Attribute.autohandup = true;
            human.function_Attribute.oldlargeyaw = true;
            base.StartCoroutine(this.enLargeYaw(human));
        }
        human.function_Attribute.oldyawangle = human.controls.cameraYawAngle;
        human.function_Attribute.oldautotime = Time.time;
    }

    // Token: 0x06003039 RID: 12345 RVA: 0x00020FA7 File Offset: 0x0001F1A7
    public IEnumerator handdownGround2(Human human)
    {
        yield return new WaitForSeconds(0.005f);
        human.function_Attribute.autohandup = false;
        if (NetGame.isClient)
        {
            yield return new WaitForSeconds(0.3f);
            human.function_Attribute.autohandup = true;
        }
        yield break;
    }

    // Token: 0x0600303A RID: 12346 RVA: 0x00146A64 File Offset: 0x00144C64
    public void OnFixedUpdate(Human human, float leftExtend, float rightExtend, bool leftGrab, bool rightGrab)
    {
        float targetPitchAngle = human.controls.targetPitchAngle;
        float targetYawAngle = human.controls.targetYawAngle;
        bool flag = leftGrab;
        bool flag2 = rightGrab;
        bool onGround = human.onGround;
        if ((human.ragdoll.partLeftHand.transform.position - human.ragdoll.partChest.transform.position).sqrMagnitude > 6f)
        {
            flag = false;
        }
        if ((human.ragdoll.partRightHand.transform.position - human.ragdoll.partChest.transform.position).sqrMagnitude > 6f)
        {
            flag2 = false;
        }
        Quaternion quaternion = Quaternion.Euler(targetPitchAngle, targetYawAngle, 0f);
        Quaternion quaternion2 = Quaternion.Euler(0f, targetYawAngle, 0f);
        Vector3 vector = Vector3.zero;
        Vector3 vector2 = Vector3.zero;
        float num = 0f;
        float num2 = 0f;
        if (targetPitchAngle > 0f && onGround)
        {
            num2 = 0.4f * targetPitchAngle / 90f;
        }
        HandMuscles.TargetingMode targetingMode = ((!(human.ragdoll.partLeftHand.sensor.grabJoint != null)) ? this.targetingMode : this.grabTargetingMode);
        HandMuscles.TargetingMode targetingMode2 = ((!(human.ragdoll.partRightHand.sensor.grabJoint != null)) ? this.targetingMode : this.grabTargetingMode);
        switch (targetingMode)
        {
            case HandMuscles.TargetingMode.Shoulder:
                vector = human.ragdoll.partLeftArm.transform.position + quaternion * new Vector3(0f, 0f, leftExtend * human.ragdoll.handLength);
                break;
            case HandMuscles.TargetingMode.Chest:
                vector = human.ragdoll.partChest.transform.position + quaternion2 * new Vector3(-0.2f, 0.15f, 0f) + quaternion * new Vector3(0f, 0f, leftExtend * human.ragdoll.handLength);
                break;
            case HandMuscles.TargetingMode.Hips:
                if (targetPitchAngle > 0f)
                {
                    num = -0.3f * targetPitchAngle / 90f;
                }
                vector = human.ragdoll.partHips.transform.position + quaternion2 * new Vector3(-0.2f, 0.65f + num, num2) + quaternion * new Vector3(0f, 0f, leftExtend * human.ragdoll.handLength);
                break;
            case HandMuscles.TargetingMode.Ball:
                if (targetPitchAngle > 0f)
                {
                    num = -0.2f * targetPitchAngle / 90f;
                }
                if (human.ragdoll.partLeftHand.sensor.grabJoint != null)
                {
                    num2 = ((!human.isClimbing) ? 0f : (-0.2f));
                }
                vector = human.ragdoll.partBall.transform.position + quaternion2 * new Vector3(-0.2f, 0.7f + num, num2) + quaternion * new Vector3(0f, 0f, leftExtend * human.ragdoll.handLength);
                break;
        }
        switch (targetingMode2)
        {
            case HandMuscles.TargetingMode.Shoulder:
                vector2 = human.ragdoll.partRightArm.transform.position + quaternion * new Vector3(0f, 0f, rightExtend * human.ragdoll.handLength);
                break;
            case HandMuscles.TargetingMode.Chest:
                vector2 = human.ragdoll.partChest.transform.position + quaternion2 * new Vector3(0.2f, 0.15f, 0f) + quaternion * new Vector3(0f, 0f, rightExtend * human.ragdoll.handLength);
                break;
            case HandMuscles.TargetingMode.Hips:
                if (targetPitchAngle > 0f)
                {
                    num = -0.3f * targetPitchAngle / 90f;
                }
                vector2 = human.ragdoll.partHips.transform.position + quaternion2 * new Vector3(0.2f, 0.65f + num, num2) + quaternion * new Vector3(0f, 0f, rightExtend * human.ragdoll.handLength);
                break;
            case HandMuscles.TargetingMode.Ball:
                if (targetPitchAngle > 0f)
                {
                    num = -0.2f * targetPitchAngle / 90f;
                }
                if (human.ragdoll.partRightHand.sensor.grabJoint != null)
                {
                    num2 = ((!human.isClimbing) ? 0f : (-0.2f));
                }
                vector2 = human.ragdoll.partBall.transform.position + quaternion2 * new Vector3(0.2f, 0.7f + num, num2) + quaternion * new Vector3(0f, 0f, rightExtend * human.ragdoll.handLength);
                break;
        }
        this.ProcessHand(human, this.leftMem, human.ragdoll.partLeftArm, human.ragdoll.partLeftForearm, human.ragdoll.partLeftHand, vector, leftExtend, flag, human.motionControl2.legs.legPhase + 0.5f, false);
        this.ProcessHand(human, this.rightMem, human.ragdoll.partRightArm, human.ragdoll.partRightForearm, human.ragdoll.partRightHand, vector2, rightExtend, flag2, human.motionControl2.legs.legPhase, true);
    }

    // Token: 0x0600303B RID: 12347 RVA: 0x00147028 File Offset: 0x00145228
    private void ProcessHand(Human human, AutomaticHand.ScanMem mem, HumanSegment arm, HumanSegment forearm, HumanSegment hand, Vector3 worldPos, float extend, bool grab, float animationPhase, bool right)
    {
        double num = 0.1 + (double)(0.14f * Mathf.Abs(human.controls.targetPitchAngle - mem.grabAngle) / 80f);
        double num2 = num * 2.0;
        if (CheatCodes.climbCheat)
        {
            num = (num2 = num / 4.0);
        }
        if (grab && !hand.sensor.grab)
        {
            if ((double)mem.grabTime > num)
            {
                mem.pos = arm.transform.position;
            }
            else
            {
                grab = false;
            }
        }
        if (hand.sensor.grab && !grab)
        {
            mem.grabTime = 0f;
            mem.grabAngle = human.controls.targetPitchAngle;
        }
        else
        {
            mem.grabTime += Time.fixedDeltaTime;
        }
        hand.sensor.grab = (double)mem.grabTime > num2 && grab;
        if (extend > 0.2f)
        {
            hand.sensor.targetPosition = worldPos;
            mem.shoulder = arm.transform.position;
            mem.hand = hand.transform.position;
            if (hand.sensor.grabJoint == null)
            {
                worldPos = this.FindTarget(mem, worldPos, out hand.sensor.grabFilter, human.motionControl2);
            }
            this.PlaceHand(arm, hand, worldPos, true, hand.sensor.grabJoint != null, hand.sensor.grabBody, human);
            if (hand.sensor.grabBody != null)
            {
                this.LiftBody(human, hand, hand.sensor.grabBody);
            }
            hand.sensor.grabPosition = worldPos;
            return;
        }
        hand.sensor.grabFilter = null;
        if (human.state == HumanState.Walk)
        {
            this.AnimateHand(human, arm, forearm, hand, animationPhase, 1f, right);
            return;
        }
        if (human.state == HumanState.FreeFall)
        {
            Vector3 targetDirection = human.targetDirection;
            targetDirection.y = 0f;
            HumanMotion2.AlignToVector(arm, arm.transform.up, -targetDirection, 2f);
            HumanMotion2.AlignToVector(forearm, forearm.transform.up, targetDirection, 2f);
            return;
        }
        Vector3 targetDirection2 = human.targetDirection;
        targetDirection2.y = 0f;
        HumanMotion2.AlignToVector(arm, arm.transform.up, -targetDirection2, 20f);
        HumanMotion2.AlignToVector(forearm, forearm.transform.up, targetDirection2, 20f);
    }

    // Token: 0x0600303C RID: 12348 RVA: 0x001472AC File Offset: 0x001454AC
    private void AnimateHand(Human human, HumanSegment arm, HumanSegment forearm, HumanSegment hand, float phase, float tonus, bool right)
    {
        tonus *= 50f * human.controls.walkSpeed;
        phase -= Mathf.Floor(phase);
        Vector3 vector = Quaternion.Euler(0f, human.controls.targetYawAngle, 0f) * Vector3.forward;
        Vector3 vector2 = Quaternion.Euler(0f, human.controls.targetYawAngle, 0f) * Vector3.right;
        if (!right)
        {
            vector2 = -vector2;
        }
        if (phase < 0.5f)
        {
            HumanMotion2.AlignToVector(arm, arm.transform.up, Vector3.down + vector2 / 2f, 3f * tonus);
            HumanMotion2.AlignToVector(forearm, forearm.transform.up, vector / 2f - vector2, 3f * tonus);
            return;
        }
        HumanMotion2.AlignToVector(arm, arm.transform.up, -vector + vector2 / 2f, 3f * tonus);
        HumanMotion2.AlignToVector(forearm, forearm.transform.up, vector + Vector3.down, 3f * tonus);
    }

    // Token: 0x0600303D RID: 12349 RVA: 0x001473E8 File Offset: 0x001455E8
    private void LiftBody(Human human, HumanSegment hand, Rigidbody body)
    {
        if (human.GetComponent<GroundManager>().IsStanding(body.gameObject))
        {
            return;
        }
        if (body.tag == "NoLift")
        {
            return;
        }
        float num = 0.5f + 0.5f * Mathf.InverseLerp(0f, 100f, body.mass);
        Vector3 vector = (human.targetLiftDirection.ZeroY() * this.maxPushForce).SetY(Mathf.Max(0f, human.targetLiftDirection.y) * this.maxLiftForce);
        float magnitude = (hand.transform.position - body.worldCenterOfMass).magnitude;
        float num2 = num;
        float num3 = 1f;
        float num4 = 1f;
        Carryable component = body.GetComponent<Carryable>();
        if (component != null)
        {
            num2 *= component.liftForceMultiplier;
            num3 = component.forceHalfDistance;
            num4 = component.damping;
            if (num3 <= 0f)
            {
                throw new InvalidOperationException("halfdistance cant be 0 or less!");
            }
        }
        float num5 = num3 / (num3 + magnitude);
        vector *= num2;
        vector *= num5;
        body.SafeAddForce(vector, ForceMode.Force);
        hand.rigidbody.SafeAddForce(-vector * 0.5f, ForceMode.Force);
        human.ragdoll.partChest.rigidbody.SafeAddForce(-vector * 0.5f, ForceMode.Force);
        body.SafeAddTorque(-body.angularVelocity * this.liftDamp * num4, ForceMode.Acceleration);
        body.SafeAddTorque(-body.angularVelocity.normalized * body.angularVelocity.sqrMagnitude * this.liftDampSqr * num4, ForceMode.Acceleration);
        if (component != null && component.aiming != CarryableAiming.None)
        {
            Vector3 vector2 = human.targetLiftDirection;
            if (component.limitAlignToHorizontal)
            {
                vector2.y = 0f;
                vector2.Normalize();
            }
            Vector3 vector3 = ((component.aiming != CarryableAiming.ForwardAxis) ? (body.worldCenterOfMass - hand.transform.position).normalized : body.transform.forward);
            float aimSpring = component.aimSpring;
            float num6 = ((component.aimTorque >= float.PositiveInfinity) ? aimSpring : component.aimTorque);
            if (!component.alwaysForward)
            {
                float num7 = Vector3.Dot(vector3, vector2);
                if (num7 < 0f)
                {
                    vector2 = -vector2;
                    num7 = -num7;
                }
                num6 *= Mathf.Pow(num7, component.aimAnglePower);
            }
            else
            {
                float num8 = Vector3.Dot(vector3, vector2);
                num8 = 0.5f + num8 / 2f;
                num6 *= Mathf.Pow(num8, component.aimAnglePower);
            }
            if (component.aimDistPower != 0f)
            {
                num6 *= Mathf.Pow((body.worldCenterOfMass - hand.transform.position).magnitude, component.aimDistPower);
            }
            HumanMotion2.AlignToVector(body, vector3, vector2, aimSpring, num6);
        }
    }

    // Token: 0x0600303E RID: 12350 RVA: 0x00147700 File Offset: 0x00145900
    private void PlaceHand(HumanSegment arm, HumanSegment hand, Vector3 worldPos, bool active, bool grabbed, Rigidbody grabbedBody, Human human)
    {
        if (!active)
        {
            return;
        }
        Rigidbody rigidbody = hand.rigidbody;
        Vector3 worldCenterOfMass = rigidbody.worldCenterOfMass;
        Vector3 vector = worldPos - worldCenterOfMass;
        new Vector3(0f, vector.y, 0f);
        Vector3 vector2 = rigidbody.velocity - human.ragdoll.partBall.rigidbody.velocity;
        float num = this.armMass;
        float num2 = this.maxForce;
        if (grabbed)
        {
            if (grabbedBody != null)
            {
                num += Mathf.Clamp(grabbedBody.mass / 2f, 0f, this.bodyMass);
                num2 = Mathf.Lerp(this.grabMaxForce, this.climbMaxForce, (human.controls.targetPitchAngle - 50f) / 30f);
            }
            else
            {
                num += this.bodyMass;
                num2 = Mathf.Lerp(this.grabMaxForce, this.climbMaxForce, (human.controls.targetPitchAngle - 50f) / 30f);
            }
        }
        float num3 = num2 / num;
        Vector3 vector3 = ConstantAccelerationControl.Solve(vector, vector2, num3, 0.1f);
        int num4 = 600;
        Vector3 vector4 = vector3 * num + Vector3.up * this.gravityForce;
        if (human.grabbedByHuman != null && human.grabbedByHuman.state == HumanState.Climb)
        {
            vector4 *= 1.7f;
            num4 *= 2;
        }
        if (!grabbed)
        {
            rigidbody.SafeAddForce(vector4, ForceMode.Force);
            human.ragdoll.partHips.rigidbody.SafeAddForce(-vector4, ForceMode.Force);
            return;
        }
        Vector3 normalized = human.targetDirection.ZeroY().normalized;
        Vector3 vector5 = Mathf.Min(0f, Vector3.Dot(normalized, vector4)) * normalized;
        Vector3 vector6 = vector4 - vector5;
        Vector3 vector7 = vector4.SetX(0f).SetZ(0f);
        Vector3 vector8 = -vector4 * 0.25f;
        Vector3 vector9 = -vector4 * 0.75f;
        Vector3 vector10 = -vector4 * 0.1f - vector7 * 0.5f - vector6 * 0.25f;
        Vector3 vector11 = -vector7 * 0.2f - vector6 * 0.4f;
        if (grabbedBody != null)
        {
            Carryable component = grabbedBody.GetComponent<Carryable>();
            if (component != null)
            {
                vector8 *= component.handForceMultiplier;
                vector9 *= component.handForceMultiplier;
            }
        }
        float num5 = ((human.state != HumanState.Climb) ? 1f : Mathf.Clamp01((human.controls.targetPitchAngle - 10f) / 60f));
        Vector3 vector12 = Vector3.Lerp(vector10, vector8, vector.y + 0.5f) * num5;
        Vector3 vector13 = Vector3.Lerp(vector11, vector9, vector.y + 0.5f) * num5;
        float num6 = Mathf.Abs(vector12.y + vector13.y);
        if (num6 > (float)num4)
        {
            vector12 *= (float)num4 / num6;
            vector13 *= (float)num4 / num6;
        }
        human.ragdoll.partChest.rigidbody.SafeAddForce(vector12, ForceMode.Force);
        human.ragdoll.partBall.rigidbody.SafeAddForce(vector13, ForceMode.Force);
        rigidbody.SafeAddForce(-vector12 - vector13, ForceMode.Force);
    }

    // Token: 0x0600303F RID: 12351 RVA: 0x00147A9C File Offset: 0x00145C9C
    private Vector3 FindTarget(AutomaticHand.ScanMem mem, Vector3 worldPos, out Collider targetCollider, HumanMotion2 motion)
    {
        targetCollider = null;
        Vector3 vector = worldPos - mem.shoulder;
        Ray ray = new Ray(mem.shoulder, vector.normalized);
        int num = Physics.OverlapCapsuleNonAlloc(ray.origin, worldPos, 0.5f, this.colliders, motion.grabLayers, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < num; i++)
        {
            Collider collider = this.colliders[i];
            TargetHelper componentInChildren = collider.GetComponentInChildren<TargetHelper>();
            if (componentInChildren != null)
            {
                Vector3 vector2 = componentInChildren.transform.position - worldPos;
                float magnitude = (Math3d.ProjectPointOnLineSegment(ray.origin, worldPos, componentInChildren.transform.position) - componentInChildren.transform.position).magnitude;
                if (magnitude < 0.3f && (componentInChildren.transform.position - mem.hand).magnitude < 0.3f)
                {
                    worldPos = componentInChildren.transform.position;
                    targetCollider = collider;
                }
                else
                {
                    worldPos += vector2 * Mathf.InverseLerp(0.5f, 0.3f, magnitude);
                }
                return worldPos;
            }
        }
        Vector3 vector3 = mem.hand + Vector3.ClampMagnitude(worldPos - mem.hand, 0.3f);
        targetCollider = null;
        Vector3 vector4 = vector3 - mem.pos;
        Ray ray2 = new Ray(mem.pos, vector4.normalized);
        Debug.DrawRay(ray2.origin, ray2.direction * vector4.magnitude, Color.yellow, 0.2f);
        float num2 = float.PositiveInfinity;
        Vector3 vector5 = vector3;
        for (float num3 = 0.05f; num3 <= 0.5f; num3 += 0.05f)
        {
            RaycastHit raycastHit;
            if (Physics.SphereCast(ray2, num3, out raycastHit, vector4.magnitude, motion.grabLayers, QueryTriggerInteraction.Ignore))
            {
                float num4 = (vector3 - raycastHit.point).magnitude;
                num4 += num3 / 10f;
                if (raycastHit.collider.tag == "Target")
                {
                    num4 /= 100f;
                }
                else
                {
                    if (num3 > 0.2f)
                    {
                        goto IL_288;
                    }
                    Vector3 normalized = (worldPos - mem.shoulder).normalized;
                    Vector3 normalized2 = (raycastHit.point - mem.shoulder).normalized;
                    if (Vector3.Dot(normalized, normalized2) < 0.7f)
                    {
                        goto IL_288;
                    }
                }
                if (num4 < num2)
                {
                    num2 = num4;
                    vector5 = raycastHit.point;
                    targetCollider = raycastHit.collider;
                }
            }
        IL_288:;
        }
        if (targetCollider != null)
        {
            Vector3 vector6 = vector5 - vector3;
            float magnitude2 = (Math3d.ProjectPointOnLineSegment(ray2.origin, vector3, vector5) - vector5).magnitude;
            if (targetCollider.tag == "Target")
            {
                if (magnitude2 < 0.2f && (mem.hand - vector5).magnitude < 0.5f)
                {
                    worldPos = vector5;
                }
                else
                {
                    worldPos = vector3 + vector6 * Mathf.InverseLerp(0.5f, 0.2f, magnitude2);
                    targetCollider = null;
                }
            }
            else if (magnitude2 < 0.1f && vector6.magnitude < 0.1f)
            {
                worldPos = vector5;
            }
            else
            {
                worldPos = vector3 + vector6 * Mathf.InverseLerp(0.2f, 0.1f, magnitude2);
                targetCollider = null;
            }
        }
        mem.pos = vector3;
        return worldPos;
    }

    // Token: 0x06003040 RID: 12352 RVA: 0x00147E2C File Offset: 0x0014602C
    public AutomaticHand()
    {
        this.autoHuman = new List<Human>();
        this.maxPushForce = 200f;
        this.maxLiftForce = 500f;
        this.liftDampSqr = 0.1f;
        this.liftDamp = 0.1f;
        this.armMass = 20f;
        this.bodyMass = 50f;
        this.maxForce = 300f;
        this.grabMaxForce = 450f;
        this.climbMaxForce = 800f;
        this.gravityForce = 100f;
    }

    // Token: 0x06003041 RID: 12353 RVA: 0x00147F50 File Offset: 0x00146150
    public static GroundManager GetGroundManager(Human human)
    {
        human.function_Attribute.manageri++;
        if (human.function_Attribute.manageri > 10)
        {
            human.function_Attribute.manageri = 0;
            human.function_Attribute.groundManager = human.GetComponent<GroundManager>();
        }
        if (human.function_Attribute.groundManager == null)
        {
            human.function_Attribute.groundManager = human.GetComponent<GroundManager>();
        }
        return human.function_Attribute.groundManager;
    }

    // Token: 0x06003043 RID: 12355 RVA: 0x00020FB6 File Offset: 0x0001F1B6
    public void Awake()
    {
        if (AutomaticHand.ins != null)
        {
            global::UnityEngine.Object.Destroy(this);
            return;
        }
        AutomaticHand.ins = this;
        this.autoHuman = new List<Human>();
    }

    // Token: 0x06003044 RID: 12356 RVA: 0x00020FDD File Offset: 0x0001F1DD
    public static bool Contains(Human human)
    {
        return !(AutomaticHand.ins == null) && AutomaticHand.ins.autoHuman.Contains(human);
    }

    // Token: 0x06003045 RID: 12357 RVA: 0x00147FCC File Offset: 0x001461CC
    public static void Add(Human human)
    {
        if (AutomaticHand.ins == null)
        {
            AutomaticHand.AutomaticHandMod();
        }
        if (!AutomaticHand.ins.autoHuman.Contains(human))
        {
            human.function_Attribute.oldyawangle = human.controls.cameraYawAngle;
            human.function_Attribute.oldautotime = Time.time;
            human.function_Attribute.oldlargeyaw = false;
            AutomaticHand.ins.autoHuman.Add(human);
            Debug.Log("玩家" + human.player.host.name + "已进入自动伸手");
        }
    }

    // Token: 0x06003046 RID: 12358 RVA: 0x00148064 File Offset: 0x00146264
    public static void Remove(Human human)
    {
        if (AutomaticHand.ins == null)
        {
            AutomaticHand.AutomaticHandMod();
        }
        if (AutomaticHand.ins.autoHuman.Contains(human))
        {
            AutomaticHand.ins.autoHuman.Remove(human);
            Debug.Log("玩家" + human.player.host.name + "已退出自动伸手");
            if (AutomaticHand.ins.autoHuman.Count == 0)
            {
                AutomaticHand.AutomaticHandMod();
            }
        }
    }

    // Token: 0x04002F83 RID: 12163
    public float maxLiftForce = 500f;

    // Token: 0x04002F84 RID: 12164
    public float maxPushForce = 200f;

    // Token: 0x04002F85 RID: 12165
    public float liftDampSqr = 0.1f;

    // Token: 0x04002F86 RID: 12166
    public float liftDamp = 0.1f;

    // Token: 0x04002F87 RID: 12167
    public float armMass = 20f;

    // Token: 0x04002F88 RID: 12168
    public float bodyMass = 50f;

    // Token: 0x04002F89 RID: 12169
    public float maxForce = 300f;

    // Token: 0x04002F8A RID: 12170
    public float grabMaxForce = 450f;

    // Token: 0x04002F8B RID: 12171
    public float climbMaxForce = 800f;

    // Token: 0x04002F8C RID: 12172
    public float gravityForce = 100f;

    // Token: 0x04002F8D RID: 12173
    public static AutomaticHand ins;

    // Token: 0x04002F8E RID: 12174
    private Collider[] colliders = new Collider[20];

    // Token: 0x04002F8F RID: 12175
    public List<Human> autoHuman;

    // Token: 0x04002F90 RID: 12176
    public HandMuscles.TargetingMode targetingMode;

    // Token: 0x04002F91 RID: 12177
    public HandMuscles.TargetingMode grabTargetingMode = HandMuscles.TargetingMode.Ball;

    // Token: 0x04002F92 RID: 12178
    private AutomaticHand.ScanMem leftMem = new AutomaticHand.ScanMem();

    // Token: 0x04002F93 RID: 12179
    private AutomaticHand.ScanMem rightMem = new AutomaticHand.ScanMem();

    // Token: 0x04002F94 RID: 12180
    private static bool AutoHandCamera;

    // Token: 0x0200087F RID: 2175
    private class ScanMem
    {
        // Token: 0x04002F95 RID: 12181
        public Vector3 pos;

        // Token: 0x04002F96 RID: 12182
        public Vector3 shoulder;

        // Token: 0x04002F97 RID: 12183
        public Vector3 hand;

        // Token: 0x04002F98 RID: 12184
        public float grabTime;

        // Token: 0x04002F99 RID: 12185
        public float grabAngle;
    }
    #endif
}
