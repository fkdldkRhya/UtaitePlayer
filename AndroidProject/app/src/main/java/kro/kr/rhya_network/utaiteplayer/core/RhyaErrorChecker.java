package kro.kr.rhya_network.utaiteplayer.core;

import android.content.Context;

import kro.kr.rhya_network.utaiteplayer.utils.RhyaSharedPreferences;

public class RhyaErrorChecker {
    public RhyaSharedPreferences rhyaSharedPreferences;
    public Context context;


    public void clearErrorCount() {
        String ERROR_COUNT_KEY_NAME = "ERROR_COUNT";

        rhyaSharedPreferences.setStringNoAES(ERROR_COUNT_KEY_NAME, "0", context);
    }


    public boolean checkErrorCount() {
        int count;

        String ERROR_COUNT_KEY_NAME = "ERROR_COUNT";

        try {
            count = Integer.parseInt(rhyaSharedPreferences.getStringNoAES(ERROR_COUNT_KEY_NAME, context));
        }catch (Exception ex) {
            ex.printStackTrace();

            rhyaSharedPreferences.setStringNoAES(ERROR_COUNT_KEY_NAME, "0", context);

            count = 0;
        }

        if (count >= 3) {
            rhyaSharedPreferences.setStringNoAES(ERROR_COUNT_KEY_NAME, "0", context);

            return true;
        }else {
            count ++;

            rhyaSharedPreferences.setStringNoAES(ERROR_COUNT_KEY_NAME, Integer.toString(count), context);

           return false;
        }
    }
}
