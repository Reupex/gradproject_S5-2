using System;
using UnityEngine;

#pragma warning disable 649
namespace UnityStandardAssets.Utility
{
    public class WaypointProgressTracker : MonoBehaviour
    {
        
        // 이 스크립트는 경유지로 표시된 경로를 따라야하는 모든 객체에 사용할 수 있습니다.

        // 이 스크립트는 경로를 따라 앞을 볼 양을 관리하고 진행 상황과 랩을 추적합니다.

        [SerializeField] public WaypointCircuit circuit; // 따라야 할 경유지 기반 경로에 대한 참조

        [SerializeField] private float lookAheadForTargetOffset = 5;
        // 우리가 목표로 할 경로를 따라 전방 오프셋

        [SerializeField] private float lookAheadForTargetFactor = .1f;
        //현재 속도에 따라 조준 할 경로를 따라 전방 거리를 추가하는 승수

        [SerializeField] private float lookAheadForSpeedOffset = 10;
        //속도 조정을위한 경로 만 전방 오프셋 (웨이 포인트 대상 변환의 회전으로 적용됨)

        [SerializeField] private float lookAheadForSpeedFactor = .2f;
        //속도 조정을 위해 경로를 따라 전방 거리를 추가하는 승수

        [SerializeField] private ProgressStyle progressStyle = ProgressStyle.SmoothAlongRoute;
        //경로를 따라 부드럽게 위치를 업데이트할지 (곡선 경로에 적합) 또는 각 웨이 포인트에 도달했을 때만

        [SerializeField] private float pointToPointThreshold = 4;
        // 목표를 다음 웨이 포인트로 전환하기 위해 도달해야하는 웨이 포인트와의 근접성 : PointToPoint 모드에서만 사용됩니다.

        public enum ProgressStyle
        {
            SmoothAlongRoute,
            PointToPoint,
        }

        // 이들은 공개되어 다른 객체에서 읽을 수 있습니다. 즉 AI가 어디로 향해야하는지 알 수 있습니다!
        public WaypointCircuit.RoutePoint targetPoint { get; private set; }
        public WaypointCircuit.RoutePoint speedPoint { get; private set; }
        public WaypointCircuit.RoutePoint progressPoint { get; private set; }

        public Transform target;

        private float progressDistance; // 부드러운 모드에서 사용되는 경로 주변의 진행 상황.
        private int progressNum; //지점 간 모드에서 사용되는 현재 웨이 포인트 번호.
        private Vector3 lastPosition; //현재 속도를 계산하는 데 사용됩니다 (리지드 바디 구성 요소가 없을 수 있으므로)
        private float speed; //이 개체의 현재 속도 (마지막 프레임 이후 델타에서 계산 됨)

        // setup script properties
        private void Start()
        {
            // 우리는 조준 할 지점과 향후 속도 변화를 고려할 지점을 나타내는 변환을 사용합니다. 
            // 이를 통해 이 구성 요소는 추가 종속성 없이 이 정보를 AI에 전달할 수 있습니다.

            // 수동으로 변환을 만들고 이 구성 요소 * 및 * AI에 할당 할 수 있습니다.
            // 그러면이 컴포넌트가 이를 업데이트하고 AI가 읽을 수 있습니다.
            if (target == null)
            {
                target = new GameObject(name + " Waypoint Target").transform;
            }

            Reset();
        }


        // 개체를 합리적인 값으로 재설정
        public void Reset()
        {
            progressDistance = 0;
            progressNum = 0;
            if (progressStyle == ProgressStyle.PointToPoint)
            {
                target.position = circuit.Waypoints[progressNum].position;
                target.rotation = circuit.Waypoints[progressNum].rotation;
            }
        }


        private void Update()
        {
            if (progressStyle == ProgressStyle.SmoothAlongRoute)
            {
                // 현재 목표로 삼아야하는 위치를 결정한다.
                // 현재 진행 위치와는 다르며 경로를 따라 일정량 앞서 있다.
                // Lerp를 사용하며 시간이 지남에 따라 속도를 부드럽게 한다.
                if (Time.deltaTime > 0)
                {
                    speed = Mathf.Lerp(speed, (lastPosition - transform.position).magnitude/Time.deltaTime,
                                       Time.deltaTime);
                }
                target.position =
                    circuit.GetRoutePoint(progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor*speed)
                           .position;
                target.rotation =
                    Quaternion.LookRotation(
                        circuit.GetRoutePoint(progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor*speed)
                               .direction);


                // get our current progress along the route
                progressPoint = circuit.GetRoutePoint(progressDistance);
                Vector3 progressDelta = progressPoint.position - transform.position;
                if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
                {
                    progressDistance += progressDelta.magnitude*0.5f;
                }

                lastPosition = transform.position;
            }
            else
            {
                // 포인트 투 포인트 모드. 충분히 가까우면 경유지를 늘리십시오.

                Vector3 targetDelta = target.position - transform.position;
                if (targetDelta.magnitude < pointToPointThreshold)
                {
                    progressNum = (progressNum + 1)%circuit.Waypoints.Length;
                }


                target.position = circuit.Waypoints[progressNum].position;
                target.rotation = circuit.Waypoints[progressNum].rotation;

                // get our current progress along the route
                progressPoint = circuit.GetRoutePoint(progressDistance);
                Vector3 progressDelta = progressPoint.position - transform.position;
                if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
                {
                    progressDistance += progressDelta.magnitude;
                }
                lastPosition = transform.position;
            }
        }


        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, target.position);
                Gizmos.DrawWireSphere(circuit.GetRoutePosition(progressDistance), 1);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(target.position, target.position + target.forward);
            }
        }
    }
}
