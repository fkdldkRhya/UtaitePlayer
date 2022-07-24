package kro.kr.rhya_network.utaiteplayer.service;
import android.annotation.SuppressLint;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Intent;
import android.os.Build;

import androidx.annotation.NonNull;
import androidx.core.app.NotificationCompat;
import androidx.core.app.NotificationManagerCompat;

import com.google.firebase.messaging.FirebaseMessagingService;
import com.google.firebase.messaging.RemoteMessage;

import java.util.Objects;
import java.util.concurrent.atomic.AtomicInteger;

import kro.kr.rhya_network.utaiteplayer.activity.ActivitySplash;
import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSharedPreferences;

public class FirebaseInstanceIDService extends FirebaseMessagingService {
    // 랜덤 정수 생성
    private final AtomicInteger atomicInteger = new AtomicInteger(0);
    // Notification 전용 변수
    private NotificationManagerCompat notificationManager;
    private NotificationCompat.Builder notificationCompat;
    private NotificationChannel channel;
    // SharedPreferences
    private RhyaSharedPreferences rhyaSharedPreferences = null;



    @Override
    public void onNewToken(@NonNull String s) {
        super.onNewToken(s);
    }



    @Override
    public void onMessageReceived(@NonNull RemoteMessage remoteMessage) {
        if (rhyaSharedPreferences == null)
            rhyaSharedPreferences = new RhyaSharedPreferences(getApplicationContext(), false);

        // 메시지 수신 확인
        if (remoteMessage.getData().size() > 0) {
            String result = rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE;

            try {
                result = rhyaSharedPreferences.getStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_ALARM_FOR_NOTIFICATION, getApplicationContext());
            }catch (Exception ex) {
                ex.printStackTrace();
            }

            if (result.equals(rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE))
                showNotification(remoteMessage);
        }
    }


    /**
     * Notification 출력
     * @param remoteMessage RemoteMessage
     */
    @SuppressLint("UnspecifiedImmutableFlag")
    private void showNotification(@NonNull RemoteMessage remoteMessage) {
        // Notification 채널 정보 [ ID, Name ]
        final String CHANNEL_ID = "kro_kr_rhya_network_utaite_player_fcm";
        final String CHANNEL_NAME = "kro_kr_rhya_network_utaite_player_fcm";

        // 예외 처리
        try {
            // NULL 확인
            if (notificationManager == null)
                // NotificationManagerCompat
                notificationManager = NotificationManagerCompat.from(getApplicationContext());

            // Android 버전 확인
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                if (notificationManager.getNotificationChannel(CHANNEL_ID) == null) {
                    // NULL 확인
                    if (channel == null)
                        channel = new NotificationChannel(CHANNEL_ID, CHANNEL_NAME, NotificationManager.IMPORTANCE_HIGH);
                    // Channel 설정
                    notificationManager.createNotificationChannel(channel);
                } // if end
            } // if end

            // Notification 클릭 이벤트 설정
            Intent mainIntent = new Intent(getApplicationContext(), ActivitySplash.class);
            mainIntent.setAction(Intent.ACTION_MAIN);
            mainIntent.addCategory(Intent.CATEGORY_LAUNCHER);
            mainIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            PendingIntent pendingIntent;
            if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.M)
                pendingIntent = PendingIntent.getActivity(this, 0, mainIntent, PendingIntent.FLAG_IMMUTABLE);
            else
                pendingIntent = PendingIntent.getBroadcast(this, 0, mainIntent, PendingIntent.FLAG_UPDATE_CURRENT);

            // NULL 확인
            if (notificationCompat == null)
                // Notification builder Channel 설정
                notificationCompat = new NotificationCompat.Builder(getApplicationContext(), CHANNEL_ID);
            // Notification builder 데이터

            // Notification 데이터 JSON 키
            String JSON_KEY_NOTIFICATION_TITLE = "notification-title";
            final String title = Objects.requireNonNull(remoteMessage.getData()).get(JSON_KEY_NOTIFICATION_TITLE);
            String JSON_KEY_NOTIFICATION_MESSAGE = "notification-message";
            final String message = Objects.requireNonNull(Objects.requireNonNull(remoteMessage.getData()).get(JSON_KEY_NOTIFICATION_MESSAGE)).replace("#</br>#", System.lineSeparator());
            // Notification builder 데이터 설정
            notificationCompat
                    .setContentTitle(title)
                    .setContentText(message)
                    .setContentIntent(pendingIntent)
                    .setShowWhen(true)
                    .setSmallIcon(R.drawable.img_up_logo_sub)
                    .setStyle(new NotificationCompat.BigTextStyle().bigText(message))
                    .setPriority(NotificationCompat.PRIORITY_HIGH);

            // Notification 생성
            Notification notification = notificationCompat.build();
            notificationManager.notify(getRandomNotificationID() + 1, notification);
        }catch (Exception ex) {
            ex.printStackTrace();
        } // try-catch end
    }



    /**
     * 정수형 난수 발생 - 알림 ID
     * @return int
     */
    private int getRandomNotificationID() {
        return atomicInteger.incrementAndGet();
    }
}