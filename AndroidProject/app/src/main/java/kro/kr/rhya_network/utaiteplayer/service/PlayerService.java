package kro.kr.rhya_network.utaiteplayer.service;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.Bitmap;
import android.graphics.drawable.Drawable;
import android.media.AudioManager;
import android.media.MediaPlayer;
import android.net.wifi.WifiManager;
import android.os.Binder;
import android.os.Build;
import android.os.IBinder;
import android.os.PowerManager;
import android.support.v4.media.MediaMetadataCompat;
import android.support.v4.media.session.MediaSessionCompat;
import android.support.v4.media.session.PlaybackStateCompat;
import android.telephony.PhoneStateListener;
import android.telephony.TelephonyCallback;
import android.telephony.TelephonyManager;
import android.widget.ScrollView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.RequiresApi;
import androidx.core.app.NotificationCompat;

import com.bumptech.glide.Glide;
import com.bumptech.glide.request.target.CustomTarget;
import com.bumptech.glide.request.transition.Transition;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedInputStream;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URL;
import java.net.URLConnection;
import java.util.ArrayList;
import java.util.Random;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.core.RhyaCore;
import kro.kr.rhya_network.utaiteplayer.fragment.PlayInfoFragment;
import kro.kr.rhya_network.utaiteplayer.fragment.SearchFragment;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAESManager;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHttpsConnection;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMainUtils;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaNotificationActionService;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaPlayMusicInterface;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSharedPreferences;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSingerDataVO;

public class PlayerService extends Service implements RhyaPlayMusicInterface{
    // 서비스 이벤트 종류
    public static final String ACTION_INIT = "kro.kr.rhya_network.utaiteplayer.ACTION_INIT";
    public static final String ACTION_PREVIUOS = "kro.kr.rhya_network.utaiteplayer.ACTION_PREVIUOS";
    public static final String ACTION_PLAY = "kro.kr.rhya_network.utaiteplayer.ACTION_PLAY";
    public static final String ACTION_PAUSE = "kro.kr.rhya_network.utaiteplayer.ACTION_PAUSE";
    public static final String ACTION_NEXT = "kro.kr.rhya_network.utaiteplayer.ACTION_NEXT";
    public static final String ACTION_CLOSE = "kro.kr.rhya_network.utaiteplayer.ACTION_CLOSE";

    // Binder given to clients
    private final IBinder mBinder = new LocalBinder();
    // Service
    public class LocalBinder extends Binder {
        public PlayerService getService() {
            // Return this instance of LocalService so clients can call public methods
            return PlayerService.this;
        }
    }

    // Download mp3 file uuid list
    public ArrayList<String> downloadFileList;
    public boolean isMp3FilePlay = false;

    // RHYA API
    private RhyaCore rhyaCore;
    private RhyaMainUtils rhyaMainUtils;

    // Auth token
    public String authToken = "";

    // 현재 노래 UUID
    public String nowPlayMusicUUID = null;

    // 노래 인덱스
    public int playMusicIndex = -1;

    // Media Player
    public MediaPlayer mMediaPlayer;

    // 서비스 시작 여부
    public static boolean isStartService = false;

    // 알림
    private NotificationCompat.Builder notification;

    // Main Activity
    public Activity activity = null;


    // 노래 종료 감지
    public boolean isEndCheck = false;
    // 반복 재생 변수
    public boolean isLoopCheck = false;
    public boolean isAllLoopCheck = false;
    // 셔플 사용 확인
    public boolean isShuffleCheck = false;


    // 처음 시작 감지 변수
    public boolean isFirstStartMusic = true;

    // Buffering checker
    private boolean isBufferingChecking = false;
    private int bufferingCheckValue = 0;
    public int nowBufferingValue = 0;

    // Receiver
    private MusicIntentReceiver musicIntentReceiver;

    // Audio Manager
    private AudioManager mAudioManager;
    private AudioFocusHelper mAudioFocusHelper;
    private boolean mPlayOnAudioFocus = false;
    private boolean dontFocusAbandon = false;
    private boolean mAudioNoisyReceiverRegistered = false;

    // WakeLock
    private WifiManager.WifiLock wifiLock;

    // Thread 관리
    private ScheduledExecutorService scheduledExecutorService = null;

    // Media Session
    private MediaSessionCompat mediaSession;
    private PlaybackStateCompat.Builder playbackStateCompatBuilder;
    private MediaMetadataCompat.Builder metaDataBuilder;
    private androidx.media.app.NotificationCompat.MediaStyle mediaStyle;

    // Activity pause and resume checker
    public boolean isMainActivityPauseAndResumeCheck = false;

    // File name
    public String NOW_PLAY_LIST_SAVE_FILE_NAME = null;

    // AES Manager
    public RhyaAESManager rhyaAESManager;

    // Call play music
    private TelephonyManager telephonyManager;
    // Call checker
    private boolean isCallPlayerCheck = false;

    // Toast message
    private Toast toast;

    // Search fragment
    public int subscribeIndex = 0;



    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return mBinder;
    }



    @Override
    public void onCreate() {
        super.onCreate();
    }



    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        String receiveAction = intent.getAction();

        if (toast == null) {
            toast = Toast.makeText(getApplicationContext(), null, Toast.LENGTH_SHORT);
        }

        // 미디어 플레이어 초기화
        if (mMediaPlayer == null) {
            MediaSessionCompat.Callback callback = new MediaSessionCompat.Callback() {
                @Override
                public void onPlay() {
                    onTrackPlay();
                    playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_PLAYING, mMediaPlayer.getCurrentPosition(), 1F);
                }

                @Override
                public void onPause() {
                    onTrackPause();
                    playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_PAUSED, mMediaPlayer.getCurrentPosition(), 1F);
                }

                @Override
                public void onSkipToNext() {
                    onTrackNext(null, true);
                }

                @Override
                public void onSkipToPrevious() {
                    onTrackPrevious();
                }

                @Override
                public void onSeekTo(long pos) {
                    super.onSeekTo(pos);

                    // 버퍼링 확인
                    int progressCheck = (int)((double) pos / (double) mMediaPlayer.getDuration() * 100);
                    int getNowBuffering = nowBufferingValue;

                    if (isMp3FilePlay) {
                        mMediaPlayer.seekTo(Long.valueOf(pos).intValue());
                        mMediaPlayer.setOnSeekCompleteListener(mp -> {
                            playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_PLAYING, pos, 1F);
                            mediaSession.setPlaybackState(playbackStateCompatBuilder.build());
                        });

                        return;
                    }

                    if (progressCheck <= getNowBuffering) {
                        mMediaPlayer.seekTo(Long.valueOf(pos).intValue());
                        mMediaPlayer.setOnSeekCompleteListener(mp -> {
                            playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_PLAYING, pos, 1F);
                            mediaSession.setPlaybackState(playbackStateCompatBuilder.build());
                        });
                    }else {
                        noBufferedTimePlay(Long.valueOf(pos).intValue());
                    }
                }
            };

            mediaSession = new MediaSessionCompat(getApplicationContext(), "RHYA_Network_MediaSession_FOR_UP"); // Debugging tag, any string
            mediaSession.setFlags(MediaSessionCompat.FLAG_HANDLES_MEDIA_BUTTONS | MediaSessionCompat.FLAG_HANDLES_TRANSPORT_CONTROLS);
            mediaSession.setCallback(callback);
            mediaSession.setActive(true);

            metaDataBuilder = new MediaMetadataCompat.Builder();

            rhyaCore = new RhyaCore();
            rhyaMainUtils = new RhyaMainUtils();

            playbackStateCompatBuilder = new PlaybackStateCompat.Builder();
            playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_SKIPPING_TO_PREVIOUS, 0, 1);
            playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_SKIPPING_TO_NEXT, 2, 1);
            playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_STOPPED, 3, 1);
            playbackStateCompatBuilder.setActions(
                    PlaybackStateCompat.ACTION_SKIP_TO_NEXT |
                            PlaybackStateCompat.ACTION_PLAY |
                            PlaybackStateCompat.ACTION_PAUSE |
                            PlaybackStateCompat.ACTION_SKIP_TO_PREVIOUS |
                            PlaybackStateCompat.ACTION_PLAY_PAUSE |
                            PlaybackStateCompat.ACTION_STOP |
                            PlaybackStateCompat.ACTION_SEEK_TO
            );

            mediaStyle = new androidx.media.app.NotificationCompat.MediaStyle();

            mAudioManager = (AudioManager) getSystemService(Context.AUDIO_SERVICE);
            mAudioFocusHelper = new AudioFocusHelper();

            wifiLock = ((WifiManager) getApplicationContext().getSystemService(Context.WIFI_SERVICE)).createWifiLock(WifiManager.WIFI_MODE_FULL, "rhya_up_service_wifi_lock");

            rhyaAESManager = new RhyaAESManager(getApplicationContext());

            mMediaPlayer = new MediaPlayer();
            mMediaPlayer.setAudioStreamType(AudioManager.STREAM_MUSIC);
            mMediaPlayer.setWakeMode(this, PowerManager.PARTIAL_WAKE_LOCK);
            mMediaPlayer.reset();

            mMediaPlayer.setOnErrorListener((mp, what, extra) -> {
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }

                return false;
            });

            mMediaPlayer.setOnBufferingUpdateListener((mp, percent) -> {
                nowBufferingValue = percent;

                if (isBufferingChecking) {
                    int getBufferedMaxValue = (mp.getDuration() * nowBufferingValue / 100);
                    if (getBufferedMaxValue == mp.getDuration()) {
                        getBufferedMaxValue = getBufferedMaxValue - 500;
                    }

                    mMediaPlayer.seekTo(getBufferedMaxValue);

                    if (getBufferedMaxValue >= bufferingCheckValue) {
                        isBufferingChecking = false;

                        mMediaPlayer.seekTo(bufferingCheckValue);

                        if (!mMediaPlayer.isPlaying()) {
                            mMediaPlayer.start();

                            // ---------------------- //
                            // -----노래 재생 확인----- //
                            // ---------------------- //
                            songPlayChecker();
                            // ---------------------- //
                            // ---------------------- //
                            // ---------------------- //
                        }

                        playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_PLAYING, bufferingCheckValue, 1F);
                        mediaSession.setPlaybackState(playbackStateCompatBuilder.build());

                        if (isActivityAlive()) {
                            ((ActivityMain) activity).dismissTaskDialog();
                        }
                    }
                }
            });

            mMediaPlayer.setOnCompletionListener(mp -> {
                if (isBufferingChecking) return;

                if (isFirstStartMusic) {
                    if (wifiLock.isHeld()) wifiLock.release();

                    return;
                }

                if (isActivityAlive()) {
                    ((ActivityMain) activity).musicProgressBar.setProgress(((ActivityMain) activity).musicProgressBar.getMax());
                    ((ActivityMain) activity).showTaskDialog();
                }

                if (isShuffleCheck) {
                    int getIndex = musicRandomPlayIndexGet();
                    String uuid = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(getIndex);
                    playMusicIndex = getIndex;
                    nowPlayMusicUUID = uuid;

                    putNewTrack(uuid);

                    return;
                }

                if (isLoopCheck) {
                    putNewTrack(nowPlayMusicUUID);

                    return;
                }

                RhyaMusicInfoVO rhyaMusicInfoVO = nextMusicChecker();

                if (rhyaMusicInfoVO != null) {
                    isEndCheck = false;
                    onTrackNext(rhyaMusicInfoVO, false);
                } else {
                    if (isAllLoopCheck) {
                        if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() > 0) {
                            playMusicIndex = 0;
                            putNewTrack(RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(0));
                        }else {
                            if (isActivityAlive()) {
                                ((ActivityMain) activity).dismissTaskDialog();
                                ((ActivityMain) activity).setNowPlayMusic();
                            }

                            showNotification(true);
                            playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_PLAYING, mMediaPlayer.getDuration(), 1F);
                            mediaSession.setPlaybackState(playbackStateCompatBuilder.build());
                        }

                        return;
                    }

                    if (wifiLock.isHeld()) wifiLock.release();

                    mMediaPlayer.reset();

                    showNotification(true);

                    // Activity 활성화 확인
                    if (isActivityAlive()) {
                        ((ActivityMain) activity).setNowPlayMusic();
                        ((ActivityMain) activity).dismissTaskDialog();
                    }

                    isEndCheck = true;
                }
            });

            downloadFileList = new ArrayList<>();
            readMp3List();

            telephonyManager = (TelephonyManager) getSystemService(TELEPHONY_SERVICE);
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
                telephonyCallback = new CustomTelephonyCallback(this::phoneStateListenerTask);
                telephonyManager.registerTelephonyCallback(getApplicationContext().getMainExecutor(), telephonyCallback);
            }else {
                phoneStateListener = new PhoneStateListener() {
                    @Override
                    public void onCallStateChanged(int state, String phoneNumber) {
                        phoneStateListenerTask(state);

                        super.onCallStateChanged(state, phoneNumber);
                    }
                };

                telephonyManager.listen(phoneStateListener, PhoneStateListener.LISTEN_CALL_STATE);
            }

            // 리시버 등록
            // ------------------------------------------------------------------------------------------- //
            //
            // Music Intent Receiver
            //
            musicIntentReceiver = new MusicIntentReceiver();
            //
            // registerReceiver
            //
            registerReceiver(broadcastReceiver, new IntentFilter("RHYA_NETWORK_UTAITE_PLAYER"));
            registerReceiver(musicIntentReceiver, new IntentFilter(Intent.ACTION_HEADSET_PLUG));
            registerAudioNoisyReceiver();
            // ------------------------------------------------------------------------------------------- //

            // Progressbar 연동 Thread 시작
            mainActivityProgressbarThread();
        }

        // Action 구분
        if (ACTION_INIT.equals(receiveAction)) {
            showNotificationInit();
        }

        isStartService = true;

        if (nowPlayMusicUUID != null) {
            showNotification(true);
        }

        return START_NOT_STICKY;
    }


    public void mediaSessionUpdateFragment(int pos) {
        playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_PLAYING, pos, 1F);
        mediaSession.setPlaybackState(playbackStateCompatBuilder.build());
    }

    BroadcastReceiver broadcastReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getExtras().getString("actionName");

            switch (action) {
                case ACTION_PREVIUOS:
                    onTrackPrevious();
                    break;

                case ACTION_PLAY:
                    onTrackPlay();
                    break;

                case ACTION_PAUSE:
                    onTrackPause();
                    break;

                case ACTION_NEXT:
                    onTrackNext(null, true);
                    break;

                case ACTION_CLOSE:
                    stopService(true);
                    break;
            }
        }
    };


    public void putNewTrack(String uuid) {
        if (mMediaPlayer == null) return;

        if (telephonyManager != null) {
            if (telephonyManager.getCallState() == TelephonyManager.CALL_STATE_RINGING ||
                    telephonyManager.getCallState() == TelephonyManager.CALL_STATE_OFFHOOK) {
                if (toast != null) {
                    toast.cancel();
                    toast.setText("통화 중에는 재생할 수 없습니다.");
                    toast.show();
                }

                return;
            }
        }

        // Activity 활성화 확인
        if (isActivityAlive()) {
            ((ActivityMain) activity).showTaskDialog();
        }

        try {
            mAudioFocusHelper.requestAudioFocus();

            RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);

            if (rhyaMusicInfoVO != null) {
                mMediaPlayer.reset();

                isEndCheck = false;

                nowPlayMusicUUID = uuid;

                playSongCount(rhyaMusicInfoVO.getUuid());
                playMp3OrUrl(rhyaMusicInfoVO.getUuid());

                mMediaPlayer.prepareAsync();
                mMediaPlayer.setOnPreparedListener(mp -> {
                    try {
                        mp.start();

                        if (!wifiLock.isHeld()) wifiLock.acquire();

                        if (isFirstStartMusic) isFirstStartMusic = false;

                        // Activity 활성화 확인
                        if (isActivityAlive()) {
                            ((ActivityMain) activity).setNowPlayMusic();
                            ((ActivityMain) activity).dismissTaskDialog();

                            if (((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).serviceUIActivityCheck()) {
                                ((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).scrollView.fullScroll(ScrollView.FOCUS_UP);
                            }
                        }


                        // ---------------------- //
                        // -----노래 재생 확인----- //
                        // ---------------------- //
                        songPlayChecker();
                        // ---------------------- //
                        // ---------------------- //
                        // ---------------------- //


                        showNotification(true);
                    }catch (Exception ex) {
                        ex.printStackTrace();

                        if (isActivityAlive()) {
                            ((ActivityMain) activity).dismissTaskDialog();
                        }
                    }
                });
            }
        } catch (Exception ex) {
            ex.printStackTrace();

            if (toast != null) {
                toast.cancel();
                toast.setText("Service Error! 00045");
                toast.show();
            }

            if (isActivityAlive()) {
                ((ActivityMain) activity).dismissTaskDialog();
            }
        }
    }
    public void putNewTrackLast() {
        if (mMediaPlayer == null) return;

        if (telephonyManager != null) {
            if (telephonyManager.getCallState() == TelephonyManager.CALL_STATE_RINGING ||
                    telephonyManager.getCallState() == TelephonyManager.CALL_STATE_OFFHOOK) {

                if (toast != null) {
                    toast.cancel();
                    toast.setText("통화 중에는 재생할 수 없습니다.");
                    toast.show();
                }

                return;
            }
        }

        // Activity 활성화 확인
        if (isActivityAlive()) {
            ((ActivityMain) activity).showTaskDialog();
        }

        accessPermissionChecker(result -> {
            if (result == null) {
                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }else if (result.equals("true")) {
                try {
                    mAudioFocusHelper.requestAudioFocus();

                    RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(0));

                    if (rhyaMusicInfoVO != null) {
                        mMediaPlayer.reset();

                        isEndCheck = false;

                        playSongCount(rhyaMusicInfoVO.getUuid());
                        playMp3OrUrl(rhyaMusicInfoVO.getUuid());

                        mMediaPlayer.prepareAsync();
                        mMediaPlayer.setOnPreparedListener(mp -> {
                            try {
                                mp.start();

                                if (!wifiLock.isHeld()) wifiLock.acquire();

                                if (isFirstStartMusic) isFirstStartMusic = false;

                                // Activity 활성화 확인
                                if (isActivityAlive()) {
                                    ((ActivityMain) activity).setNowPlayMusic();
                                    ((ActivityMain) activity).dismissTaskDialog();

                                    if (((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).serviceUIActivityCheck()) {
                                        ((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).scrollView.fullScroll(ScrollView.FOCUS_UP);
                                    }
                                }

                                // ---------------------- //
                                // -----노래 재생 확인----- //
                                // ---------------------- //
                                songPlayChecker();
                                // ---------------------- //
                                // ---------------------- //
                                // ---------------------- //

                                showNotification(true);
                            }catch (Exception ex) {
                                ex.printStackTrace();

                                if (isActivityAlive()) {
                                    ((ActivityMain) activity).dismissTaskDialog();
                                }
                            }
                        });
                    }
                } catch (Exception ex) {
                    ex.printStackTrace();

                    if (toast != null) {
                        toast.cancel();
                        toast.setText("Service Error! 00071");
                        toast.show();
                    }

                    if (isActivityAlive()) {
                        ((ActivityMain) activity).dismissTaskDialog();
                    }
                }
            }else {
                if (toast != null) {
                    toast.cancel();
                    toast.setText("현재 노래 재생 세션에 접근할 수 없습니다.");
                    toast.show();
                }

                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }
        });
    }
    @Override
    public void onTrackPrevious() {
        if (nowPlayMusicUUID == null || mMediaPlayer == null) return;

        if (telephonyManager != null) {
            if (telephonyManager.getCallState() == TelephonyManager.CALL_STATE_RINGING ||
                    telephonyManager.getCallState() == TelephonyManager.CALL_STATE_OFFHOOK) {

                if (toast != null) {
                    toast.cancel();
                    toast.setText("통화 중에는 재생할 수 없습니다.");
                    toast.show();
                }

                return;
            }
        }

        // Activity 활성화 확인
        if (isActivityAlive()) {
            ((ActivityMain) activity).showTaskDialog();
        }

        accessPermissionChecker(result -> {
            if (result == null) {
                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }else if (result.equals("true")) {
                try {
                    if (mMediaPlayer.getCurrentPosition() >= 4000) {
                        onTrackPause();

                        mMediaPlayer.seekTo(0);
                        mMediaPlayer.setOnSeekCompleteListener(mp -> {
                            onTrackPlay();

                            if (isActivityAlive()) {
                                ((ActivityMain) activity).dismissTaskDialog();
                            }
                        });

                        return;
                    }

                    mAudioFocusHelper.requestAudioFocus();

                    RhyaMusicInfoVO rhyaMusicInfoVO = previousMusicChecker();

                    if (rhyaMusicInfoVO != null) {
                        mMediaPlayer.reset();

                        isEndCheck = false;

                        playSongCount(rhyaMusicInfoVO.getUuid());
                        playMp3OrUrl(rhyaMusicInfoVO.getUuid());

                        mMediaPlayer.prepareAsync();

                        nowPlayMusicUUID = rhyaMusicInfoVO.getUuid();

                        mMediaPlayer.setOnPreparedListener(mp -> {
                            try {
                                // Activity 활성화 확인
                                if (isActivityAlive()) {
                                    ((ActivityMain) activity).setNowPlayMusic();
                                    ((ActivityMain) activity).dismissTaskDialog();
                                }

                                mp.start();

                                if (!wifiLock.isHeld()) wifiLock.acquire();

                                if (isFirstStartMusic) isFirstStartMusic = false;

                                // Activity 활성화 확인
                                if (isActivityAlive()) {
                                    ((ActivityMain) activity).setNowPlayMusic();
                                    ((ActivityMain) activity).dismissTaskDialog();

                                    if (((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).serviceUIActivityCheck()) {
                                        ((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).scrollView.fullScroll(ScrollView.FOCUS_UP);
                                    }
                                }

                                // ---------------------- //
                                // -----노래 재생 확인----- //
                                // ---------------------- //
                                songPlayChecker();
                                // ---------------------- //
                                // ---------------------- //
                                // ---------------------- //

                                showNotification(true);
                            }catch (Exception ex) {
                                ex.printStackTrace();

                                if (isActivityAlive()) {
                                    ((ActivityMain) activity).dismissTaskDialog();
                                }
                            }
                        });
                    }else {
                        mAudioFocusHelper.requestAudioFocus();

                        if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() != 0) {
                            int index = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() - 1;
                            nowPlayMusicUUID = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(index);
                            playMusicIndex = index;

                            mMediaPlayer.reset();

                            playSongCount(nowPlayMusicUUID);
                            playMp3OrUrl(nowPlayMusicUUID);

                            mMediaPlayer.prepareAsync();
                            mMediaPlayer.setOnPreparedListener(mp -> {
                                try {
                                    // Activity 활성화 확인
                                    if (isActivityAlive()) {
                                        ((ActivityMain) activity).setNowPlayMusic();
                                        ((ActivityMain) activity).dismissTaskDialog();
                                    }

                                    mp.start();

                                    if (!wifiLock.isHeld()) wifiLock.acquire();

                                    if (isFirstStartMusic) isFirstStartMusic = false;

                                    // Activity 활성화 확인
                                    if (isActivityAlive()) {
                                        ((ActivityMain) activity).setNowPlayMusic();
                                        ((ActivityMain) activity).dismissTaskDialog();

                                        if (((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).serviceUIActivityCheck()) {
                                            ((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).scrollView.fullScroll(ScrollView.FOCUS_UP);
                                        }
                                    }

                                    // ---------------------- //
                                    // -----노래 재생 확인----- //
                                    // ---------------------- //
                                    songPlayChecker();
                                    // ---------------------- //
                                    // ---------------------- //
                                    // ---------------------- //

                                    showNotification(true);
                                }catch (Exception ex) {
                                    ex.printStackTrace();

                                    if (isActivityAlive()) {
                                        ((ActivityMain) activity).dismissTaskDialog();
                                    }
                                }
                            });
                        }else {
                            if (isActivityAlive()) {
                                ((ActivityMain) activity).dismissTaskDialog();
                            }
                        }
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    if (toast != null) {
                        toast.cancel();
                        toast.setText("Service Error! 00044");
                        toast.show();
                    }

                    if (isActivityAlive()) {
                        ((ActivityMain) activity).dismissTaskDialog();
                    }
                }
            }else {
                if (toast != null) {
                    toast.cancel();
                    toast.setText("현재 노래 재생 세션에 접근할 수 없습니다.");
                    toast.show();
                }

                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }
        });
    }
    @Override
    public void onTrackPlay() {
        if (mMediaPlayer == null) return;

        if (telephonyManager != null) {
            if (telephonyManager.getCallState() == TelephonyManager.CALL_STATE_RINGING ||
                    telephonyManager.getCallState() == TelephonyManager.CALL_STATE_OFFHOOK) {

                if (toast != null) {
                    toast.cancel();
                    toast.setText("통화 중에는 재생할 수 없습니다.");
                    toast.show();
                }

                return;
            }
        }

        accessPermissionChecker(result -> {
            if (result == null) {
                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }else if (result.equals("true")) {
                try {
                    mAudioFocusHelper.requestAudioFocus();

                    if (isEndCheck) {
                        if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() > 0) {
                            playMusicIndex = 0;
                            putNewTrack(RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(0));

                            saveNowPlayListFile();
                        }

                        return;
                    }

                    mMediaPlayer.start();

                    if (!wifiLock.isHeld()) wifiLock.acquire();

                    showNotification(false);

                    playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_PLAYING, mMediaPlayer.getCurrentPosition(), 1F);
                    mediaSession.setPlaybackState(playbackStateCompatBuilder.build());

                    // Activity 활성화 확인
                    if (isActivityAlive()) {
                        ((ActivityMain) activity).setNowPlayMusic();
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    if (toast != null) {
                        toast.cancel();
                        toast.setText("Service Error! 00043");
                        toast.show();
                    }
                }
            }else {
                if (toast != null) {
                    toast.cancel();
                    toast.setText("현재 노래 재생 세션에 접근할 수 없습니다.");
                    toast.show();
                }
            }
        });
    }
    @Override
    public void onTrackPause() {
        try {
            if (mMediaPlayer == null) return;

            mMediaPlayer.pause();

            if (wifiLock.isHeld()) wifiLock.release();

            if (!dontFocusAbandon) {
                mAudioFocusHelper.abandonAudioFocus();
            }else {
                dontFocusAbandon = false;
            }

            showNotification(false);

            playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_PAUSED,  mMediaPlayer.getCurrentPosition(), 1F);
            mediaSession.setPlaybackState(playbackStateCompatBuilder.build());

            // Activity 활성화 확인
            if (isActivityAlive()) {
                ((ActivityMain) activity).setNowPlayMusic();
            }
        }catch (Exception ex) {
            ex.printStackTrace();

            if (toast != null) {
                toast.cancel();
                toast.setText("Service Error! 00042");
                toast.show();
            }
        }
    }
    @Override
    public void onTrackNext(RhyaMusicInfoVO rhyaMusicInfoVOInput, boolean createDialog) {
        if (nowPlayMusicUUID == null || mMediaPlayer == null) return;

        if (telephonyManager != null) {
            if (telephonyManager.getCallState() == TelephonyManager.CALL_STATE_RINGING ||
                    telephonyManager.getCallState() == TelephonyManager.CALL_STATE_OFFHOOK) {

                if (toast != null) {
                    toast.cancel();
                    toast.setText("통화 중에는 재생할 수 없습니다.");
                    toast.show();
                }

                return;
            }
        }

        // Activity 활성화 확인
        if (isActivityAlive() && createDialog) {
            ((ActivityMain) activity).showTaskDialog();
        }

        accessPermissionChecker(result -> {
            if (result == null) {
                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }else if (result.equals("true")) {
                try {
                    mAudioFocusHelper.requestAudioFocus();

                    RhyaMusicInfoVO rhyaMusicInfoVO;

                    if (rhyaMusicInfoVOInput != null) {
                        rhyaMusicInfoVO = rhyaMusicInfoVOInput;
                    }else {
                        rhyaMusicInfoVO = nextMusicChecker();
                    }

                    if (rhyaMusicInfoVO != null) {
                        mMediaPlayer.reset();

                        isEndCheck = false;


                        playMp3OrUrl(rhyaMusicInfoVO.getUuid());
                        playSongCount(rhyaMusicInfoVO.getUuid());


                        mMediaPlayer.prepareAsync();

                        nowPlayMusicUUID = rhyaMusicInfoVO.getUuid();

                        mMediaPlayer.setOnPreparedListener(mp -> {
                            try {
                                // Activity 활성화 확인
                                if (isActivityAlive()) {
                                    ((ActivityMain) activity).setNowPlayMusic();
                                    ((ActivityMain) activity).dismissTaskDialog();
                                }


                                mp.start();

                                if (!wifiLock.isHeld()) wifiLock.acquire();

                                if (isFirstStartMusic) isFirstStartMusic = false;

                                // Activity 활성화 확인
                                if (isActivityAlive()) {
                                    ((ActivityMain) activity).setNowPlayMusic();
                                    ((ActivityMain) activity).dismissTaskDialog();

                                    if (((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).serviceUIActivityCheck()) {
                                        ((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).scrollView.fullScroll(ScrollView.FOCUS_UP);
                                    }
                                }

                                // ---------------------- //
                                // -----노래 재생 확인----- //
                                // ---------------------- //
                                songPlayChecker();
                                // ---------------------- //
                                // ---------------------- //
                                // ---------------------- //

                                showNotification(true);
                            }catch (Exception ex) {
                                ex.printStackTrace();

                                if (isActivityAlive()) {
                                    ((ActivityMain) activity).dismissTaskDialog();
                                }
                            }
                        });
                    }else {
                        if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() != 0) {
                            nowPlayMusicUUID = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(0);
                            playMusicIndex = 0;

                            mMediaPlayer.reset();

                            playSongCount(nowPlayMusicUUID);
                            playMp3OrUrl(nowPlayMusicUUID);

                            mMediaPlayer.prepareAsync();
                            mMediaPlayer.setOnPreparedListener(mp -> {
                                try {
                                    // Activity 활성화 확인
                                    if (isActivityAlive()) {
                                        ((ActivityMain) activity).setNowPlayMusic();
                                        ((ActivityMain) activity).dismissTaskDialog();
                                    }

                                    mp.start();

                                    if (!wifiLock.isHeld()) wifiLock.acquire();

                                    if (isFirstStartMusic) isFirstStartMusic = false;

                                    // Activity 활성화 확인
                                    if (isActivityAlive()) {
                                        ((ActivityMain) activity).setNowPlayMusic();
                                        ((ActivityMain) activity).dismissTaskDialog();

                                        if (((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).serviceUIActivityCheck()) {
                                            ((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).scrollView.fullScroll(ScrollView.FOCUS_UP);
                                        }
                                    }

                                    // ---------------------- //
                                    // -----노래 재생 확인----- //
                                    // ---------------------- //
                                    songPlayChecker();
                                    // ---------------------- //
                                    // ---------------------- //
                                    // ---------------------- //

                                    showNotification(true);
                                }catch (Exception ex) {
                                    ex.printStackTrace();

                                    if (isActivityAlive()) {
                                        ((ActivityMain) activity).dismissTaskDialog();
                                    }
                                }
                            });
                        }else {
                            if (isActivityAlive()) {
                                ((ActivityMain) activity).dismissTaskDialog();
                            }
                        }
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    if (toast != null) {
                        toast.cancel();
                        toast.setText("Service Error! 00041");
                        toast.show();
                    }

                    if (isActivityAlive()) {
                        ((ActivityMain) activity).dismissTaskDialog();
                    }
                }
            }else {
                if (toast != null) {
                    toast.cancel();
                    toast.setText("현재 노래 재생 세션에 접근할 수 없습니다.");
                    toast.show();
                }

                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }
        });
    }
    public void putNewSongNoPlay(String uuid) {
        // Activity 활성화 확인
        if (isActivityAlive()) {
            ((ActivityMain) activity).showTaskDialog();
        }

        accessPermissionChecker(result -> {
            if (result == null) {
                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }else if (result.equals("true")) {
                try {
                    RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);

                    if (rhyaMusicInfoVO != null) {
                        int getSize = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size();

                        RhyaApplication.addRhyaNowPlayMusicUUIDArrayList(uuid);
                        saveNowPlayListFile();

                        if (getSize == 0) {
                            nowPlayMusicUUID = uuid;
                            playMusicIndex = 0;

                            putNewTrack(uuid);

                            return;
                        }

                        if (isActivityAlive()) {
                            ((ActivityMain) activity).dismissTaskDialog();
                        }
                    }
                } catch (Exception ex) {
                    ex.printStackTrace();

                    if (toast != null) {
                        toast.cancel();
                        toast.setText("Service Error! 00069");
                        toast.show();
                    }

                    if (isActivityAlive()) {
                        ((ActivityMain) activity).dismissTaskDialog();
                    }
                }
            }else {
                if (toast != null) {
                    toast.cancel();
                    toast.setText("현재 노래 재생 세션에 접근할 수 없습니다.");
                    toast.show();
                }

                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }
        });
    }
    public void noBufferedTimePlay(int time) {
        if (nowPlayMusicUUID == null || mMediaPlayer == null) return;

        if (telephonyManager != null) {
            if (telephonyManager.getCallState() == TelephonyManager.CALL_STATE_RINGING ||
                    telephonyManager.getCallState() == TelephonyManager.CALL_STATE_OFFHOOK) {

                if (toast != null) {
                    toast.cancel();
                    toast.setText("통화 중에는 재생할 수 없습니다.");
                    toast.show();
                }

                return;
            }
        }

        // Activity 활성화 확인
        if (isActivityAlive()) {
            ((ActivityMain) activity).showTaskDialog();
        }

        accessPermissionChecker(result -> {
            if (result == null) {
                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }else if (result.equals("true")) {
                try {
                    if (mMediaPlayer.isPlaying()) {
                        dontFocusAbandon = true;
                        mMediaPlayer.pause();
                    }

                    bufferingCheckValue = time;

                    isBufferingChecking = true;
                }catch (Exception ex) {
                    ex.printStackTrace();

                    if (toast != null) {
                        toast.cancel();
                        toast.setText("Service Error! 00070");
                        toast.show();
                    }

                    if (isActivityAlive()) {
                        ((ActivityMain) activity).dismissTaskDialog();
                    }
                }
            }else {
                if (toast != null) {
                    toast.cancel();
                    toast.setText("현재 노래 재생 세션에 접근할 수 없습니다.");
                    toast.show();
                }

                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }
        });
    }



    public void showNotificationInit() {
        NotificationCompat.Builder notification = new NotificationCompat.Builder(getApplicationContext(), "kro_kr_rhya_network_utaite_player_id")
                .setSmallIcon(R.drawable.img_up_logo_sub)
                .setContentText("Utaite Player 앱이 실행 중입니다.")
                .setOnlyAlertOnce(true)
                .setShowWhen(false)
                .setPriority(NotificationCompat.PRIORITY_DEFAULT);
        // Android 오레오 버전 검사
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
            NotificationChannel channel = new NotificationChannel("kro_kr_rhya_network_utaite_player_id", "kro_kr_rhya_network_utaite_player", NotificationManager.IMPORTANCE_NONE);
            channel.setVibrationPattern(new long[]{0});
            channel.enableVibration(false);

            NotificationManager mNotificationManager = ((NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE));
            mNotificationManager.createNotificationChannel(channel);

            mNotificationManager.notify(1, notification.build());
        }

        startForeground(1, notification.build());
    }
    @SuppressLint("UnspecifiedImmutableFlag")
    private void showNotification(boolean setMetaData) {
        try {
            Intent mainIntent = new Intent(getApplicationContext(), ActivityMain.class);
            mainIntent.setAction(Intent.ACTION_MAIN);
            mainIntent.addCategory(Intent.CATEGORY_LAUNCHER);
            mainIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            PendingIntent pendingIntent;
            if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.M)
                pendingIntent = PendingIntent.getActivity(this, 0, mainIntent, PendingIntent.FLAG_IMMUTABLE);
            else
                pendingIntent = PendingIntent.getBroadcast(this, 0, mainIntent, PendingIntent.FLAG_UPDATE_CURRENT);


            RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(nowPlayMusicUUID);

            // 재생 여부 확인
            int iconPlayAndPause;
            String eventPlayAndPause;
            String nextEventPlayAndPause;

            if (mMediaPlayer.isPlaying()) {
                iconPlayAndPause = R.drawable.ic_baseline_pause_24;
                eventPlayAndPause = "Pause";
                nextEventPlayAndPause = ACTION_PAUSE;
                playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_PLAYING, 1, 1);
            }else {
                iconPlayAndPause = R.drawable.ic_baseline_play_arrow_24;
                eventPlayAndPause = "Play";
                nextEventPlayAndPause = ACTION_PLAY;

                playbackStateCompatBuilder.setState(PlaybackStateCompat.STATE_PAUSED, 1, 1);
            }


            // 버튼 클릭
            Intent intentPrevious = new Intent(this, RhyaNotificationActionService.class).setAction(ACTION_PREVIUOS);
            PendingIntent pendingIntentPrevious;
            if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.M)
                pendingIntentPrevious = PendingIntent.getBroadcast(this, 0, intentPrevious, PendingIntent.FLAG_IMMUTABLE);
            else
                pendingIntentPrevious = PendingIntent.getBroadcast(this, 0, intentPrevious, PendingIntent.FLAG_UPDATE_CURRENT);

            PendingIntent pendingIntentPlay;
            Intent intentPlay = new Intent(this, RhyaNotificationActionService.class).setAction(nextEventPlayAndPause);
            if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.M)
                pendingIntentPlay = PendingIntent.getBroadcast(this, 0, intentPlay, PendingIntent.FLAG_IMMUTABLE);
            else
                pendingIntentPlay = PendingIntent.getBroadcast(this, 0, intentPlay, PendingIntent.FLAG_UPDATE_CURRENT);

            PendingIntent pendingIntentNext;
            Intent intentNext = new Intent(this, RhyaNotificationActionService.class).setAction(ACTION_NEXT);
            if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.M)
                pendingIntentNext = PendingIntent.getBroadcast(this, 0, intentNext, PendingIntent.FLAG_IMMUTABLE);
            else
                pendingIntentNext = PendingIntent.getBroadcast(this, 0, intentNext, PendingIntent.FLAG_UPDATE_CURRENT);

            PendingIntent pendingIntentClose;
            Intent intentClose = new Intent(this, RhyaNotificationActionService.class).setAction(ACTION_CLOSE);
            if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.M)
                pendingIntentClose = PendingIntent.getBroadcast(this, 0, intentClose, PendingIntent.FLAG_IMMUTABLE);
            else
                pendingIntentClose = PendingIntent.getBroadcast(this, 0, intentClose, PendingIntent.FLAG_UPDATE_CURRENT);

            // Notification 세팅
            if (rhyaMusicInfoVO != null) {
                if (notification == null) {
                    notification = new NotificationCompat.Builder(getApplicationContext(), "kro_kr_rhya_network_utaite_player_id")
                            .setSmallIcon(R.drawable.img_up_logo_sub)
                            .setContentTitle(rhyaMusicInfoVO.getName())
                            .setContentText(rhyaMusicInfoVO.getSinger())
                            .setContentIntent(pendingIntent)
                            .addAction(R.drawable.ic_baseline_next_arrow_l_24, "Previous", pendingIntentPrevious)
                            .addAction(iconPlayAndPause, eventPlayAndPause, pendingIntentPlay)
                            .addAction(R.drawable.ic_baseline_next_arrow_r_24, "Next", pendingIntentNext)
                            .addAction(R.drawable.ic_baseline_close_24, "Close", pendingIntentClose)
                            .setPriority(NotificationCompat.PRIORITY_DEFAULT);

                    mediaStyle.setMediaSession(mediaSession.getSessionToken());
                    mediaStyle.setShowActionsInCompactView(0, 1, 2);
                    notification.setStyle(mediaStyle);
                }else {
                    notification.clearActions();
                    notification.setContentTitle(rhyaMusicInfoVO.getName())
                            .setContentText(rhyaMusicInfoVO.getSinger())
                            .addAction(R.drawable.ic_baseline_next_arrow_l_24, "Previous", pendingIntentPrevious)
                            .addAction(iconPlayAndPause, eventPlayAndPause, pendingIntentPlay)
                            .addAction(R.drawable.ic_baseline_next_arrow_r_24, "Next", pendingIntentNext)
                            .addAction(R.drawable.ic_baseline_close_24, "Close", pendingIntentClose);

                }

                Glide.with(this)
                        .asBitmap()
                        .load(rhyaMusicInfoVO.getImage())
                        .into(new CustomTarget<Bitmap>() {
                            @Override
                            public void onResourceReady(@NonNull Bitmap resource, @Nullable Transition<? super Bitmap> transition) {
                                try {
                                    int reSize = (resource.getHeight() + resource.getWidth()) / 2;

                                    resource = Bitmap.createScaledBitmap(resource, reSize / 4, reSize / 4, false);


                                    if (setMetaData) {
                                        metaDataBuilder.putString(MediaMetadataCompat.METADATA_KEY_ALBUM_ARTIST, rhyaMusicInfoVO.getSongWriter());
                                        metaDataBuilder.putString(MediaMetadataCompat.METADATA_KEY_AUTHOR, rhyaMusicInfoVO.getSongWriter());
                                        metaDataBuilder.putString(MediaMetadataCompat.METADATA_KEY_COMPOSER, rhyaMusicInfoVO.getSongWriter());
                                        metaDataBuilder.putString(MediaMetadataCompat.METADATA_KEY_ARTIST, rhyaMusicInfoVO.getSinger());
                                        metaDataBuilder.putString(MediaMetadataCompat.METADATA_KEY_ALBUM, rhyaMusicInfoVO.getName());
                                        metaDataBuilder.putString(MediaMetadataCompat.METADATA_KEY_TITLE, rhyaMusicInfoVO.getName());
                                        metaDataBuilder.putString(MediaMetadataCompat.METADATA_KEY_DISPLAY_TITLE, rhyaMusicInfoVO.getName());
                                        metaDataBuilder.putString(MediaMetadataCompat.METADATA_KEY_DISPLAY_DESCRIPTION, rhyaMusicInfoVO.getSinger());
                                        metaDataBuilder.putLong(MediaMetadataCompat.METADATA_KEY_DURATION, mMediaPlayer.getDuration());
                                        metaDataBuilder.putBitmap(MediaMetadataCompat.METADATA_KEY_ART, resource);
                                        mediaSession.setMetadata(metaDataBuilder.build());
                                    }
                                    mediaSession.setPlaybackState(playbackStateCompatBuilder.build());

                                    // Android 오레오 버전 검사
                                    if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
                                        NotificationChannel channel = new NotificationChannel("kro_kr_rhya_network_utaite_player_id", "kro_kr_rhya_network_utaite_player", NotificationManager.IMPORTANCE_DEFAULT);
                                        channel.setVibrationPattern(new long[]{0});
                                        channel.enableVibration(false);

                                        NotificationManager mNotificationManager = ((NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE));
                                        mNotificationManager.createNotificationChannel(channel);

                                        mNotificationManager.notify(1, notification.build());
                                    }

                                    resource.recycle();

                                    startForeground(1, notification.build());
                                }catch (Exception ex) {
                                    ex.printStackTrace();

                                    if (toast != null) {
                                        toast.cancel();
                                        toast.setText("Service Error! 00047");
                                        toast.show();
                                    }

                                    stopService(false);
                                }
                            }

                            @Override
                            public void onLoadCleared(@Nullable Drawable placeholder) {
                            }
                        });
            }
        }catch (Exception ex) {
            ex.printStackTrace();

            if (toast != null) {
                toast.cancel();
                toast.setText("Service Error! 00046");
                toast.show();
            }

            stopService(false);
        }
    }




    @Override
    public void onDestroy() {
        super.onDestroy();

        stopService(false);
    }




    /*-------------------------------------------------------------------------------------------------*/
    /*--------------------------------------------- Utils ---------------------------------------------*/
    /*-------------------------------------------------------------------------------------------------*/
    public RhyaMusicInfoVO previousMusicChecker() {
        if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() <= 0) {
            return null;
        }

        if (playMusicIndex - 1 < 0) {
            return null;
        }

        playMusicIndex = playMusicIndex - 1;

        saveNowPlayListFile();

        return RhyaApplication.rhyaMusicInfoVOHashMap.get(RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(playMusicIndex));
    }

    public RhyaMusicInfoVO nextMusicChecker() {
        if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() <= 0) {
            return null;
        }

        if (playMusicIndex + 1 > RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() - 1) {
            return null;
        }

        playMusicIndex = playMusicIndex + 1;

        saveNowPlayListFile();

        return RhyaApplication.rhyaMusicInfoVOHashMap.get(RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(playMusicIndex));
    }

    private boolean isActivityAlive() {
        // Activity null check
        if (activity != null) {
            // Activity 활성화 확인
            return !activity.isFinishing();
        }

        return false;
    }

    private void mainActivityProgressbarThread() {
        scheduledExecutorService = Executors.newScheduledThreadPool(2);

        Runnable task = () -> {
            try {
                // Check activity
                if (isActivityAlive() && mMediaPlayer != null) {
                    // Check play music
                    if (mMediaPlayer.isPlaying()) {
                        long endTime = mMediaPlayer.getDuration();
                        long startTime = mMediaPlayer.getCurrentPosition();

                        // Main activity progressbar null check
                        if (((ActivityMain) activity).musicProgressBar != null) {
                            ((ActivityMain) activity).musicProgressBar.setMax((int) endTime);
                            ((ActivityMain) activity).musicProgressBar.setProgress((int) startTime);
                        }

                        // Music info fragment seekBar setting
                        if (!((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).seekBarMoveChecker) {
                            ((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).setPlaySeekBarInfo(startTime, endTime);
                        }
                    }

                    // Music info fragment textview setting check
                    ((PlayInfoFragment) ((ActivityMain) activity).playInfoFragment).checkTextViewSetting();
                }
            }catch (Exception ex) {
                ex.printStackTrace();
            }
        };
        Runnable task2 = () -> {
            try {
                // Check activity
                if (isActivityAlive()) {
                    if (((ActivityMain) activity).searchFragment.isAdded()) {
                        if (((SearchFragment)((ActivityMain) activity).searchFragment).isActivityCheck()) {

                            ArrayList<RhyaSingerDataVO> subscribeList = RhyaApplication.rhyaMusicDataVO.getSubscribeList();
                            if (subscribeList != null) {
                                if (subscribeList.size() != 0) {
                                    if (subscribeList.size() - 1 < subscribeIndex) {
                                        subscribeIndex = 0;
                                    }
                                }
                            }

                            ((SearchFragment) ((ActivityMain) activity).searchFragment).setSubscribeList(subscribeIndex);
                            subscribeIndex++;
                        }else {
                            subscribeIndex = 0;
                        }
                    }else {
                        subscribeIndex = 0;
                    }
                }else {
                    subscribeIndex = 0;
                }
            }catch (Exception ex) {
                ex.printStackTrace();
            }
        };
        scheduledExecutorService.scheduleAtFixedRate(task, 0, 300, TimeUnit.MILLISECONDS);
        scheduledExecutorService.scheduleAtFixedRate(task2, 0, 5000, TimeUnit.MILLISECONDS);
    }

    private class MusicIntentReceiver extends BroadcastReceiver {
        @Override
        public void onReceive(Context context, Intent intent) {
            // 헤드샛, 이어폰 등 관련
            if (intent.getAction().equals(Intent.ACTION_HEADSET_PLUG)) {
                int state = intent.getIntExtra("state", -1);
                switch(state) {
                    case 0:
                        // 노래 중지
                        dontFocusAbandon = false;
                        onTrackPause();

                        break;

                    case 1: // 헤드셋 연결
                        break;
                }
            }
        }
    }

    private final class AudioFocusHelper implements AudioManager.OnAudioFocusChangeListener {
        private void requestAudioFocus() {
            if (!mPlayOnAudioFocus) {
                mPlayOnAudioFocus = true;
                mAudioManager.requestAudioFocus(this,
                        AudioManager.STREAM_MUSIC,
                        AudioManager.AUDIOFOCUS_GAIN);
            }
        }

        private void abandonAudioFocus() {
            if (mPlayOnAudioFocus) {
                mPlayOnAudioFocus = false;
                mAudioManager.abandonAudioFocus(this);
            }
        }

        @Override
        public void onAudioFocusChange(int focusChange) {
            float MEDIA_VOLUME_DUCK = 0.2f;
            switch (focusChange) {
                case AudioManager.AUDIOFOCUS_GAIN:
                    if (mPlayOnAudioFocus && !mMediaPlayer.isPlaying()) {
                        onTrackPlay();
                    } else if (mMediaPlayer.isPlaying()) {
                        // Audio Event 볼륨
                        float MEDIA_VOLUME_DEFAULT = 1.0f;
                        mMediaPlayer.setVolume(MEDIA_VOLUME_DEFAULT, MEDIA_VOLUME_DEFAULT);
                    }

                    break;
                case AudioManager.AUDIOFOCUS_LOSS_TRANSIENT_CAN_DUCK:
                    mMediaPlayer.setVolume(MEDIA_VOLUME_DUCK, MEDIA_VOLUME_DUCK);

                    break;
                case AudioManager.AUDIOFOCUS_LOSS_TRANSIENT:
                    if (mMediaPlayer.isPlaying()) {
                        dontFocusAbandon = true;

                        onTrackPause();
                    }

                    break;
                case AudioManager.AUDIOFOCUS_LOSS:
                    if (toast != null) {
                        toast.setText("시스템에서 오디오 기능을 사용해서 음악 재생이 중지되었습니다.");
                        toast.show();
                    }

                    mAudioManager.abandonAudioFocus(this);
                    onTrackPause();
                    mPlayOnAudioFocus = false;
                    break;
            }
        }
    }

    private void registerAudioNoisyReceiver() {
        if (!mAudioNoisyReceiverRegistered) {
            registerReceiver(mAudioNoisyReceiver, new IntentFilter(AudioManager.ACTION_AUDIO_BECOMING_NOISY));
            mAudioNoisyReceiverRegistered = true;
        }
    }

    private void unregisterAudioNoisyReceiver() {
        if (mAudioNoisyReceiverRegistered) {
            unregisterReceiver(mAudioNoisyReceiver);
            mAudioNoisyReceiverRegistered = false;
        }
    }

    private final BroadcastReceiver mAudioNoisyReceiver =
            new BroadcastReceiver() {
                @Override
                public void onReceive(Context context, Intent intent) {
                    if (AudioManager.ACTION_AUDIO_BECOMING_NOISY.equals(intent.getAction())) {
                        if (mMediaPlayer.isPlaying()) {
                            onTrackPause();
                        }
                    }
                }
            };

    public void stopService(boolean checkMainActivity) {
        if (checkMainActivity && !isMainActivityPauseAndResumeCheck) {
            return;
        }
        if (mMediaPlayer != null) {
            if (mMediaPlayer.isPlaying()) {
                return;
            }
        }

        // Progressbar Thread 종료
        scheduledExecutorService.shutdownNow();

        // MediaPlayer 초기화
        if (mMediaPlayer != null) {
            if (mMediaPlayer.isPlaying()) {
                onTrackPause();
            }

            mMediaPlayer.release();
        }

        // Wake lock 해제
        if (wifiLock.isHeld()) wifiLock.release();


        // 리시버 해제 1
        try {
            unregisterReceiver(broadcastReceiver);
        }catch (Exception ex) {
            ex.printStackTrace();
        }

        // 리시버 해제 2
        try {
            unregisterReceiver(musicIntentReceiver);
        }catch (Exception ex) {
            ex.printStackTrace();
        }

        // 리시버 해제 3
        try {
            unregisterAudioNoisyReceiver();
        }catch (Exception ex) {
            ex.printStackTrace();
        }

        if (telephonyManager != null) {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
                telephonyManager.unregisterTelephonyCallback(telephonyCallback);
            }else {
                telephonyManager.listen(phoneStateListener, PhoneStateListener.LISTEN_NONE);
            }
        }

        mMediaPlayer = null;
        notification = null;
        musicIntentReceiver = null;
        playbackStateCompatBuilder = null;
        mediaSession = null;
        isStartService = false;
        metaDataBuilder= null;
        nowPlayMusicUUID = null;
        rhyaAESManager = null;

        if (isActivityAlive()) {
            activity.moveTaskToBack(true);
            activity.finish();
            android.os.Process.killProcess(android.os.Process.myPid());
        }

        // 종료
        stopForeground(true);
        stopSelf();
    }

    public String getMusicURL(String uuid) {
        try {
            StringBuilder sb;
            sb = new StringBuilder();
            sb.append(rhyaCore.MAIN_URL);
            sb.append("?mode=4&auth=");
            sb.append(authToken);
            sb.append("&uuid=");
            sb.append(uuid);

            return sb.toString();
        }catch (Exception ex) {
            ex.printStackTrace();

            return "error";
        }
    }

    public void saveNowPlayListFile() {
        try {
            JSONObject jsonObject = new JSONObject();
            for (int i = 0; i < RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size(); i++) {
                String uuid = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(i);

                jsonObject.put(Integer.toString(i), uuid);
            }

            jsonObject.put("nowindex", playMusicIndex);
            String result = rhyaAESManager.aesEncode(jsonObject.toString());

            // 파일 쓰기 변수 초기화
            BufferedWriter buf;
            FileWriter fw;
            // 파일 쓰기
            fw = new FileWriter(NOW_PLAY_LIST_SAVE_FILE_NAME, false);
            buf = new BufferedWriter(fw);
            buf.append(result);
            buf.close();
            fw.close();
        }catch (Exception ex) {
            ex.printStackTrace();
        }
    }


    private void accessPermissionChecker(AccessPermissionCheckerInterface accessPermissionCheckerInterface) {
        new RhyaAsyncTask<String, String>() {
            @Override
            protected void onPreExecute() {
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    try {
                        RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                        ContentValues urlParm = new ContentValues();

                        urlParm.put("mode", "5");

                        if (!(authToken.length() > 0)) {
                            RhyaCore rhyaCore = new RhyaCore();
                            RhyaSharedPreferences rhyaSharedPreferences = new RhyaSharedPreferences(getApplicationContext(), false);
                            authToken = rhyaCore.getAutoLogin(rhyaSharedPreferences, getApplicationContext());
                        }

                        urlParm.put("auth", authToken);

                        String getResult = rhyaHttpsConnection.request(rhyaCore.MAIN_URL, urlParm);
                        if (getResult != null) {
                            if (getResult.equals("IOException")) {
                                return "true";
                            }
                        }

                        assert getResult != null;
                        JSONObject jsonObject = new JSONObject(getResult);
                        boolean result = jsonObject.getString("result").equals("success");

                        urlParm.put("mode", "10");
                        urlParm.remove("auth");
                        JSONObject jsonObject1 = new JSONObject(rhyaHttpsConnection.request(rhyaCore.MAIN_URL, urlParm));
                        if (!rhyaMainUtils.updateChecker(jsonObject1.getString("version"))) {
                            return "unkError";
                        }
                        if (!jsonObject1.getString("access").equals("1")) {
                            return "unkError";
                        }

                        if (result)  return "true";
                        else return "false";
                    } catch (JSONException e) {
                        e.printStackTrace();

                        return "jsonError";
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    return "unkError";
                }
            }


            @Override
            protected void onPostExecute(String result) {
                if (result.equals("jsonError")) {
                    if (toast != null) {
                        toast.cancel();
                        toast.setText("서버의 정보를 얻어오는 데 실패하였습니다. (00064)");
                        toast.show();
                    }
                    result = null;
                }else if (result.equals("unkError")) {
                    if (toast != null) {
                        toast.cancel();
                        toast.setText("알 수 없는 오류가 발생하였습니다. (00063)");
                        toast.show();
                    }
                    result = null;
                }

                accessPermissionCheckerInterface.taskEndEventListener(result);
            }
        }.execute(null);
    }
    private interface AccessPermissionCheckerInterface {
        void taskEndEventListener(String result);
    }

    private PhoneStateListener phoneStateListener;
    private CustomTelephonyCallback telephonyCallback;
    @RequiresApi(Build.VERSION_CODES.S)
    private class CustomTelephonyCallback extends TelephonyCallback implements TelephonyCallback.CallStateListener {
        private final CallBack mCallBack;

        public CustomTelephonyCallback(CallBack callBack) {
            mCallBack = callBack;
        }

        @Override
        public void onCallStateChanged(int state) {

            mCallBack.callStateChanged(state);

        }
    }
    interface CallBack {
        void callStateChanged(int state);
    }
    private void phoneStateListenerTask(int state) {
        if (state == TelephonyManager.CALL_STATE_RINGING || state == TelephonyManager.CALL_STATE_OFFHOOK) {
            if (mMediaPlayer != null) {
                if (mMediaPlayer.isPlaying()) {
                    isCallPlayerCheck = true;

                    dontFocusAbandon = false;
                    onTrackPause();
                }
            }
        }else if (state == TelephonyManager.CALL_STATE_IDLE) {
            if (mMediaPlayer != null) {
                if (isCallPlayerCheck) {
                    if (!mMediaPlayer.isPlaying()) {
                        onTrackPlay();
                    }

                    isCallPlayerCheck = false;
                }
            }
        }
    }

    public boolean mp3DownloadChecker(String uuid) {
        readMp3List();

        if (downloadFileList == null) return false;
        if (downloadFileList.size() == 0) return false;

        return downloadFileList.contains(uuid);
    }
    public void readMp3List() {
        StringBuilder sb;
        sb = new StringBuilder();
        sb.append(getFilesDir().getAbsolutePath());
        sb.append(File.separator);
        sb.append("music");

        File root = new File(sb.toString());

        if (root.exists()) {
            File[] deleteFolderList = root.listFiles();

            if (deleteFolderList != null) {
                for (File file : deleteFolderList) {
                    downloadFileList.add(file.getName().replace(".rhya_music", ""));
                }
            }
        }
    }
    public void mp3Downloader(String uuid) {
        StringBuilder sb = new StringBuilder();
        sb.append(rhyaCore.MAIN_URL);
        sb.append("?mode=8&auth=");
        sb.append(authToken);
        sb.append("&uuid=");
        sb.append(uuid);

        new RhyaAsyncTask<String, String>() {
            @Override
            protected void onPreExecute() {
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    URL url = new URL(sb.toString());

                    sb.setLength(0);

                    sb.append(getFilesDir().getAbsolutePath());
                    sb.append(File.separator);
                    sb.append("music");

                    File dir = new File(sb.toString());
                    if (!dir.exists()) dir.mkdir();

                    sb.append(File.separator);
                    sb.append(uuid);
                    sb.append(".rhya_music");

                    File file = new File(sb.toString());

                    URLConnection connection = url.openConnection();
                    connection.connect();
                    int count;
                    // download the file
                    InputStream input = new BufferedInputStream(url.openStream(),
                            8192);
                    // Output stream
                    OutputStream output = new FileOutputStream(file, false);
                    byte[] data = new byte[1024];
                    while ((count = input.read(data)) != -1) {
                        // writing data to file
                        output.write(data, 0, count);
                    }
                    // flushing output
                    output.flush();
                    // closing streams
                    output.close();
                    input.close();
                }catch (Exception ex) {
                    ex.printStackTrace();

                    return "unkError";
                }

                return "success";
            }


            @Override
            protected void onPostExecute(String result) {
                if (result.equals("success")) {
                    if (!downloadFileList.contains(uuid)) {
                        downloadFileList.add(uuid);
                    }
                }
            }
        }.execute(null);
    }
    private void playMp3OrUrl(String uuid) throws IOException {
        StringBuilder sb = new StringBuilder();
        if (mp3DownloadChecker(uuid)) {
            sb.append(getFilesDir().getAbsolutePath());
            sb.append(File.separator);
            sb.append("music");
            sb.append(File.separator);
            sb.append(uuid);
            sb.append(".rhya_music");
            File file = new File(sb.toString());
            if (file.exists()) {
                isMp3FilePlay = true;
                mMediaPlayer.setDataSource(sb.toString());
            }else {
                isMp3FilePlay = false;
                mp3Downloader(uuid);
                mMediaPlayer.setDataSource(getMusicURL(uuid));
            }
        }else {
            isMp3FilePlay = false;
            mp3Downloader(uuid);
            mMediaPlayer.setDataSource(getMusicURL(uuid));
        }
    }
    private int musicRandomPlayIndexGet() {
        return new Random().nextInt(RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size());
    }
    public void playSongCount(String uuid) {
        new RhyaAsyncTask<String, String>() {
            @Override
            protected void onPreExecute() {
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                    ContentValues urlParm = new ContentValues();

                    urlParm.put("mode", "11");

                    if (!(authToken.length() > 0)) {
                        RhyaCore rhyaCore = new RhyaCore();
                        RhyaSharedPreferences rhyaSharedPreferences = new RhyaSharedPreferences(getApplicationContext(), false);
                        authToken = rhyaCore.getAutoLogin(rhyaSharedPreferences, getApplicationContext());
                    }

                    urlParm.put("auth", authToken);
                    urlParm.put("music", uuid);

                    rhyaHttpsConnection.request(rhyaCore.MAIN_URL, urlParm);
                } catch (Exception ex) {
                    ex.printStackTrace();
                }

                return null;
            }

            @Override
            protected void onPostExecute(String result) {

            }
        }.execute(null);
    }
    private void songPlayChecker() {
        // 노래 재생 허용 확인
        // -------------------------------------------------------------------------------------------------- //
        accessPermissionChecker(result -> {
            if (result == null) {
                // 노래 재생 중지
                dontFocusAbandon = true;
                mMediaPlayer.pause();

                if (toast != null) {
                    toast.cancel();
                    toast.setText("현재 노래 재생 세션에 접근할 수 없습니다.");
                    toast.show();
                }

                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }else if (!result.equals("true")) {
                // 노래 재생 중지
                dontFocusAbandon = false;
                mMediaPlayer.pause();

                if (toast != null) {
                    toast.cancel();
                    toast.setText("현재 노래 재생 세션에 접근할 수 없습니다.");
                    toast.show();
                }

                // Activity 활성화 확인
                if (isActivityAlive()) {
                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }
        });
        // -------------------------------------------------------------------------------------------------- //
    }
    /*-------------------------------------------------------------------------------------------------*/
    /*-------------------------------------------------------------------------------------------------*/
    /*-------------------------------------------------------------------------------------------------*/
}
