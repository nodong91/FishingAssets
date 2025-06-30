using UnityEngine;
using System.Collections;

public class MosaicTest : MonoBehaviour
{
    public Material fullScreen;
    private Vector2 mosaicPoint;
    private float mosaicRadius;
    public float thickness;
    Coroutine mosaicCoroutine;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetFadeShader(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            SetFadeShader(false);
        }
    }

    void SetShader(bool _isOn)
    {
        if (mosaicCoroutine != null)
            StopCoroutine(mosaicCoroutine);
        if (_isOn == true)
            mosaicCoroutine = StartCoroutine(SetShader());
    }

    IEnumerator SetShader()
    {
        mosaicRadius = 0f;
        while (true)
        {
            mosaicPoint = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            fullScreen.SetVector("_MosaicPoint", mosaicPoint);
            mosaicRadius += Time.deltaTime;
            fullScreen.SetFloat("_MosaicRadius", mosaicRadius);
            float thiness = (1f - mosaicRadius) * thickness;
            fullScreen.SetFloat("_thiness", thiness);
            yield return null;

            if (mosaicRadius > 1f)
                mosaicRadius = 0f;
        }
    }

    void SetFadeShader(bool _isOn)
    {
        if (mosaicCoroutine != null)
            StopCoroutine(mosaicCoroutine);
        if (_isOn == true)
            mosaicCoroutine = StartCoroutine(FadeMosaic());
    }

    public float mosaicCell;
    IEnumerator FadeMosaic()
    {
        float normalize = 0f;
        while (normalize < 1f)
        {
            normalize += Time.deltaTime;
            float cell = Mathf.Lerp(0f, mosaicCell, normalize);
            fullScreen.SetFloat("_MosaicCell", cell);
            fullScreen.SetFloat("_FadeOut", normalize);

            yield return null;
        }
    }
}
