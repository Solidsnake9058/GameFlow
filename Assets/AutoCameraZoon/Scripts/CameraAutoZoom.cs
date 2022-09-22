using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if CINEMACHINE
using Cinemachine;
#endif

public class CameraAutoZoom : MonoBehaviour
{
    [SerializeField]
    private Camera m_Camera = default;

#if CINEMACHINE
    [SerializeField]
    private CinemachineVirtualCamera[] m_CinemachineVirtual = default;
#endif

    [SerializeField]
    private bool m_IsSettingWhenStart = true;

    public float m_BaseCameraSize;
    public float m_BaseCameraFOV;

    private const float m_BaseScreenRatio = 16f / 9f;

    private void Start()
    {
        if (m_IsSettingWhenStart)
        {
            SetCameraSize();
        }
    }

    public void SetBaseCameraSize(float baseCameraSize)
    {
        m_BaseCameraSize = baseCameraSize;
        SetCameraSize();
    }

    private void SetCameraSize()
    {
        float screenRatio = (float)Screen.height / (float)Screen.width;
        float scale = Mathf.Max(screenRatio / m_BaseScreenRatio, 1);

        if (m_Camera.orthographic)
        {
            m_Camera.orthographicSize = m_BaseCameraSize * scale;
        }
        else
        {
            //Set distance = 1
            float dis1Height = Mathf.Tan(Mathf.Deg2Rad * m_BaseCameraFOV);
            float dis1TargetHeight = dis1Height * scale;

            m_Camera.fieldOfView = Mathf.Atan2(dis1TargetHeight, 1) * Mathf.Rad2Deg;

#if CINEMACHINE
            for (int i = 0; i < m_CinemachineVirtual.Length; i++)
            {
                m_CinemachineVirtual[i].m_Lens.FieldOfView = Mathf.Atan2(dis1TargetHeight, 1) * Mathf.Rad2Deg;

            }
#endif

            //Debug.Log($"dis1Height:{dis1Height} dis1TargetHeight:{dis1TargetHeight} screenRatio:{screenRatio} dis1DegB:{Mathf.Atan2(dis1TargetHeight / 2, 1) * Mathf.Rad2Deg}");
        }
    }
}