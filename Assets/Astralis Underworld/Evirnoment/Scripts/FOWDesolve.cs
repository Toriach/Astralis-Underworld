using Assets.Astralis_Underworld.Entities.Player.Scripts;
using System.Collections;
using UnityEngine;

namespace Assets.Astralis_Underworld.Evirnoment.Scripts
{
    public class FOWDesolve : MonoBehaviour
    {
        [SerializeField] private Color _orginalColor;
        [SerializeField] private AnimationCurve desolveCurve;
        [SerializeField] private float desolveTime = 1f;
        [SerializeField] private float moveSpeed = 0.5f;
        private static readonly int _color = Shader.PropertyToID("_BaseColor");
        private Renderer _renderer;
        private MaterialPropertyBlock propertyBlock;
        private float desolveTimer = 0f;
        private Vector3 _direction;

        public void SetRenderer(Renderer renderer)
        {
            _renderer = renderer;
            propertyBlock = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(propertyBlock);
            enabled = false;
        }
        public void Desolve()
        {
            _direction = GetDirection();
            _direction.y = 0f;
            desolveTimer = desolveTime;
            enabled = true;
        }

        private Vector3 GetDirection()
        {
            return (transform.position - PlayerFacade.instance.transform.position).normalized;
        }

        private void Update()
        {
            if(desolveTimer <= 0)
            {
                enabled = false;
                _renderer.enabled = false;
                Destroy(gameObject);
                return;
            }

            desolveTimer -= Time.deltaTime;
            var t = 1 - desolveTimer / desolveTime;
            Color color = new Color(_orginalColor.r, _orginalColor.g, _orginalColor.b, desolveCurve.Evaluate(t));
            propertyBlock.SetColor(_color, color);

            _renderer.SetPropertyBlock(propertyBlock);

            transform.position += _direction * Time.deltaTime  * moveSpeed;
        }
    }
}