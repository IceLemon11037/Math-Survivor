using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrawLinesWithMouse : MonoBehaviour
{
    // 用于存储每条线的点的列表
    private List<List<Vector2>> drawingPointsList = new List<List<Vector2>>();
    //用于存储每条线的 LineRenderer 组件的列表
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    // 线条宽度
    public float lineWidth = 0.2f;
    // 线条颜色
    public Color lineColor = Color.red;
    // 排序层级
    public int sortingOrder = 1;

    // 划线范围
    //public Rect drawingArea;
    //public GameObject OCRManager;

    public TextMeshProUGUI resultText;

    private void Start()
    {
        //drawingArea = OCRManager.GetComponent<HandwritingRecognition>().screenshotRect;
        //Debug.Log(drawingArea);
    }
    void Update()
    {
        //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 检查鼠标位置是否在划线范围内
        //if (drawingArea.Contains(mousePosition))
        //{
            if (Input.GetMouseButtonDown(0))
            {
                // 创建新的 Gameobject 作为线条对象
                GameObject lineObject = new GameObject("Line");
                // 添加 LineRenderer 组件
                LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
                // 设置线条宽度
                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;
                // 设置线条颜色
                lineRenderer.startColor = lineColor;
                lineRenderer.endColor = lineColor;
                // 创建新的材质并设置颜色
                Material material = new Material(Shader.Find("Sprites/Default"));
                material.color = lineColor;
                lineRenderer.material = material;
                // 设置排序层级
                lineRenderer.sortingOrder = sortingOrder;
                // 将 LineRenderer 组件添加到列表中
                lineRenderers.Add(lineRenderer);

                // 创建新的列表用于存储当前线条的点
                List<Vector2> drawingPoints = new List<Vector2>();
                drawingPointsList.Add(drawingPoints);
            }

            if (Input.GetMouseButton(0))
            {
                // 将屏幕坐标转换为世界坐标，并获取当前鼠标位置
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // 获取当前正在绘制的线条的点列表
                List<Vector2> drawingPoints = drawingPointsList[drawingPointsList.Count - 1];

                // 检查是否需要添加新的点
                if (drawingPoints.Count == 0 || Vector2.Distance(drawingPoints[drawingPoints.Count - 1], mousePosition) > 0.1f)
                {
                    drawingPoints.Add(mousePosition);
                    LineRenderer lineRenderer = lineRenderers[lineRenderers.Count - 1];
                    lineRenderer.positionCount = drawingPoints.Count;
                    lineRenderer.SetPosition(drawingPoints.Count - 1, mousePosition);
                }
            //}
        }
    }

    public void ClearDrawing()
    {
        resultText.SetText("");
        //清空所有线条的点列表
        foreach (List<Vector2> points in drawingPointsList)
        {
            points.Clear();
        }

        //将所有 LineRenderer 组件的点数设为 0，从而清空所有线条
        foreach(LineRenderer lineRenderer in lineRenderers)
        {
            lineRenderer.positionCount = 0;
        }

        
    }
}
