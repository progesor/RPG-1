using UnityEngine;
using RPG.Movement;
using System;
using Control;
using RPG.Atributes;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health _health;
        [SerializeField] private LayerMask rayDiscardLayer;
     
        [Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] cursorMapings = null;
        [SerializeField] private float maxNavMeshProjectionDistance = 1f;
        [SerializeField] private float raycastRadius = 1f;
        
        

        void Awake()
        {
            _health = GetComponent<Health>();
        }

        void Update()
        {
            
            if(IntercatWithUI()) return;
            
            if (_health.IsDead())
            {
                SetCursor(CursorType.Death);
                return;
            }

            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;         
            
            SetCursor(CursorType.None);
        }

        

        private bool IntercatWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();

                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.getCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(),raycastRadius);

            float[] disntances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                disntances[i] = hits[i].distance;
            }
            Array.Sort(disntances,hits);
            return hits;
        }

        

        // ReSharper disable Unity.PerformanceAnalysis
        private bool InteractWithMovement()
        {
            //RaycastHit hit;
            //bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;
                
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target,1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }
        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit,1000f,~rayDiscardLayer);
            if (!hasHit) return false;
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance,
                NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;
            
            target = navMeshHit.position;
            
            return true;
        }

       

        private void SetCursor(CursorType type)
        {
            CursorMapping maping = getCursorMapping(type);
            Cursor.SetCursor(maping.texture,maping.hotspot,CursorMode.Auto);
        }

        private CursorMapping getCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMapings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }

            return cursorMapings[0];
        }
        
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
