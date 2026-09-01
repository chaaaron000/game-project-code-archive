using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerModelController : NetworkBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _headBone;
    [Networked] float _upDownHeadRotation {get; set; }= 0.0f;

    public override void Spawned()
    {
        if (HasStateAuthority == false) return;

        _camera = Camera.main;

        //Father인지 daughter인지를  확인하여 culling mask 설정
        bool isFather = TryGetComponent<Father>(out _);
        ChangeCullingMask(isFather);
    }

    public void ChangeCullingMask(bool isFather)
    {
        Camera camera = Camera.main;
        if (camera == null) return;

        int baseMask = -1; // 모든 레이어를 보여주는 기본 마스크
        int invisibleLayer = 1 << LayerMask.NameToLayer("Invisible");
        int selfHeadLayer = isFather
            ? 1 << LayerMask.NameToLayer("Father Head")
            : 1 << LayerMask.NameToLayer("Daughter Head");

        if(isFather) {
            _camera.cullingMask = camera.cullingMask = baseMask & ~(selfHeadLayer | invisibleLayer);
        }
        else {
            _camera.cullingMask = camera.cullingMask = baseMask & ~(selfHeadLayer | invisibleLayer);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false) return;

        if (_camera == null) return;
        _upDownHeadRotation = _camera.transform.rotation.eulerAngles.x;
    }

    public void LateUpdate(){
        _headBone.localRotation = Quaternion.Euler(_upDownHeadRotation, 0, 0);
    }
}
