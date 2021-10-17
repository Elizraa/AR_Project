using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotManager : MonoBehaviour
{
    public Image pictureHolder;

    public Text debug;

    public GameObject panelResult;
    public Canvas canvasNotIncluded;
    public Canvas canvasIncluded;
    public GameObject buttonScreenshot;

    private Texture2D croppedTexture;
    private string filename;

    //Hide picture holder
    void Start()
    {
        //this method will show and hide the picture holder
        ShowPictureHolder(false);
    }

    private void ShowPictureHolder(bool visible)
    {
        panelResult.SetActive(visible);
        buttonScreenshot.SetActive(!visible);
    }

    //Create take screenshot method. It will be called by the button
    [System.Obsolete]
    public void TakeScreenshots()
    {
        StartCoroutine(CaptureScreen());
    }

    //called after a screenshot is captured
    private void ScreenshotCaptured(Sprite sprite)
    {
        if (sprite != null)
        {
            //set captured image on picture holder and enable it
            pictureHolder.sprite = sprite;
            debug.text = "";
            ShowPictureHolder(true);
            // assume "sprite" is your Sprite object
            croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                    (int)sprite.textureRect.y,
                                                    (int)sprite.textureRect.width,
                                                    (int)sprite.textureRect.height);
            //debug.text = "Berhasil";
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
        }
        //else
        //    debug.text = "Gagal";

        canvasIncluded.enabled = false;
        canvasNotIncluded.enabled = true;
    }

    //create the cancel method
    public void Cancel()
    {
        Manager.instance.PlayButtonClickSound();
        ShowPictureHolder(false);
    }

    public void Save()
    {
        Manager.instance.PlayButtonClickSound();
        debug.text = "Loading...";
        filename = "SS_Arutala"+".png";
        // salin file gambar ke gallery
        string album = "ARUTALA_AR"; // folder album tempat menyimpan gambar
        NativeGallery.SaveImageToGallery(croppedTexture, album, filename, (success, path) => debug.text = "Saved in "+path);
    }

    //Create share method
    public void Share()
    {
        Manager.instance.PlayButtonClickSound();
        GleyShare.Manager.SharePicture();
    }

    [System.Obsolete]
    public IEnumerator CaptureScreen()
    {
        // Wait till the last possible moment before screen rendering to hide the UI
        yield return null;
        canvasIncluded.enabled = true;
        canvasNotIncluded.enabled = false;
        Manager.instance.PlayButtonCaptureSound();
        // Wait for screen rendering to complete
        yield return new WaitForEndOfFrame();

        // Take screenshot
        GleyShare.Manager.CaptureScreenshot(ScreenshotCaptured);
        // Show UI after we're done
    }

}
