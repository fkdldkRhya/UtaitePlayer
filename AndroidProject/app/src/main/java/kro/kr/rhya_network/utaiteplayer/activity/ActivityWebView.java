package kro.kr.rhya_network.utaiteplayer.activity;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.webkit.CookieManager;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.pnikosis.materialishprogress.ProgressWheel;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.core.RhyaCore;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAESManager;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaDialogManager;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaLoginWebViewClient;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSharedPreferences;

public class ActivityWebView extends AppCompatActivity {
    // Context
    private Context context;
    // 뒤로가기 버튼 시간
    private long backBtnTime = 0;


    @SuppressLint({"SetJavaScriptEnabled"})
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_webview);


        context = this;
        Activity activity = this;


        // UI Object
        ProgressWheel progressWheel = findViewById(R.id.progressWheel);


        // RHYA Utils
        RhyaCore rhyaCore = new RhyaCore();
        RhyaDialogManager rhyaDialogManager = new RhyaDialogManager(activity);
        RhyaSharedPreferences rhyaSharedPreferences = new RhyaSharedPreferences(context, false);
        RhyaAESManager rhyaAESManager = new RhyaAESManager(context);


        // WebView 설정
        // UI Object
        WebView webView = findViewById(R.id.webView);
        CookieManager cookieManager = CookieManager.getInstance();
        cookieManager.removeSessionCookies(null);
        cookieManager.removeAllCookies(null);
        WebSettings webSettings = webView.getSettings();
        webSettings.setJavaScriptEnabled(true);
        webSettings.setLoadWithOverviewMode(true);
        webSettings.setDefaultTextEncodingName("UTF-8");
        webSettings.setDomStorageEnabled(true);
        webSettings.setTextZoom(100);
        webView.setWebViewClient(new RhyaLoginWebViewClient(progressWheel,
                rhyaCore,
                rhyaSharedPreferences,
                rhyaDialogManager,
                rhyaAESManager,
                context,
                activity));

        webView.loadUrl(rhyaCore.USER_LOGIN_URL);
    }



    @Override
    public void onBackPressed() {
        /*
        rhyaDialogManager.createDialog_YesOrNo(context,
                getWindowManager().getDefaultDisplay(),
                "종료하시겠습니까?",
                "정말 종료하시겠습니까?",
                "종료",
                "취소",
                true,
                new RhyaDialogManager.DialogListener_YesOrNo() {
                    @Override
                    public void onClickListenerButtonYes(Dialog dialog) {
                        dialog.dismiss();

                        moveTaskToBack(true);
                        finish();
                        android.os.Process.killProcess(android.os.Process.myPid());
                    }

                    @Override
                    public void onClickListenerButtonNo(Dialog dialog) {
                        dialog.dismiss();
                    }
                });*/

        long curTime = System.currentTimeMillis();
        long gapTime = curTime - backBtnTime;

        if(0 <= gapTime && 2000 >= gapTime) {
            // 종료
            super.onBackPressed();
        } else {
            backBtnTime = curTime;
            Toast.makeText(context, "'뒤로' 버튼을 한번 더 누르시면 종료됩니다.", Toast.LENGTH_SHORT).show();
        }
    }
}
