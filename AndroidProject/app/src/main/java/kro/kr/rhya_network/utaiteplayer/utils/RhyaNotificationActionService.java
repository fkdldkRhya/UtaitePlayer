package kro.kr.rhya_network.utaiteplayer.utils;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

public class RhyaNotificationActionService extends BroadcastReceiver {
    @Override
    public void onReceive(Context context, Intent intent) {
        context.sendBroadcast(new Intent("RHYA_NETWORK_UTAITE_PLAYER")
                .putExtra("actionName", intent.getAction()));
    }
}
