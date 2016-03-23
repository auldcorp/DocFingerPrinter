package auldcorporation.docfingerprinterbeta;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;

/**
 * Created by groge on 3/22/2016.
 */
public class UploadImage extends Activity implements OnClickListener {

    private EditText mLocation;
    private Button mBrowse;
    private Button mUpload;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_upload_image);
    }

    @Override
    public void onClick(View v) {

    }
}
