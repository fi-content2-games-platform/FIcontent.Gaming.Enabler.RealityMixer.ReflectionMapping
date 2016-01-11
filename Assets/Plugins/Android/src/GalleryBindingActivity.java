/**
 * This activity acts as an intermediary that communicates with Unity and
 * starts the main activity on Android.
 */

package com.farpeek.Aerial;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.text.SimpleDateFormat;
import java.util.Date;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.util.Log;
import android.widget.ImageView;
import android.widget.VideoView;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class GalleryBindingActivity extends UnityPlayerActivity {

	private static String mCurrentPhotoPath;
	private static String unityPhotoPath;

	private static final String JPEG_FILE_PREFIX = "IMG_";
	private static final String JPEG_FILE_SUFFIX = ".png";

	private static AlbumStorageDirFactory mAlbumStorageDirFactory = new FroyoAlbumDirFactory();
	private ImageView mImageView;
	private Uri mVideoUri;
	private VideoView mVideoView;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
	}

	public static void startSaveActivity() {
		UnityPlayer.currentActivity.runOnUiThread(new Runnable() {

			@Override
			public void run() {
				Intent intent = new Intent(UnityPlayer.currentActivity.getApplicationContext(), SaveToGalleryActivity.class);
				UnityPlayer.currentActivity.startActivity(intent);
			}
		});
	}
	
	
	
	public static void savePhotoToGallery(final String path) {
		//UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
		//	@Override
		//public void run() {
			copyPhotoToGallery(path);
			handleBigCameraPhoto();
		//}
		//});
		
	}
	
	//################################################################
	
	
	
	private static void handleBigCameraPhoto() {

		if (mCurrentPhotoPath != null) {
		//	setPic();
			galleryAddPic();
			mCurrentPhotoPath = null;
		}
	}

	private static void galleryAddPic() {
		Intent mediaScanIntent = new Intent("android.intent.action.MEDIA_SCANNER_SCAN_FILE");
		File f = new File(mCurrentPhotoPath);
		Uri contentUri = Uri.fromFile(f);
		mediaScanIntent.setData(contentUri);
		UnityPlayer.currentActivity.sendBroadcast(mediaScanIntent);
	}


	private static File createImageFile() throws IOException {
		// Create an image file name
		String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").format(new Date());
		String imageFileName = JPEG_FILE_PREFIX + timeStamp + "_";
		File albumF = getAlbumDir();
		//check whether createTempFile is OK to use - it's possible that it's not ;)
		File imageF = File.createTempFile(imageFileName, JPEG_FILE_SUFFIX, albumF);
		
		Log.d("Unity", imageF.getAbsolutePath());
		return imageF;
	}

	private static File setUpPhotoFile() throws IOException {

		File f = createImageFile();
		mCurrentPhotoPath = f.getAbsolutePath();

		return f;
	}

	private static File getAlbumDir() {
		File storageDir = null;

		if (Environment.MEDIA_MOUNTED.equals(Environment.getExternalStorageState())) {

			storageDir = mAlbumStorageDirFactory.getAlbumStorageDir(getAlbumName());

			if (storageDir != null) {
				if (! storageDir.mkdirs()) {
					if (! storageDir.exists()) {
						Log.d("CameraSample", "failed to create directory");
						return null;
					}
				}
			}

		} else {
			Log.v("Unity", "External storage is not mounted READ/WRITE.");
		}

		return storageDir;
	}

	/* Photo album for this application */
	private static String getAlbumName() {
		return "ARPix";
		//return getString(R.string.album_name);
	}

	public static void copy(File src, File dst) throws IOException {
		InputStream in = new FileInputStream(src);
		OutputStream out = new FileOutputStream(dst);

		Log.v("copy", src.getAbsolutePath());
		Log.v("copy", dst.getAbsolutePath());
		
		byte[] buffer = new byte[1024];
		int len;
		while((len = in.read(buffer)) > 0) {
			out.write(buffer, 0, len);
		}

		Log.v("Unity", "copied");
		
		in.close();
		out.close();
	}

	/**
	 * Copies the file with path @param path to the camera folder.
	 * @param path
	 */

	//Change this method so that it copies the image into the new file here
	//(rather than firing an intent to the camera)
	public static void copyPhotoToGallery(String path) {

		File dst = null;
		File src = null;
		try {
			dst = setUpPhotoFile();
			mCurrentPhotoPath = dst.getAbsolutePath();
			Log.v("Unity", mCurrentPhotoPath);
			setUnityPhotoPath(path);
			src = setUpSourceFile();
			copy(src, dst);
		} catch (IOException e) {
			e.printStackTrace();
			dst = null;
			mCurrentPhotoPath = null;
		}
	} 

	public static void setUnityPhotoPath(String path) {
		unityPhotoPath = path;
	}

	private static File setUpSourceFile() {
		return new File(unityPhotoPath);
	}
}
