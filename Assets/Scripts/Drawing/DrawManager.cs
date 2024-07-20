using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DrawManager : MonoBehaviour
{
    // ���ڻ���������Ԥ����
    [SerializeField] private Line BlacklinePrefab;
    [SerializeField] private Line WhitelinePrefab;
    // ������壬�������ڼ���״̬ʱ����ֹ����
    [SerializeField] private GameObject UIPanel;

    // �������ʱ���¼�
    public UnityEvent OnFinishDrawing;

    // ��ǰ���ڻ��Ƶ�����
    private Line currentBlackLine;
    private Line currentWhiteLine;
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
            currentBlackLine = Instantiate(BlacklinePrefab, Vector3.zero, Quaternion.identity, transform);
            currentWhiteLine = Instantiate(WhitelinePrefab, Vector3.zero, Quaternion.identity, transform);
            // �����굱ǰλ����Ϊ�����ĵ�һ����
            currentBlackLine.AddPosition(mousePos);
            currentWhiteLine.AddPosition(mousePos);
        }

        // �����ס��������������������
        if (Input.GetMouseButton(0))
        {
            // ���û�е�ǰ����������
            if (!currentBlackLine || !currentWhiteLine) return;

            // ��������ڵ�ǰλ�ü�����ӵ�
            if (currentBlackLine.CanAppend(mousePos))
            {
                // �����굱ǰλ����Ϊ��������һ����
                currentBlackLine.AddPosition(mousePos);
            }
            // ����γ���ǣ�������ǰ��������ʼ������
            else if (currentBlackLine.IsAcuteAngle(mousePos))
            {
                // ��ȡ��ǰ���������һ�����λ��
                Vector2 lastPos = currentBlackLine.LastPos;
                // ʵ�����µ�����Ԥ����
                currentBlackLine = Instantiate(BlacklinePrefab, Vector3.zero, Quaternion.identity, transform);
                // �����һ�����������һ������Ϊ�������ĵ�һ����
                currentBlackLine.AddPosition(lastPos);
            }

            // ��������ڵ�ǰλ�ü�����ӵ�
            if (currentWhiteLine.CanAppend(mousePos))
            {
                // �����굱ǰλ����Ϊ��������һ����
                currentWhiteLine.AddPosition(mousePos);
            }
            // ����γ���ǣ�������ǰ��������ʼ������
            else if (currentWhiteLine.IsAcuteAngle(mousePos))
            {
                // ��ȡ��ǰ���������һ�����λ��
                Vector2 lastPos = currentWhiteLine.LastPos;
                // ʵ�����µ�����Ԥ����
                currentWhiteLine = Instantiate(WhitelinePrefab, Vector3.zero, Quaternion.identity, transform);
                // �����һ�����������һ������Ϊ�������ĵ�һ����
                currentWhiteLine.AddPosition(lastPos);
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
