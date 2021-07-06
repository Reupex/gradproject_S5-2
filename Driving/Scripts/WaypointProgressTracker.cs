using System;
using UnityEngine;

#pragma warning disable 649
namespace UnityStandardAssets.Utility
{
    public class WaypointProgressTracker : MonoBehaviour
    {
        
        // �� ��ũ��Ʈ�� �������� ǥ�õ� ��θ� ������ϴ� ��� ��ü�� ����� �� �ֽ��ϴ�.

        // �� ��ũ��Ʈ�� ��θ� ���� ���� �� ���� �����ϰ� ���� ��Ȳ�� ���� �����մϴ�.

        [SerializeField] public WaypointCircuit circuit; // ����� �� ������ ��� ��ο� ���� ����

        [SerializeField] private float lookAheadForTargetOffset = 5;
        // �츮�� ��ǥ�� �� ��θ� ���� ���� ������

        [SerializeField] private float lookAheadForTargetFactor = .1f;
        //���� �ӵ��� ���� ���� �� ��θ� ���� ���� �Ÿ��� �߰��ϴ� �¼�

        [SerializeField] private float lookAheadForSpeedOffset = 10;
        //�ӵ� ���������� ��� �� ���� ������ (���� ����Ʈ ��� ��ȯ�� ȸ������ �����)

        [SerializeField] private float lookAheadForSpeedFactor = .2f;
        //�ӵ� ������ ���� ��θ� ���� ���� �Ÿ��� �߰��ϴ� �¼�

        [SerializeField] private ProgressStyle progressStyle = ProgressStyle.SmoothAlongRoute;
        //��θ� ���� �ε巴�� ��ġ�� ������Ʈ���� (� ��ο� ����) �Ǵ� �� ���� ����Ʈ�� �������� ����

        [SerializeField] private float pointToPointThreshold = 4;
        // ��ǥ�� ���� ���� ����Ʈ�� ��ȯ�ϱ� ���� �����ؾ��ϴ� ���� ����Ʈ���� ������ : PointToPoint ��忡���� ���˴ϴ�.

        public enum ProgressStyle
        {
            SmoothAlongRoute,
            PointToPoint,
        }

        // �̵��� �����Ǿ� �ٸ� ��ü���� ���� �� �ֽ��ϴ�. �� AI�� ���� ���ؾ��ϴ��� �� �� �ֽ��ϴ�!
        public WaypointCircuit.RoutePoint targetPoint { get; private set; }
        public WaypointCircuit.RoutePoint speedPoint { get; private set; }
        public WaypointCircuit.RoutePoint progressPoint { get; private set; }

        public Transform target;

        private float progressDistance; // �ε巯�� ��忡�� ���Ǵ� ��� �ֺ��� ���� ��Ȳ.
        private int progressNum; //���� �� ��忡�� ���Ǵ� ���� ���� ����Ʈ ��ȣ.
        private Vector3 lastPosition; //���� �ӵ��� ����ϴ� �� ���˴ϴ� (������ �ٵ� ���� ��Ұ� ���� �� �����Ƿ�)
        private float speed; //�� ��ü�� ���� �ӵ� (������ ������ ���� ��Ÿ���� ��� ��)

        // setup script properties
        private void Start()
        {
            // �츮�� ���� �� ������ ���� �ӵ� ��ȭ�� ����� ������ ��Ÿ���� ��ȯ�� ����մϴ�. 
            // �̸� ���� �� ���� ��Ҵ� �߰� ���Ӽ� ���� �� ������ AI�� ������ �� �ֽ��ϴ�.

            // �������� ��ȯ�� ����� �� ���� ��� * �� * AI�� �Ҵ� �� �� �ֽ��ϴ�.
            // �׷����� ������Ʈ�� �̸� ������Ʈ�ϰ� AI�� ���� �� �ֽ��ϴ�.
            if (target == null)
            {
                target = new GameObject(name + " Waypoint Target").transform;
            }

            Reset();
        }


        // ��ü�� �ո����� ������ �缳��
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
                // ���� ��ǥ�� ��ƾ��ϴ� ��ġ�� �����Ѵ�.
                // ���� ���� ��ġ�ʹ� �ٸ��� ��θ� ���� ������ �ռ� �ִ�.
                // Lerp�� ����ϸ� �ð��� ������ ���� �ӵ��� �ε巴�� �Ѵ�.
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
                // ����Ʈ �� ����Ʈ ���. ����� ������ �������� �ø��ʽÿ�.

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
