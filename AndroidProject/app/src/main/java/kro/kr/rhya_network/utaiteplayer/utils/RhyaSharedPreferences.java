package kro.kr.rhya_network.utaiteplayer.utils;

import android.content.Context;
import android.content.SharedPreferences;

import java.io.UnsupportedEncodingException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;

/*
 * SharedPreferences 관리자
 */
public class RhyaSharedPreferences {
    /*
     * SharedPreferences 저장 정보
     */
    public final String SHARED_PREFERENCES_APP_NAME = "RHYA.Network_PREFERENCES"; // 설정 저장 이름
    public final String SHARED_PREFERENCES_AUTH_TOKEN = "AUTO_LOGIN_AUTH_TOKEN"; // 사용자 인증 토큰 데이터
    public final String SHARED_PREFERENCES_AES_KEY = "AES_SECRET_KEY"; // AES 암호화-복호화 키
    public final String SHARED_PREFERENCES_USER_DATA_CHANGE = "IS_USER_DATA_CHANGE"; // 사용자 정보 변경 확인
    public final String SHARED_PREFERENCES_USER_DATA_CHANGE_DATA = "TRUE"; // 사용자 정보 변경 확인 데이터
    public final String SHARED_PREFERENCES_USER_METADATA_CHANGE = "IS_USER_METADATA_CHANGE"; // 사용자 메타 정보 변경 확인
    public final String SHARED_PREFERENCES_CHECK_NETWORK_DIALOG = "IS_USE_NETWORK_CHECK_DIALOG"; // 인터넷 연결 확인 Dialog 사용 여부
    public final String SHARED_PREFERENCES_ALARM_FOR_NOTIFICATION = "IS_USE_ALARM_FOR_NOTIFICATION"; // 공지사항 notification 수신 여부
    // String 기본 반환 데이터
    public final String DEFAULT_RETURN_STRING_VALUE = "<Null>";
    // AES 암호화 관리자
    private final RhyaAESManager rhyaAESManager;


    /**
     * 생성자 - 초기화 작업
     */
    public RhyaSharedPreferences(Context context, boolean noInit) {
        if (!noInit) {
            rhyaAESManager = new RhyaAESManager(context);
        }else {
            rhyaAESManager = null;
        }
    }



    // SharedPreferences 함수
    // **********************************************************************
    /**
     * SharedPreferences [ String ] 설정
     * @param key 설정 이름
     * @param value 설정 데이터
     */
    public void setString(String key, String value, Context mContext) throws NoSuchPaddingException,
            InvalidKeyException,
            UnsupportedEncodingException,
            IllegalBlockSizeException,
            BadPaddingException,
            NoSuchAlgorithmException,
            InvalidAlgorithmParameterException {

        SharedPreferences prefs = mContext.getSharedPreferences(SHARED_PREFERENCES_APP_NAME,
                Context.MODE_PRIVATE);

        assert rhyaAESManager != null;
        value = rhyaAESManager.aesEncode(value);

        SharedPreferences.Editor editor = prefs.edit();
        editor.putString(key, value);
        editor.apply();
    }
    /**
     * SharedPreferences [ String ] 가져오기
     * @param key 설정 이름
     * @return 설정 데이터
     */
    public String getString(String key, Context mContext) throws NoSuchPaddingException,
            InvalidKeyException,
            UnsupportedEncodingException,
            IllegalBlockSizeException,
            BadPaddingException,
            NoSuchAlgorithmException,
            InvalidAlgorithmParameterException {

        SharedPreferences prefs = mContext.getSharedPreferences(SHARED_PREFERENCES_APP_NAME,
                Context.MODE_PRIVATE);

        String returnValue = prefs.getString(key, DEFAULT_RETURN_STRING_VALUE);

        if (returnValue.equals(DEFAULT_RETURN_STRING_VALUE)) {
            return DEFAULT_RETURN_STRING_VALUE;
        }

        assert rhyaAESManager != null;
        return rhyaAESManager.aesDecode(returnValue);
    }
    /**
     * SharedPreferences (No Encrypt) [ String ] 설정
     * @param key 설정 이름
     * @param value 설정 데이터
     */
    public void setStringNoAES(String key, String value, Context mContext) {
        SharedPreferences prefs = mContext.getSharedPreferences(SHARED_PREFERENCES_APP_NAME,
                Context.MODE_PRIVATE);

        SharedPreferences.Editor editor = prefs.edit();
        editor.putString(key, value);
        editor.apply();
    }
    /**
     * SharedPreferences (No Decrypt) [ String ] 가져오기
     * @param key 설정 이름
     * @return 설정 데이터
     */
    public String getStringNoAES(String key, Context mContext) {
        SharedPreferences prefs = mContext.getSharedPreferences(SHARED_PREFERENCES_APP_NAME,
                Context.MODE_PRIVATE);
        return prefs.getString(key, DEFAULT_RETURN_STRING_VALUE);
    }
    /**
     * SharedPreferences [ String ] 제거
     * @param key 설정 이름
     */
    public void removeString(String key, Context mContext) {

        SharedPreferences prefs = mContext.getSharedPreferences(SHARED_PREFERENCES_APP_NAME,
                Context.MODE_PRIVATE);

        SharedPreferences.Editor editor = prefs.edit();
        editor.remove(key);
        editor.apply();
    }
    // **********************************************************************
}
