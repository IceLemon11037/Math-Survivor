using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrawLinesWithMouse : MonoBehaviour
{
    // ���ڴ洢ÿ���ߵĵ���б�
    private List<List<Vector2>> drawingPointsList = new List<List<Vector2>>();
    //���ڴ洢ÿ���ߵ� LineRenderer ������б�
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    // �������
    public float lineWidth = 0.2f;
    // ������ɫ
    public Color lineColor = Color.red;
    // ����㼶
    public int sortingOrder = 1;

    // ���߷�Χ
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

        // ������λ���Ƿ��ڻ��߷�Χ��
        //if (drawingArea.Contains(mousePosition))
        //{
            if (Input.GetMouseButtonDown(0))
            {
                // �����µ� Gameobject ��Ϊ��������
                GameObject lineObject = new GameObject("Line");
                // ��� LineRenderer ���
                LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
                // �����������
                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;
                // ����������ɫ
                lineRenderer.startColor = lineColor;
                lineRenderer.endColor = lineColor;
                // �����µĲ��ʲ�������ɫ
                Material material = new Material(Shader.Find("Sprites/Default"));
                material.color = lineColor;
                lineRenderer.material = material;
                // ��������㼶
                lineRenderer.sortingOrder = sortingOrder;
                // �� LineRenderer �����ӵ��б���
                lineRenderers.Add(lineRenderer);

                // �����µ��б����ڴ洢��ǰ�����ĵ�
                List<Vector2> drawingPoints = new List<Vector2>();
                drawingPointsList.Add(drawingPoints);
            }

            if (Input.GetMouseButton(0))
            {
                // ����Ļ����ת��Ϊ�������꣬����ȡ��ǰ���λ��
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // ��ȡ��ǰ���ڻ��Ƶ������ĵ��б�
                List<Vector2> drawingPoints = drawingPointsList[drawingPointsList.Count - 1];

                // ����Ƿ���Ҫ����µĵ�
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
        //������������ĵ��б�
        foreach (List<Vector2> points in drawingPointsList)
        {
            points.Clear();
        }

        //������ LineRenderer ����ĵ�����Ϊ 0���Ӷ������������
        foreach(LineRenderer lineRenderer in lineRenderers)
        {
            lineRenderer.positionCount = 0;
        }

        
    }
}
