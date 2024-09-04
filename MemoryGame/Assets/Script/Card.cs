using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Card : MonoBehaviour
{

    private int Sprite_ID;
    private int id;
    public bool Isflipped;
    private bool Isturning;
    [SerializeField] internal Image img;


    private IEnumerator Flip90(Transform thisTransform, float time, bool changeSprite)
    {
        Quaternion startRotation = thisTransform.rotation;
        Quaternion endRotation = thisTransform.rotation * Quaternion.Euler(new Vector3(0, 90, 0));
        float rate = 1.0f / time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            thisTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            yield return null;
        }
       
        if (changeSprite)
        {
            Isflipped = !Isflipped;
            ChangeSprite();
            StartCoroutine(Flip90(transform, time, false));
        }
        else
            Isturning = false;
    }
    
    public void Flip()
    {
        Isturning = true;
        AudioPlayer.Instance.Play(0);
        StartCoroutine(Flip90(transform, 0.25f, true));
    }
    
    private void ChangeSprite()
    {
        if (Sprite_ID == -1 || img == null) return;
        if (Isflipped)
            img.sprite = GameManager.Instance.GetSprite(Sprite_ID);
        else
            img.sprite = GameManager.Instance.CardBack();
    }
    
    public void Inactive()
    {
        StartCoroutine(Fade());
    }
    
    private IEnumerator Fade()
    {
        float rate = 1.0f / 2.5f;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            img.color = Color.Lerp(img.color, Color.clear, t);

            yield return null;
        }
    }
    
    public void Active()
    {
        if (img)
            img.color = Color.white;

    }
   
    public int SpriteID
    {
        set
        {
            Sprite_ID = value;
            Isflipped = true;
            ChangeSprite();
        }
        get { return Sprite_ID; }
    }
    
    public int ID
    {
        set { id = value; }
        get { return id; }
    }
    
    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        Isflipped = true;
    }
    
    public void CardBtn()
    {
        if (Isflipped || Isturning) return;
        if (!GameManager.Instance.canClick()) return;
        Flip();
        StartCoroutine(SelectionEvent());
    }
    
    private IEnumerator SelectionEvent()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.cardClicked(Sprite_ID, id);
    }
}
