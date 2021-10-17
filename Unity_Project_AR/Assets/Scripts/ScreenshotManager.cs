using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotManager : MonoBehaviour
{
    [Tooltip("For holding the image screenshot taken")]
    public Image pictureHolder;

    [Tooltip("Text on-screen to debug about the process")]
    public Text debug;

    [Tooltip("The button and other object to show with the screenshot result")]
    public GameObject panelResult;

    [Tooltip("UI That didn't want to appear on the screenshot")]
    public Canvas canvasNotIncluded;

    [Tooltip("UI That want to appear on the screenshot")]
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

            // Convert the sprite to texture2D so it can be saved on android
            croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                    (int)sprite.textureRect.y,
                                                    (int)sprite.textureRect.width,
                                                    (int)sprite.textureRect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
        }

        canvasIncluded.enabled = false;
        canvasNotIncluded.enabled = true;
    }

    // create the cancel method
    public void Cancel()
    {
        Manager.instance.PlayButtonClickSound();
        ShowPictureHolder(false);
    }

    /// <summary>
    /// Function to save the image to the gallery
    /// </summary>
    public void Save()
    {
        Manager.instance.PlayButtonClickSound();
        debug.text = "Loading...";

        // name of the file
        filename = "SS_Arutala"+".png";

        // name of the album the file will be saved
        string album = "ARUTALA_AR"; 
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
    }
}
