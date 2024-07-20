using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DrawManager : MonoBehaviour
{
    // 用于绘制线条的预制体
    [SerializeField] private Line BlacklinePrefab;
    [SerializeField] private Line WhitelinePrefab;
    // 审阅面板，当它处于激活状态时，禁止绘制
    [SerializeField] private GameObject UIPanel;

    // 绘制完成时的事件
    public UnityEvent OnFinishDrawing;

    // 当前正在绘制的线条
    private Line currentBlackLine;
    private Line currentWhiteLine;
    // 主摄像机
    private Camera mainCam;

    // 在游戏开始时获取主摄像机
    private void Start()
    {
        mainCam = Camera.main;

    }

    // 每帧更新
    private void Update()
    {
        // 如果审阅面板处于激活状态，则返回
        if (UIPanel && UIPanel.gameObject.activeSelf) return;

        // 获取鼠标在世界坐标中的位置
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // 如果按下鼠标左键，开始绘制新的线条
        if (Input.GetMouseButtonDown(0))
        {
            // 实例化新的线条预制体
            currentBlackLine = Instantiate(BlacklinePrefab, Vector3.zero, Quaternion.identity, transform);
            currentWhiteLine = Instantiate(WhitelinePrefab, Vector3.zero, Quaternion.identity, transform);
            // 添加鼠标当前位置作为线条的第一个点
            currentBlackLine.AddPosition(mousePos);
            currentWhiteLine.AddPosition(mousePos);
        }

        // 如果按住鼠标左键，继续绘制线条
        if (Input.GetMouseButton(0))
        {
            // 如果没有当前线条，返回
            if (!currentBlackLine || !currentWhiteLine) return;

            // 如果可以在当前位置继续添加点
            if (currentBlackLine.CanAppend(mousePos))
            {
                // 添加鼠标当前位置作为线条的下一个点
                currentBlackLine.AddPosition(mousePos);
            }
            // 如果形成锐角，结束当前线条并开始新线条
            else if (currentBlackLine.IsAcuteAngle(mousePos))
            {
                // 获取当前线条的最后一个点的位置
                Vector2 lastPos = currentBlackLine.LastPos;
                // 实例化新的线条预制体
                currentBlackLine = Instantiate(BlacklinePrefab, Vector3.zero, Quaternion.identity, transform);
                // 添加上一个线条的最后一个点作为新线条的第一个点
                currentBlackLine.AddPosition(lastPos);
            }

            // 如果可以在当前位置继续添加点
            if (currentWhiteLine.CanAppend(mousePos))
            {
                // 添加鼠标当前位置作为线条的下一个点
                currentWhiteLine.AddPosition(mousePos);
            }
            // 如果形成锐角，结束当前线条并开始新线条
            else if (currentWhiteLine.IsAcuteAngle(mousePos))
            {
                // 获取当前线条的最后一个点的位置
                Vector2 lastPos = currentWhiteLine.LastPos;
                // 实例化新的线条预制体
                currentWhiteLine = Instantiate(WhitelinePrefab, Vector3.zero, Quaternion.identity, transform);
                // 添加上一个线条的最后一个点作为新线条的第一个点
                currentWhiteLine.AddPosition(lastPos);
            }
        }

        // 如果松开鼠标左键，触发绘制完成事件
        if (Input.GetMouseButtonUp(0))
        {
            OnFinishDrawing?.Invoke();
        }
    }

    // 清除所有线条
    public void ClearAllLines()
    {
        // 遍历所有子对象并销毁它们
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }
}
