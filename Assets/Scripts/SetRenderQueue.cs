//此脚本附加在粒子特效的物体上，改变粒子特效的渲染顺序，使粒子最后才被渲染，这样粒子特效就不会被轨道所遮挡

using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
	public int renderQueue = 30000;
	
	Material mMat;
	
	void Start ()
	{
		Renderer ren = renderer;
		
		if (ren == null)
		{
			ParticleSystem sys = GetComponent<ParticleSystem>();
			if (sys != null) ren = sys.renderer;
		}
		
		if (ren != null)
		{
			mMat = new Material(ren.sharedMaterial);
			mMat.renderQueue = renderQueue;
			ren.material = mMat;
		}
	}
	
	void OnDestroy () { if (mMat != null) Destroy(mMat); }

	public void SetQueueRunTime(int q) {
		renderQueue = q;
		Start();
	}
}