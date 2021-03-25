using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    [SerializeField, Min(1)] private int m_initialLength;
    public int m_length;
    public float m_speed;
    private LineRenderer m_lineRenderer;
    public List<Vector3> m_segmentPos;
    public List<Vector3> m_segmentVel;

    [SerializeField] private Transform m_targetTransform;
    [SerializeField] private float m_targetDist;
    [SerializeField] private float m_smoothSpeed;
    [SerializeField] private float m_trailSpeed;

    [SerializeField] private GameObject m_segementPrefab;
    [SerializeField] private List<GameObject> m_segments;
    [SerializeField] private GameObject m_trainingEnviroment;

    private void Start()
    {
        m_lineRenderer = GetComponent<LineRenderer>();

        ResetTail();
    }

    public void OnEnable()
    {
        foreach(GameObject segment in m_segments)
        {
            segment.SetActive(true);
        }
    }

    public void OnDisable()
    {
        foreach (GameObject segment in m_segments)
        {
            segment.SetActive(false);
        }
    }

    public void SetTailColour(Color colour)
    {
        //m_lineRenderer.colorGradient = new Gradient();

        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = colour;
        colorKey[0].time = 0f;
        colorKey[1].color = colour;
        colorKey[1].time = 1f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1f;
        alphaKey[0].time = 0f;
        alphaKey[1].alpha = 1f;
        alphaKey[1].time = 1f;

        m_lineRenderer.colorGradient.SetKeys(colorKey, alphaKey);
    }

    public void AddSegement()
    {
        m_segmentPos.Add(m_segmentPos[m_segmentPos.Count - 1]);
        m_segmentVel.Add(Vector3.zero);
        GameObject segment = Instantiate(m_segementPrefab, m_segmentPos[m_segmentPos.Count - 1], Quaternion.identity);
        segment.transform.SetParent(m_trainingEnviroment.transform);
        m_segments.Add(segment);
        m_lineRenderer.positionCount++;
        m_length++;
    }

    public void ResetTail()
    {
        if(m_segments != null && m_segments.Count > 0)
            foreach(GameObject segement in m_segments)
                Destroy(segement);

        m_length = m_initialLength;

        m_lineRenderer.positionCount = m_initialLength;
        m_segmentPos = new List<Vector3>();
        m_segmentVel = new List<Vector3>();
        m_segments = new List<GameObject>();

        float offset = transform.position.y - (m_targetDist);
        for (int i = 0; i < m_initialLength; i++)
        {
            m_segmentPos.Add(new Vector3(transform.position.x, offset, transform.position.z));
            offset -= m_targetDist;
            m_segmentVel.Add(Vector3.zero);
            if (i > 0)
            {
                GameObject segment = Instantiate(m_segementPrefab, m_segmentPos[i], Quaternion.identity);
                segment.transform.SetParent(m_trainingEnviroment.transform);
                m_segments.Add(segment);
            }
        }
    }

    private void Update()
    {
        m_segmentPos[0] = m_targetTransform.position;

        for(int i = 1; i < m_segmentPos.Count; i++)
        {
            Vector3 vectorToTarget = m_segmentPos[i - 1] - m_segmentPos[i];
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            angle -= 90;
            m_segments[i-1].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            Vector3 i_velocity = m_segmentVel[i];

            Vector3 targetPos;
            if (i == 1)
                targetPos = m_segmentPos[i - 1] - m_segments[i - 1].transform.up * m_targetDist;
            else
                targetPos = m_segmentPos[i - 1] - m_segments[i - 2].transform.up * m_targetDist;


            m_segmentPos[i] = Vector3.SmoothDamp(m_segmentPos[i], targetPos, ref i_velocity, m_smoothSpeed);
            m_segmentVel[i] = i_velocity;
            m_segments[i-1].transform.position = m_segmentPos[i];

            /*float dist = Vector3.Distance(m_segmentPos[i - 1], m_segmentPos[i]);

            float T = Time.deltaTime * dist / m_targetDist * m_speed;

            if(T > 0.5f)
                T = 0.5f;

            m_segmentPos[i] = Vector3.Slerp(m_segmentPos[i], m_segmentPos[i - 1], T);

            m_segments[i - 1].transform.position = m_segmentPos[i];*/
        }
        m_lineRenderer.SetPositions(m_segmentPos.ToArray());
    }

}
