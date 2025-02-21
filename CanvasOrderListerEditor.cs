using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _SCRIPTS_.Extensions.Editor
{
    public class CanvasOrderListerEditor : EditorWindow
    {
        private List<CanvasInfo> canvasesList = new List<CanvasInfo>();
        private Vector2 scrollPosition;
        private bool includeInactive;

        [MenuItem("Tools/Canvas Order Lister")]
        public static void ShowWindow()
        {
            GetWindow<CanvasOrderListerEditor>("Canvas Order Lister").Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Canvas Order Lister", EditorStyles.boldLabel);

            includeInactive = EditorGUILayout.Toggle("Include Inactive Canvases", includeInactive);

            if (GUILayout.Button("List Canvases"))
            {
                ListCanvases();
            }

            if (canvasesList.Count > 0)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                foreach (var canvasInfo in canvasesList)
                {
                    EditorGUILayout.BeginHorizontal("box");
                
                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        SelectCanvas(canvasInfo);
                    }

                    canvasInfo.Name = EditorGUILayout.TextField("Name", canvasInfo.Name);
                    canvasInfo.SortingOrder = EditorGUILayout.IntField("Sorting Order", canvasInfo.SortingOrder);
                    EditorGUILayout.LabelField("Active", canvasInfo.IsActive.ToString(), GUILayout.Width(50));

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();

                if (GUILayout.Button("Apply Changes to Scene"))
                {
                    ApplyChanges();
                }
            }
        }

        private void ListCanvases()
        {
            canvasesList.Clear();

            Canvas[] canvases = includeInactive ? Resources.FindObjectsOfTypeAll<Canvas>() : FindObjectsOfType<Canvas>();

            if (canvases.Length == 0)
            {
                Debug.LogWarning("No Canvas found in the scene.");
                return;
            }

            var sortedCanvases = canvases.OrderByDescending(c => c.sortingOrder).ToList();

            foreach (var canvas in sortedCanvases)
            {
                if (includeInactive || canvas.gameObject.activeInHierarchy)
                {
                    canvasesList.Add(new CanvasInfo
                    {
                        Name = canvas.name,
                        SortingOrder = canvas.sortingOrder,
                        CanvasReference = canvas,
                        IsActive = canvas.gameObject.activeInHierarchy
                    });
                }
            }

            Debug.Log("=== Canvas Order List ===");
            foreach (var item in canvasesList)
            {
                Debug.Log($"Canvas: {item.Name}, Order: {item.SortingOrder}, Active: {item.IsActive}");
            }
        }

        private void ApplyChanges()
        {
            foreach (var canvasInfo in canvasesList)
            {
                if (canvasInfo.CanvasReference != null)
                {
                    canvasInfo.CanvasReference.sortingOrder = canvasInfo.SortingOrder;
                    canvasInfo.CanvasReference.name = canvasInfo.Name;
                }
            }
            Debug.Log("Canvas sorting orders and names updated in the scene.");
        }

        private void SelectCanvas(CanvasInfo canvasInfo)
        {
            if (canvasInfo.CanvasReference != null)
            {
                Selection.activeObject = canvasInfo.CanvasReference.gameObject;
                EditorGUIUtility.PingObject(canvasInfo.CanvasReference.gameObject);
            }
        }

        private class CanvasInfo
        {
            public string Name;
            public int SortingOrder;
            public bool IsActive;
            public Canvas CanvasReference;
        }
    }
}