using Cinemachine;
using UnityEngine;

namespace Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform target = null;
        [SerializeField] CinemachineVirtualCamera gameMainCamera = null;
        [SerializeField] float scale = 3.5f;
        [SerializeField] int minimumScreen = 25;
        [SerializeField] int maximumScreen = 60;

        float scrollSpeed = 10f;
        public float topBarrier;
        public float botBarrier;
        public float leftBarrier;
        public float rightBarrier;

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                transform.position = target.position;
            }

            if (Input.mousePosition.y >= Screen.height * topBarrier)
            {
                transform.Translate(Vector3.forward * (Time.deltaTime * scrollSpeed), Space.World);
            }

            if (Input.mousePosition.y <= Screen.height * botBarrier)
            {
                transform.Translate(Vector3.forward * (Time.deltaTime * -scrollSpeed), Space.World);
            }

            if (Input.mousePosition.x >= Screen.width * rightBarrier)
            {
                transform.Translate(Vector3.right * (Time.deltaTime * scrollSpeed), Space.World);
            }

            if (Input.mousePosition.x <= Screen.width * leftBarrier)
            {
                transform.Translate(Vector3.right * (Time.deltaTime * -scrollSpeed), Space.World);
            }
            //transform.position = target.position;
            ScreenZoom();
        }

        private void ScreenZoom()
        {
            float zoomScale = Input.GetAxis("Mouse ScrollWheel");
            float FOV = gameMainCamera.m_Lens.FieldOfView;
            FOV -= zoomScale * scale;
            if (FOV > maximumScreen)
            {
                FOV = maximumScreen;
            }else if (FOV < minimumScreen)
            {
                FOV = minimumScreen;
            }
            gameMainCamera.m_Lens.FieldOfView = FOV;
        }
    }
}
