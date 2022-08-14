package kro.kr.rhya_network.utaiteplayer.core;

import android.app.Activity;
import android.app.Dialog;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileOutputStream;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.URL;
import java.net.URLConnection;
import java.net.URLDecoder;
import java.net.URLEncoder;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Locale;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAESManager;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaDialogManager;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHTMLNoComment;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHttpConnection;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHttpsConnection;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicDataVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSharedPreferences;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSingerDataVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaUserDataVO;

public class RhyaCore {
    // CallBack URL
    public final String SERVER_CALL_BACK_URL = "https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/callback.jsp";
    // 로그인 URL
    public final String USER_LOGIN_URL = "https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/sign_in.jsp?rpid=10&ctoken=1";
    // Auth token 발급 URL
    public final String AUTH_TOKEN_GET_URL = "https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/auth_token.jsp";
    // Main URL
    public final String MAIN_URL = "https://rhya-network.kro.kr/RhyaNetwork/utaite_player_manager";
    // Cookie 데이터
    public final String COOKIE_NAME_USER_UUID = "AutoLogin_UserUUID";
    public final String COOKIE_NAME_TOKEN_UUID = "AutoLogin_TokenUUID";
    // 서비스 앱 이름
    public final String SERVICE_APP_NAME = "kro_kr_rhya__network_jp__player";



    // 서버 정보 가져오기
    public String[] getServerInfo() {
        RhyaHttpConnection rhyaHttpConnection = new RhyaHttpConnection();
        RhyaHTMLNoComment rhyaHTMLNoComment = new RhyaHTMLNoComment();

        try {
            String result = rhyaHTMLNoComment.getNoComment(rhyaHttpConnection.request("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/server_info.jsp", null));
            JSONObject jsonObject = new JSONObject(result);

            String server = jsonObject.getString("server");
            String privacy_policy_version = jsonObject.getString("privacy_policy_version");

            String result2 = rhyaHTMLNoComment.getNoComment(rhyaHttpConnection.request("https://rhya-network.kro.kr/RhyaNetwork/utaite_player_manager?mode=6", null));
            JSONObject jsonObject2 = new JSONObject(result2);

            String version = jsonObject2.getString("version");

            return new String[] {
                    server,
                    version,
                    privacy_policy_version
            };
        }catch (Exception ex) {
            ex.printStackTrace();

            return null;
        }
    }


    // 사용자 정보 수정 URL
    public String getEditUserURL(RhyaSharedPreferences rhyaSharedPreferences, Context context) throws IOException, NoSuchPaddingException, InvalidKeyException, NoSuchAlgorithmException, IllegalBlockSizeException, BadPaddingException, InvalidAlgorithmParameterException {
        StringBuilder sb;
        sb = new StringBuilder();
        sb.append("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/edit_my_account.jsp?rpid=10&isNoRed=1&backurl=1&auth=");
        sb.append(getAutoLogin(rhyaSharedPreferences, context));

        return sb.toString();
    }


    // 공지사항 URL
    public String getAnnouncementURL(RhyaSharedPreferences rhyaSharedPreferences, Context context) throws IOException, NoSuchPaddingException, InvalidKeyException, NoSuchAlgorithmException, IllegalBlockSizeException, BadPaddingException, InvalidAlgorithmParameterException {
        StringBuilder sb;
        sb = new StringBuilder();
        sb.append("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/main/rhya_network_announcement.jsp?authToken=");
        sb.append(getAutoLogin(rhyaSharedPreferences, context));

        return sb.toString();
    }


    // Auth token 확인
    public boolean isOKAuthToken(String authToken) throws JSONException {
        RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
        ContentValues urlParm = new ContentValues();

        urlParm.put("token", authToken);
        urlParm.put("name", SERVICE_APP_NAME);

        JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/auth_token_checker.jsp", urlParm));

        return jsonObject.getString("result").equals("success");
    }


    // RHYA.Network 도메인 확인
    public boolean isAccessURL(String url) {
        return url.contains("https://rhya-network.kro.kr/");
    }


    // 개인정보처리방침 확인
    public boolean checkPrivacyPolicyVersion(RhyaSharedPreferences rhyaSharedPreferences, String version, Context context) {
        String result = rhyaSharedPreferences.getStringNoAES("PRIVACY_POLICY_VERSION", context);
        if (result.equals(rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE)) {
            return false;
        }

        // 버전 확인
        int intVersionInput = Integer.parseInt(version.replace(".", ""));
        int intVersionGet = Integer.parseInt(result.replace(".", ""));

        return intVersionInput == intVersionGet;
    }
    // 개인정보처리방침 설정
    public void setPrivacyPolicyVersion(RhyaSharedPreferences rhyaSharedPreferences, String version, Context context) {
        final String keyName = "PRIVACY_POLICY_VERSION";
        rhyaSharedPreferences.setStringNoAES(keyName, version, context);
    }


    // 자동 로그인 설정 저장 함수
    public void setAutoLogin(RhyaSharedPreferences rhyaSharedPreferences, String value, Context context) throws NoSuchPaddingException,
            InvalidAlgorithmParameterException,
            UnsupportedEncodingException,
            IllegalBlockSizeException,
            BadPaddingException,
            NoSuchAlgorithmException,
            InvalidKeyException {
        rhyaSharedPreferences.setString(rhyaSharedPreferences.SHARED_PREFERENCES_AUTH_TOKEN, value, context);
    }


    // 자동 로그인 인증 토큰 불러오기
    public String getAutoLogin(RhyaSharedPreferences rhyaSharedPreferences, Context context) throws NoSuchPaddingException,
            InvalidKeyException,
            UnsupportedEncodingException,
            IllegalBlockSizeException,
            BadPaddingException,
            NoSuchAlgorithmException,
            InvalidAlgorithmParameterException {
        return rhyaSharedPreferences.getString(rhyaSharedPreferences.SHARED_PREFERENCES_AUTH_TOKEN, context);
    }


    // 사용자 데이터 불러오기
    public RhyaUserDataVO getUserInfo(String authToken, RhyaSharedPreferences rhyaSharedPreferences, RhyaAESManager rhyaAESManager, Context context, Activity activity) throws JSONException, NoSuchPaddingException, InvalidKeyException, UnsupportedEncodingException, IllegalBlockSizeException, BadPaddingException, NoSuchAlgorithmException, InvalidAlgorithmParameterException {
        final String uuid = "uuid";
        final String name = "name";
        final String birthday = "birthday";
        final String regdate = "regdate";
        final String id = "id";
        final String email = "email";

        StringBuilder sb = new StringBuilder();
        sb.append(activity.getFilesDir().getAbsolutePath());
        sb.append(File.separator);
        sb.append("user_info.rhya");

        RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
        ContentValues urlParm = new ContentValues();

        urlParm.put("token", authToken);
        urlParm.put("name", SERVICE_APP_NAME);

        JSONObject jsonObject;

        if (isNoChangeUserData(rhyaSharedPreferences, context)) {
            // 로컬 데이터 로딩
            File file = new File(sb.toString());
            if (file.exists()) {
                try {
                    jsonObject = new JSONObject(rhyaAESManager.aesDecode(readFile(sb.toString())));

                    return new RhyaUserDataVO(jsonObject.getString(uuid),
                            jsonObject.getString(name),
                            jsonObject.getString(birthday),
                            jsonObject.getString(regdate),
                            jsonObject.getString(id),
                            jsonObject.getString(email));
                } catch (Exception e) {
                    e.printStackTrace();

                    if (file.delete()) {
                        return null;
                    }

                    return null;
                }
            }
        }

        rhyaSharedPreferences.setStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_USER_DATA_CHANGE, rhyaSharedPreferences.SHARED_PREFERENCES_USER_DATA_CHANGE_DATA, context);
        // 서버 접속
        try {
            jsonObject = new JSONObject(URLDecoder.decode(rhyaHttpsConnection.request("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/auth_info.jsp", urlParm), "UTF-8"));
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();

            return null;
        }

        // 파일 생성
        try {
            writeFile(sb.toString(), rhyaAESManager.aesEncode(jsonObject.toString()));
        } catch (IOException e) {
            e.printStackTrace();

            return null;
        }

        return new RhyaUserDataVO(jsonObject.getString(uuid),
                jsonObject.getString(name),
                jsonObject.getString(birthday),
                jsonObject.getString(regdate),
                jsonObject.getString(id),
                jsonObject.getString(email));
    }
    public RhyaMusicDataVO getMetaDataInfo(String authToken, RhyaSharedPreferences rhyaSharedPreferences, RhyaAESManager rhyaAESManager, Context context, Activity activity)  throws JSONException, NoSuchPaddingException, InvalidKeyException, UnsupportedEncodingException, IllegalBlockSizeException, BadPaddingException, NoSuchAlgorithmException, InvalidAlgorithmParameterException {
        StringBuilder sb = new StringBuilder();
        sb.append(activity.getFilesDir().getAbsolutePath());
        sb.append(File.separator);
        sb.append("user_metadata.rhya");

        RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
        ContentValues urlParm = new ContentValues();

        urlParm.put("mode", "0");
        urlParm.put("auth", authToken);
        urlParm.put("name", SERVICE_APP_NAME);

        JSONObject jsonObject;

        try {
            if (isNoChangeUserMetaData(rhyaSharedPreferences, context)) {
                // 로컬 데이터 로딩
                File file = new File(sb.toString());
                if (new File(sb.toString()).exists()) {
                    try {
                        jsonObject = new JSONObject(rhyaAESManager.aesDecode(readFile(sb.toString())));
                    } catch (IOException e) {
                        e.printStackTrace();

                        if (file.delete()) {
                            return null;
                        }

                        return null;
                    }
                }else {
                    jsonObject = new JSONObject(rhyaHttpsConnection.request(MAIN_URL, urlParm));
                }
            }else {
                jsonObject = new JSONObject(rhyaHttpsConnection.request(MAIN_URL, urlParm));
            }

            rhyaSharedPreferences.setStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_USER_METADATA_CHANGE, rhyaSharedPreferences.SHARED_PREFERENCES_USER_DATA_CHANGE_DATA, context);

            // 실패 여부 확인
            String result = jsonObject.getString("result");
            if (result.equals("success")) {
                String subscribeList = URLDecoder.decode(jsonObject.getString("subscribe_list"), "UTF-8");

                // 구독 리스트
                JSONArray subscribeListJsonArray = new JSONArray(new JSONObject(subscribeList).getString("list"));
                ArrayList<RhyaSingerDataVO> rhyaSingerDataVOS = new ArrayList<>();
                for (int i = 0; i < subscribeListJsonArray.length(); i++) {
                    rhyaSingerDataVOS.add(new RhyaSingerDataVO(subscribeListJsonArray.getString(i), null, null));
                }

                // 파일 생성
                try {
                    writeFile(sb.toString(),  rhyaAESManager.aesEncode(jsonObject.toString()));
                } catch (IOException e) {
                    e.printStackTrace();

                    return null;
                }

                return new RhyaMusicDataVO(rhyaSingerDataVOS);
            }

            return null;
        } catch (JSONException | UnsupportedEncodingException e) {
            e.printStackTrace();

            return null;
        }
    }

    @SuppressWarnings("unchecked")
    public void getUserInfoTask(RhyaSharedPreferences rhyaSharedPreferences, RhyaDialogManager rhyaDialogManager, RhyaAESManager rhyaAESManager, Context context, Activity activity) {
        RhyaErrorChecker rhyaErrorChecker = new RhyaErrorChecker();
        rhyaErrorChecker.rhyaSharedPreferences = rhyaSharedPreferences;
        rhyaErrorChecker.context = context;

        // 로그인 성공 - App 전용 자동 로그인 토큰 발급
        rhyaDialogManager.createDialog_Task(context,
                "Initializing...",
                false,
                dialog -> new RhyaAsyncTask<String, String>() {
                    private int resultInt = -1;

                    @Override
                    protected void onPreExecute() {
                    }

                    @Override
                    protected String doInBackground(String arg) {
                        try {
                            try {
                                try {
                                    String authToken = getAutoLogin(rhyaSharedPreferences, context);


                                    RhyaUserDataVO rhyaUserDataVO = getUserInfo(authToken, rhyaSharedPreferences, rhyaAESManager, context, activity);

                                    if (rhyaUserDataVO != null) {
                                        RhyaApplication.rhyaUserDataVO = rhyaUserDataVO;
                                    }else {
                                        if (!rhyaErrorChecker.checkErrorCount()) {
                                            activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                                    "Data Decode Error",
                                                    "데이터를 디코딩하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00023)",
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
                                                    "너무 많은 비정상적인 종료를 감지했습니다. 해결 버튼을 누르면 발생한 오류를 해결합니다. (단, 해결 작업은 계정의 모든 데이터를 제거합니다. 신중하게 선택해 주세요.) 현재 발생한 오류: Data Decode Error (00024)",
                                                    "해결",
                                                    "종료",
                                                    false,
                                                    new RhyaDialogManager.DialogListener_YesOrNo() {
                                                        @Override
                                                        public void onClickListenerButtonYes(Dialog dialog) {
                                                            dialog.dismiss();
                                                            // 문제 해결 코드
                                                            resetAllInfo(activity, context, rhyaSharedPreferences, rhyaDialogManager);
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


                                    RhyaMusicDataVO rhyaMusicDataVO = getMetaDataInfo(authToken, rhyaSharedPreferences, rhyaAESManager, context, activity);

                                    if (rhyaMusicDataVO != null) {
                                        RhyaApplication.rhyaMusicDataVO = rhyaMusicDataVO;
                                    } else {
                                        if (!rhyaErrorChecker.checkErrorCount()) {
                                            activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                                    "Data Decode Error",
                                                    "데이터를 디코딩하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00022)",
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
                                                    "너무 많은 비정상적인 종료를 감지했습니다. 해결 버튼을 누르면 발생한 오류를 해결합니다. (단, 해결 작업은 계정의 모든 데이터를 제거합니다. 신중하게 선택해 주세요.) 현재 발생한 오류: Data Decode Error (00025)",
                                                    "해결",
                                                    "종료",
                                                    false,
                                                    new RhyaDialogManager.DialogListener_YesOrNo() {
                                                        @Override
                                                        public void onClickListenerButtonYes(Dialog dialog) {
                                                            dialog.dismiss();
                                                            // 문제 해결 코드
                                                            resetAllInfo(activity, context, rhyaSharedPreferences, rhyaDialogManager);
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


                                    // 다운로드 확인
                                    if (!musicInfoReload(rhyaAESManager, rhyaSharedPreferences, context, activity, authToken)) {
                                        if (!rhyaErrorChecker.checkErrorCount()) {
                                            activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                                    "Data Decode Error",
                                                    "데이터를 디코딩하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00021)",
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
                                                    "너무 많은 비정상적인 종료를 감지했습니다. 해결 버튼을 누르면 발생한 오류를 해결합니다. (단, 해결 작업은 계정의 모든 데이터를 제거합니다. 신중하게 선택해 주세요.) 현재 발생한 오류: Data Decode Error (00026)",
                                                    "해결",
                                                    "종료",
                                                    false,
                                                    new RhyaDialogManager.DialogListener_YesOrNo() {
                                                        @Override
                                                        public void onClickListenerButtonYes(Dialog dialog) {
                                                            dialog.dismiss();
                                                            // 문제 해결 코드
                                                            resetAllInfo(activity, context, rhyaSharedPreferences, rhyaDialogManager);
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


                                    // 현재 재생 목록 읽기
                                    try {
                                        ArrayList<Object> returnResult = loadNowPlayList(rhyaAESManager, activity);
                                        if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() == 0) {
                                            RhyaApplication.setRhyaNowPlayMusicUUIDArrayList((ArrayList<String>) returnResult.get(0));
                                            resultInt = (int) returnResult.get(1);
                                        }
                                    }catch (Exception ex) {
                                        ex.printStackTrace();

                                        resultInt = -1;
                                    }

                                    InitSingerInfo();

                                    return "success";
                                } catch (NoSuchPaddingException | InvalidKeyException |
                                        UnsupportedEncodingException | IllegalBlockSizeException |
                                        BadPaddingException | NoSuchAlgorithmException | InvalidAlgorithmParameterException e) {
                                    e.printStackTrace();

                                    // 예외 처리
                                    activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                            "AES Manager Error",
                                            "데이터를 암호화/복호화하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00020)",
                                            "종료",
                                            false,
                                            dialog -> {
                                                activity.moveTaskToBack(true);
                                                activity.finish();
                                                android.os.Process.killProcess(android.os.Process.myPid());
                                            }));
                                }
                            } catch (JSONException e) {
                                e.printStackTrace();

                                // 예외 처리
                                if (!rhyaErrorChecker.checkErrorCount()) {
                                    activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                            "JSON Parsing Error",
                                            "서버의 정보를 얻어오는 데 실패하였습니다. 다시 시도해주십시오. 이 문제가 계속 발생하면 관리자에게 문의하시길 바랍니다. (00019)",
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
                                            "너무 많은 비정상적인 종료를 감지했습니다. 해결 버튼을 누르면 발생한 오류를 해결합니다. (단, 해결 작업은 계정의 모든 데이터를 제거합니다. 신중하게 선택해 주세요.) 현재 발생한 오류: JSON Parsing Error (00027)",
                                            "해결",
                                            "종료",
                                            false,
                                            new RhyaDialogManager.DialogListener_YesOrNo() {
                                                @Override
                                                public void onClickListenerButtonYes(Dialog dialog) {
                                                    dialog.dismiss();
                                                    // 문제 해결 코드
                                                    resetAllInfo(activity, context, rhyaSharedPreferences, rhyaDialogManager);
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
                        } catch (Exception ex) {
                            ex.printStackTrace();

                            // 예외 처리
                            if (!rhyaErrorChecker.checkErrorCount()) {
                                activity.runOnUiThread(() -> rhyaDialogManager.createDialog_Confirm(context,
                                        "Unknown Error",
                                        "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00018)",
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
                                        "너무 많은 비정상적인 종료를 감지했습니다. 해결 버튼을 누르면 발생한 오류를 해결합니다. (단, 해결 작업은 계정의 모든 데이터를 제거합니다. 신중하게 선택해 주세요.) 현재 발생한 오류: Unknown Error (00028)",
                                        "해결",
                                        "종료",
                                        false,
                                        new RhyaDialogManager.DialogListener_YesOrNo() {
                                            @Override
                                            public void onClickListenerButtonYes(Dialog dialog) {
                                                dialog.dismiss();
                                                // 문제 해결 코드
                                                resetAllInfo(activity, context, rhyaSharedPreferences, rhyaDialogManager);
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

                        if (result != null) {
                            rhyaErrorChecker.clearErrorCount();

                            Intent intent = new Intent(context, ActivityMain.class);
                            intent.putExtra("nowPlayIndex", resultInt);
                            intent.setAction(Intent.ACTION_MAIN);
                            intent.addCategory(Intent.CATEGORY_LAUNCHER);
                            intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                            activity.startActivity(intent);
                            activity.finish();
                            activity.overridePendingTransition(R.anim.anim_fadein, R.anim.anim_fadeout);
                        }
                    }
                }.execute(null));
    }


    // 가수 데이터 초기화
    public void InitSingerInfo() {
        ArrayList<RhyaSingerDataVO> newRhyaSingerDataVOArrayList = new ArrayList<>();
        for (RhyaSingerDataVO rhyaSingerDataVO : RhyaApplication.rhyaMusicDataVO.getSubscribeList()) {
            String uuid = rhyaSingerDataVO.getUuid();
            boolean checker = false;
            for (String rhyaMusicInfoVOKey : RhyaApplication.rhyaMusicInfoVOHashMap.keySet()) {
                try {
                    RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(rhyaMusicInfoVOKey);
                    assert rhyaMusicInfoVO != null;
                    if (rhyaMusicInfoVO.getSingerUuid() != null) {
                        if (rhyaMusicInfoVO.getSingerUuid().equals(uuid)) {
                            rhyaSingerDataVO.setName(rhyaMusicInfoVO.getSinger());
                            rhyaSingerDataVO.setImage(rhyaMusicInfoVO.getSingerImage());

                            checker = true;
                        }
                    }
                }catch (NullPointerException ex) {
                    ex.printStackTrace();
                }
            }

            if (!checker) {
                rhyaSingerDataVO.setName("null");
                rhyaSingerDataVO.setImage("null");
            }

            newRhyaSingerDataVOArrayList.add(rhyaSingerDataVO);
        }

        RhyaApplication.rhyaMusicDataVO.setSubscribeList(newRhyaSingerDataVOArrayList);
    }


    // 노래 데이터 파일 감지
    public boolean checkMusicInfoFile(Activity activity) {
        File file = new File(getMusicInfoFile(activity));

        // 파일 감지
        return file.exists();
    }
    // 노래 데이터 날자 감지
    public boolean checkMusicInfoDate(RhyaSharedPreferences rhyaSharedPreferences, Context context) throws ParseException {
        String date = rhyaSharedPreferences.getStringNoAES("LOAD_DATA_DATE", context);
        if (date.equals(rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE)){
            return false;
        }

        // 날자 확인
        SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd", Locale.KOREA);
        Date saveDate = dateFormat.parse(date);
        Date nowDate = new Date();

        if (saveDate == null) {
            return false;
        }

        long diffSec = (nowDate.getTime() - saveDate.getTime()) / 1000;
        long diffDays = diffSec / (24*60*60);

        return diffDays < 1;
    }
    // 노래 데이터 파일 이름
    public String getMusicInfoFile(Activity activity) {
        // 파일 확인
        StringBuilder sb;
        sb = new StringBuilder();
        sb.append(activity.getFilesDir().getAbsolutePath());
        sb.append(File.separator);
        sb.append("music_info.rhya");

        return sb.toString();
    }
    // 노래 가사 폴더 생성 및 폴더 이름
    public String getMusicInfoLyricsDirectory(Activity activity) {
        // 파일 확인
        StringBuilder sb = new StringBuilder();
        sb.append(activity.getFilesDir().getAbsolutePath());
        sb.append(File.separator);
        sb.append("Lyrics");

        File file = new File(sb.toString());
        boolean wasSuccessful = file.mkdir();
        if (!wasSuccessful) {
            System.out.println("was not successful.");
        }

        sb.append(File.separator);

        return sb.toString();
    }
    // 노래 파일 다운로드 문자열
    public String getDownloadSize() throws JSONException {
        RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
        ContentValues urlParm = new ContentValues();

        urlParm.put("mode", "2");

        JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(MAIN_URL, urlParm));

        double size = Double.parseDouble(jsonObject.getString("size"));

        StringBuilder sb;
        sb = new StringBuilder();
        sb.append("Utaite Player에 필요한 노래 데이터 파일의 다운로드를 시작할 것입니다. 다운로드를 진행하시겠습니까? ( 약 ");
        sb.append(size);
        sb.append(" MB )");

        return sb.toString();
    }


    // 노래 정보 로딩
    public boolean musicInfoReload(RhyaAESManager rhyaAESManager, RhyaSharedPreferences rhyaSharedPreferences, Context context, Activity activity, String authToken) throws ParseException, JSONException, NoSuchPaddingException, InvalidAlgorithmParameterException, NoSuchAlgorithmException, IOException, BadPaddingException, IllegalBlockSizeException, InvalidKeyException {
        if (checkMusicInfoFile(activity)) {
            // 서버 접속 확인
            if (checkMusicInfoDate(rhyaSharedPreferences, context)) {
                // 파일 읽기
                HashMap<String, RhyaMusicInfoVO> rhyaMusicInfoVOHashMap = new HashMap<>();
                ArrayList<RhyaMusicInfoVO> rhyaMusicInfoVOArrayList = readMusicInfoFile(rhyaAESManager, activity);
                if (rhyaMusicInfoVOArrayList != null) {
                    for (RhyaMusicInfoVO rhyaMusicInfoVO : rhyaMusicInfoVOArrayList) {
                        rhyaMusicInfoVOHashMap.put(rhyaMusicInfoVO.getUuid(), rhyaMusicInfoVO);
                    }
                }

                RhyaApplication.rhyaMusicInfoVOHashMap = rhyaMusicInfoVOHashMap;
            }else {
                rhyaSharedPreferences.setStringNoAES("LOAD_DATA_DATE", new SimpleDateFormat("yyyy-MM-dd", Locale.KOREA).format(new Date()), context);

                ArrayList<RhyaMusicInfoVO> arrayListServer = getMusicVersion(authToken);
                ArrayList<RhyaMusicInfoVO> arrayListFile = readMusicInfoFile(rhyaAESManager, activity);
                ArrayList<RhyaMusicInfoVO> arrayListNew = new ArrayList<>();

                for (RhyaMusicInfoVO rhyaMusicInfoVO : arrayListServer) {
                    String uuid = rhyaMusicInfoVO.getUuid();
                    int version = rhyaMusicInfoVO.getVersion();

                    boolean isNoExt = false;

                    RhyaMusicInfoVO inputValues = null;
                    for (RhyaMusicInfoVO rhyaMusicInfoVOIn : arrayListFile) {
                        inputValues = rhyaMusicInfoVOIn;
                        if (uuid.equals(rhyaMusicInfoVOIn.getUuid())) {
                            if (version == rhyaMusicInfoVOIn.getVersion()) {
                                isNoExt = true;
                            }

                            break;
                        }
                    }

                    if (!isNoExt) {
                        RhyaMusicInfoVO rhyaMusicInfoVOGet = getMusicInfo(rhyaAESManager, authToken, uuid, activity);

                        StringBuilder sb = new StringBuilder();
                        sb.append(MAIN_URL);
                        sb.append("?mode=8&auth=");
                        sb.append(authToken);
                        sb.append("&uuid=");
                        sb.append(uuid);

                        arrayListNew.add(rhyaMusicInfoVOGet);
                    }else {
                        arrayListNew.add(inputValues);
                    }
                }

                HashMap<String, RhyaMusicInfoVO> rhyaMusicInfoVOHashMap = new HashMap<>();
                for (RhyaMusicInfoVO rhyaMusicInfoVO : arrayListNew) {
                    rhyaMusicInfoVOHashMap.put(rhyaMusicInfoVO.getUuid(), rhyaMusicInfoVO);
                }

                RhyaApplication.rhyaMusicInfoVOHashMap = rhyaMusicInfoVOHashMap;

                saveMusicInfoFile(rhyaAESManager, activity);
            }

            return true;
        }else {
            rhyaSharedPreferences.setStringNoAES("LOAD_DATA_DATE", new SimpleDateFormat("yyyy-MM-dd", Locale.KOREA).format(new Date()), context);

            ArrayList<RhyaMusicInfoVO> result = getAllMusicInfo(rhyaAESManager, authToken, activity);
            if (result != null) {
                HashMap<String, RhyaMusicInfoVO> rhyaMusicInfoVOHashMap = new HashMap<>();
                for (RhyaMusicInfoVO rhyaMusicInfoVO : result) {
                    rhyaMusicInfoVOHashMap.put(rhyaMusicInfoVO.getUuid(), rhyaMusicInfoVO);
                }

                RhyaApplication.rhyaMusicInfoVOHashMap = rhyaMusicInfoVOHashMap;

                return true;
            }else {
                return false;
            }
        }
    }


    // 노래 정보 파일 읽기
    public ArrayList<RhyaMusicInfoVO> readMusicInfoFile(RhyaAESManager rhyaAESManager, Activity activity) throws IOException, NoSuchPaddingException, InvalidKeyException, NoSuchAlgorithmException, IllegalBlockSizeException, BadPaddingException, InvalidAlgorithmParameterException, JSONException {
        String read = rhyaAESManager.aesDecode(readFile(getMusicInfoFile(activity)));

        ArrayList<RhyaMusicInfoVO> arrayList = null;

        JSONObject jsonObject = new JSONObject(read);
        for (Iterator<String> iter = jsonObject.keys(); iter.hasNext(); ) {
            if (arrayList == null) arrayList = new ArrayList<>();

            String key = iter.next();
            JSONObject data = new JSONObject(jsonObject.getString(key));

            if (data.getString("uuid").equals("[null]")) {
                return null;
            }

            data.remove("lyrics");
            data.remove("lyrics_sub");

            jsonObject.put(key, data);

            arrayList.add(new RhyaMusicInfoVO(data.getString("uuid"),
                    URLDecoder.decode(data.getString("name"), "UTF-8"),
                    URLDecoder.decode(data.getString("time"), "UTF-8"),
                    null,
                    URLDecoder.decode(data.getString("singer"), "UTF-8"),
                    data.getString("singeruuid"),
                    URLDecoder.decode(data.getString("singerimage"), "UTF-8"),
                    URLDecoder.decode(data.getString("songwriter"), "UTF-8"),
                    URLDecoder.decode(data.getString("image"), "UTF-8"),
                    URLDecoder.decode(data.getString("mp3"), "UTF-8"),
                    URLDecoder.decode(data.getString("type"), "UTF-8"),
                    URLDecoder.decode(data.getString("date"), "UTF-8"),
                    data.getInt("version")));
        }

        return arrayList;
    }
    // 노래 데이터 파일 저장
    public void saveMusicInfoFile(RhyaAESManager rhyaAESManager, Activity activity) throws JSONException, IOException, NoSuchPaddingException, InvalidKeyException, NoSuchAlgorithmException, IllegalBlockSizeException, BadPaddingException, InvalidAlgorithmParameterException {
        JSONObject rootObject = new JSONObject();

        int index = 1;
        for (String keys : RhyaApplication.rhyaMusicInfoVOHashMap.keySet()) {
            RhyaMusicInfoVO music = RhyaApplication.rhyaMusicInfoVOHashMap.get(keys);

            JSONObject jsonObject = new JSONObject();
            assert music != null;
            jsonObject.put("uuid", music.getUuid());
            jsonObject.put("name", URLEncoder.encode(music.getName(), "UTF-8"));
            jsonObject.put("time", URLEncoder.encode(music.getTime(),  "UTF-8"));
            jsonObject.put("singer", URLEncoder.encode(music.getSinger(), "UTF-8"));
            jsonObject.put("singeruuid", music.getSingerUuid());
            jsonObject.put("singerimage", URLEncoder.encode(music.getSingerImage(), "UTF-8"));
            jsonObject.put("songwriter", URLEncoder.encode(music.getSongWriter(), "UTF-8"));
            jsonObject.put("image", URLEncoder.encode(music.getImage(), "UTF-8"));
            jsonObject.put("mp3", URLEncoder.encode(music.getMp3(), "UTF-8"));
            jsonObject.put("type", URLEncoder.encode(music.getType(), "UTF-8"));
            jsonObject.put("date", URLEncoder.encode(music.getDate(), "UTF-8"));
            jsonObject.put("version", URLEncoder.encode(Integer.toString(music.getVersion()), "UTF-8"));
            rootObject.put(Integer.toString(index), jsonObject);
            index ++;
        }

        String dirPath = getMusicInfoFile(activity);
        writeFile(dirPath, rhyaAESManager.aesEncode(rootObject.toString()));
    }
    // 특정 노래 정보 서버 접속
    public RhyaMusicInfoVO getMusicInfo(RhyaAESManager rhyaAESManager, String authToken, String musicUUID, Activity activity) throws JSONException, IOException, NoSuchPaddingException, InvalidKeyException, NoSuchAlgorithmException, IllegalBlockSizeException, BadPaddingException, InvalidAlgorithmParameterException {
        RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
        ContentValues urlParm = new ContentValues();

        urlParm.put("mode", "1");
        urlParm.put("auth", authToken);
        urlParm.put("suuid", musicUUID);

        StringBuilder sb = new StringBuilder();

        String dirPath = getMusicInfoLyricsDirectory(activity);

        JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(MAIN_URL, urlParm));

        if (jsonObject.getString("uuid").equals("[null]")) {
            return null;
        }

        sb.append(dirPath);
        sb.append(jsonObject.getString("uuid"));
        sb.append(".lyrics");

        dirPath = sb.toString();

        sb.setLength(0);

        sb.append(URLDecoder.decode(jsonObject.getString("lyrics"), "UTF-8"));
        sb.append(System.lineSeparator());
        sb.append(URLDecoder.decode(jsonObject.getString("lyrics_sub"), "UTF-8"));

        writeFile(dirPath, rhyaAESManager.aesEncode(sb.toString()));

        return new RhyaMusicInfoVO(jsonObject.getString("uuid"),
                URLDecoder.decode(jsonObject.getString("name"), "UTF-8"),
                URLDecoder.decode(jsonObject.getString("time"), "UTF-8"),
                null,
                URLDecoder.decode(jsonObject.getString("singer"), "UTF-8"),
                jsonObject.getString("singeruuid"),
                URLDecoder.decode(jsonObject.getString("singerimage"), "UTF-8"),
                URLDecoder.decode(jsonObject.getString("songwriter"), "UTF-8"),
                URLDecoder.decode(jsonObject.getString("image"), "UTF-8"),
                URLDecoder.decode(jsonObject.getString("mp3"), "UTF-8"),
                URLDecoder.decode(jsonObject.getString("type"), "UTF-8"),
                URLDecoder.decode(jsonObject.getString("date"), "UTF-8"),
                jsonObject.getInt("version"));
    }
    // 모든 노래 정보 서버 접속
    public ArrayList<RhyaMusicInfoVO> getAllMusicInfo(RhyaAESManager rhyaAESManager, String authToken, Activity activity) throws JSONException, IOException, NoSuchPaddingException, InvalidKeyException, NoSuchAlgorithmException, IllegalBlockSizeException, BadPaddingException, InvalidAlgorithmParameterException {
        ArrayList<RhyaMusicInfoVO> arrayList = null;

        RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
        ContentValues urlParm = new ContentValues();

        urlParm.put("mode", "1");
        urlParm.put("auth", authToken);
        urlParm.put("all", "1");

        String dirPath = getMusicInfoLyricsDirectory(activity);
        String tempPath;

        StringBuilder sb = new StringBuilder();

        JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(MAIN_URL, urlParm));
        for (Iterator<String> iter = jsonObject.keys(); iter.hasNext(); ) {
            if (arrayList == null) arrayList = new ArrayList<>();

            String key = iter.next();
            JSONObject data = new JSONObject(jsonObject.getString(key));

            sb.append(dirPath);
            sb.append(data.getString("uuid"));
            sb.append(".lyrics");

            if (data.getString("uuid").equals("[null]")) {
                return null;
            }

            tempPath = sb.toString();

            sb.setLength(0);

            sb.append(URLDecoder.decode(data.getString("lyrics"), "UTF-8"));
            sb.append(System.lineSeparator());
            sb.append(URLDecoder.decode(data.getString("lyrics_sub"), "UTF-8"));

            writeFile(tempPath, rhyaAESManager.aesEncode(sb.toString()));

            data.remove("lyrics");
            data.remove("lyrics_sub");

            jsonObject.put(key, data);

            arrayList.add(new RhyaMusicInfoVO(data.getString("uuid"),
                    URLDecoder.decode(data.getString("name"), "UTF-8"),
                    URLDecoder.decode(data.getString("time"), "UTF-8"),
                    null,
                    URLDecoder.decode(data.getString("singer"), "UTF-8"),
                    data.getString("singeruuid"),
                    URLDecoder.decode(data.getString("singerimage"), "UTF-8"),
                    URLDecoder.decode(data.getString("songwriter"), "UTF-8"),
                    URLDecoder.decode(data.getString("image"), "UTF-8"),
                    URLDecoder.decode(data.getString("mp3"), "UTF-8"),
                    URLDecoder.decode(data.getString("type"), "UTF-8"),
                    URLDecoder.decode(data.getString("date"), "UTF-8"),
                    data.getInt("version")));
            sb.setLength(0);
        }

        writeFile(getMusicInfoFile(activity), rhyaAESManager.aesEncode(jsonObject.toString()));

        return arrayList;
    }
    // 노래 버전 정보 서버 접속
    public ArrayList<RhyaMusicInfoVO> getMusicVersion(String authToken) throws JSONException {
        ArrayList<RhyaMusicInfoVO> arrayList = null;

        RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
        ContentValues urlParm = new ContentValues();

        urlParm.put("mode", "1");
        urlParm.put("auth", authToken);
        urlParm.put("version", "1");

        JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(MAIN_URL, urlParm));
        for (Iterator<String> iter = jsonObject.keys(); iter.hasNext(); ) {
            if (arrayList == null) arrayList = new ArrayList<>();

            String key = iter.next();
            JSONObject data = new JSONObject(jsonObject.getString(key));

            if (data.getString("uuid").equals("[null]")) {
                return null;
            }

            String uuid = data.getString("uuid");
            int version = data.getInt("version");
            arrayList.add(new RhyaMusicInfoVO(uuid, null, null, null, null, null, null, null, null, null, null, null, version));
        }

        return arrayList;
    }

    // 플레이리스트 읽어오기
    public ArrayList<Object> loadNowPlayList(RhyaAESManager rhyaAESManager, Activity activity) throws IOException, NoSuchPaddingException, InvalidKeyException, NoSuchAlgorithmException, IllegalBlockSizeException, BadPaddingException, InvalidAlgorithmParameterException, JSONException {
        ArrayList<Object> rootR = new ArrayList<>();
        ArrayList<String> result = new ArrayList<>();

        StringBuilder sb = new StringBuilder();
        sb.append(activity.getFilesDir().getAbsolutePath());
        sb.append(File.separator);
        sb.append("play_list.rhya");

        if (!new File(sb.toString()).exists()) {
            rootR.add(0, result);
            rootR.add(1, 0);

            return rootR;
        }

        int nowindex = 0;

        String decode = rhyaAESManager.aesDecode(readFile(sb.toString()));
        JSONObject jsonObject = new JSONObject(decode);
        for (Iterator<String> iter = jsonObject.keys(); iter.hasNext(); ) {
            String root = iter.next();
            if (root.equals("nowindex")) {
                nowindex = Integer.parseInt(jsonObject.getString(root));
                continue;
            }

            int key = Integer.parseInt(root);
            String getValue = jsonObject.getString(root);

            if (key >= 100) break;

            result.add(key, getValue);
        }

        rootR.add(0, result);
        rootR.add(1, nowindex);

        return rootR;
    }
    public void saveNowPlayList(int index, ArrayList<String> rhyaNowPlayMusicUUIDArrayList, RhyaAESManager rhyaAESManager, Activity activity) {
        try {
            JSONObject jsonObject = new JSONObject();
            for (int i = 0; i < rhyaNowPlayMusicUUIDArrayList.size(); i++) {
                String uuid = rhyaNowPlayMusicUUIDArrayList.get(i);

                jsonObject.put(Integer.toString(i), uuid);
            }

            jsonObject.put("nowindex", index);

            StringBuilder sb;
            sb = new StringBuilder();
            sb.append(activity.getFilesDir().getAbsolutePath());
            sb.append(File.separator);
            sb.append("play_list.rhya");

            String result = rhyaAESManager.aesEncode(jsonObject.toString());
            writeFile(sb.toString(), result);
        }catch (Exception ex) {
            ex.printStackTrace();
        }
    }
    // 노래 플레이 접근 허용 확인
    public boolean accessMusicPlayer(String authToken) throws JSONException {
        RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
        ContentValues urlParm = new ContentValues();

        urlParm.put("mode", "5");
        urlParm.put("auth", authToken);

        JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(MAIN_URL, urlParm));
        return jsonObject.getString("result").equals("success");
    }

    // 계정 포함 모든 정보 초기화
    public void resetAllInfo(Activity activity, Context context, RhyaSharedPreferences rhyaSharedPreferences, RhyaDialogManager rhyaDialogManager) {
        rhyaDialogManager.createDialog_Task(context,
                "문제 해결 중...",
                false,
                dialog -> new RhyaAsyncTask<String, String>() {
                    @Override
                    protected void onPreExecute() {

                    }

                    @Override
                    protected String doInBackground(String arg) {
                        try {
                            // 파일 데이터 제거
                            removeDataFile(activity);

                            // 서버 데이터 초기화
                            RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                            ContentValues urlParm = new ContentValues();

                            urlParm.put("mode", "3");
                            urlParm.put("auth", getAutoLogin(rhyaSharedPreferences, context));
                            urlParm.put("name", SERVICE_APP_NAME);
                            urlParm.put("index", "0");
                            urlParm.put("value", URLEncoder.encode("{}", "UTF-8"));

                            JSONObject jsonObject1 = new JSONObject(rhyaHttpsConnection.request(MAIN_URL, urlParm));

                            urlParm.put("index", "1");
                            urlParm.put("value", URLEncoder.encode("{\"list\":[]}", "UTF-8"));
                            JSONObject jsonObject2 = new JSONObject(rhyaHttpsConnection.request(MAIN_URL, urlParm));

                            boolean resultTOF = jsonObject1.getString("result").equals("success") && jsonObject2.getString("result").equals("success");

                            if (resultTOF) {
                                return "success";
                            }else {
                                return "false";
                            }
                        }catch (Exception ex) {
                            ex.printStackTrace();

                            return "exception";
                        }
                    }

                    @Override
                    protected void onPostExecute(String result) {
                        String title;
                        String message;

                        dialog.dismiss();

                        if (result.equals("success")) {
                            title = "문제 해결 성공";
                            message = "문제 해결에 성공하였습니다. 앱 종료 후 다시 실행해 주세요.";
                        }else if (result.equals("fail")) {
                            title = "문제 해결 실패";
                            message = "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00036-2)";
                        }else {
                            title = "Unknown Error";
                            message = "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00036)";
                        }

                        // 메시지 출력
                        rhyaDialogManager.createDialog_Confirm(context,
                                title,
                                message,
                                "종료",
                                false,
                                dialog -> {
                                    activity.moveTaskToBack(true);
                                    activity.finish();
                                    android.os.Process.killProcess(android.os.Process.myPid());
                                });
                    }
                }.execute(null));
    }
    // 데이터 파일 제거
    public void removeDataFile(Activity activity) {
        // 디렉토리 지우기 - 가사 파일
        StringBuilder sb = new StringBuilder();
        sb.append(activity.getFilesDir().getAbsolutePath());
        sb.append(File.separator);
        sb.append("Lyrics");

        File deleteDir = new File(sb.toString()); sb.setLength(0);
        File[] deleteFolderList = deleteDir.listFiles();

        if (deleteFolderList != null) {
            for (File file : deleteFolderList) {
                if (!file.delete()) {
                    System.out.println("DELETE FAIL - Lyrics FILE");
                }
            }
        }

        if (!deleteDir.delete()) {
            System.out.println("DELETE FAIL - Lyrics DIRECTORY");
        }


        // 노래 정보 파일 지우기
        File musicInfoFile = new File(getMusicInfoFile(activity));
        if (!musicInfoFile.delete()) {
            System.out.println("DELETE FAIL - Music INFO FILE");
        }

        sb.append(activity.getFilesDir().getAbsolutePath());
        sb.append(File.separator);
        sb.append("play_list.rhya");
        if (!new File(sb.toString()).delete()) {
            System.out.println("DELETE FAIL - Now Play List FILE");
        }
        sb.setLength(0);

        // 사용자 부과 정보 파일 지우기
        sb.append(activity.getFilesDir().getAbsolutePath());
        sb.append(File.separator);
        sb.append("user_metadata.rhya");

        File metadataFile = new File(sb.toString());
        if (!metadataFile.delete()) {
            System.out.println("DELETE FAIL - Meta DATA FILE");
        }
        sb.setLength(0);


        // 사용자 정보 파일 지우기
        sb.append(activity.getFilesDir().getAbsolutePath());
        sb.append(File.separator);
        sb.append("user_info.rhya");

        File userInfo = new File(sb.toString());
        if (!userInfo.delete()) {
            System.out.println("DELETE FAIL - User INFO FILE");
        }
        sb.setLength(0);


        // Mp3 다운로드 제거
        sb.append(activity.getFilesDir().getAbsolutePath());
        sb.append(File.separator);
        sb.append("music");

        File root = new File(sb.toString());

        if (root.exists()) {
            File[] deleteFolderList2 = root.listFiles();

            if (deleteFolderList2 != null) {
                for (File file : deleteFolderList2) {
                    file.delete();
                }
            }
        }
    }


    public String readLyrics(Activity activity, String uuid) throws IOException {
        StringBuilder sb = new StringBuilder();
        sb.append(activity.getFilesDir().getAbsolutePath());
        sb.append(File.separator);
        sb.append("Lyrics");
        sb.append(File.separator);
        sb.append(uuid);
        sb.append(".lyrics");

        if (new File(sb.toString()).exists()) {
            return readFile(sb.toString());
        }

        return null;
    }



   // 사용자 데이터 변경 감지
    public boolean isNoChangeUserData(RhyaSharedPreferences rhyaSharedPreferences, Context context) {
        String result = rhyaSharedPreferences.getStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_USER_DATA_CHANGE, context);

        if (!result.equals(rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE)) {
            return result.equals(rhyaSharedPreferences.SHARED_PREFERENCES_USER_DATA_CHANGE_DATA);
        }

        return false;
    }
    public boolean isNoChangeUserMetaData(RhyaSharedPreferences rhyaSharedPreferences, Context context) {
        String result = rhyaSharedPreferences.getStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_USER_METADATA_CHANGE, context);

        if (!result.equals(rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE)) {
            return result.equals(rhyaSharedPreferences.SHARED_PREFERENCES_USER_DATA_CHANGE_DATA);
        }

        return false;
    }



    // 파일 작성
    public void writeFile(String file, String data) throws IOException {
        // 파일 쓰기 변수 초기화
        BufferedWriter buf;
        FileWriter fw;
        // 파일 쓰기
        fw = new FileWriter(file, false);
        buf = new BufferedWriter(fw);
        buf.append(data);
        buf.close();
        fw.close();
    }
    // 파일 읽기
    public String readFile(String file) throws IOException {
        // 파일 읽기 변수 초기화
        BufferedReader buf;
        FileReader fr;
        String readLine;

        // 파일 읽기
        fr = new FileReader(file);
        buf = new BufferedReader(fr);

        StringBuilder readText = new StringBuilder();
        while ((readLine = buf.readLine()) != null) {
            readText.append(readLine);
            readText.append(System.lineSeparator());
        }

        buf.close();
        fr.close();

        return readText.toString();
    }
}
