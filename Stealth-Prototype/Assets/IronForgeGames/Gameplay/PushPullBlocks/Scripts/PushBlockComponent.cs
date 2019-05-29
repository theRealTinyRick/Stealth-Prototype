using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

[System.Serializable]
public class PushBlockStarted : UnityEngine.Events.UnityEvent { }

[System.Serializable]
public class PushBlockEnded : UnityEngine.Events.UnityEvent { }

public class PushBlockComponent : MonoBehaviour
{
    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public Vector3 playerOffset;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public LayerMask layerMask;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public float speed;

    [TabGroup(Tabs.Events)]
    [SerializeField]
    public PushBlockStarted pushBlockStartedEvent = new PushBlockStarted();

    [TabGroup(Tabs.Events)]
    [SerializeField]
    public PushBlockEnded pushBlockEndedEvent = new PushBlockEnded();

    private bool inPosition = false;
    private bool pushingBlock = false;

    private new Rigidbody rigidbody;
    private Coroutine getIntoPosition;

    private PushPullBlock currentBlock;
    private Vector3 pushDirection;

    void OnEnable()
    {
        rigidbody = GetComponentInChildren<Rigidbody>();
    }

    public bool InitPushBlock(PushPullBlock pushPullBlock)
    {
        Vector3 _origin = transform.position + Vector3.up;
        RaycastHit _hit;

        Vector3 _playerPosition = new Vector3();

        if(Physics.Raycast(_origin, transform.forward, out _hit, 2, layerMask))
        {
            pushDirection = -_hit.normal;

            _playerPosition = _hit.point;
            _playerPosition.y = transform.position.y;
            _playerPosition += pushDirection * playerOffset.z;

            transform.position = _playerPosition;
        }
        else
        {
            return false;
        }

        currentBlock = pushPullBlock;
        inPosition = true;
        pushingBlock = true;

        pushBlockStartedEvent.Invoke();

        return false;
    }

    public void StopPushBlock()
    {
        currentBlock.StopPushPull();

        inPosition = false;
        pushingBlock = false;
        currentBlock = null;

        pushBlockEndedEvent.Invoke();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(inPosition && pushingBlock)
            {
                currentBlock.StopPushPull();
                StopPushBlock();
            }
        }
    }

    private void FixedUpdate()
    {
        if(inPosition && pushingBlock)
        {
            float _vertical = Input.GetAxis("Vertical");

            Rigidbody _rigidbody = currentBlock.GetComponentInChildren<Rigidbody>();
            if(_rigidbody == null)
            {
                StopPushBlock();
                return;
            }

            _rigidbody.velocity = (pushDirection * _vertical) * speed;
            rigidbody.velocity = (pushDirection * _vertical) * speed;
        }
    }

    private IEnumerator GetIntoPosition()
    {
        yield break;
    }
}
