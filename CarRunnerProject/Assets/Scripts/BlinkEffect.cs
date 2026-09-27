using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private List<BlinkRenderer> _blinkRenderers;

    [SerializeField] private Material _blinkMaterial;
    [SerializeField] private float blinkTime;
    [SerializeField] private float blinkDelay;


    private CancellationTokenSource _ctBlink;

    public void ActivateBlink()
    {
        _ctBlink?.Cancel();
        _ctBlink?.Dispose();

        _ctBlink = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
        BlinkEffectAsync(_ctBlink.Token).Forget();
    }

    private async UniTask BlinkEffectAsync(CancellationToken ct)
    {
        ChangingMaterial(true);

        await UniTask.Delay(TimeSpan.FromSeconds(blinkTime), cancellationToken: ct);

        ChangingMaterial(false);

        await UniTask.Delay(TimeSpan.FromSeconds(blinkDelay), cancellationToken: ct);

        ChangingMaterial(true);

        await UniTask.Delay(TimeSpan.FromSeconds(blinkTime), cancellationToken: ct);

        ChangingMaterial(false);
    }

    private void ChangingMaterial(bool blink)
    {
        Material currentMaterial;

        foreach (var renderer in _blinkRenderers)
        {
            if (blink)
                currentMaterial = _blinkMaterial;
            else
                currentMaterial = renderer.meshMaterial;

            if (renderer.meshRenderer == null)
                renderer.skinnedRenderer.material = currentMaterial;
            else
                renderer.meshRenderer.material = currentMaterial;





        }
    }

}

[System.Serializable]
public struct BlinkRenderer
{
    public MeshRenderer meshRenderer;
    public SkinnedMeshRenderer skinnedRenderer;

    public Material meshMaterial;
}