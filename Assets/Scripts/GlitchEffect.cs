/**
This work is licensed under a Creative Commons Attribution 3.0 Unported License.
http://creativecommons.org/licenses/by/3.0/deed.en_GB

You are free:

to copy, distribute, display, and perform the work
to make derivative works
to make commercial use of the work
*/

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/GlitchEffect")]
[RequireComponent(typeof(Camera))]
public class GlitchEffect : MonoBehaviour
{
	public Texture2D displacementMap;
	public Shader Shader;
	[Header("Glitch Intensity")]

	[Range(0, 1)]
	public float intensity;

	[Range(0, 1)]
	public float flipIntensity;

	[Range(0, 1)]
	public float colorIntensity;

    public bool reg = true;
    public float glitchUpTime = 0.05f;
    public float glitchDownTime = 0.05f;
    public float flickerTime = 0.05f;

    private float _glitchupTime = 0.05f;
    private float _glitchdownTime = 0.05f;
    private float _flickerTime = 0.5f;

    private float _glitchup;
	private float _glitchdown;
	private float flicker;
	
	
	private Material _material;

	void Start()
	{
		_material = new Material(Shader);
	}

	// Called by camera to apply image effect
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		_material.SetFloat("_Intensity", intensity);
		_material.SetFloat("_ColorIntensity", colorIntensity);
		_material.SetTexture("_DispTex", displacementMap);

		flicker += Time.deltaTime * colorIntensity;
		if (flicker > _flickerTime)
		{
			_material.SetFloat("filterRadius", Random.Range(-3f, 3f) * colorIntensity);
			_material.SetVector("direction", Quaternion.AngleAxis(Random.Range(0, 360) * colorIntensity, Vector3.forward) * Vector4.one);
			flicker = 0;
            if (reg)
                _flickerTime = Random.value;
            else
                _flickerTime = flickerTime;
            
		}

		if (colorIntensity == 0)
			_material.SetFloat("filterRadius", 0);

		_glitchup += Time.deltaTime * flipIntensity;
		if (_glitchup > _glitchupTime)
		{
			if (Random.value < 0.1f * flipIntensity)
				_material.SetFloat("flip_up", Random.Range(0, 1f) * flipIntensity);
			else
				_material.SetFloat("flip_up", 0);

			_glitchup = 0;
            if(reg)
			    _glitchupTime = Random.value / 10f;
            else
                _glitchupTime = glitchUpTime;
        }

		if (flipIntensity == 0)
			_material.SetFloat("flip_up", 0);

		_glitchdown += Time.deltaTime * flipIntensity;
		if (_glitchdown > _glitchdownTime)
		{
			if (Random.value < 0.1f * flipIntensity)
				_material.SetFloat("flip_down", 1 - Random.Range(0, 1f) * flipIntensity);
			else
				_material.SetFloat("flip_down", 1);

			_glitchdown = 0;
            if (reg)
                _glitchdownTime = Random.value / 10f;
            else
                _glitchdownTime = glitchUpTime;
		}

		if (flipIntensity == 0)
			_material.SetFloat("flip_down", 1);

		if (Random.value < 0.05 * intensity)
		{
			_material.SetFloat("displace", Random.value * intensity);
			_material.SetFloat("scale", 1 - Random.value * intensity);
		}
		else
			_material.SetFloat("displace", 0);

		Graphics.Blit(source, destination, _material);
	}
}
