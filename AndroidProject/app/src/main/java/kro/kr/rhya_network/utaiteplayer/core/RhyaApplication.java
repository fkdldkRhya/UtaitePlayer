package kro.kr.rhya_network.utaiteplayer.core;

import android.app.Application;

import androidx.appcompat.app.AppCompatDelegate;

import java.util.ArrayList;
import java.util.HashMap;

import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicDataVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSingerDataVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaUserDataVO;

public class RhyaApplication extends Application {
    public static RhyaUserDataVO rhyaUserDataVO = null;
    public static RhyaMusicDataVO rhyaMusicDataVO = null;
    public static HashMap<String, RhyaMusicInfoVO> rhyaMusicInfoVOHashMap = null;
    public static HashMap<String, RhyaSingerDataVO> stringRhyaSingerDataVOHashMap = null;
    public static String appVersion = null;
    public static String searchText = "";



    private static ArrayList<String> rhyaNowPlayMusicUUIDArrayList = null;
    public static void addRhyaNowPlayMusicUUIDArrayList(String uuid) {
        if (rhyaNowPlayMusicUUIDArrayList == null) {
            rhyaNowPlayMusicUUIDArrayList = new ArrayList<>();
        }

        if (sizeMaxCheckerRhyaNowPlayMusicUUIDArrayList()) {
            rhyaNowPlayMusicUUIDArrayList.remove(0);
        }

        rhyaNowPlayMusicUUIDArrayList.add(uuid);
    }
    /*
    public static void removeRhyaNowPlayMusicUUIDArrayList(String uuid) {
        if (rhyaNowPlayMusicUUIDArrayList == null) {
            rhyaNowPlayMusicUUIDArrayList = new ArrayList<>();
        }

        rhyaNowPlayMusicUUIDArrayList.remove(uuid);

        if (rhyaNowPlayMusicUUIDArrayList.size() == 0) {
            rhyaNowPlayMusicUUIDArrayList = null;
        }
    }
    public static boolean sizeCheckerRhyaNowPlayMusicUUIDArrayList() {
        if (rhyaNowPlayMusicUUIDArrayList == null) {
            rhyaNowPlayMusicUUIDArrayList = new ArrayList<>();
        }

        return rhyaNowPlayMusicUUIDArrayList.size() != 0;
    }
     */
    public static boolean sizeMaxCheckerRhyaNowPlayMusicUUIDArrayList() {
        if (rhyaNowPlayMusicUUIDArrayList == null) {
            rhyaNowPlayMusicUUIDArrayList = new ArrayList<>();
        }

        return rhyaNowPlayMusicUUIDArrayList.size() == 100;
    }
    public static ArrayList<String> getRhyaNowPlayMusicUUIDArrayList() {
        if (rhyaNowPlayMusicUUIDArrayList == null) {
            rhyaNowPlayMusicUUIDArrayList = new ArrayList<>();
        }

        return rhyaNowPlayMusicUUIDArrayList;
    }
    public static void clearRhyaNowPlayMusicUUIDArrayList() {
        if (rhyaNowPlayMusicUUIDArrayList == null) {
            rhyaNowPlayMusicUUIDArrayList = new ArrayList<>();
        }

        rhyaNowPlayMusicUUIDArrayList.clear();
    }
    public static void setRhyaNowPlayMusicUUIDArrayList(ArrayList<String> input) {
        rhyaNowPlayMusicUUIDArrayList = input;
    }


    @Override
    public void onCreate() {
        super.onCreate();

        // 다크모드 비활성화
        AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO);
    }
}
