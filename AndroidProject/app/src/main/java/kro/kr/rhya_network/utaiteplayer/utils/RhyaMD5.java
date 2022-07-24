package kro.kr.rhya_network.utaiteplayer.utils;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

/*
 * RHYA.Network MD5 생성
 */
public class RhyaMD5 {
    /**
     * getMD5 - MD5 생성 함수
     * @param input 원본 문자열
     * @return 암호화 문자열
     */
    public String getMD5(String input) {
        String MD5;
        try {
            MessageDigest md = MessageDigest.getInstance("MD5");
            md.update(input.getBytes());
            byte[] byteData = md.digest();
            StringBuilder sb = new StringBuilder();
            for (byte byteDatum : byteData) {
                sb.append(Integer.toString((byteDatum & 0xff) + 0x100, 16).substring(1));
            }
            MD5 = sb.toString();
        } catch (NoSuchAlgorithmException e) {
            e.printStackTrace();
            MD5 = null;
        }
        return MD5;
    }
}
