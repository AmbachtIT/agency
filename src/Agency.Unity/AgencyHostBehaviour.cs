using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Agency.Network.RoadRunner;
using UnityEngine;

namespace Agency.Unity
{
    public class AgencyHostBehaviour : MonoBehaviour
    {
        
        
        #region Inspector setting

        /// <summary>
        /// Prefab used to visualize nodes
        /// </summary>
        public GameObject prefabNode;
        
        /// <summary>
        /// Prefab used to visualize edges
        /// </summary>
        public GameObject prefabEdge;
        
        #endregion

        private AgencyHost host;

        void Start()
        {
            Debug.Log("Agency Host Behaviour");
            host = new AgencyHost();

            if (prefabNode == null)
            {
                Debug.Log("prefabNode not set");
                return;
            }

            var vertices = host.Map.Vertices;
            var centroid = vertices.First().Location;
            var count = 1;
            foreach (var vertex in vertices.Skip(1))
            {
                centroid += vertex.Location;
                count++;
            }

            centroid /= count;
            
            var vertexObjects = new Dictionary<int, GameObject>();
            
            foreach (var vertex in vertices)
            {
                var width = GetWidth(vertex);
                var obj = Instantiate(prefabNode, Convert(vertex.Location - centroid), Quaternion.identity);
                obj.transform.localScale = new Vector3(width, thickness, width);
                vertexObjects.Add(vertex.Id, obj);
            }

            foreach (var edge in host.Map.Edges)
            {
                var width = GetWidth(edge);
                var from = Convert(edge.From.Location - centroid);
                var to = Convert(edge.To.Location - centroid);
                var delta = from - to;
                var length = delta.magnitude;
                if (length == 0)
                {
                    continue;
                }

                var obj = Instantiate(prefabEdge, from, Quaternion.identity);
                obj.transform.localScale = new Vector3(width,  thickness, length);
                obj.transform.LookAt(to);
            }
        }

        private Vector3 Convert(System.Numerics.Vector2 v, float z = 0f)
        {
            return new Vector3(v.X, z, v.Y);
        }

        private float GetWidth(Edge edge)
        {
            return (float) edge.Type.Width;
        }
        
        private float GetWidth(Vertex vertex)
        {
            if (vertex.Edges.Count == 0)
            {
                return 1f;
            }

            return vertex.Edges.Max(GetWidth);
        }

        private const float thickness = 0.1f;



    }
}