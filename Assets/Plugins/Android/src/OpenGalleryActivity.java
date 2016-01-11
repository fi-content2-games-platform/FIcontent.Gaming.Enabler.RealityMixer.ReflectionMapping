package com.farpeek.Aerial;

import java.io.File;
import java.util.Arrays;

import com.unity3d.player.UnityPlayerActivity;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.media.MediaScannerConnection;
import android.media.MediaScannerConnection.MediaScannerConnectionClient;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;

public class OpenGalleryActivity extends UnityPlayerActivity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
	}
	
	
	//main method to call from Unity
	public static void openGallery(Context context) {
		File folder = getARPixFolderPath(context);
		File[] allFiles = folder.listFiles();
		//new SingleMediaScanner(OpenGalleryActivity.this, allFiles[0]);
		new SingleMediaScanner(context, allFiles[0]);
	}
	
	private static File getARPixFolderPath(Context context) {
		String pathToARPix = getImageFolderPathFromURI(context, android.provider.MediaStore.Images.Media.EXTERNAL_CONTENT_URI, "ARPix");
		File folder = new File(pathToARPix);
		
		return folder;
	}

	private static String getImageFolderPathFromURI(Context context, Uri contentUri, String folderName) {

		String rawStringPath = null;

		String res = null;
		Cursor cursor = context.getContentResolver().query(contentUri, null, null, null, null);

		if(cursor.moveToFirst()){
			do {
				int column_index = cursor.getColumnIndexOrThrow(MediaStore.Images.Media.DATA);
				res = cursor.getString(column_index);
				Log.v("Unity", res);
				if (res.contains(folderName)) {
					rawStringPath = res.substring(0, res.indexOf(folderName) + 5);
					Log.v("Unity", "RAW " + rawStringPath);
				}
			} while (cursor.moveToNext());
		}
		cursor.close();
		return rawStringPath;
	}
}
