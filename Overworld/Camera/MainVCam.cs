using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainVCam : MonoBehaviour
{
    private static MainVCam instance;

    public static MainVCam Instance
    {
        get
        {
            if (instance == null) {
                instance = FindObjectOfType<MainVCam>();
            }
            return instance;
        }
    }

    // Declare this object as a Singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if(Party.Instance != null) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Disable() {
        // GetComponent<Cinemachine.CinemachineVirtualCamera>().enabled = false;
    }

    public void ChangeScenes() {
        /*Debug.Log("HELLO!");
        Vector2 pos = Party.Instance.current_party[0].transform.position;
        GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = null;
        transform.position = pos;
        CameraController.Instance.transform.position = pos;
        Debug.Log(transform.position);
        Debug.Log(CameraController.Instance.transform.position);
        GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = Party.Instance.current_party[0].transform;*/
        
        GetComponent<Cinemachine.CinemachineVirtualCamera>().ForceCameraPosition(Party.Instance.current_party[0].transform.position, Quaternion.identity);
    }

    public void EnterBattle()
    {
        GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = null;
        GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = BattleManager.Instance.transform;
        transform.position = new Vector3(0, 0, -9.9999f);
        //GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.Orthographic = false;
        GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.OrthographicSize = 6.016932f;
        
        //GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.OrthographicSize = 6.016932f;
    }

    public void ExitBattle()
    {
        GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = Party.Instance.current_party[0].transform;
        GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = null;
        GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.OrthographicSize = 10f;
        //GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.Orthographic = false;
        transform.position = Party.Instance.current_party[0].transform.position + new Vector3(0, 0, -9.9999f);
        //GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.OrthographicSize = 10f;
    }

    public void SwitchToCam(Cinemachine.CinemachineVirtualCamera vcam) {
        GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 9;
        vcam.Priority = 10;
    }
    
    public void SwitchFromCam(Cinemachine.CinemachineVirtualCamera vcam) {
        vcam.Priority = 9;
        GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 10;
    }
}
