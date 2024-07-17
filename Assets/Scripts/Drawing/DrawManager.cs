using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DrawManager : MonoBehaviour
{
    // ���ڻ���������Ԥ����
    [SerializeField] private Line linePrefab;
    // ������壬�������ڼ���״̬ʱ����ֹ����
    [SerializeField] private GameObject UIPanel;

    // �������ʱ���¼�
    public UnityEvent OnFinishDrawing;

    // ��ǰ���ڻ��Ƶ�����
    private Line currentLine;
    // �������
    private Camera mainCam;

    // ����Ϸ��ʼʱ��ȡ�������
    private void Start()
    {
        mainCam = Camera.main;
    }

    // ÿ֡����
    private void Update()
    {
        // ���������崦�ڼ���״̬���򷵻�
        if (UIPanel && UIPanel.gameObject.activeSelf) return;

        // ��ȡ��������������е�λ��
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // �����������������ʼ�����µ�����
        if (Input.GetMouseButtonDown(0))
        {
            // ʵ�����µ�����Ԥ����
            currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, transform);
            // �����굱ǰλ����Ϊ�����ĵ�һ����
            currentLine.AddPosition(mousePos);
        }

        // �����ס��������������������
        if (Input.GetMouseButton(0))
        {
            // ���û�е�ǰ����������
            if (!currentLine) return;

            // ��������ڵ�ǰλ�ü�����ӵ�
            if (currentLine.CanAppend(mousePos))
            {
                // �����굱ǰλ����Ϊ��������һ����
                currentLine.AddPosition(mousePos);
            }
            // ����γ���ǣ�������ǰ��������ʼ������
            else if (currentLine.IsAcuteAngle(mousePos))
            {
                // ��ȡ��ǰ���������һ�����λ��
                Vector2 lastPos = currentLine.LastPos;
                // ʵ�����µ�����Ԥ����
                currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, transform);
                // �����һ�����������һ������Ϊ�������ĵ�һ����
                currentLine.AddPosition(lastPos);
            }
        }

        // ����ɿ���������������������¼�
        if (Input.GetMouseButtonUp(0))
        {
            OnFinishDrawing?.Invoke();
        }
    }

    // �����������
    public void ClearAllLines()
    {
        // ���������Ӷ�����������
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }
}
