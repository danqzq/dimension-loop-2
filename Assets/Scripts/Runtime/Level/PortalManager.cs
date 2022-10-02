using UnityEngine;

namespace Dan.Level
{ 
    public class PortalManager : MonoBehaviour
    {
        [System.Serializable]
        private struct BoxBounds
        {
            public GameObject[] bounds;
        }
        
        [SerializeField] private GameObject[] _portals;
        [SerializeField] private BoxBounds[] _boundsToDeactivate;
        
        public int CurrentActivePortal { get; private set; }
        
        public void ActivateRandomPortal()
        {
            var randomIndex = Random.Range(0, _portals.Length);
            _portals[randomIndex].SetActive(true);
            _boundsToDeactivate[randomIndex].bounds.ForEach(b => b.tag = "Untagged");
            CurrentActivePortal = randomIndex;
        }
        
        public void DeactivateAllPortals()
        {
            foreach (var portal in _portals) portal.SetActive(false);
            _boundsToDeactivate.ForEach(b => b.bounds.ForEach(x => x.tag = "Box"));
        }
    }
}
