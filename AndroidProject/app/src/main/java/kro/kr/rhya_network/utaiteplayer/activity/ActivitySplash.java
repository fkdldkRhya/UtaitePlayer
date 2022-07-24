package kro.kr.rhya_network.utaiteplayer.activity;

import android.Manifest;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.Dialog;
import android.app.DownloadManager;
import android.content.BroadcastReceiver;
import android.content.ComponentName;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.ServiceConnection;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.Signature;
import android.database.Cursor;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.os.IBinder;
import android.view.View;
import android.view.animation.Animation;
import android.view.animation.AnimationUtils;
import android.webkit.CookieManager;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.ImageView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.FileProvider;

import com.gun0912.tedpermission.PermissionListener;
import com.gun0912.tedpermission.normal.TedPermission;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.File;
import java.io.UnsupportedEncodingException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.List;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;

import kro.kr.rhya_network.utaiteplayer.BuildConfig;
import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.core.RhyaCore;
import kro.kr.rhya_network.utaiteplayer.service.PlayerService;
import kro.kr.rhya_network.utaiteplayer.utils.CellphoneRoutingCheck;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAESManager;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaDialogManager;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHttpsConnection;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMainUtils;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSharedPreferences;


public class ActivitySplash extends AppCompatActivity {
    // Activity
    private Context context;
    private Activity activity;
    // RHYA Utils
    private RhyaCore rhyaCore;
    private RhyaMainUtils rhyaMainUtils;
    private RhyaDialogManager rhyaDialogManager;
    private RhyaSharedPreferences rhyaSharedPreferences;
    private RhyaAESManager rhyaAESManager;
    // Version
    private String ppVersion;
    private String temp;
    // 업데이트 데이터
    private long enqueue;
    private DownloadManager downloadManager;
    // 뒤로가기 버튼 시간
    private long backBtnTime = 0;
    // Toast message
    private Toast toast;




    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_splash);

        // UI Object
        ImageView imageView = findViewById(R.id.imageView);

        context = this;
        activity = this;


        rhyaCore = new RhyaCore();
        rhyaMainUtils = new RhyaMainUtils();
        rhyaDialogManager = new RhyaDialogManager(activity);
        rhyaSharedPreferences = new RhyaSharedPreferences(context, false);
        rhyaAESManager = new RhyaAESManager(context);

        toast = Toast.makeText(getApplicationContext(), null, Toast.LENGTH_SHORT);

        Animation animation = AnimationUtils.loadAnimation(context, R.anim.anim_fast_speed_fadein);


        animation.setAnimationListener(new Animation.AnimationListener() {
            @Override
            public void onAnimationStart(Animation animation) {
                imageView.setVisibility(View.VISIBLE);
            }

            @Override
            public void onAnimationEnd(Animation animation) {
                permissionChecker();
            }

            @Override
            public void onAnimationRepeat(Animation animation) {
                // No task
            }
        });
        // 애니메이션 재생
        imageView.startAnimation(animation);
    }


    private void permissionChecker() {
        TedPermission.create()
                .setGotoSettingButton(false)
                .setPermissionListener(permissionlistener)
                .setPermissions(Manifest.permission.READ_PHONE_STATE)
                .check();
    }
    private final PermissionListener permissionlistener = new PermissionListener() {
        @Override
        public void onPermissionGranted() {
            MainTask();
        }

        @Override
        public void onPermissionDenied(List<String> deniedPermissions) {
            toast.cancel();
            toast.setText("권한 허용을 하지 않으면 서비스를 이용할 수 없습니다.");
            toast.show();

            finish();
        }
    };


    public void MainTask() {
        // 비동기 처리 결과
        final String noHaveAuth = "NO_HAVE_AUTH";
        final String haveAuth = "HAVE_AUTH";
        final String aesDecryptError = "AES_DECRYPT_ERROR";
        final String closeServer = "CLOSE_SERVER";
        final String updateApp = "UPDATE_APP";
        final String jsonError = "JSON_ERROR";
        final String unknownError = "UNKNOWN_ERROR";
        final String rootingApp = "ROOTING_APP";


        // 서버 통신 비동기 작업
        new RhyaAsyncTask<String, String>() {
            @Override
            protected void onPreExecute() {
            }

            @Override
            protected String doInBackground(String arg) {
                String[] result;

                try {
                    try {
                        result = rhyaCore.getServerInfo();

                        if (result == null)
                            return "error";

                        ppVersion = result[2];
                        RhyaApplication.appVersion = result[1];



                        Context context = getApplicationContext();
                        PackageManager pm = context.getPackageManager();
                        String packageName = context.getPackageName();
                        String cert;

                        try {
                            @SuppressLint("PackageManagerGetSignatures")
                            PackageInfo packageInfo = pm.getPackageInfo(packageName, PackageManager.GET_SIGNATURES);

                            Signature certSignature =  packageInfo.signatures[0];
                            MessageDigest msgDigest = MessageDigest.getInstance("SHA1");
                            msgDigest.update(certSignature.toByteArray());

                            byte[] byteData = msgDigest.digest();

                            StringBuffer sb;
                            sb = new StringBuffer();

                            for (byte byteDatum : byteData) {
                                sb.append(Integer.toString((byteDatum & 0xff) + 0x100, 16).substring(1));
                                sb.append(":");
                            }

                            cert = sb.toString();

                            RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                            ContentValues urlParm = new ContentValues();

                            urlParm.put("mode", "6");

                            try {
                                JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(rhyaCore.MAIN_URL, urlParm));
                                String getSignKey = jsonObject.getString("key");

                                if (!getSignKey.equals(cert)) {
                                    // 메시지 출력
                                    activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(ActivitySplash.this,
                                            "Integrity Check",
                                            "앱의 무결성 검사 중 앱의 위변조가 감지되었습니다. 서명된 SHA 지문이 일치하지 않습니다.",
                                            "종료",
                                            false,
                                            dialog -> {
                                                moveTaskToBack(true);
                                                finish();
                                                android.os.Process.killProcess(android.os.Process.myPid());
                                            }));

                                    return "appClose";
                                }
                            }catch (Exception ex) {
                                ex.printStackTrace();

                                activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(ActivitySplash.this,
                                        "Integrity Check",
                                        "앱의 무결성 검사 중 앱의 위변조가 감지되었습니다. 서명된 SHA 지문이 일치하지 않습니다.",
                                        "종료",
                                        false,
                                        dialog -> {
                                            moveTaskToBack(true);
                                            finish();
                                            android.os.Process.killProcess(android.os.Process.myPid());
                                        }));

                                return "appClose";
                            }
                        } catch (PackageManager.NameNotFoundException | NoSuchAlgorithmException e) {
                            e.printStackTrace();

                            // 메시지 출력
                            activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(ActivitySplash.this,
                                    "Integrity Check Error",
                                    "앱의 무결성 검사 과정에서 오류가 발생하였습니다. 앱의 서명된 지문을 읽을 수 없습니다.",
                                    "종료",
                                    false,
                                    dialog -> {
                                        moveTaskToBack(true);
                                        finish();
                                        android.os.Process.killProcess(android.os.Process.myPid());
                                    }));

                            return "appClose";
                        }


                        if (!rhyaCore.checkMusicInfoFile(activity)) {
                            temp = rhyaCore.getDownloadSize();
                        }

                        // 서버 상태 확인
                        if (result[0].equals("close")) {
                            // 메시지 출력
                            activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(ActivitySplash.this,
                                    "Server Close",
                                    "서버가 열리지 않았습니다. 나중에 다시 시도해주십시오.",
                                    "종료",
                                    false,
                                    dialog -> {
                                        moveTaskToBack(true);
                                        finish();
                                        android.os.Process.killProcess(android.os.Process.myPid());
                                    }));

                            return closeServer;
                        }else {
                            // 버전 확인
                            if (!rhyaMainUtils.updateChecker(result[1])) {
                                rhyaSharedPreferences.removeString("_UPDATE_TEXT_CHECKER_", context);
                                // 메시지 출력
                                activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(ActivitySplash.this,
                                        "New Version Detected",
                                        "새로운 버전이 감지되었습니다. 업데이트가 필요합니다. 확인을 누르면 업데이트가 진행됩니다.",
                                        "확인",
                                        false,
                                        dialog -> {
                                            dialog.dismiss();

                                            try {
                                                if (PlayerService.isStartService) {
                                                    getApplicationContext().bindService(new Intent(getApplicationContext(), PlayerService.class), new ServiceConnection() {
                                                        @Override
                                                        public void onServiceConnected(ComponentName name, IBinder service) {
                                                            PlayerService.LocalBinder myBinder = (PlayerService.LocalBinder) service;
                                                            PlayerService myService = myBinder.getService();
                                                            if (myService.mMediaPlayer != null) {
                                                                if (myService.mMediaPlayer.isPlaying()) {
                                                                    myService.mMediaPlayer.pause();
                                                                }

                                                                myService.mMediaPlayer.reset();
                                                            }

                                                            myService.stopService(false);
                                                            updateApp();
                                                        }

                                                        @Override
                                                        public void onServiceDisconnected(ComponentName name) {
                                                            updateApp();
                                                        }
                                                    }, Context.BIND_AUTO_CREATE);
                                                }else {
                                                    updateApp();
                                                }
                                            }catch (Exception ex) {
                                                ex.printStackTrace();

                                                // 예외 처리
                                                activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(ActivitySplash.this,
                                                        "Unknown Error",
                                                        "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00065)",
                                                        "종료",
                                                        false,
                                                        dialogSub -> {
                                                            moveTaskToBack(true);
                                                            finish();
                                                            android.os.Process.killProcess(android.os.Process.myPid());
                                                        }));
                                            }
                                        }));

                                return updateApp;
                            }else {
                                // 루팅 확인
                                CellphoneRoutingCheck cellphoneRoutingCheck = new CellphoneRoutingCheck();
                                if (cellphoneRoutingCheck.checkSuperUser()) {
                                    // 메시지 출력
                                    activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(ActivitySplash.this,
                                            "Unauthorized Access",
                                            "비정상적인 접근이 감지되었습니다. 해당 접근은 보안정책에 위배되어 접근을 제한합니다.",
                                            "종료",
                                            false,
                                            dialog -> {
                                                moveTaskToBack(true);
                                                finish();
                                                android.os.Process.killProcess(android.os.Process.myPid());
                                            }));

                                    return rootingApp;
                                }else {
                                    // Auth token 확인
                                    try {
                                        if (rhyaCore.isOKAuthToken(rhyaSharedPreferences.getString(rhyaSharedPreferences.SHARED_PREFERENCES_AUTH_TOKEN, context))) {
                                            return haveAuth;
                                        }else {
                                            return noHaveAuth;
                                        }
                                    } catch (NoSuchPaddingException | InvalidKeyException |
                                            UnsupportedEncodingException | IllegalBlockSizeException |
                                            BadPaddingException | NoSuchAlgorithmException | InvalidAlgorithmParameterException e) {
                                        e.printStackTrace();

                                        // 예외 처리
                                        activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(ActivitySplash.this,
                                                "AES Decryption Error",
                                                "데이터를 복호화하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오.",
                                                "종료",
                                                false,
                                                dialog -> {
                                                    moveTaskToBack(true);
                                                    finish();
                                                    android.os.Process.killProcess(android.os.Process.myPid());
                                                }));

                                        return aesDecryptError;
                                    }
                                }
                            }
                        }
                    } catch (JSONException | NullPointerException e) {
                        e.printStackTrace();

                        // 예외 처리
                        activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(ActivitySplash.this,
                                "JSON Parsing Error",
                                "서버의 정보를 얻어오는 데 실패하였습니다. 다시 시도해주십시오. 이 문제가 계속 발생하면 관리자에게 문의하시길 바랍니다.",
                                "종료",
                                false,
                                dialog -> {
                                    moveTaskToBack(true);
                                    finish();
                                    android.os.Process.killProcess(android.os.Process.myPid());
                                }));

                        return jsonError;
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    // 예외 처리
                    activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(ActivitySplash.this,
                            "Unknown Error",
                            "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00001)",
                            "종료",
                            false,
                            dialog -> {
                                moveTaskToBack(true);
                                finish();
                                android.os.Process.killProcess(android.os.Process.myPid());
                            }));

                    return unknownError;
                }
            }

            @SuppressLint("SetJavaScriptEnabled")
            @Override
            protected void onPostExecute(String result) {
                if (result != null && result.equals("error")) {
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
                    webView.setWebViewClient(new WebViewClient() {
                        @Override
                        public void onPageFinished(WebView view, String url) {
                            super.onPageFinished(view, url);
                            webView.setVisibility(View.VISIBLE);
                        }
                    });
                    webView.loadUrl("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/main/rhya_network_block.jsp");


                    return;
                }

                if (ppVersion == null) {
                    rhyaDialogManager.createDialog_Confirm(context,
                            "JSON Parsing Error",
                            "서버의 정보를 얻어오는 데 실패하였습니다. 다시 시도해주십시오. 이 문제가 계속 발생하면 관리자에게 문의하시길 바랍니다. (00014)",
                            "종료",
                            false,
                            dialog -> {
                                moveTaskToBack(true);
                                finish();
                                android.os.Process.killProcess(android.os.Process.myPid());
                            });
                }else if (!rhyaCore.checkPrivacyPolicyVersion(rhyaSharedPreferences, ppVersion, context)) { // 개인정보처리방침 확인
                    // 메시지 출력
                    rhyaDialogManager.createDialog_YesOrNo(context,
                            "개인정보처리방침 동의",
                            "RHYA.Network의 개인정보 처리 방침을 동의하지 않았거나 개인정보 처리 방침이 업데이트되었습니다. 개인정보 처리 방침을 확인해 주세요.",
                            "동의",
                            "처리방침",
                            false,
                            new RhyaDialogManager.DialogListener_YesOrNo() {
                                @Override
                                public void onClickListenerButtonYes(Dialog dialog) {
                                    dialog.dismiss();

                                    rhyaCore.setPrivacyPolicyVersion(rhyaSharedPreferences, ppVersion, context);

                                    assert result != null;
                                    nextTask(result);
                                }

                                @Override
                                public void onClickListenerButtonNo(Dialog dialog) {
                                    Intent mIntent = new Intent(Intent.ACTION_VIEW, Uri.parse("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/main/rhya_network_pp.jsp"));
                                    startActivity(mIntent);
                                }
                            });
                }else {
                    if (result != null && !result.equals("appClose")) {
                        nextTask(result);
                    }
                }
            }
        }.execute(null);
    }


    @Override
    public void onBackPressed() {
        long curTime = System.currentTimeMillis();
        long gapTime = curTime - backBtnTime;

        if(0 <= gapTime && 2000 >= gapTime) {
            // 종료
            finish();
        } else {
            backBtnTime = curTime;
            Toast.makeText(context, "'뒤로' 버튼을 한번 더 누르시면 종료됩니다.", Toast.LENGTH_SHORT).show();
        }
    }


    // 다음 작업
    private void nextTask(String result) {
        switch (result) {
            case "NO_HAVE_AUTH" : { // 로그인 화면 전환
                Intent intent = new Intent(context, ActivityWebView.class);
                startActivity(intent);
                finish();

                overridePendingTransition(R.anim.anim_stay, R.anim.anim_stay);

                break;
            }

            case "HAVE_AUTH" : { // 데이터 로딩 및 메인 화면 전환
                try {
                    if (!new File(rhyaCore.getMusicInfoFile(activity)).exists()) {
                        rhyaDialogManager.createDialog_YesOrNo(context,
                                "리소스 다운로드",
                                temp,
                                "다운로드",
                                "취소",
                                false,
                                new RhyaDialogManager.DialogListener_YesOrNo() {
                                    @Override
                                    public void onClickListenerButtonYes(Dialog dialog) {
                                        dialog.dismiss();

                                        rhyaCore.getUserInfoTask(rhyaSharedPreferences, rhyaDialogManager, rhyaAESManager,  context, activity);
                                    }

                                    @Override
                                    public void onClickListenerButtonNo(Dialog dialog) {
                                        moveTaskToBack(true);
                                        finish();
                                        android.os.Process.killProcess(android.os.Process.myPid());
                                    }
                                });
                    }else {
                        rhyaCore.getUserInfoTask(rhyaSharedPreferences, rhyaDialogManager, rhyaAESManager,  context, activity);
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    // 예외 처리
                    rhyaDialogManager.createDialog_Confirm(context,
                            
                            "Unknown Error",
                            "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00013)",
                            "종료",
                            false,
                            dialog -> {
                                activity.moveTaskToBack(true);
                                activity.finish();
                                android.os.Process.killProcess(android.os.Process.myPid());
                            });
                }

                break;
            }
        }
    }


    @Override
    protected void onResume() {
        super.onResume();

        IntentFilter completeFilter = new IntentFilter(DownloadManager.ACTION_DOWNLOAD_COMPLETE);
        registerReceiver(completeReceiver, completeFilter);
    }

    @Override
    protected void onPause() {
        super.onPause();
        unregisterReceiver(completeReceiver);
    }


    private final BroadcastReceiver completeReceiver = new BroadcastReceiver(){
        @Override
        public void onReceive(Context context, Intent intent) {
            try {
                long reference = intent.getLongExtra(DownloadManager.EXTRA_DOWNLOAD_ID, -1);

                if(enqueue == reference) {
                    DownloadManager.Query query = new DownloadManager.Query();
                    query.setFilterById(reference);
                    Cursor cursor = downloadManager.query(query);

                    cursor.moveToFirst();

                    int columnIndex = cursor.getColumnIndex(DownloadManager.COLUMN_STATUS);
                    int columnReason = cursor.getColumnIndex(DownloadManager.COLUMN_REASON);

                    int status = cursor.getInt(columnIndex);
                    int reason = cursor.getInt(columnReason);

                    cursor.close();

                    switch (status){
                        default:
                            Toast.makeText(context, "다운로드 중 알 수 없는 오류가 발생하였습니다. (00068)", Toast.LENGTH_SHORT).show();

                            if (isActivityAlive()) {
                                finish();
                            }

                            break;

                        case DownloadManager.STATUS_SUCCESSFUL:
                            Toast.makeText(context, "다운로드를 성공했습니다.", Toast.LENGTH_SHORT).show();

                            try {
                                File file = getExternalFilesDir(Environment.DIRECTORY_DOWNLOADS);
                                String get = file.getPath();
                                File fileFinal = new File(get, "utaite_player_update_apk.apk");

                                if (Build.VERSION.SDK_INT >= 24) {
                                    // Android Nougat ( 7.0 ) and later
                                    Uri uri = FileProvider.getUriForFile(context, BuildConfig.APPLICATION_ID + ".fileprovider", fileFinal);
                                    Intent intentSub = new Intent(Intent.ACTION_VIEW);
                                    intentSub.setDataAndType(uri, "application/vnd.android.package-archive");
                                    intentSub.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
                                    context.startActivity(intentSub);
                                }else {
                                    Intent intentSub = new Intent(Intent.ACTION_VIEW);
                                    Uri apkUri = Uri.fromFile(fileFinal);
                                    intentSub.setDataAndType(apkUri, "application/vnd.android.package-archive");
                                    intentSub.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                                    getApplicationContext().startActivity(intentSub);
                                }
                            }catch (Exception ex) {
                                ex.printStackTrace();

                                Toast.makeText(context, "다운로드 중 알 수 없는 오류가 발생하였습니다. (00072)", Toast.LENGTH_SHORT).show();
                            }

                            break;

                        case DownloadManager.STATUS_PAUSED:
                            Toast.makeText(getApplicationContext(), "다운로드가 취소되었습니다. ERROR CODE:" + reason, Toast.LENGTH_SHORT).show();

                            if (isActivityAlive()) {

                                finish();
                            }


                            break;

                        case DownloadManager.STATUS_FAILED:
                            Toast.makeText(getApplicationContext(), "다운로드 중 오류가 발생하였습니다. ERROR CODE:" + reason, Toast.LENGTH_SHORT).show();

                            if (isActivityAlive()) {

                                finish();
                            }

                            break;
                    }
                }
            }catch (Exception ex) {
                ex.printStackTrace();

                Toast.makeText(context, "다운로드 중 알 수 없는 오류가 발생하였습니다. (00067)", Toast.LENGTH_SHORT).show();

                if (isActivityAlive()) {

                    moveTaskToBack(true);
                    finish();
                    android.os.Process.killProcess(android.os.Process.myPid());
                }
            }
        }
    };


    // 업데이트 작업 처리
    public void updateApp() {
        try {
            File file = getExternalFilesDir(Environment.DIRECTORY_DOWNLOADS);
            String get = file.getPath();
            File fileFinal = new File(get, "utaite_player_update_apk.apk");
            if (fileFinal.exists()) //noinspection ResultOfMethodCallIgnored
                fileFinal.delete();

            downloadManager = (DownloadManager) getSystemService(DOWNLOAD_SERVICE);
            DownloadManager.Request request = new DownloadManager.Request(Uri.parse("https://rhya-network.kro.kr/RhyaNetwork/utaite_player_manager?mode=7"));
            request.setTitle("Utaite Player 업데이트");
            request.setDescription("다운로드 중...");
            request.setNotificationVisibility(0);
            request.setVisibleInDownloadsUi(true);
            request.setAllowedNetworkTypes(DownloadManager.Request.NETWORK_WIFI | DownloadManager.Request.NETWORK_MOBILE);
            request.setMimeType("application/vnd.android.package-archive");
            request.setDestinationInExternalFilesDir(context, Environment.DIRECTORY_DOWNLOADS, "utaite_player_update_apk.apk");
            enqueue = downloadManager.enqueue(request);
        }catch (Exception ex) {
            ex.printStackTrace();

            Toast.makeText(context, "다운로드 중 알 수 없는 오류가 발생하였습니다. (00066)", Toast.LENGTH_SHORT).show();
        }
    }


    private boolean isActivityAlive() {
        // Activity null check
        if (activity != null) {
            // Activity 활성화 확인
            return !activity.isFinishing();
        }

        return false;
    }
}
