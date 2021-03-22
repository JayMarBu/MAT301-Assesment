using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    [SerializeField] private int m_length;
    private LineRenderer m_lineRenderer;
    public List<Vector3> m_segmentPos;
    public List<Vector3> m_segmentVel;

    [SerializeField] private Transform m_targetTransform;
    [SerializeField] private float m_targetDist;
    [SerializeField] private float m_smoothSpeed;
    [SerializeField] private float m_trailSpeed;

    private void Start()
    {
        m_lineRenderer = GetComponent<LineRenderer>();

        m_lineRenderer.positionCount = m_length;
        m_segmentPos = new List<Vector3>();
        m_segmentVel = new List<Vector3>();
        for (int i = 0; i < m_length; i++)
        {
            m_segmentPos.Add(new Vector3());
            m_segmentVel.Add(new Vector3());
        }

        
    }

    private void Update()
    {
        m_segmentPos[0] = m_targetTransform.position;

        for(int i = 1; i < m_segmentPos.Count; i++)
        {
            Vector3 i_velocity = m_segmentVel[i];
            m_segmentPos[i] = Vector3.SmoothDamp(m_segmentPos[i], m_segmentPos[i - 1] + m_targetTransform.up * m_targetDist, ref i_velocity, m_smoothSpeed + i/m_trailSpeed);
            m_segmentVel[i] = i_velocity;
        }
        m_lineRenderer.SetPositions(m_segmentPos.ToArray());
    }

}
