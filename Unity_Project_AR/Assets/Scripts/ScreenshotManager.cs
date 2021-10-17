using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotManager : MonoBehaviour
{
    public Image pictureHolder;

    public Text debug;

    public GameObject panel;
    public GameObject canvasNotIncluded;

    private Texture2D croppedTexture;

    //Hide picture holder
    void Start()
    {
        //this method will show and hide the picture holder
        ShowPictureHolder(false);
    }

    private void ShowPictureHolder(bool visible)
    {
        panel.SetActive(visible);
    }

    //Create take screenshot method. It will be called by the button
    //[System.Obsolete]
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
            ShowPictureHolder(true);
            // assume "sprite" is your Sprite object
            croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                    (int)sprite.textureRect.y,
                                                    (int)sprite.textureRect.width,
                                                    (int)sprite.textureRect.height);
            debug.text = "Berhasil";
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
        }
        else
            debug.text = "Gagal";


        canvasNotIncluded.GetComponent<Canvas>().enabled = true;
    }

    //create the cancel method
    public void Cancel()
    {
        ShowPictureHolder(false);
    }

    public void Save()
    {
        filename = "SS_2_" + Random.Range(0, 1000) + ".png";
        debug.text = "Udah Ada2";
        // salin file gambar ke gallery
        string album = "ARUTALA_AR"; // folder album tempat menyimpan gambar
        bool yey = false;
        NativeGallery.SaveImageToGallery(croppedTexture, album, filename, (success, path) => yey = success);
        debug.text = "aaa__"+yey;
    }

    //Create share method
    public void Share()
    {
        GleyShare.Manager.SharePicture();
    }

    [System.Obsolete]
    public IEnumerator CaptureScreen()
    {
        // Wait till the last possible moment before screen rendering to hide the UI
        yield return null;
        canvasNotIncluded.GetComponent<Canvas>().enabled = false;

        // Wait for screen rendering to complete
        yield return new WaitForEndOfFrame();

        // Take screenshot
        GleyShare.Manager.CaptureScreenshot(ScreenshotCaptured);
        // Show UI after we're done
    }

    private string filename;

    public void SaveScreenshot()
    {
        StartCoroutine(SaveScreenshotEnum());
    }

    IEnumerator SaveScreenshotEnum()
    {
        // tunggu sampai file benar-benar tersimpan dengan memeriksa apakah file ada
        bool fileExist;
        do
        {
            // selalu gunakan Path.Combine untuk menggabungkan direktori & nama file
            fileExist = File.Exists(Path.Combine(Application.persistentDataPath, filename));
            debug.text = "fileExist--" + fileExist;
            yield return null;
        } while (!fileExist);

        debug.text = "Udah Ada";
        // salin file gambar ke gallery
        string album = "ARUTALA_AR"; // folder album tempat menyimpan gambar
        NativeGallery.SaveImageToGallery(Application.persistentDataPath, album, filename, (success,path) => debug.text = "Media save result: " + success + " " + path);
        //debug.text = "Selesai";
    }

    //public void TakeScreenshot()
    //{
    //    StartCoroutine(TakeScrenshotEnum());
    //}

    IEnumerator TakeScrenshotEnum()
    {
        // nama file screenshot
        filename = "SS_" + Random.Range(0, 1000) + ".png";

        // simpan screenshot ke persistent data path
        ScreenCapture.CaptureScreenshot(filename);

        bool fileExist;
        do
        {
            // selalu gunakan Path.Combine untuk menggabungkan direktori & nama file
            fileExist = File.Exists(Path.Combine(Application.persistentDataPath, filename));
            debug.text = "fileExist2--" + fileExist;
            yield return null;
        } while (!fileExist);

        pictureHolder.sprite = LoadSprite(filename);

        pictureHolder.enabled = true;

    }

    private IEnumerator TakeScreenshotAndSave()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        // Save the screenshot to Gallery/Photos
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(ss, "GalleryTest", "Image.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));

        Debug.Log("Permission result: " + permission);

        // To avoid memory leaks
        Destroy(ss);
    }

    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (File.Exists(Path.Combine(Application.persistentDataPath, path)))
        {
            byte[] bytes = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, path));
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        pictureHolder.color = Color.blue;
        return null;
    }
}
