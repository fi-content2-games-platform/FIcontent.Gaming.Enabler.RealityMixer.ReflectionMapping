package com.farpeek.Aerial;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.widget.RelativeLayout;
import android.widget.TextView;
import android.widget.Toast;

public class SaveToGalleryActivity extends Activity {
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
        RelativeLayout relativeLayout = new RelativeLayout(this);


        TextView tv = new TextView(this);
        tv.setText("Custom Message");

        RelativeLayout.LayoutParams lp = new RelativeLayout.LayoutParams(
                RelativeLayout.LayoutParams.WRAP_CONTENT,
                RelativeLayout.LayoutParams.WRAP_CONTENT);
        lp.addRule(RelativeLayout.CENTER_IN_PARENT);

        tv.setLayoutParams(lp);

        relativeLayout.addView(tv, lp);

        setContentView(relativeLayout);
		
		tip();
		logResult();
	}

	public void tip() {
		Toast.makeText(this, "SaveToGalleryActivity", Toast.LENGTH_LONG).show();
	}
	
	public void logResult() {
		Log.e("Unity", "Did this finally work?");
	}
	
	public void printStuff() {
		Log.v("Unity", "WORK WORK WORK");
		Log.v("Unity", "can I get this non-static method to work???????");
	}
}
