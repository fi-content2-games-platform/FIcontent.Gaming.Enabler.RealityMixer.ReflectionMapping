package com.farpeek.Aerial;

import java.io.File;
import java.util.List;

import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.media.MediaScannerConnection;
import android.media.MediaScannerConnection.MediaScannerConnectionClient;
import android.net.Uri;

public class SingleMediaScanner implements MediaScannerConnectionClient {

	private MediaScannerConnection mMs;
	private File mFile;
	private Context mContext;

	public SingleMediaScanner(Context context, File f) {
		mContext = context;
		mFile = f;
		mMs = new MediaScannerConnection(context, this);
		mMs.connect();
	}

	public void onMediaScannerConnected() {
		mMs.scanFile(mFile.getAbsolutePath(), null);
	}

	public void onScanCompleted(String path, Uri uri) {
		Intent intent = new Intent(Intent.ACTION_VIEW);
		intent.setData(uri);
		
		PackageManager pkManager = mContext.getPackageManager();
		List<ResolveInfo> activities = pkManager.queryIntentActivities(intent, 0);

		if (activities.size() > 1) {
			// Create and start the chooser
			Intent chooser = Intent.createChooser(intent, "Open with");
			mContext.startActivity(chooser);

		} else {
			mContext.startActivity( intent );
		}
		
		mMs.disconnect();
	}
}