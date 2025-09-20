using System.Reflection;
using UnityEngine;
using YxModDll.Patches;

namespace YxMod测试
{
    public class Patcher_LegMuscles : MonoBehaviour
    {


        private static FieldInfo _human;
        private static FieldInfo _groundManager;
        private static FieldInfo _grabManager;

        private static FieldInfo _ragdoll;
        //private float ballRadius;
        private static FieldInfo _ballRadius;


        private void Awake()
        {

            _human = typeof(LegMuscles).GetField("human", BindingFlags.NonPublic | BindingFlags.Instance);

            _groundManager = typeof(Human).GetField("groundManager", BindingFlags.NonPublic | BindingFlags.Instance);
            _grabManager = typeof(Human).GetField("grabManager", BindingFlags.NonPublic | BindingFlags.Instance);

            _ragdoll = typeof(LegMuscles).GetField("ragdoll", BindingFlags.NonPublic | BindingFlags.Instance);
            _ballRadius = typeof(LegMuscles).GetField("ballRadius", BindingFlags.NonPublic | BindingFlags.Instance);



            Patcher2.MethodPatch(typeof(LegMuscles), "RunAnimation", new[] { typeof(Vector3), typeof(float) }, typeof(Patcher_LegMuscles), "RunAnimation", new[] { typeof(LegMuscles), typeof(Vector3), typeof(float) });

        }

        public static void RunAnimation(LegMuscles instance, Vector3 torsoFeedback, float tonus)
        {
            var ragdoll = (Ragdoll)_ragdoll.GetValue(instance);
            var human = (Human)_human.GetValue(instance);
            instance.legPhase = Time.realtimeSinceStartup * human.GetExt().bufasudu;
            torsoFeedback += AnimateLeg(instance, ragdoll.partLeftThigh, ragdoll.partLeftLeg, ragdoll.partLeftFoot, instance.legPhase, torsoFeedback, tonus);
            torsoFeedback += AnimateLeg(instance, ragdoll.partRightThigh, ragdoll.partRightLeg, ragdoll.partRightFoot, instance.legPhase + 0.5f, torsoFeedback, tonus);
            ragdoll.partBall.rigidbody.SafeAddForce(torsoFeedback);
            RotateBall(instance);
            AddWalkForce(instance);
        }

        private static Vector3 AnimateLeg(LegMuscles instance, HumanSegment thigh, HumanSegment leg, HumanSegment foot, float phase, Vector3 torsoFeedback, float tonus)
        {
            var human = (Human)_human.GetValue(instance);

            tonus *= 20f;
            phase -= Mathf.Floor(phase);
            if (phase < 0.2f)
            {
                HumanMotion2.AlignToVector(thigh, thigh.transform.up, human.controls.walkDirection + Vector3.down, 3f * tonus);
                HumanMotion2.AlignToVector(leg, thigh.transform.up, -human.controls.walkDirection - Vector3.up, tonus);
                Vector3 vector = Vector3.up * 20f;
                foot.rigidbody.SafeAddForce(vector);
                return -vector;
            }
            if (phase < 0.5f)
            {
                HumanMotion2.AlignToVector(thigh, thigh.transform.up, human.controls.walkDirection, 2f * tonus);
                HumanMotion2.AlignToVector(leg, thigh.transform.up, human.controls.walkDirection, 3f * tonus);
            }
            else
            {
                if (phase < 0.7f)
                {
                    Vector3 vector2 = torsoFeedback * 0.2f;
                    foot.rigidbody.SafeAddForce(vector2);
                    HumanMotion2.AlignToVector(thigh, thigh.transform.up, human.controls.walkDirection + Vector3.down, tonus);
                    HumanMotion2.AlignToVector(leg, thigh.transform.up, Vector3.down, tonus);
                    return -vector2;
                }
                if (phase < 0.9f)
                {
                    Vector3 vector3 = torsoFeedback * 0.2f;
                    foot.rigidbody.SafeAddForce(vector3);
                    HumanMotion2.AlignToVector(thigh, thigh.transform.up, -human.controls.walkDirection + Vector3.down, tonus);
                    HumanMotion2.AlignToVector(leg, thigh.transform.up, -human.controls.walkDirection + Vector3.down, tonus);
                    return -vector3;
                }
                HumanMotion2.AlignToVector(thigh, thigh.transform.up, -human.controls.walkDirection + Vector3.down, tonus);
                HumanMotion2.AlignToVector(leg, thigh.transform.up, -human.controls.walkDirection, tonus);
            }
            return Vector3.zero;
        }
        private static void RotateBall(LegMuscles instance)
        {
            var human = (Human)_human.GetValue(instance);
            var ragdoll = (Ragdoll)_ragdoll.GetValue(instance);
            var ballRadius = (float)_ballRadius.GetValue(instance);

            float num = ((human.state != HumanState.Walk) ? 1.2f : 2.5f);
            Vector3 vector = new Vector3(human.controls.walkDirection.z, 0f, 0f - human.controls.walkDirection.x);
            ragdoll.partBall.rigidbody.angularVelocity = num / ballRadius * vector;
            ragdoll.partBall.rigidbody.maxAngularVelocity = ragdoll.partBall.rigidbody.angularVelocity.magnitude;
        }

        private static void AddWalkForce(LegMuscles instance)
        {
            var human = (Human)_human.GetValue(instance);
            var groundManager = (GroundManager)_groundManager.GetValue(human);
            var grabManager = (GrabManager)_grabManager.GetValue(human);

            var ragdoll = (Ragdoll)_ragdoll.GetValue(instance);
            float num = 300f;
            Vector3 vector = human.controls.walkDirection * num;
            ragdoll.partBall.rigidbody.SafeAddForce(vector);
            if (human.onGround)
            {
                groundManager.DistributeForce(-vector, ragdoll.partBall.rigidbody.position);
            }
            else if (human.hasGrabbed)
            {
                grabManager.DistributeForce(-vector * 0.5f);
            }
        }
    }
}
