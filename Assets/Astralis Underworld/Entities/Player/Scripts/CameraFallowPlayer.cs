using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Astralis_Underworld.Entities.Player.Scripts
{
    public class CameraFallowPlayer : MonoBehaviour
    {
        [SerializeField] public float distance;
        [SerializeField] public float height;
        // Use this for initialization
        Transform player;
        void Start()
        {
            player = PlayerFacade.instance.transform;
        }

        // Update is called once per frame
        void Update()
        {
            var newPosition = new Vector3(player.position.x, player.position.y + height, player.position.z - distance);
            transform.position = newPosition;
        }
    }
}