package com.farpeek.Aerial;

import java.io.File;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class ShareActivity extends  UnityPlayerActivity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
	}
	
	@Override
	protected void onStop() {
		super.onStop();
		Log.v("Unity", "from onStop");
	}
	
	public static void shareIt(String filename, String textToShare) {
		Intent sharingIntent = new Intent(Intent.ACTION_SEND);		
		sharingIntent.setType("image/*");
		
		//sharingIntent.putExtra(android.content.Intent.EXTRA_TEXT, textToShare);
		
		//String filename = "/mnt/sdcard/Android/data/com.farpeek.Aerial/files/photo.png";
		Uri uri = Uri.fromFile(new File(filename));
		sharingIntent.putExtra(Intent.EXTRA_STREAM, uri);
		
		UnityPlayer.currentActivity.startActivity(Intent.createChooser(sharingIntent, "Share via"));
	}
}
