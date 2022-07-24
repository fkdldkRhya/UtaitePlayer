package kro.kr.rhya_network.utaiteplayer.utils;

import android.app.Activity;
import android.app.Dialog;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.view.Display;
import android.view.KeyEvent;
import android.view.View;
import android.webkit.CookieManager;
import android.webkit.HttpAuthHandler;
import android.webkit.WebView;
import android.webkit.WebViewClient;

import com.pnikosis.materialishprogress.ProgressWheel;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.UnsupportedEncodingException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;
import java.util.HashMap;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.core.RhyaCore;
import kro.kr.rhya_network.utaiteplayer.core.RhyaErrorChecker;

public class RhyaLoginWebViewClient extends WebViewClient {
    // UI Object
    private final ProgressWheel progressWheel;
    // RHYA Utils
    private final RhyaCore rhyaCore;
    private final RhyaDialogManager rhyaDialogManager;
    private final RhyaSharedPreferences rhyaSharedPreferences;
    private final RhyaAESManager rhyaAESManager;
    // Context
    private final Context context;
    // Activity
    private final Activity activity;
    // Task check
    private boolean isTaskConnection;


    public RhyaLoginWebViewClient(ProgressWheel progressWheel,
                                  RhyaCore rhyaCore,
                                  RhyaSharedPreferences rhyaSharedPreferences,
                                  RhyaDialogManager rhyaDialogManager,
                                  RhyaAESManager rhyaAESManager,
                                  Context context,
                                  Activity activity) {
        this.progressWheel = progressWheel;
        this.rhyaCore = rhyaCore;
        this.rhyaSharedPreferences = rhyaSharedPreferences;
        this.rhyaDialogManager = rhyaDialogManager;

        this.rhyaAESManager = rhyaAESManager;

        this.context = context;

        this.activity = activity;

        isTaskConnection = false;
    }

    // 페이지 로딩
    @Override
    public void onPageStarted(WebView view, String url, Bitmap favicon) {
        super.onPageStarted(view, url, favicon);

        // UI 설정
        view.setVisibility(View.INVISIBLE);
        progressWheel.setVisibility(View.VISIBLE);

        // URL 확인
        if (!rhyaCore.isAccessURL(url)) {
            view.stopLoading();
            view.loadUrl(rhyaCore.USER_LOGIN_URL);
        }

        // 로그인 성공 확인
        if (url.equals(rhyaCore.SERVER_CALL_BACK_URL)) {
            // 로그인 성공 - 쿠키 데이터 가져오기
            CookieManager cookieManager = CookieManager.getInstance();
            RhyaCookieFinder rhyaCookieFinder = new RhyaCookieFinder();

            final String cookieResult = cookieManager.getCookie(url);
            final String userUUID = rhyaCookieFinder.findCookie(cookieResult, rhyaCore.COOKIE_NAME_USER_UUID);
            final String tokenUUID = rhyaCookieFinder.findCookie(cookieResult, rhyaCore.COOKIE_NAME_TOKEN_UUID);

            // Cookie 데이터 확인
            if (userUUID == null || tokenUUID == null) {
                // 로그인 성공 - 자동 로그인 등록 실패
                try {
                    rhyaCore.setAutoLogin(rhyaSharedPreferences, rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE, context);
                } catch (NoSuchPaddingException | InvalidKeyException |
                        UnsupportedEncodingException | IllegalBlockSizeException |
                        BadPaddingException | NoSuchAlgorithmException | InvalidAlgorithmParameterException  e) {
                    e.printStackTrace();

                    // 예외 처리
                    rhyaDialogManager.createDialog_Confirm(context,
                            "AES Encryption Error",
                            "데이터를 암호화하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00026)",
                            "종료",
                            false,
                            dialog -> {
                                view.stopLoading();
                                view.loadUrl(rhyaCore.USER_LOGIN_URL);
                            });
                }
            }else {
                rhyaDialogManager.createDialog_Task(context,
                        "작업 처리 중...",
                        false,
                        dialogSub -> onGetAuthTokenTask(view, userUUID, tokenUUID, dialogSub));
            }
        }
    }

    // Auth token 발급
    private void onGetAuthTokenTask(WebView view, String userUUID, String tokenUUID, Dialog dialog) {
        if (!isTaskConnection) {
            isTaskConnection = true;

            RhyaErrorChecker rhyaErrorChecker = new RhyaErrorChecker();
            rhyaErrorChecker.rhyaSharedPreferences = rhyaSharedPreferences;
            rhyaErrorChecker.context = context;

            // 서버 통신 비동기 작업
            new RhyaAsyncTask<String, String>() {
                private int resultInt = -1;

                @Override
                protected void onPreExecute() {

                }

                @Override
                protected String doInBackground(String arg) {
                    try {
                        // HTTPS 통신
                        RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                        ContentValues urlParm = new ContentValues();

                        urlParm.put("user", userUUID);
                        urlParm.put("token", tokenUUID);
                        urlParm.put("name", rhyaCore.SERVICE_APP_NAME);

                        try {
                            JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(rhyaCore.AUTH_TOKEN_GET_URL, urlParm));

                            try {
                                rhyaCore.setAutoLogin(rhyaSharedPreferences, jsonObject.getString("message"), context);

                                RhyaUserDataVO rhyaUserDataVO = rhyaCore.getUserInfo(rhyaCore.getAutoLogin(rhyaSharedPreferences, context), rhyaSharedPreferences, rhyaAESManager, context, activity);

                                if (rhyaUserDataVO != null) {
                                    RhyaApplication.rhyaUserDataVO = rhyaUserDataVO;
                                } else {
                                    if (!rhyaErrorChecker.checkErrorCount()) {
                                        activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                                "Data Decode Error",
                                                "데이터를 디코딩하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00002)",
                                                "종료",
                                                false,
                                                dialog -> {
                                                    activity.moveTaskToBack(true);
                                                    activity.finish();
                                                    android.os.Process.killProcess(android.os.Process.myPid());
                                                }));
                                    }else {
                                        activity.runOnUiThread(() -> rhyaDialogManager.createDialog_YesOrNo(context,
                                                "비정상적인 종료",
                                                "너무 많은 비정상적인 종료를 감지했습니다. 해결 버튼을 누르면 발생한 오류를 해결합니다. (단, 해결 작업은 계정의 모든 데이터를 제거합니다. 신중하게 선택해 주세요.) 현재 발생한 오류: Data Decode Error (00029)",
                                                "해결",
                                                "종료",
                                                false,
                                                new RhyaDialogManager.DialogListener_YesOrNo() {
                                                    @Override
                                                    public void onClickListenerButtonYes(Dialog dialog) {
                                                        dialog.dismiss();
                                                        // 문제 해결 코드
                                                        rhyaCore.resetAllInfo(activity, context, rhyaSharedPreferences, rhyaDialogManager);
                                                    }

                                                    @Override
                                                    public void onClickListenerButtonNo(Dialog dialog) {
                                                        activity.moveTaskToBack(true);
                                                        activity.finish();
                                                        android.os.Process.killProcess(android.os.Process.myPid());
                                                    }
                                                }));
                                    }

                                    return null;
                                }


                                RhyaMusicDataVO rhyaMusicDataVO = rhyaCore.getMetaDataInfo(rhyaCore.getAutoLogin(rhyaSharedPreferences, context), rhyaSharedPreferences, rhyaAESManager, context, activity);

                                if (rhyaMusicDataVO != null) {
                                    RhyaApplication.rhyaMusicDataVO = rhyaMusicDataVO;
                                } else {
                                    if (!rhyaErrorChecker.checkErrorCount()) {
                                        activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                                "Data Decode Error",
                                                "데이터를 디코딩하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00003)",
                                                "종료",
                                                false,
                                                dialog -> {
                                                    activity.moveTaskToBack(true);
                                                    activity.finish();
                                                    android.os.Process.killProcess(android.os.Process.myPid());
                                                }));
                                    }else {
                                        activity.runOnUiThread(() -> rhyaDialogManager.createDialog_YesOrNo(context,
                                                "비정상적인 종료",
                                                "너무 많은 비정상적인 종료를 감지했습니다. 해결 버튼을 누르면 발생한 오류를 해결합니다. (단, 해결 작업은 계정의 모든 데이터를 제거합니다. 신중하게 선택해 주세요.) 현재 발생한 오류: Data Decode Error (00030)",
                                                "해결",
                                                "종료",
                                                false,
                                                new RhyaDialogManager.DialogListener_YesOrNo() {
                                                    @Override
                                                    public void onClickListenerButtonYes(Dialog dialog) {
                                                        dialog.dismiss();
                                                        // 문제 해결 코드
                                                        rhyaCore.resetAllInfo(activity, context, rhyaSharedPreferences, rhyaDialogManager);
                                                    }

                                                    @Override
                                                    public void onClickListenerButtonNo(Dialog dialog) {
                                                        activity.moveTaskToBack(true);
                                                        activity.finish();
                                                        android.os.Process.killProcess(android.os.Process.myPid());
                                                    }
                                                }));
                                    }

                                    return null;
                                }

                                // 현재 재생 목록 읽기
                                try {
                                    ArrayList<Object> returnResult = rhyaCore.loadNowPlayList(rhyaAESManager, activity);
                                    if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList() == null) {
                                        //noinspection unchecked
                                        RhyaApplication.setRhyaNowPlayMusicUUIDArrayList((ArrayList<String>) returnResult.get(0));
                                        resultInt = (int) returnResult.get(1);
                                    }else {
                                        RhyaApplication.clearRhyaNowPlayMusicUUIDArrayList();
                                    }
                                }catch (Exception ex) {
                                    ex.printStackTrace();

                                    resultInt = -1;
                                }

                                if (!rhyaCore.checkMusicInfoFile(activity)) {
                                    String message = rhyaCore.getDownloadSize();
                                    activity.runOnUiThread(() -> rhyaDialogManager.createDialog_YesOrNo(context,
                                            "리소스 다운로드",
                                            message,
                                            "다운로드",
                                            "취소",
                                            false,
                                            new RhyaDialogManager.DialogListener_YesOrNo() {
                                                @Override
                                                public void onClickListenerButtonYes(Dialog dialog) {
                                                    new RhyaAsyncTask<String, String>() {

                                                        @Override
                                                        protected void onPreExecute() {

                                                        }

                                                        @Override
                                                        protected String doInBackground(String arg) {
                                                            try {
                                                                if (!rhyaCore.musicInfoReload(rhyaAESManager, rhyaSharedPreferences, context, activity, rhyaCore.getAutoLogin(rhyaSharedPreferences, context))) {
                                                                    if (!rhyaErrorChecker.checkErrorCount()) {
                                                                        activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                                                                "Data Decode Error",
                                                                                "데이터를 디코딩하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00011)",
                                                                                "종료",
                                                                                false,
                                                                                dialog -> {
                                                                                    activity.moveTaskToBack(true);
                                                                                    activity.finish();
                                                                                    android.os.Process.killProcess(android.os.Process.myPid());
                                                                                }));
                                                                    }else {
                                                                        activity.runOnUiThread(() -> rhyaDialogManager.createDialog_YesOrNo(context,
                                                                                "비정상적인 종료",
                                                                                "너무 많은 비정상적인 종료를 감지했습니다. 해결 버튼을 누르면 발생한 오류를 해결합니다. (단, 해결 작업은 계정의 모든 데이터를 제거합니다. 신중하게 선택해 주세요.) 현재 발생한 오류: Data Decode Error (00031)",
                                                                                "해결",
                                                                                "종료",
                                                                                false,
                                                                                new RhyaDialogManager.DialogListener_YesOrNo() {
                                                                                    @Override
                                                                                    public void onClickListenerButtonYes(Dialog dialog) {
                                                                                        dialog.dismiss();
                                                                                        // 문제 해결 코드
                                                                                        rhyaCore.resetAllInfo(activity, context, rhyaSharedPreferences, rhyaDialogManager);
                                                                                    }

                                                                                    @Override
                                                                                    public void onClickListenerButtonNo(Dialog dialog) {
                                                                                        activity.moveTaskToBack(true);
                                                                                        activity.finish();
                                                                                        android.os.Process.killProcess(android.os.Process.myPid());
                                                                                    }
                                                                                }));
                                                                    }

                                                                    return null;
                                                                }

                                                                rhyaCore.InitSingerInfo();

                                                                HashMap<String, RhyaSingerDataVO> stringRhyaSingerDataVOHashMap = new HashMap<>();
                                                                for (String uuid : RhyaApplication.rhyaMusicInfoVOHashMap.keySet()) {
                                                                    RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);

                                                                    assert rhyaMusicInfoVO != null;
                                                                    RhyaSingerDataVO rhyaSingerDataVO = new RhyaSingerDataVO(rhyaMusicInfoVO.getSingerUuid(), rhyaMusicInfoVO.getSinger(), rhyaMusicInfoVO.getSingerImage());
                                                                    if (!stringRhyaSingerDataVOHashMap.containsKey(rhyaMusicInfoVO.getSingerUuid())) {
                                                                        stringRhyaSingerDataVOHashMap.put(rhyaMusicInfoVO.getSingerUuid(), rhyaSingerDataVO);
                                                                    }
                                                                }
                                                                RhyaApplication.stringRhyaSingerDataVOHashMap = stringRhyaSingerDataVOHashMap;

                                                                return "success";
                                                            }catch (Exception ex) {
                                                                ex.printStackTrace();

                                                                // 예외 처리
                                                                if (!rhyaErrorChecker.checkErrorCount()) {
                                                                    activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                                                            "Unknown Error",
                                                                            "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00012)",
                                                                            "종료",
                                                                            false,
                                                                            dialog -> {
                                                                                activity.moveTaskToBack(true);
                                                                                activity.finish();
                                                                                android.os.Process.killProcess(android.os.Process.myPid());
                                                                            }));
                                                                }else {
                                                                    activity.runOnUiThread(() -> rhyaDialogManager.createDialog_YesOrNo(context,
                                                                            "비정상적인 종료",
                                                                            "너무 많은 비정상적인 종료를 감지했습니다. 해결 버튼을 누르면 발생한 오류를 해결합니다. (단, 해결 작업은 계정의 모든 데이터를 제거합니다. 신중하게 선택해 주세요.) 현재 발생한 오류: Unknown Error (00032)",
                                                                            "해결",
                                                                            "종료",
                                                                            false,
                                                                            new RhyaDialogManager.DialogListener_YesOrNo() {
                                                                                @Override
                                                                                public void onClickListenerButtonYes(Dialog dialog) {
                                                                                    dialog.dismiss();
                                                                                    // 문제 해결 코드
                                                                                    rhyaCore.resetAllInfo(activity, context, rhyaSharedPreferences, rhyaDialogManager);
                                                                                }

                                                                                @Override
                                                                                public void onClickListenerButtonNo(Dialog dialog) {
                                                                                    activity.moveTaskToBack(true);
                                                                                    activity.finish();
                                                                                    android.os.Process.killProcess(android.os.Process.myPid());
                                                                                }
                                                                            }));
                                                                }

                                                                return null;
                                                            }
                                                        }

                                                        @Override
                                                        protected void onPostExecute(String result) {
                                                            dialog.dismiss();
                                                            isTaskConnection = false;

                                                            if (result != null) {
                                                                if (result.equals("success")) {
                                                                    Intent intent = new Intent(context, ActivityMain.class);
                                                                    intent.putExtra("nowPlayIndex", resultInt);
                                                                    activity.startActivity(intent);
                                                                    activity.finish();

                                                                    activity.overridePendingTransition(R.anim.anim_fadein, R.anim.anim_fadeout);
                                                                }
                                                            }
                                                        }
                                                    }.execute(null);
                                                }

                                                @Override
                                                public void onClickListenerButtonNo(Dialog dialog) {
                                                    activity.moveTaskToBack(true);
                                                    activity.finish();
                                                    android.os.Process.killProcess(android.os.Process.myPid());
                                                }
                                            }));

                                    return null;
                                }else {
                                    if (!rhyaCore.musicInfoReload(rhyaAESManager, rhyaSharedPreferences, context, activity, rhyaCore.getAutoLogin(rhyaSharedPreferences, context))) {
                                        if (!rhyaErrorChecker.checkErrorCount()) {
                                            activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                                    "Data Loading Error",
                                                    "데이터를 로딩하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00004)",
                                                    "종료",
                                                    false,
                                                    dialog -> {
                                                        activity.moveTaskToBack(true);
                                                        activity.finish();
                                                        android.os.Process.killProcess(android.os.Process.myPid());
                                                    }));
                                        }else {
                                            activity.runOnUiThread(() -> rhyaDialogManager.createDialog_YesOrNo(context,
                                                    "비정상적인 종료",
                                                    "너무 많은 비정상적인 종료를 감지했습니다. 해결 버튼을 누르면 발생한 오류를 해결합니다. (단, 해결 작업은 계정의 모든 데이터를 제거합니다. 신중하게 선택해 주세요.) 현재 발생한 오류: Data Loading Error (00033)",
                                                    "해결",
                                                    "종료",
                                                    false,
                                                    new RhyaDialogManager.DialogListener_YesOrNo() {
                                                        @Override
                                                        public void onClickListenerButtonYes(Dialog dialog) {
                                                            dialog.dismiss();
                                                            // 문제 해결 코드
                                                            rhyaCore.resetAllInfo(activity, context, rhyaSharedPreferences, rhyaDialogManager);
                                                        }

                                                        @Override
                                                        public void onClickListenerButtonNo(Dialog dialog) {
                                                            activity.moveTaskToBack(true);
                                                            activity.finish();
                                                            android.os.Process.killProcess(android.os.Process.myPid());
                                                        }
                                                    }));
                                        }
                                        return null;
                                    }
                                }

                                HashMap<String, RhyaSingerDataVO> stringRhyaSingerDataVOHashMap = new HashMap<>();
                                for (String uuid : RhyaApplication.rhyaMusicInfoVOHashMap.keySet()) {
                                    RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);

                                    assert rhyaMusicInfoVO != null;
                                    RhyaSingerDataVO rhyaSingerDataVO = new RhyaSingerDataVO(rhyaMusicInfoVO.getSingerUuid(), rhyaMusicInfoVO.getSinger(), rhyaMusicInfoVO.getSingerImage());
                                    if (!stringRhyaSingerDataVOHashMap.containsKey(rhyaMusicInfoVO.getSingerUuid())) {
                                        stringRhyaSingerDataVOHashMap.put(rhyaMusicInfoVO.getSingerUuid(), rhyaSingerDataVO);
                                    }
                                }
                                RhyaApplication.stringRhyaSingerDataVOHashMap = stringRhyaSingerDataVOHashMap;

                                rhyaCore.InitSingerInfo();

                                return "success";
                            } catch (NoSuchPaddingException | InvalidKeyException |
                                    UnsupportedEncodingException | IllegalBlockSizeException |
                                    BadPaddingException | NoSuchAlgorithmException | InvalidAlgorithmParameterException  e) {
                                e.printStackTrace();

                                // 예외 처리
                                activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                        "AES Encryption Error",
                                        "데이터를 암호화하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00025)",
                                        "확인",
                                        false,
                                        dialog -> {
                                            view.stopLoading();
                                            view.loadUrl(rhyaCore.USER_LOGIN_URL);
                                        }));
                            }
                        }catch (JSONException e) {
                            e.printStackTrace();

                            // 예외 처리
                            if (!rhyaErrorChecker.checkErrorCount()) {
                                activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                        "JSON Parsing Error",
                                        "서버의 정보를 얻어오는 데 실패하였습니다. 다시 시도해주십시오. 이 문제가 계속 발생하면 관리자에게 문의하시길 바랍니다. (00024)",
                                        "종료",
                                        false,
                                        dialog -> {
                                            activity.moveTaskToBack(true);
                                            activity.finish();
                                            android.os.Process.killProcess(android.os.Process.myPid());
                                        }));
                            }else {
                                activity.runOnUiThread(() -> rhyaDialogManager.createDialog_YesOrNo(context,
                                        "비정상적인 종료",
                                        "너무 많은 비정상적인 종료를 감지했습니다. 해결 버튼을 누르면 발생한 오류를 해결합니다. (단, 해결 작업은 계정의 모든 데이터를 제거합니다. 신중하게 선택해 주세요.) 현재 발생한 오류: JSON Parsing Error (00034)",
                                        "해결",
                                        "종료",
                                        false,
                                        new RhyaDialogManager.DialogListener_YesOrNo() {
                                            @Override
                                            public void onClickListenerButtonYes(Dialog dialog) {
                                                dialog.dismiss();
                                                // 문제 해결 코드
                                                rhyaCore.resetAllInfo(activity, context, rhyaSharedPreferences, rhyaDialogManager);
                                            }

                                            @Override
                                            public void onClickListenerButtonNo(Dialog dialog) {
                                                activity.moveTaskToBack(true);
                                                activity.finish();
                                                android.os.Process.killProcess(android.os.Process.myPid());
                                            }
                                        }));
                            }
                        }
                    }catch (Exception ex) {
                        ex.printStackTrace();

                        // 예외 처리
                        if (!rhyaErrorChecker.checkErrorCount()) {
                            activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                    "Unknown Error",
                                    "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00005)",
                                    "종료",
                                    false,
                                    dialog -> {
                                        activity.moveTaskToBack(true);
                                        activity.finish();
                                        android.os.Process.killProcess(android.os.Process.myPid());
                                    }));
                        }else {
                            activity.runOnUiThread(() -> rhyaDialogManager.createDialog_YesOrNo(context,
                                    "비정상적인 종료",
                                    "너무 많은 비정상적인 종료를 감지했습니다. 해결 버튼을 누르면 발생한 오류를 해결합니다. (단, 해결 작업은 계정의 모든 데이터를 제거합니다. 신중하게 선택해 주세요.) 현재 발생한 오류: Unknown Error (00035)",
                                    "해결",
                                    "종료",
                                    false,
                                    new RhyaDialogManager.DialogListener_YesOrNo() {
                                        @Override
                                        public void onClickListenerButtonYes(Dialog dialog) {
                                            dialog.dismiss();
                                            // 문제 해결 코드
                                            rhyaCore.resetAllInfo(activity, context, rhyaSharedPreferences, rhyaDialogManager);
                                        }

                                        @Override
                                        public void onClickListenerButtonNo(Dialog dialog) {
                                            activity.moveTaskToBack(true);
                                            activity.finish();
                                            android.os.Process.killProcess(android.os.Process.myPid());
                                        }
                                    }));
                        }
                    }

                    return null;
                }


                @Override
                protected void onPostExecute(String result) {
                    dialog.dismiss();
                    isTaskConnection = false;

                    if (result != null) {
                        if (result.equals("success")) {
                            rhyaErrorChecker.clearErrorCount();

                            Intent intent = new Intent(context, ActivityMain.class);
                            intent.setAction(Intent.ACTION_MAIN);
                            intent.addCategory(Intent.CATEGORY_LAUNCHER);
                            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                            intent.putExtra("nowPlayIndex", resultInt);
                            activity.startActivity(intent);
                            activity.finish();

                            activity.overridePendingTransition(R.anim.anim_fadein, R.anim.anim_fadeout);
                        }
                    }
                }
            }.execute(null);
        }
    }

    // 리소스를 로드하는 중 여러번 호출
    @Override
    public void onLoadResource(WebView view, String url) {
        super.onLoadResource(view, url);
    }

    // 방문 내역을 히스토리에 업데이트 할 때
    @Override
    public void doUpdateVisitedHistory(WebView view, String url, boolean isReload) {
        super.doUpdateVisitedHistory(view, url, isReload);
    }

    // 로딩이 완료됬을 때 한번 호출
    @Override
    public void onPageFinished(WebView view, String url) {
        super.onPageFinished(view, url);

        // UI 설정
        view.setVisibility(View.VISIBLE);
        progressWheel.setVisibility(View.INVISIBLE);
    }

    // 오류가 났을 경우, 오류는 복수할 수 없음
    @Override
    public void onReceivedError(WebView view, int errorCode, String description, String failingUrl) {
        super.onReceivedError(view, errorCode, description, failingUrl);

        switch (errorCode) {
            case ERROR_AUTHENTICATION:
                break;               // 서버에서 사용자 인증 실패
            case ERROR_BAD_URL:
                break;                           // 잘못된 URL
            case ERROR_CONNECT:
                break;                          // 서버로 연결 실패
            case ERROR_FAILED_SSL_HANDSHAKE:
                break;    // SSL handshake 수행 실패
            case ERROR_FILE:
                break;                                  // 일반 파일 오류
            case ERROR_FILE_NOT_FOUND:
                break;               // 파일을 찾을 수 없습니다
            case ERROR_HOST_LOOKUP:
                break;           // 서버 또는 프록시 호스트 이름 조회 실패
            case ERROR_IO:
                break;                              // 서버에서 읽거나 서버로 쓰기 실패
            case ERROR_PROXY_AUTHENTICATION:
                break;   // 프록시에서 사용자 인증 실패
            case ERROR_REDIRECT_LOOP:
                break;               // 너무 많은 리디렉션
            case ERROR_TIMEOUT:
                break;                          // 연결 시간 초과
            case ERROR_TOO_MANY_REQUESTS:
                break;     // 페이지 로드중 너무 많은 요청 발생
            case ERROR_UNKNOWN:
                break;                        // 일반 오류
            case ERROR_UNSUPPORTED_AUTH_SCHEME:
                break; // 지원되지 않는 인증 체계
            case ERROR_UNSUPPORTED_SCHEME:
                break;          // URI가 지원되지 않는 방식
        }

    }

    // http 인증 요청이 있는 경우, 기본 동작은 요청 취소
    @Override
    public void onReceivedHttpAuthRequest(WebView view, HttpAuthHandler handler, String host, String realm) {
        super.onReceivedHttpAuthRequest(view, handler, host, realm);
    }

    // 확대나 크기 등의 변화가 있는 경우
    @Override
    public void onScaleChanged(WebView view, float oldScale, float newScale) {
        super.onScaleChanged(view, oldScale, newScale);
    }

    // 잘못된 키 입력이 있는 경우
    @Override
    public boolean shouldOverrideKeyEvent(WebView view, KeyEvent event) {
        return super.shouldOverrideKeyEvent(view, event);
    }

    // 새로운 URL이 webview에 로드되려 할 경우 컨트롤을 대신할 기회를 줌
    @Override
    public boolean shouldOverrideUrlLoading(WebView view, String url) {
        view.loadUrl(url);
        return true;
    }
}
