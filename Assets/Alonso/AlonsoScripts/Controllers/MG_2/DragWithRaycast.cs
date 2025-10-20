using System.Threading;
using UnityEngine;

public class DragWithRaycast : MonoBehaviour
{
    private Camera _cam;
    private Transform _selectedObject;
    private float _zDistance;


    [SerializeField] private MinigameManager_2 _manager;
    [SerializeField] private TimeManager _timer;
    void Start()
    {
        _cam = Camera.main;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.name == "CANDY")
                {
                    _manager.Win();
                    Debug.Log("CANDY");
                    return;
                }

                if (hit.transform.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {

                    rb.useGravity = false;

                    _selectedObject = hit.transform;
                    _zDistance = _cam.WorldToScreenPoint(_selectedObject.position).z;
                }
            }
        }

        if (Input.GetMouseButton(0) && _selectedObject != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = _zDistance;
            Vector3 worldPos = _cam.ScreenToWorldPoint(mousePos);

            //_selectedObject.position = new Vector3(worldPos.x, worldPos.y, _selectedObject.position.z);

            _selectedObject.position = Vector3.Lerp(_selectedObject.position, new Vector3(worldPos.x, worldPos.y, _selectedObject.position.z), Time.deltaTime * 10f);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (_selectedObject != null)
            {
                _selectedObject.GetComponent<Rigidbody>().useGravity = true;
            }
            _selectedObject = null;
        }
    }
}