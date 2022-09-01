package kro.kr.rhya_network.utaiteplayer.activity;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.Dialog;
import android.content.ComponentName;
import android.content.ContentValues;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.graphics.Color;
import android.graphics.Insets;
import android.graphics.Point;
import android.graphics.PorterDuff;
import android.graphics.Rect;
import android.graphics.drawable.ColorDrawable;
import android.net.ConnectivityManager;
import android.net.Network;
import android.net.NetworkInfo;
import android.net.NetworkRequest;
import android.os.Build;
import android.os.Bundle;
import android.os.IBinder;
import android.util.Size;
import android.view.Gravity;
import android.view.View;
import android.view.Window;
import android.view.WindowInsets;
import android.view.WindowManager;
import android.view.WindowMetrics;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.PopupMenu;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.view.ContextThemeWrapper;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.viewpager2.widget.ViewPager2;

import com.bumptech.glide.Glide;
import com.bumptech.glide.signature.ObjectKey;
import com.google.android.material.tabs.TabLayout;
import com.google.firebase.messaging.FirebaseMessaging;
import com.pnikosis.materialishprogress.ProgressWheel;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.File;
import java.io.UnsupportedEncodingException;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.net.URLDecoder;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Objects;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaNewSongAdapter;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaViewPagerAdapter;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.core.RhyaCore;
import kro.kr.rhya_network.utaiteplayer.fragment.HomeFragment;
import kro.kr.rhya_network.utaiteplayer.fragment.MyPlayListFragment;
import kro.kr.rhya_network.utaiteplayer.fragment.MyPlayListInfoFragment;
import kro.kr.rhya_network.utaiteplayer.fragment.NowPlayListFragment;
import kro.kr.rhya_network.utaiteplayer.fragment.PlayInfoFragment;
import kro.kr.rhya_network.utaiteplayer.fragment.RequestFragment;
import kro.kr.rhya_network.utaiteplayer.fragment.SearchFragment;
import kro.kr.rhya_network.utaiteplayer.fragment.SearchInputFragment;
import kro.kr.rhya_network.utaiteplayer.fragment.SearchResultFragment;
import kro.kr.rhya_network.utaiteplayer.lib.com_github_amarjain07_StickyScrollView_1_0_2.ui.interfaces.IScrollViewListener;
import kro.kr.rhya_network.utaiteplayer.service.PlayerService;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAESManager;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaDialogManager;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHttpsConnection;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMainUtils;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaPlayList;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSharedPreferences;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSingerDataVO;


public class ActivityMain extends AppCompatActivity {
    // Fragment
    public Fragment homeFragment;
    public Fragment searchFragment;
    public Fragment searchInputFragment;
    public Fragment myPlayListFragment;
    public Fragment requestFragment;
    public Fragment playInfoFragment;
    public Fragment nowPlayListFragment;
    public Fragment searchResultFragment;
    public Fragment myPlayListInfoFragment;
    // Activity
    private Activity activity;
    // Context
    public Context context;
    // RHYA Utils
    public RhyaCore rhyaCore;
    public RhyaMainUtils rhyaMainUtils;
    public RhyaDialogManager rhyaDialogManager;
    public RhyaSharedPreferences rhyaSharedPreferences;
    public RhyaAESManager rhyaAESManager;
    // 뒤로가기 버튼 시간
    private long backBtnTime = 0;
    // Color
    private final int tabItemSelectColor = Color.parseColor("#919191");
    private final int tabItemUnSelectColor = Color.parseColor("#000000");
    // Service
    public boolean playerServiceBindTOF = false;
    public PlayerService playerService = null;
    // UI Object
    private ConstraintLayout topPanel;
    private ConstraintLayout playMusicPanel;
    private kro.kr.rhya_network.utaiteplayer.utils.RhyaMarqueeTextView songTitle;
    private ImageButton playlistAllDelete;
    private TextView songSinger;
    private ImageButton musicBackButton;
    private ImageButton playAndPauseImageBtn;
    private ImageButton musicNextButton;
    private TabLayout mainTabLayout;
    private ViewPager2 viewPager2;
    public ProgressBar musicProgressBar = null;
    public Button playListSongRemoveButton;
    public ImageButton playListAddRandomSongButton;
    // Task Dialog
    private Dialog mainActivityTaskDialog;
    // 강제 재생 확인
    private boolean isPlayPower = false;
    private String playPowerUUID = null;
    // 이전 Fragment index
    private int backFragmentIndex = 0;
    // 현재 Fragment index
    private int nowFragmentIndex = 0;
    // Toast message
    private Toast toast;
    // 네트워크 확인
    private Dialog networkConnectionDialog;
    // PlayList info
    public RhyaPlayList rhyaPlayList;
    public boolean isPlayListScrollToTop = false;




    @SuppressWarnings("ResultOfMethodCallIgnored")
    @SuppressLint("SetJavaScriptEnabled")
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        ImageView viewPlayInfoImageView = findViewById(R.id.viewPlayInfoImageView);
        ImageButton settingButton = findViewById(R.id.settingButton);
        topPanel = findViewById(R.id.topPanel);
        playlistAllDelete = findViewById(R.id.playlistDeleteButton);
        songTitle = findViewById(R.id.songName);
        songSinger = findViewById(R.id.singerName);
        musicBackButton = findViewById(R.id.nextArrowLImageView);
        playAndPauseImageBtn = findViewById(R.id.PlayAndPauseImageView);
        musicNextButton = findViewById(R.id.nextArrowRImageView);
        ImageButton playListButton = findViewById(R.id.playListButton);
        musicProgressBar = findViewById(R.id.musicProgressBar);
        playMusicPanel = findViewById(R.id.playMusicPanel);
        playListSongRemoveButton = findViewById(R.id.playListSongRemoveButton);
        playListAddRandomSongButton = findViewById(R.id.playlistAddRandomMusicButton);

        toast = Toast.makeText(getApplicationContext(), null, Toast.LENGTH_SHORT);


        context = this;
        activity = this;

        initTaskDialog();
        initTaskDialogForNetworkManager();


        NetworkRequest.Builder networkRequestBuilder = new NetworkRequest.Builder();
        ConnectivityManager connectivityManager = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        connectivityManager.registerNetworkCallback(networkRequestBuilder.build(), new ConnectivityManager.NetworkCallback() {
            @Override
            public void onAvailable(@NonNull Network network) {
                // 네트워크를 사용할 준비가 되었을 때
                if (!activity.isFinishing())
                    activity.runOnUiThread(() -> dismissTaskDialogForNetworkManager());
            }

            @Override
            public void onLost(@NonNull Network network) {
                // 네트워크가 끊겼을 때
                if (!activity.isFinishing())
                    activity.runOnUiThread(() -> showTaskDialogForNetworkManager());
            }
        });


        rhyaCore = new RhyaCore();
        rhyaMainUtils = new RhyaMainUtils();
        rhyaDialogManager = new RhyaDialogManager(activity);
        rhyaSharedPreferences = new RhyaSharedPreferences(context, false);
        rhyaAESManager = new RhyaAESManager(context);

        viewPager2 = findViewById(R.id.mainViewPager);
        viewPager2.setUserInputEnabled(false);


        if (homeFragment == null) homeFragment = HomeFragment.newInstance();
        if (searchFragment == null) searchFragment = SearchFragment.newInstance();
        if (searchInputFragment == null) searchInputFragment = SearchInputFragment.newInstance();
        if (myPlayListFragment == null) myPlayListFragment = MyPlayListFragment.newInstance();
        if (requestFragment == null) requestFragment = RequestFragment.newInstance();
        if (nowPlayListFragment == null) nowPlayListFragment = NowPlayListFragment.newInstance();
        if (playInfoFragment == null) playInfoFragment = PlayInfoFragment.newInstance();
        if (searchResultFragment == null) searchResultFragment = SearchResultFragment.newInstance();
        if (myPlayListInfoFragment == null) myPlayListInfoFragment = MyPlayListInfoFragment.newInstance();


        RhyaViewPagerAdapter rhyaViewPagerAdapter = new RhyaViewPagerAdapter(getSupportFragmentManager(), getLifecycle());
        rhyaViewPagerAdapter.addFragment(homeFragment);
        rhyaViewPagerAdapter.addFragment(searchFragment);
        rhyaViewPagerAdapter.addFragment(myPlayListFragment);
        rhyaViewPagerAdapter.addFragment(requestFragment);
        rhyaViewPagerAdapter.addFragment(nowPlayListFragment);
        rhyaViewPagerAdapter.addFragment(playInfoFragment);
        rhyaViewPagerAdapter.addFragment(searchInputFragment);
        rhyaViewPagerAdapter.addFragment(searchResultFragment);
        rhyaViewPagerAdapter.addFragment(myPlayListInfoFragment);

        viewPager2.setAdapter(rhyaViewPagerAdapter);

        mainTabLayout = findViewById(R.id.mainTabLayout);
        mainTabLayout.setTabRippleColor(null);



        Objects.requireNonNull(
                Objects.requireNonNull(
                        mainTabLayout.getTabAt(0)
                ).getIcon()
        ).setColorFilter(tabItemSelectColor, PorterDuff.Mode.SRC_IN);


        for (int i = 1; i < mainTabLayout.getTabCount(); i++) {
            Objects.requireNonNull(
                    Objects.requireNonNull(
                            mainTabLayout.getTabAt(i)
                    ).getIcon()
            ).setColorFilter(tabItemUnSelectColor, PorterDuff.Mode.SRC_IN);
        }


        mainTabLayout.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override
            public void onTabSelected(TabLayout.Tab tab) {
                // 색상 변경
                colorChange(tab);
                // Fragment 변경
                changeFragment(tab.getPosition());
            }

            @Override
            public void onTabUnselected(TabLayout.Tab tab) {
                // 색상 변경
                Objects.requireNonNull(
                        tab.getIcon()
                ).setColorFilter(tabItemUnSelectColor, PorterDuff.Mode.SRC_IN);
            }

            @Override
            public void onTabReselected(TabLayout.Tab tab) {
                // 색상 변경
                colorChange(tab);
                // Fragment 변경
                changeFragment(tab.getPosition());
            }



            private void colorChange(TabLayout.Tab tab) {
                // 색상 변경
                Objects.requireNonNull(
                        tab.getIcon()
                ).setColorFilter(tabItemSelectColor, PorterDuff.Mode.SRC_IN);
            }
            private void changeFragment(int pos) {
                nowFragmentIndex = pos;

                // Fragment 변경
                switch (pos)
                {
                    case 0 : // Home
                        backFragmentIndex = 0;
                        // Top panel visibility
                        topPanel.setVisibility(View.VISIBLE);

                        break;
                    case 1 : // Search
                        backFragmentIndex = 1;
                        // Top panel visibility
                        topPanel.setVisibility(View.GONE);

                        break;
                    case 2 : // My play list
                        backFragmentIndex = 2;
                        // Top panel visibility
                        topPanel.setVisibility(View.GONE);

                        break;
                    case 3 : // Request
                        backFragmentIndex = 3;
                        // Top panel visibility
                        topPanel.setVisibility(View.GONE);

                        break;
                }

                setStatePlayListSongsRemoveButton(false);

                viewPager2.setCurrentItem(backFragmentIndex, false);

                musicProgressBar.setVisibility(View.VISIBLE);
                playMusicPanel.setVisibility(View.VISIBLE);

                playlistAllDelete.setVisibility(View.INVISIBLE);
                playListAddRandomSongButton.setVisibility(View.INVISIBLE);
            }
        });


        Context wrapper = new ContextThemeWrapper(this, R.style.RoundPopupMenuStyle);
        PopupMenu settingPopupMenu = new PopupMenu(wrapper, settingButton, Gravity.END);
        try {
            Field[] fields = settingPopupMenu.getClass().getDeclaredFields();
            for (Field field : fields) {
                if ("mPopup".equals(field.getName())) {
                    field.setAccessible(true);
                    Object menuPopupHelper = field.get(settingPopupMenu);
                    assert menuPopupHelper != null;
                    Class<?> classPopupHelper = Class.forName(menuPopupHelper.getClass().getName());
                    Method setForceIcons = classPopupHelper.getMethod("setForceShowIcon", boolean.class);
                    setForceIcons.invoke(menuPopupHelper, true);

                    break;
                }
            }
        } catch (Exception e) {
            e.printStackTrace();

            rhyaDialogManager.createDialog_Confirm(context,
                    "Unknown Error",
                    "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00037)",
                    "종료",
                    false,
                    dialog -> {
                        moveTaskToBack(true);
                        finish();
                        android.os.Process.killProcess(android.os.Process.myPid());
                    });
        }


        getMenuInflater().inflate(R.menu.menu_setting_popupmenu, settingPopupMenu.getMenu());


        settingButton.setOnClickListener(v -> settingPopupMenu.show());
        settingPopupMenu.setOnMenuItemClickListener(item -> {
            if (item.getTitle().equals("공지사항")) {
                try {
                    try {
                        rhyaSharedPreferences.setStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_USER_DATA_CHANGE, rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE, context);

                        rhyaDialogManager.createDialog_WebView(context,
                                rhyaCore.getAnnouncementURL(rhyaSharedPreferences, context),
                                "공지사항",
                                true,
                                rhyaCore,
                                null);
                    } catch (NoSuchPaddingException | InvalidKeyException |
                            UnsupportedEncodingException | IllegalBlockSizeException |
                            BadPaddingException | NoSuchAlgorithmException | InvalidAlgorithmParameterException e) {
                        e.printStackTrace();

                        rhyaDialogManager.createDialog_Confirm(context,
                                "AES Decryption Error",
                                "데이터를 복호화하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00076)",
                                "닫기",
                                false,
                                Dialog::dismiss);
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    rhyaDialogManager.createDialog_Confirm(context,
                            "Unknown Error",
                            "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00077)",
                            "종료",
                            false,
                            Dialog::dismiss);
                }
            }


            return true;
        });

        // 리스너 설정
        playListButton.setOnClickListener(v -> showPlayListFragment());

        playListAddRandomSongButton.setOnClickListener(v -> rhyaDialogManager.createDialog_YesOrNo(context,
                "Add Random Music",
                "플레이리스트에 20개의 무작위 노래를 추가하시겠습니까?",
                "추가",
                "취소",
                true,
                new RhyaDialogManager.DialogListener_YesOrNo() {
                    @Override
                    public void onClickListenerButtonYes(Dialog dialog) {

                        dialog.dismiss();

                        rhyaDialogManager.createDialog_Task(context,
                                "작업 처리 중...",
                                false,
                                dialogSub -> {
                                    try {
                                        new RhyaAsyncTask<String, String>() {
                                            private final ArrayList<RhyaMusicInfoVO> rhyaMusicInfoVOS = new ArrayList<>();
                                            @Override
                                            protected void onPreExecute() {
                                            }

                                            @Override
                                            protected String doInBackground(String arg) {
                                                try {

                                                    ArrayList<String> uuids = new ArrayList<>();

                                                    for (String key : RhyaApplication.rhyaMusicInfoVOHashMap.keySet()) {
                                                        RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(key);
                                                        if (rhyaMusicInfoVO != null && rhyaMusicInfoVO.getType() != null && !rhyaMusicInfoVO.getType().contains("#모음집")) {
                                                            uuids.add(key);
                                                        }
                                                    }

                                                    Collections.shuffle(uuids);

                                                    for (int i = 0; i < 20; i++)
                                                        rhyaMusicInfoVOS.add(RhyaApplication.rhyaMusicInfoVOHashMap.get(uuids.get(i)));

                                                    return "success";
                                                }catch (Exception ex) {
                                                    ex.printStackTrace();
                                                }

                                                return null;
                                            }

                                            @Override
                                            protected void onPostExecute(String result) {
                                                try {
                                                    if (result.equals("success")) {
                                                        putMusicArrayListRhyaMusicVO(rhyaMusicInfoVOS);

                                                        toast.cancel();
                                                        toast.setText("노래 추가 성공!");
                                                        toast.show();

                                                        ((NowPlayListFragment) nowPlayListFragment).loadData(false);
                                                    }
                                                }catch (Exception ex) {
                                                    ex.printStackTrace();

                                                    toast.cancel();
                                                    toast.setText("로딩 중 오류가 발생하였습니다. (00092)");
                                                    toast.show();
                                                }

                                                dialogSub.dismiss();
                                            }
                                        }.execute(null);
                                    }catch (Exception ex) {
                                        ex.printStackTrace();

                                        toast.cancel();
                                        toast.setText("로딩 중 오류가 발생하였습니다. (00091)");
                                        toast.show();
                                    }
                                });
                    }

                    @Override
                    public void onClickListenerButtonNo(Dialog dialog) {
                        dialog.dismiss();
                    }
                }));

        playlistAllDelete.setOnClickListener(v -> {
            if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() == 0) {
                toast.cancel();
                toast.setText("이미 플레이리스트가 제거되었거나 플레이리스트에 아무것도 없습니다.");
                toast.show();

                return;
            }

            rhyaDialogManager.createDialog_YesOrNo(context,
                    "Remove",
                    "플레이리스트에 있는 모든 항목을 제거하시겠습니까?",
                    "제거",
                    "취소",
                    true,
                    new RhyaDialogManager.DialogListener_YesOrNo() {
                        @Override
                        public void onClickListenerButtonYes(Dialog dialog) {

                            dialog.dismiss();

                            rhyaDialogManager.createDialog_Task(context,
                                    "작업 처리 중...",
                                    false,
                                    dialogSub -> {
                                        try {
                                            if (playerServiceBindTOF) {
                                                if (playerService.mMediaPlayer != null) {
                                                    if (playerService.mMediaPlayer.isPlaying()) {
                                                        playerService.onTrackPause();
                                                    }

                                                    playerService.mMediaPlayer.reset();

                                                    playerService.showNotificationInit();

                                                    playerService.playMusicIndex = -1;
                                                    playerService.nowPlayMusicUUID = null;
                                                }
                                            }

                                            StringBuilder sb = new StringBuilder();

                                            sb.append(activity.getFilesDir().getAbsolutePath());
                                            sb.append(File.separator);
                                            sb.append("play_list.rhya");

                                            new File(sb.toString()).delete();

                                            RhyaApplication.clearRhyaNowPlayMusicUUIDArrayList();
                                            rhyaCore.saveNowPlayList(-1, RhyaApplication.getRhyaNowPlayMusicUUIDArrayList(), rhyaAESManager, activity);

                                            setNowPlayMusic();

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

                                            songTitle.setText("현재 재생 중인 노래 없음");
                                            songSinger.setText("현재 재생 중인 노래 없음");

                                            ((NowPlayListFragment) nowPlayListFragment).loadData(false);
                                        }catch (Exception ex) {
                                            ex.printStackTrace();

                                            toast.cancel();
                                            toast.setText("로딩 중 오류가 발생하였습니다. (00062)");
                                            toast.show();
                                        }

                                        dialogSub.dismiss();
                            });
                        }

                        @Override
                        public void onClickListenerButtonNo(Dialog dialog) {
                            dialog.dismiss();
                        }
                    });
        });

        viewPlayInfoImageView.setOnClickListener(v -> {
            if (playerServiceBindTOF) {
                if (playerService.mMediaPlayer != null && playerService.nowPlayMusicUUID != null) {
                    if (viewPager2.getCurrentItem() == 5) {
                        playlistAllDelete.setVisibility(View.INVISIBLE);
                        playListAddRandomSongButton.setVisibility(View.INVISIBLE);
                        musicProgressBar.setVisibility(View.VISIBLE);
                        playMusicPanel.setVisibility(View.VISIBLE);


                        changeBackFragment();

                        return;
                    }

                    playListAddRandomSongButton.setVisibility(View.INVISIBLE);
                    playlistAllDelete.setVisibility(View.INVISIBLE);
                    playMusicPanel.setVisibility(View.GONE);
                    musicProgressBar.setVisibility(View.GONE);

                    Objects.requireNonNull(
                            Objects.requireNonNull(mainTabLayout.getTabAt(mainTabLayout.getSelectedTabPosition())).getIcon()
                    ).setColorFilter(tabItemUnSelectColor, PorterDuff.Mode.SRC_IN);

                    nowFragmentIndex = 5;

                    topPanel.setVisibility(View.GONE);

                    viewPager2.setCurrentItem(5, false);
                }
            }
        });


        // 앱 업데이트 내역 확인
        if (rhyaSharedPreferences.getStringNoAES("_UPDATE_TEXT_CHECKER_", context).equals(rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE)) {
            if (RhyaApplication.appVersion.equals(rhyaMainUtils.getAppVersion())) {
                rhyaDialogManager.createDialog_UpdateTextConfirm(context,
                        rhyaMainUtils.getAppVersion(),
                        "확인",
                        true,
                        rhyaSharedPreferences,
                        Dialog::dismiss);
            }

            rhyaSharedPreferences.setStringNoAES("_UPDATE_TEXT_CHECKER_", "", context);
        }


        // 서비스 시작 확인
        if (PlayerService.isStartService && !playerServiceBindTOF) {
            // 서비스 바인딩
            getApplicationContext().bindService(new Intent(getApplicationContext(), PlayerService.class), serviceConnection, Context.BIND_AUTO_CREATE);
            // 데이터 동기화
            setNowPlayMusic();
        }else {
            // 서비스 실행
            Intent serviceIntent = new Intent(ActivityMain.this, PlayerService.class);
            serviceIntent.setAction(PlayerService.ACTION_INIT);

            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                startForegroundService(serviceIntent);
            } else {
                startService(serviceIntent);
            }

            getApplicationContext().bindService(new Intent(getApplicationContext(), PlayerService.class), serviceConnection, Context.BIND_AUTO_CREATE);
        }


        // FCM 등록
        FirebaseMessaging.getInstance().subscribeToTopic("all-user-topic");
    }





    private void changeBackFragment() {
        // 색상 변경
        Objects.requireNonNull(
                Objects.requireNonNull(mainTabLayout.getTabAt(backFragmentIndex)).getIcon()
        ).setColorFilter(tabItemSelectColor, PorterDuff.Mode.SRC_IN);

        if (backFragmentIndex == 0 || backFragmentIndex == 4 || backFragmentIndex == 5) {
            // Top panel visibility
            topPanel.setVisibility(View.VISIBLE);
        }else {
            // Top panel visibility
            topPanel.setVisibility(View.GONE);
        }

        if (nowFragmentIndex == 4 || nowFragmentIndex == 5) {
            musicProgressBar.setVisibility(View.VISIBLE);
            playMusicPanel.setVisibility(View.VISIBLE);
        }

        nowFragmentIndex = backFragmentIndex;

        viewPager2.setCurrentItem(backFragmentIndex, false);
    }





    private final ServiceConnection serviceConnection = new ServiceConnection() {
        // 서비스가 실행될 때 호출
        @Override
        public void onServiceConnected(ComponentName name, IBinder service) {
            PlayerService.LocalBinder myBinder = (PlayerService.LocalBinder) service;
            playerService = myBinder.getService();

            playerServiceBindTOF = true;

            playerService.activity = activity;

            playerService.isMainActivityPauseAndResumeCheck = false;

            StringBuilder sb;
            sb = new StringBuilder();
            sb.append(getFilesDir().getAbsolutePath());
            sb.append(File.separator);
            sb.append("play_list.rhya");
            playerService.NOW_PLAY_LIST_SAVE_FILE_NAME = sb.toString();

            Intent intent = getIntent();
            int nowPlayIndexGet = intent.getIntExtra("nowPlayIndex", -1);
            if (nowPlayIndexGet != -1 && RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() > 0) {
                String uuid = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(nowPlayIndexGet);

                playerService.playMusicIndex = nowPlayIndexGet;
                playerService.nowPlayMusicUUID = uuid;

                isPlayPower = true;
                playPowerUUID = uuid;
            }

            try {
                playerService.authToken = rhyaCore.getAutoLogin(rhyaSharedPreferences, context);
            }catch (Exception ex) {
                ex.printStackTrace();
            }

            // 노래 정보 동기화
            if (playerService.mMediaPlayer != null) {
                setNowPlayMusic();
            }

            playerService.readMp3List();

            // 클릭 이벤트 연결
            playAndPauseImageBtn.setOnClickListener(v -> playButtonOnClickListener());
            musicBackButton.setOnClickListener(v -> {
                playerService.onTrackPrevious();

                ((NowPlayListFragment) nowPlayListFragment).loadData(false);
            });
            musicNextButton.setOnClickListener(v -> {
                playerService.onTrackNext(null, true);

                ((NowPlayListFragment) nowPlayListFragment).loadData(false);
            });
        }

        // 서비스가 종료될 때 호출
        @Override
        public void onServiceDisconnected(ComponentName name) {
            playerService = null;
            playerServiceBindTOF = false;
        }
    };
    public void playButtonOnClickListener() {
        if (playerService.mMediaPlayer != null) {
            if (playerService.mMediaPlayer.isPlaying()) {
                playerService.onTrackPause();
            }else {
                if (!songTitle.getText().equals("현재 재생 중인 노래 없음")) {
                    if (playerService.isFirstStartMusic && !(playerService.playMusicIndex == -1)) {
                        playerService.putNewTrack(playerService.nowPlayMusicUUID);

                        isPlayPower = false;
                        playPowerUUID = null;

                        return;
                    }

                    if (isPlayPower && playPowerUUID != null) {
                        playerService.putNewTrack(playPowerUUID);
                        isPlayPower = false;
                        playPowerUUID = null;
                    }else {
                        playerService.onTrackPlay();
                    }
                }
            }
        }
    }




    @Override
    public void onBackPressed() {
        // Fragment 종료 확인
        if (nowFragmentIndex == 4) {
            playlistAllDelete.setVisibility(View.INVISIBLE);
            playListAddRandomSongButton.setVisibility(View.INVISIBLE);

            changeBackFragment();

            return;
        }else if (nowFragmentIndex == 5) {

            changeBackFragment();

            return;
        }else if (nowFragmentIndex == 6 || nowFragmentIndex == 7) {
            nowFragmentIndex = 1;
            viewPager2.setCurrentItem(1, false);

            return;
        }else if (nowFragmentIndex == 8) {
            nowFragmentIndex = 2;
            viewPager2.setCurrentItem(2, false);

            setStatePlayListSongsRemoveButton(false);

            return;
        }


        long curTime = System.currentTimeMillis();
        long gapTime = curTime - backBtnTime;

        if(0 <= gapTime && 2000 >= gapTime) {
            // 종료
            super.onBackPressed();
        } else {
            backBtnTime = curTime;

            toast.cancel();
            toast.setText("'뒤로' 버튼을 한번 더 누르시면 종료됩니다.");
            toast.show();
        }
    }





    @Override
    protected void onDestroy() {
        super.onDestroy();
    }

    @Override
    protected void onPause() {
        super.onPause();

        if (playerServiceBindTOF) playerService.isMainActivityPauseAndResumeCheck = true;
    }

    @Override
    protected void onResume() {
        super.onResume();

        if (playerServiceBindTOF) playerService.isMainActivityPauseAndResumeCheck = false;
    }



    public void setNowPlayMusic() {
        if (playerServiceBindTOF) {
            String uuid = playerService.nowPlayMusicUUID;
            RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);

            if (rhyaMusicInfoVO == null) return;

            songTitle.setText(rhyaMusicInfoVO.getName());
            songSinger.setText(rhyaMusicInfoVO.getSinger());

            if (!playerService.mMediaPlayer.isPlaying()) {
                playAndPauseImageBtn.setImageResource(R.drawable.ic_baseline_play_arrow_24);
            }else {
                playAndPauseImageBtn.setImageResource(R.drawable.ic_baseline_pause_24);
            }

            if (musicProgressBar != null) {
                musicProgressBar.setMax(playerService.mMediaPlayer.getDuration());
                musicProgressBar.setProgress(playerService.mMediaPlayer.getCurrentPosition());
            }

            ((NowPlayListFragment) nowPlayListFragment).loadData(false);
            ((PlayInfoFragment) playInfoFragment).settingUIData();
        }
    }



    public void showPlayListFragment() {
        musicProgressBar.setVisibility(View.VISIBLE);
        playMusicPanel.setVisibility(View.VISIBLE);


        if (viewPager2.getCurrentItem() == 4) {
            playlistAllDelete.setVisibility(View.INVISIBLE);
            playListAddRandomSongButton.setVisibility(View.INVISIBLE);

            changeBackFragment();

            return;
        }

        Objects.requireNonNull(
                Objects.requireNonNull(mainTabLayout.getTabAt(mainTabLayout.getSelectedTabPosition())).getIcon()
        ).setColorFilter(tabItemUnSelectColor, PorterDuff.Mode.SRC_IN);

        ((NowPlayListFragment) nowPlayListFragment).loadData(true);

        nowFragmentIndex = 4;

        playlistAllDelete.setVisibility(View.VISIBLE);
        playListAddRandomSongButton.setVisibility(View.VISIBLE);

        topPanel.setVisibility(View.VISIBLE);

        showTaskDialog();

        viewPager2.setCurrentItem(4, false);
    }



    public void initTaskDialogForNetworkManager() {
        // Dialog 기본 설정
        networkConnectionDialog = new Dialog(context);
        networkConnectionDialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        networkConnectionDialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        networkConnectionDialog.setContentView(R.layout.dialog_network);
        networkConnectionDialog.setCancelable(false);
        // Dialog 사이즈 설정
        Point size = new Point();
        WindowManager.LayoutParams params = networkConnectionDialog.getWindow().getAttributes();
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.R) {
            final WindowMetrics metrics = activity.getWindowManager().getCurrentWindowMetrics();
            // Gets all excluding insets
            final WindowInsets windowInsets = metrics.getWindowInsets();
            Insets insets = windowInsets.getInsetsIgnoringVisibility(WindowInsets.Type.navigationBars()
                    | WindowInsets.Type.displayCutout());
            int insetsWidth = insets.right + insets.left;
            int insetsHeight = insets.top + insets.bottom;

            // Legacy size that Display#getSize reports
            final Rect bounds = metrics.getBounds();
            final Size legacySize = new Size(bounds.width() - insetsWidth,
                    bounds.height() - insetsHeight);
            params.width = (int) Math.round((legacySize.getWidth() * 0.8));
        }else {
            activity.getWindowManager().getDefaultDisplay().getRealSize(size);
            params.width = (int) Math.round((size.x * 0.8));
        }
        params.height = WindowManager.LayoutParams.WRAP_CONTENT;
        networkConnectionDialog.getWindow().setAttributes(params);
        // Dialog 내용 설정
        TextView messageTextView = networkConnectionDialog.findViewById(R.id.message);
        messageTextView.setText("네트워크 연결 대기 중...");
        Button retryButton = networkConnectionDialog.findViewById(R.id.retryButton);
        retryButton.setOnClickListener(view -> {
            ConnectivityManager manager = (ConnectivityManager) context.getSystemService(CONNECTIVITY_SERVICE);

            NetworkInfo networkInfo = manager.getActiveNetworkInfo();
            if(networkInfo != null) {
                int type = networkInfo.getType();
                if(type == ConnectivityManager.TYPE_MOBILE) {
                    dismissTaskDialogForNetworkManager();

                    return;
                }else if(type == ConnectivityManager.TYPE_WIFI){
                    dismissTaskDialogForNetworkManager();

                    return;
                }
            }

            showTaskDialogForNetworkManager();
        });
    }
    public void showTaskDialogForNetworkManager() {
        if (!networkConnectionDialog.isShowing()) {
            String result = rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE;

            try {
                result = rhyaSharedPreferences.getStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_CHECK_NETWORK_DIALOG, context);
            }catch (Exception ex) {
                ex.printStackTrace();
            }

            if (result.equals(rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE))
                networkConnectionDialog.show();
        }
    }
    public void dismissTaskDialogForNetworkManager() {
        if (networkConnectionDialog.isShowing()) {
            networkConnectionDialog.dismiss();
        }
    }
    public void initTaskDialog() {
        // Dialog 기본 설정
        mainActivityTaskDialog = new Dialog(context);
        mainActivityTaskDialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        mainActivityTaskDialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        mainActivityTaskDialog.setContentView(R.layout.dialog_task);
        mainActivityTaskDialog.setCancelable(false);
        // Dialog 사이즈 설정
        Point size = new Point();
        WindowManager.LayoutParams params = mainActivityTaskDialog.getWindow().getAttributes();
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.R) {
            final WindowMetrics metrics = activity.getWindowManager().getCurrentWindowMetrics();
            // Gets all excluding insets
            final WindowInsets windowInsets = metrics.getWindowInsets();
            Insets insets = windowInsets.getInsetsIgnoringVisibility(WindowInsets.Type.navigationBars()
                    | WindowInsets.Type.displayCutout());
            int insetsWidth = insets.right + insets.left;
            int insetsHeight = insets.top + insets.bottom;

            // Legacy size that Display#getSize reports
            final Rect bounds = metrics.getBounds();
            final Size legacySize = new Size(bounds.width() - insetsWidth,
                    bounds.height() - insetsHeight);
            params.width = (int) Math.round((legacySize.getWidth() * 0.8));
        }else {
            activity.getWindowManager().getDefaultDisplay().getRealSize(size);
            params.width = (int) Math.round((size.x * 0.8));
        }
        params.height = WindowManager.LayoutParams.WRAP_CONTENT;
        mainActivityTaskDialog.getWindow().setAttributes(params);
        // Dialog 내용 설정
        TextView messageTextView =  mainActivityTaskDialog.findViewById(R.id.message);
        messageTextView.setText("잠시만 기다려 주세요.");
    }
    public void showTaskDialog() {
        if (!mainActivityTaskDialog.isShowing()) {
            mainActivityTaskDialog.show();
        }
    }
    public void dismissTaskDialog() {
        if (mainActivityTaskDialog.isShowing()) {
            mainActivityTaskDialog.dismiss();
        }
    }
    public void showMorePopupMenu(ImageButton imageButton, String uuid, String itemUUID, int pos, int version) {
        Context wrapper = new ContextThemeWrapper(this, R.style.RoundPopupMenuStyle);
        PopupMenu popupMenu = new PopupMenu(wrapper, imageButton, Gravity.END);

        try {
            switch (version) {
                case 1:
                    getMenuInflater().inflate(R.menu.menu_song_item_v1, popupMenu.getMenu());
                    break;
                case 2:
                    getMenuInflater().inflate(R.menu.menu_song_item_v2, popupMenu.getMenu());
                    break;
            }

            popupMenu.setOnMenuItemClickListener(item -> {
                if (item.getTitle().equals("노래 정보")) {
                    showSongInfoDialog(uuid);
                }else if (item.getTitle().equals("플레이리스트에서 삭제") && version == 2 && ((NowPlayListFragment) nowPlayListFragment).rhyaNowPlayListAdapter != null) {
                    new RhyaAsyncTask<String, String>() {
                        private int removeIndex = -1;

                        @Override
                        protected void onPreExecute() {
                            ((NowPlayListFragment) nowPlayListFragment).recyclerView.setVisibility(View.INVISIBLE);
                            ((NowPlayListFragment) nowPlayListFragment).progressWheel.setVisibility(View.VISIBLE);
                            ((NowPlayListFragment) nowPlayListFragment).noPlaySongTitle.setVisibility(View.INVISIBLE);

                            removeIndex = -1;
                        }

                        @Override
                        protected String doInBackground(String arg) {
                            try {
                                if (((NowPlayListFragment) nowPlayListFragment).rhyaNowPlayListAdapter.musicList.size() <= 0) return "nullList";

                                for (int i = 0; i < RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size(); i ++) {
                                    if (((NowPlayListFragment) nowPlayListFragment).rhyaNowPlayListAdapter.musicList.get(i).getItemID().equals(itemUUID)) {
                                        removeIndex = (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() - 1) - i;
                                        break;
                                    }
                                }

                                return "";
                            }catch (Exception ex) {
                                ex.printStackTrace();

                                return null;
                            }
                        }

                        @Override
                        protected void onPostExecute(String result) {
                            try {
                                if (result == null) {
                                    toast.cancel();
                                    toast.setText("로딩 중 오류가 발생하였습니다. (00060)");
                                    toast.show();

                                    endTask();
                                }else {
                                    if (result.equals("nullList")) {
                                        endTask();

                                        return;
                                    }

                                    if (removeIndex != -1) {
                                        if (playerServiceBindTOF) {
                                            if (playerService.mMediaPlayer != null) {
                                                ArrayList<String> input = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList();
                                                input.remove(removeIndex);

                                                RhyaApplication.setRhyaNowPlayMusicUUIDArrayList(input);

                                                ((NowPlayListFragment) nowPlayListFragment).rhyaNowPlayListAdapter.notifyItemRemoved(pos);

                                                if (playerService.playMusicIndex == removeIndex) {
                                                    if (playerService.mMediaPlayer.isPlaying()) {
                                                        playerService.onTrackPause();
                                                    }

                                                    playerService.mMediaPlayer.reset();

                                                    playerService.playMusicIndex = -1;
                                                    playerService.nowPlayMusicUUID = null;

                                                    playerService.showNotificationInit();

                                                    songTitle.setText("현재 재생 중인 노래 없음");
                                                    songSinger.setText("현재 재생 중인 노래 없음");
                                                }

                                                if (playerService.playMusicIndex != -1) {
                                                    if (((NowPlayListFragment) nowPlayListFragment).rhyaNowPlayListAdapter.playPos < pos) {
                                                        playerService.playMusicIndex = playerService.playMusicIndex - 1;
                                                    }

                                                    playerService.nowPlayMusicUUID = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(playerService.playMusicIndex);
                                                }

                                                rhyaCore.saveNowPlayList(playerService.playMusicIndex, RhyaApplication.getRhyaNowPlayMusicUUIDArrayList(), rhyaAESManager, activity);

                                                ((NowPlayListFragment) nowPlayListFragment).loadData(false);
                                            }else {
                                                endTask();
                                            }
                                        }else {
                                            endTask();
                                        }
                                    }else {
                                        endTask();
                                    }
                                }
                            }catch (Exception ex) {
                                ex.printStackTrace();

                                toast.cancel();
                                toast.setText("로딩 중 오류가 발생하였습니다. (00061)");
                                toast.show();
                            }
                        }

                        private void endTask() {
                            ((NowPlayListFragment) nowPlayListFragment).recyclerView.setVisibility(View.INVISIBLE);
                            ((NowPlayListFragment) nowPlayListFragment).progressWheel.setVisibility(View.INVISIBLE);
                            ((NowPlayListFragment) nowPlayListFragment).noPlaySongTitle.setVisibility(View.VISIBLE);
                        }
                    }.execute(null);
                }else if (item.getTitle().equals("플레이리스트에 넣기") && version == 1) {
                    putNowPlayListSong(uuid);
                }else if (item.getTitle().equals("노래 정보 제거")) {
                    new RhyaAsyncTask<String, String>() {
                        @Override
                        protected void onPreExecute() {
                        }

                        @Override
                        protected String doInBackground(String arg) {
                            try {
                                StringBuilder stringBuilder = new StringBuilder();
                                stringBuilder.append(activity.getFilesDir().getAbsolutePath());
                                stringBuilder.append(File.separator);
                                stringBuilder.append("music");
                                stringBuilder.append(File.separator);
                                stringBuilder.append(uuid);
                                stringBuilder.append(".rhya_music");

                                File file = new File(stringBuilder.toString());
                                if (file.exists()) {
                                    if (file.delete()) {
                                        return "";
                                    }
                                }

                                return null;
                            }catch (Exception ex) {
                                ex.printStackTrace();

                                return null;
                            }
                        }

                        @Override
                        protected void onPostExecute(String result) {
                            if (result == null) {
                                toast.cancel();
                                toast.setText("노래 정보 제거 실패!");
                                toast.show();
                            }else {
                                toast.cancel();
                                toast.setText("노래 정보 제거 성공!");
                                toast.show();
                            }
                        }
                    }.execute(null);
                }

                return false;
            });

            popupMenu.show();
        } catch (Exception e) {
            e.printStackTrace();

            rhyaDialogManager.createDialog_Confirm(context,
                    "Unknown Error",
                    "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00056)",
                    "종료",
                    false,
                    dialog -> {
                        moveTaskToBack(true);
                        finish();
                        android.os.Process.killProcess(android.os.Process.myPid());
                    });
        }
    }


    public void putNowPlayListSong(String uuid) {
        if (playerServiceBindTOF) {
            if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() + 1 > 100) {
                int setInt = (playerService.playMusicIndex - 1);

                playerService.putNewSongNoPlay(uuid);

                if (setInt < 0) {
                    if (playerService.mMediaPlayer.isPlaying()) {
                        playerService.onTrackPause();
                    }

                    playerService.mMediaPlayer.reset();

                    playerService.playMusicIndex = -1;
                    playerService.nowPlayMusicUUID = null;

                    playerService.showNotificationInit();

                    songTitle.setText("현재 재생 중인 노래 없음");
                    songSinger.setText("현재 재생 중인 노래 없음");
                }else {
                    playerService.playMusicIndex = setInt;
                }
            }else {
                playerService.putNewSongNoPlay(uuid);
            }

            toast.cancel();
            toast.setText("1곡을 재생 목록에 담았습니다.");
            toast.show();
        }
    }


    public void subscribeTask(RhyaSingerDataVO rhyaSingerDataVO) {
        rhyaDialogManager.createDialog_Task(context,
                "작업 처리 중...",
                false,
                dialogTask -> new RhyaAsyncTask<String, String>() {
                    @Override
                    protected void onPreExecute() {
                    }

                    @Override
                    protected String doInBackground(String arg) {
                        try {
                            StringBuilder sb = new StringBuilder();
                            sb.append(getFilesDir().getAbsolutePath());
                            sb.append(File.separator);
                            sb.append("user_metadata.rhya");

                            String readFile = rhyaAESManager.aesDecode(rhyaCore.readFile(sb.toString()));

                            JSONObject jsonObject = new JSONObject(URLDecoder.decode(new JSONObject(readFile).getString("subscribe_list"), "UTF-8"));
                            JSONArray jsonArray = new JSONArray(jsonObject.getString("list"));
                            jsonArray.put(rhyaSingerDataVO.getUuid());
                            jsonObject.put("list", jsonArray);
                            JSONObject changeObj = new JSONObject(readFile);
                            changeObj.put("subscribe_list", jsonObject);
                            rhyaCore.writeFile(sb.toString(), rhyaAESManager.aesEncode(changeObj.toString()));

                            RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                            ContentValues urlParm = new ContentValues();

                            urlParm.put("auth", rhyaCore.getAutoLogin(rhyaSharedPreferences, context));
                            urlParm.put("mode", 13);
                            urlParm.put("index", 0);
                            urlParm.put("value", rhyaSingerDataVO.getUuid());

                            JSONObject result = new JSONObject(rhyaHttpsConnection.request(rhyaCore.MAIN_URL, urlParm));

                            rhyaCore.getMetaDataInfo(rhyaCore.getAutoLogin(rhyaSharedPreferences, context), rhyaSharedPreferences, rhyaAESManager, context, activity);

                            if (result.getString("result").equals("success")) {
                                return "success";
                            }
                        }catch (Exception ex) {
                            ex.printStackTrace();
                        }

                        return null;
                    }

                    @Override
                    protected void onPostExecute(String result) {
                        try {
                            if (result != null) {
                                rhyaSharedPreferences.removeString(rhyaSharedPreferences.SHARED_PREFERENCES_USER_METADATA_CHANGE, context);

                                ArrayList<RhyaSingerDataVO> input = RhyaApplication.rhyaMusicDataVO.getSubscribeList();
                                input.add(rhyaSingerDataVO);
                                RhyaApplication.rhyaMusicDataVO.setSubscribeList(input);

                                ((HomeFragment) homeFragment).songListLoadingIndex = 0;
                                ((HomeFragment) homeFragment).isNoSongMoreView = false;

                                ((HomeFragment) homeFragment).reloadData(true);

                                dialogTask.dismiss();
                            }else {
                                dialogTask.dismiss();

                                rhyaDialogManager.createDialog_Confirm(context,
                                        "Unknown Error",
                                        "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00058)",
                                        "닫기",
                                        true,
                                        Dialog::dismiss);
                            }
                        }catch (Exception ex) {
                            ex.printStackTrace();

                            dialogTask.dismiss();

                            rhyaDialogManager.createDialog_Confirm(context,
                                    "Unknown Error",
                                    "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00057)",
                                    "닫기",
                                    true,
                                    Dialog::dismiss);
                        }
                    }
                }.execute(null));
    }



    public void showSongInfoDialog(String uuid) {
        // Dialog 기본 설정
        Dialog dialog = new Dialog(context);
        dialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialog.setContentView(R.layout.dialog_song_info);
        dialog.setCancelable(true);
        // Dialog 사이즈 설정
        WindowManager.LayoutParams params = dialog.getWindow().getAttributes();
        params.width = WindowManager.LayoutParams.MATCH_PARENT;
        params.height = WindowManager.LayoutParams.MATCH_PARENT;
        dialog.getWindow().setAttributes(params);
        // Dialog 내용 설정
        RhyaMusicInfoVO rhyaMusicInfoVO =  RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);
        assert rhyaMusicInfoVO != null;

        kro.kr.rhya_network.utaiteplayer.lib.com_github_amarjain07_StickyScrollView_1_0_2.ui.StickyScrollView stickyScrollView = dialog.findViewById(R.id.nestedScrollView);
        ProgressWheel progressWheel = dialog.findViewById(R.id.progressWheel);
        ImageButton backButton = dialog.findViewById(R.id.backButton);
        TextView relatedSongAllPlayTitleTextView = dialog.findViewById(R.id.relatedSongAllPlayTitleTextView);
        TextView songWriterNameTextView = dialog.findViewById(R.id.songWriterNameTextView);
        TextView titleTextView = dialog.findViewById(R.id.title);
        ImageView songImageView = dialog.findViewById(R.id.songImageView);
        ImageView subscribeImageView = dialog.findViewById(R.id.subscribeImageView);
        TextView singerNameTextView = dialog.findViewById(R.id.singerNameTextView);
        Button subscribeButton = dialog.findViewById(R.id.buttonSubscription);
        Button playButton = dialog.findViewById(R.id.buttonPlay);
        View view  = dialog.findViewById(R.id.viewSelector);
        ConstraintLayout lyricsLayout = dialog.findViewById(R.id.lyricsLayout);
        ConstraintLayout infoLayout = dialog.findViewById(R.id.infoLayout);
        ConstraintLayout relatedLayout = dialog.findViewById(R.id.relatedLayout);
        TextView lyricsTextView = dialog.findViewById(R.id.lyricsTextView);
        TextView moreLyricsTextView = dialog.findViewById(R.id.moreLyricsTextView);
        TextView songName = dialog.findViewById(R.id.songName);
        TextView songSinger = dialog.findViewById(R.id.songSinger);
        TextView songWriter = dialog.findViewById(R.id.songWriter);
        TextView songType = dialog.findViewById(R.id.songType);
        TextView songDate = dialog.findViewById(R.id.songDate);
        RecyclerView relatedRecyclerView = dialog.findViewById(R.id.relatedSongRecyclerView);
        TextView relatedSongNoTitleTextView = dialog.findViewById(R.id.relatedSongNoTitleTextView);
        songWriterNameTextView.setText(rhyaMusicInfoVO.getSongWriter());
        titleTextView.setText(rhyaMusicInfoVO.getName());
        Glide.with(context)
                .load(rhyaMusicInfoVO.getImage())
                .placeholder(R.drawable.img_load_error)
                .error(R.drawable.img_load_error)
                .fallback(R.drawable.img_load_error)
                .signature(new ObjectKey(rhyaMusicInfoVO.getVersion()))
                .into(songImageView);
        Glide.with(context)
                .load(rhyaMusicInfoVO.getSingerImage())
                .placeholder(R.drawable.img_load_error)
                .error(R.drawable.img_load_error)
                .fallback(R.drawable.img_load_error)
                .into(subscribeImageView);
        singerNameTextView.setText(rhyaMusicInfoVO.getSinger());

        boolean isSubscribe = false;
        RhyaSingerDataVO rhyaSingerDataVOGet = null;
        for (RhyaSingerDataVO rhyaSingerDataVO : RhyaApplication.rhyaMusicDataVO.getSubscribeList()) {
            String uuidSinger = rhyaSingerDataVO.getUuid();
            if (uuidSinger.equals(rhyaMusicInfoVO.getSingerUuid())) {
                isSubscribe = true;
                rhyaSingerDataVOGet = rhyaSingerDataVO;
                break;
            }
        }

        if (isSubscribe) {
            // 구독 중
            subscribeButton.setText("구독 중");
            subscribeButton.setEnabled(false);
        }else {
            subscribeButton.setText("구독");
            rhyaSingerDataVOGet = new RhyaSingerDataVO(rhyaMusicInfoVO.getSingerUuid(),
                    rhyaMusicInfoVO.getSinger(),
                    rhyaMusicInfoVO.getSingerImage());
        }

        stickyScrollView.setScrollViewListener(new IScrollViewListener() {
            @Override
            public void onScrollChanged(int l, int t, int oldl, int oldt) {
                if (t == 0) {
                    view.setVisibility(View.INVISIBLE);
                }else {
                    view.setVisibility(View.VISIBLE);
                }
            }

            @Override
            public void onScrollStopped(boolean isStoped) {

            }
        });

        final RhyaSingerDataVO finalRhyaSingerDataVOGet = rhyaSingerDataVOGet;
        subscribeButton.setOnClickListener(v -> {
            if (subscribeButton.getText().equals("구독")) {
                // 구독 작업
                try {
                    subscribeButton.setText("구독 중");
                    subscribeButton.setEnabled(false);

                    subscribeTask(finalRhyaSingerDataVOGet);
                }catch (Exception ex) {
                    ex.printStackTrace();
                }
            }
        });
        playButton.setOnClickListener(v -> putMusicUUID(rhyaMusicInfoVO.getUuid()));
        backButton.setOnClickListener(v -> dialog.dismiss());

        //<editor-fold desc="초기화 비동기 작업">
        new RhyaAsyncTask<String, String>() {
            private ArrayList<RhyaMusicInfoVO> rhyaMusicInfoVOArrayList;

            @Override
            protected void onPreExecute() {
                rhyaMusicInfoVOArrayList = new ArrayList<>();

                progressWheel.setVisibility(View.VISIBLE);
                lyricsLayout.setVisibility(View.INVISIBLE);
                infoLayout.setVisibility(View.INVISIBLE);
                relatedLayout.setVisibility(View.INVISIBLE);
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    // 가사 읽기
                    StringBuilder sb = new StringBuilder();
                    sb.append(rhyaCore.getMusicInfoLyricsDirectory(activity));
                    sb.append(rhyaMusicInfoVO.getUuid());
                    sb.append(".lyrics");
                    File file = new File(sb.toString());

                    if (file.exists()) {
                        String lyrics = rhyaAESManager.aesDecode(rhyaCore.readFile(sb.toString()));
                        String[] lines = lyrics.split(System.lineSeparator());
                        StringBuilder readLyrics = new StringBuilder();
                        int line = 0;

                        for (String s : lines) {
                            if (line > 15) break;
                            if (s.contains("[null]")) continue;

                            readLyrics.append(s.trim());
                            readLyrics.append(System.lineSeparator());
                            line ++;
                        }

                        runOnUiThread(() -> lyricsTextView.setText(readLyrics.toString()));
                    }else {
                        runOnUiThread(() -> lyricsTextView.setText("표시할 가사가 없습니다."));
                    }

                    int count = 0;

                    for (String key : RhyaApplication.rhyaMusicInfoVOHashMap.keySet()) {
                        if (count >= 20) break;

                        RhyaMusicInfoVO rhyaMusicInfoVO1 = RhyaApplication.rhyaMusicInfoVOHashMap.get(key);
                        assert rhyaMusicInfoVO1 != null;

                        if (rhyaMusicInfoVO1.getSingerUuid().equals(rhyaMusicInfoVO.getSingerUuid()) && !rhyaMusicInfoVO1.getUuid().equals(rhyaMusicInfoVO.getUuid())) {
                            rhyaMusicInfoVOArrayList.add(rhyaMusicInfoVO1);

                            count++;
                        }
                    }

                    return "";
                }catch (Exception ex) {
                    ex.printStackTrace();

                    return null;
                }
            }

            @Override
            protected void onPostExecute(String result) {
                lyricsLayout.setVisibility(View.VISIBLE);
                infoLayout.setVisibility(View.VISIBLE);
                relatedLayout.setVisibility(View.VISIBLE);
                progressWheel.setVisibility(View.INVISIBLE);

                if (rhyaMusicInfoVOArrayList.size() == 0) {
                    relatedSongNoTitleTextView.setVisibility(View.VISIBLE);
                    relatedRecyclerView.setVisibility(View.GONE);
                }else {
                    relatedSongNoTitleTextView.setVisibility(View.GONE);
                    relatedRecyclerView.setVisibility(View.VISIBLE);
                }

                relatedSongAllPlayTitleTextView.setOnClickListener(v -> {
                    try {
                        // 서비스 시작 확인
                        if (PlayerService.isStartService) {
                            if (playerServiceBindTOF) {
                                if (playerService.mMediaPlayer != null) {
                                    if (rhyaMusicInfoVOArrayList.size() > 0) {
                                        int temp = 0;

                                        int indexSetting = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size();

                                        for (int i = 0; i < rhyaMusicInfoVOArrayList.size(); i ++) {
                                            if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() + 1 > 100) temp++;
                                            RhyaApplication.addRhyaNowPlayMusicUUIDArrayList(rhyaMusicInfoVOArrayList.get(i).getUuid());
                                        }

                                        if (indexSetting == 0 && RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() != 0) {
                                            playerService.playMusicIndex = 0;
                                            playerService.nowPlayMusicUUID = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(0);
                                            playerService.putNewTrackLast();
                                        }else {
                                            if (temp != 0) {
                                                int setInt = (playerService.playMusicIndex - temp);

                                                if (setInt < 0) {
                                                    if (playerService.mMediaPlayer.isPlaying()) {
                                                        playerService.onTrackPause();
                                                    }

                                                    playerService.mMediaPlayer.reset();

                                                    playerService.playMusicIndex = -1;
                                                    playerService.nowPlayMusicUUID = null;

                                                    playerService.showNotificationInit();

                                                    songTitle.setText("현재 재생 중인 노래 없음");
                                                    songSinger.setText("현재 재생 중인 노래 없음");
                                                }else {
                                                    playerService.playMusicIndex = setInt;
                                                }
                                            }
                                        }

                                        rhyaCore.saveNowPlayList(playerService.playMusicIndex, RhyaApplication.getRhyaNowPlayMusicUUIDArrayList(), rhyaAESManager, activity);

                                        toast.cancel();
                                        toast.setText(rhyaMusicInfoVOArrayList.size() + "곡을 재생 목록에 담았습니다.");
                                        toast.show();
                                    }
                                }
                            }else {
                                // 서비스 바인딩
                                getApplicationContext().bindService(new Intent(getApplicationContext(), PlayerService.class), serviceConnection, Context.BIND_AUTO_CREATE);
                            }
                        }
                    }catch (Exception ex) {
                        ex.printStackTrace();
                    }
                });

                RhyaNewSongAdapter rhyaNewSongAdapter = new RhyaNewSongAdapter(activity);
                LinearLayoutManager relatedLinearLayoutManager = new LinearLayoutManager(context);
                relatedRecyclerView.setLayoutManager(relatedLinearLayoutManager);
                relatedLinearLayoutManager.setOrientation(RecyclerView.HORIZONTAL);
                relatedRecyclerView.setAdapter(rhyaNewSongAdapter);
                rhyaNewSongAdapter.setList(rhyaMusicInfoVOArrayList);

                if (result == null) {
                    toast.cancel();
                    toast.setText("로딩 중 오류가 발생하였습니다. (00059)");
                    toast.show();
                }
            }
        }.execute(null);
        //</editor-fold>

        moreLyricsTextView.setOnClickListener(v -> {
            if (moreLyricsTextView.getText().equals("더보기")) {
                moreLyricsTextView.setText("접기");
            }else {
                moreLyricsTextView.setText("더보기");
            }

            //<editor-fold desc="비동기 작업">
            new RhyaAsyncTask<String, String>() {
                @Override
                protected void onPreExecute() {
                    progressWheel.setVisibility(View.VISIBLE);
                    infoLayout.setVisibility(View.INVISIBLE);
                    lyricsLayout.setVisibility(View.INVISIBLE);
                    relatedLayout.setVisibility(View.INVISIBLE);
                }

                @Override
                protected String doInBackground(String arg) {
                    try {
                        // 가사 읽기
                        StringBuilder sb = new StringBuilder();
                        sb.append(rhyaCore.getMusicInfoLyricsDirectory(activity));
                        sb.append(rhyaMusicInfoVO.getUuid());
                        sb.append(".lyrics");
                        File file = new File(sb.toString());

                        if (file.exists()) {
                            String lyrics = rhyaAESManager.aesDecode(rhyaCore.readFile(sb.toString()));
                            String[] lines = lyrics.split(System.lineSeparator());
                            StringBuilder readLyrics = new StringBuilder();
                            int line = 0;

                            for (String s : lines) {
                                if (moreLyricsTextView.getText().equals("더보기")) {
                                    if (line > 15) break;
                                }

                                if (s.contains("[null]")) continue;

                                readLyrics.append(s.trim());
                                readLyrics.append(System.lineSeparator());
                                line ++;
                            }

                            runOnUiThread(() -> lyricsTextView.setText(readLyrics.toString()));

                            return "success";
                        }else {
                            runOnUiThread(() -> lyricsTextView.setText("표시할 가사가 없습니다."));

                            return "noLyrics";
                        }
                    }catch (Exception ex) {
                        ex.printStackTrace();

                        return null;
                    }
                }

                @Override
                protected void onPostExecute(String result) {
                    lyricsLayout.setVisibility(View.VISIBLE);
                    infoLayout.setVisibility(View.VISIBLE);
                    relatedLayout.setVisibility(View.VISIBLE);
                    progressWheel.setVisibility(View.INVISIBLE);

                    if (result == null) {
                        toast.cancel();
                        toast.setText("로딩 중 오류가 발생하였습니다. (00059)");
                        toast.show();
                    }
                }
            }.execute(null);
            //</editor-fold>
        });

        songName.setText(rhyaMusicInfoVO.getName());
        songSinger.setText(rhyaMusicInfoVO.getSinger());
        songWriter.setText(rhyaMusicInfoVO.getSongWriter());
        songType.setText(rhyaMusicInfoVO.getType());
        songDate.setText(rhyaMusicInfoVO.getDate());

        dialog.show();
    }


    public void showSearchInputFragment() {
        nowFragmentIndex = 6;
        viewPager2.setCurrentItem(6, false);
    }
    public void showSearchResultFragment(String input) {
        RhyaApplication.searchText = input;

        showTaskDialog();

        nowFragmentIndex = 7;
        viewPager2.setCurrentItem(7, false);
    }
    public void showMyPlayListInfoFragment(RhyaPlayList rhyaPlayList) {
        this.rhyaPlayList = rhyaPlayList;

        isPlayListScrollToTop = true;

        nowFragmentIndex = 8;
        viewPager2.setCurrentItem(8, false);
    }
    public void showMyPlayListFragment() {
        nowFragmentIndex = 2;
        viewPager2.setCurrentItem(2, false);
    }



    public void putMusicUUID(String uuid) {
        final String addPlayListText = "1곡을 재생 목록에 담았습니다.";

        try {
            // 서비스 시작 확인
            if (PlayerService.isStartService) {
                if (playerServiceBindTOF) {
                    RhyaApplication.addRhyaNowPlayMusicUUIDArrayList(uuid);
                    playerService.playMusicIndex = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() - 1;
                    playerService.nowPlayMusicUUID = uuid;

                    playerService.putNewTrack(uuid);

                    playerService.playMusicIndex = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() - 1;
                    playerService.nowPlayMusicUUID = uuid;

                    rhyaCore.saveNowPlayList(playerService.playMusicIndex, RhyaApplication.getRhyaNowPlayMusicUUIDArrayList(), rhyaAESManager, activity);

                    toast.cancel();
                    toast.setText(addPlayListText);
                    toast.show();
                }else {
                    // 서비스 바인딩
                    getApplicationContext().bindService(new Intent(getApplicationContext(), PlayerService.class), serviceConnection, Context.BIND_AUTO_CREATE);
                }
            }
        }catch (Exception ex) {
            ex.printStackTrace();
        }
    }

    public void putMusicNoAddUUID(String uuid, int index) {
        try {
            // 서비스 시작 확인
            if (PlayerService.isStartService) {
                if (playerServiceBindTOF) {
                    playerService.putNewTrack(uuid);

                    playerService.playMusicIndex = index;
                    playerService.nowPlayMusicUUID = uuid;

                    rhyaCore.saveNowPlayList(playerService.playMusicIndex, RhyaApplication.getRhyaNowPlayMusicUUIDArrayList(), rhyaAESManager, activity);
                }else {
                    // 서비스 바인딩
                    getApplicationContext().bindService(new Intent(getApplicationContext(), PlayerService.class), serviceConnection, Context.BIND_AUTO_CREATE);
                }
            }
        }catch (Exception ex) {
            ex.printStackTrace();
        }
    }

    public void putMusicArrayListRhyaMusicVO(ArrayList<RhyaMusicInfoVO> input) {
        try {
            // 서비스 시작 확인
            if (PlayerService.isStartService) {
                if (playerServiceBindTOF) {
                    if (playerService.mMediaPlayer != null) {
                        if (input.size() > 0) {
                            int temp = 0;

                            int indexSetting = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size();

                            for (int i = 0; i < input.size(); i ++) {
                                if (RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() + 1 > 100) temp++;
                                RhyaApplication.addRhyaNowPlayMusicUUIDArrayList(input.get(i).getUuid());
                            }

                            if (indexSetting == 0 && RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().size() != 0) {
                                playerService.playMusicIndex = 0;
                                playerService.nowPlayMusicUUID = RhyaApplication.getRhyaNowPlayMusicUUIDArrayList().get(0);
                                playerService.putNewTrackLast();
                            }else {
                                if (temp != 0) {
                                    int setInt = (playerService.playMusicIndex - temp);

                                    if (setInt < 0) {
                                        if (playerService.mMediaPlayer.isPlaying()) {
                                            playerService.onTrackPause();
                                        }

                                        playerService.mMediaPlayer.reset();

                                        playerService.playMusicIndex = -1;
                                        playerService.nowPlayMusicUUID = null;

                                        playerService.showNotificationInit();

                                        songTitle.setText("현재 재생 중인 노래 없음");
                                        songSinger.setText("현재 재생 중인 노래 없음");
                                    }else {
                                        playerService.playMusicIndex = setInt;
                                    }
                                }
                            }

                            rhyaCore.saveNowPlayList(playerService.playMusicIndex, RhyaApplication.getRhyaNowPlayMusicUUIDArrayList(), rhyaAESManager, activity);

                            toast.cancel();
                            toast.setText(input.size() + "곡을 재생 목록에 담았습니다.");
                            toast.show();
                        }
                    }
                }else {
                    // 서비스 바인딩
                    getApplicationContext().bindService(new Intent(getApplicationContext(), PlayerService.class), serviceConnection, Context.BIND_AUTO_CREATE);
                }
            }
        }catch (Exception ex) {
            ex.printStackTrace();
        }
    }



    public void setStatePlayListSongsRemoveButton(boolean visibility) {
        if (visibility) {
            playMusicPanel.setVisibility(View.GONE);
            musicProgressBar.setVisibility(View.GONE);
            playListSongRemoveButton.setVisibility(View.VISIBLE);
        }else {
            playListSongRemoveButton.setVisibility(View.GONE);
            musicProgressBar.setVisibility(View.VISIBLE);
            playMusicPanel.setVisibility(View.VISIBLE);
        }
    }



    public void setActionPlayListSongsRemoveButton(PlayListSongsRemoveButtonListener playListSongsRemoveButtonListener) {
        playListSongRemoveButton.setOnClickListener(v -> playListSongsRemoveButtonListener.onClickPlayListSongsRemoveButton());
    }
    public interface PlayListSongsRemoveButtonListener {
        void onClickPlayListSongsRemoveButton();
    }



    public void showDialogAllRefresh() {
        rhyaDialogManager.createDialog_YesOrNo(context,
                "Refresh",
                "전체 새로고침을 진행하시겠습니까? 이 작업은 파일을 다시 다운로드 받습니다.",
                "새로고침",
                "취소",
                true,
                new RhyaDialogManager.DialogListener_YesOrNo() {
                    @Override
                    public void onClickListenerButtonYes(Dialog dialog) {
                        rhyaDialogManager.createDialog_Task(context,
                                "새로고침 중...",
                                false,
                                dialogSub -> new RhyaAsyncTask<String, String>() {
                                    @Override
                                    protected void onPreExecute() {
                                        if (playerServiceBindTOF) {
                                            if (playerService.mMediaPlayer != null) {
                                                playerService.playMusicIndex = -1;
                                                playerService.nowPlayMusicUUID = null;
                                                if (playerService.mMediaPlayer.isPlaying()) {
                                                    playerService.mMediaPlayer.pause();
                                                }

                                                setNowPlayMusic();
                                                playerService.showNotificationInit();
                                            }
                                        }

                                        try {
                                            rhyaSharedPreferences.setStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_USER_DATA_CHANGE, rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE, context);

                                            rhyaSharedPreferences.setStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_USER_METADATA_CHANGE, rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE, context);

                                            Glide.get(context).clearMemory();
                                        }catch (Exception ex) {
                                            ex.printStackTrace();
                                        }
                                    }

                                    @Override
                                    protected String doInBackground(String arg) {
                                        try {
                                            Glide.get(context).clearDiskCache();

                                            RhyaApplication.clearRhyaNowPlayMusicUUIDArrayList();

                                            rhyaCore.removeDataFile(activity);

                                            return "success";
                                        }catch (Exception ex) {
                                            ex.printStackTrace();

                                            return null;
                                        }
                                    }

                                    @Override
                                    protected void onPostExecute(String result) {
                                        if (result != null) {
                                            dialog.dismiss();

                                            Intent intent = new Intent(context, ActivitySplash.class);
                                            startActivity(intent);
                                            finish();

                                            overridePendingTransition(R.anim.anim_stay, R.anim.anim_stay);
                                        }else {
                                            dialog.dismiss();

                                            rhyaDialogManager.createDialog_Confirm(context,
                                                    "Refresh Error",
                                                    "새로고침을 진행하는 중 오류가 발생하였습니다. 다시 시도해주십시오. (00040)",
                                                    "종료",
                                                    false,
                                                    dialogSub -> {
                                                        moveTaskToBack(true);
                                                        finish();
                                                        android.os.Process.killProcess(android.os.Process.myPid());
                                                    });
                                        }
                                    }
                                }.execute(null));
                    }

                    @Override
                    public void onClickListenerButtonNo(Dialog dialog) {
                        dialog.dismiss();
                    }
                });
    }



    public void cacheClear() {
        new RhyaAsyncTask<String, String>() {
            @Override
            protected void onPreExecute() {
                try {
                    Glide.get(context).clearMemory();
                }catch (Exception ex) {
                    ex.printStackTrace();
                }
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    Glide.get(context).clearDiskCache();

                    return "success";
                }catch (Exception ex) {
                    ex.printStackTrace();

                    return null;
                }
            }

            @Override
            protected void onPostExecute(String result) {
                if (result != null) {
                    toast.cancel();
                    toast.setText("캐시를 성공적으로 삭제하였습니다.");
                    toast.show();
                }else {
                    rhyaDialogManager.createDialog_Confirm(context,
                            "Cache Clear Error",
                            "캐시 제거를 진행하는 중 오류가 발생하였습니다. 다시 시도해주십시오. (00040)",
                            "종료",
                            false,
                            dialogSub -> {
                                moveTaskToBack(true);
                                finish();
                                android.os.Process.killProcess(android.os.Process.myPid());
                            });
                }
            }
        }.execute(null);
    }



    public void showAccountSettingDialog() {
        try {
            try {
                rhyaSharedPreferences.setStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_USER_DATA_CHANGE, rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE, context);

                rhyaDialogManager.createDialog_WebView(context,
                        rhyaCore.getEditUserURL(rhyaSharedPreferences, context),
                        "계정 정보 변경",
                        true,
                        rhyaCore,
                        null);
            } catch (NoSuchPaddingException | InvalidKeyException |
                    UnsupportedEncodingException | IllegalBlockSizeException |
                    BadPaddingException | NoSuchAlgorithmException | InvalidAlgorithmParameterException e) {
                e.printStackTrace();

                rhyaDialogManager.createDialog_Confirm(context,
                        "AES Decryption Error",
                        "데이터를 복호화하는 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00041)",
                        "닫기",
                        false,
                        Dialog::dismiss);
            }
        }catch (Exception ex) {
            ex.printStackTrace();

            rhyaDialogManager.createDialog_Confirm(context,
                    "Unknown Error",
                    "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00042)",
                    "종료",
                    false,
                    Dialog::dismiss);
        }
    }



    public void showLicensesDialog() {
        try {
            rhyaDialogManager.createDialog_WebView(context,
                    "https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/service/utaite_player_open_sources_licenses.jsp",
                    "라이센스 정보",
                    true,
                    rhyaCore,
                    null);
        }catch (Exception ex) {
            ex.printStackTrace();

            rhyaDialogManager.createDialog_Confirm(context,
                    "Unknown Error",
                    "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00042)",
                    "종료",
                    false,
                    Dialog::dismiss);
        }
    }



    public void showDialogLogout() {
        rhyaDialogManager.createDialog_YesOrNo(context,
                "Logout",
                "정말로 로그아웃하시겠습니까?",
                "로그아웃",
                "취소",
                true,
                new RhyaDialogManager.DialogListener_YesOrNo() {
                    @Override
                    public void onClickListenerButtonYes(Dialog dialog) {
                        rhyaSharedPreferences.removeString(rhyaSharedPreferences.SHARED_PREFERENCES_AUTH_TOKEN, context);

                        if (playerServiceBindTOF) {
                            if (playerService.mMediaPlayer != null) {
                                playerService.playMusicIndex = -1;
                                playerService.nowPlayMusicUUID = null;
                                if (playerService.mMediaPlayer.isPlaying()) {
                                    playerService.mMediaPlayer.pause();
                                }

                                setNowPlayMusic();
                                playerService.showNotificationInit();
                            }
                        }

                        String metaDataFile;
                        String userInfoFile;
                        String nowPlayListFile;

                        StringBuilder sb = new StringBuilder();


                        sb.append(activity.getFilesDir().getAbsolutePath());
                        sb.append(File.separator);
                        sb.append("user_metadata.rhya");

                        metaDataFile = sb.toString();
                        sb.setLength(0);


                        sb.append(activity.getFilesDir().getAbsolutePath());
                        sb.append(File.separator);
                        sb.append("user_info.rhya");

                        userInfoFile = sb.toString();
                        sb.setLength(0);

                        sb.append(activity.getFilesDir().getAbsolutePath());
                        sb.append(File.separator);
                        sb.append("play_list.rhya");

                        nowPlayListFile = sb.toString();

                        dialog.dismiss();

                        if (new File(metaDataFile).delete() || new File((userInfoFile)).delete() || new File(nowPlayListFile).delete()) {
                            Intent intent = new Intent(context, ActivitySplash.class);
                            startActivity(intent);
                            finish();

                            overridePendingTransition(R.anim.anim_stay, R.anim.anim_stay);
                        }else {
                            rhyaDialogManager.createDialog_Confirm(context,
                                    "Logout Error",
                                    "로그아웃 중 오류가 발생하였습니다. 다시 시도해주십시오. (00038)",
                                    "종료",
                                    false,
                                    dialogSub -> {
                                        moveTaskToBack(true);
                                        finish();
                                        android.os.Process.killProcess(android.os.Process.myPid());
                                    });
                        }
                    }

                    @Override
                    public void onClickListenerButtonNo(Dialog dialog) {
                        dialog.dismiss();
                    }
                });
    }



    public void setNetworkDialogSetting(boolean isUse) {
        try {
            if (isUse)
                rhyaSharedPreferences.removeString(rhyaSharedPreferences.SHARED_PREFERENCES_CHECK_NETWORK_DIALOG, context);
            else
                rhyaSharedPreferences.setStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_CHECK_NETWORK_DIALOG, "1", context);
        }catch (Exception ex) {
            ex.printStackTrace();
        }
    }
    public boolean getNetworkDialogSetting() {
        try {
            String result = rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE;

            try {
                result = rhyaSharedPreferences.getStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_CHECK_NETWORK_DIALOG, context);
            }catch (Exception ex) {
                ex.printStackTrace();
            }

            return result.equals(rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE);
        }catch (Exception ex) {
            ex.printStackTrace();
        }

        return true;
    }



    public void setAlarmNotificationNoOFF(boolean isUse) {
        try {
            if (isUse)
                rhyaSharedPreferences.removeString(rhyaSharedPreferences.SHARED_PREFERENCES_ALARM_FOR_NOTIFICATION, context);
            else
                rhyaSharedPreferences.setStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_ALARM_FOR_NOTIFICATION, "1", context);
        }catch (Exception ex) {
            ex.printStackTrace();
        }
    }
    public boolean getAlarmNotificationNoOFF() {
        try {
            String result = rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE;

            try {
                result = rhyaSharedPreferences.getStringNoAES(rhyaSharedPreferences.SHARED_PREFERENCES_ALARM_FOR_NOTIFICATION, context);
            }catch (Exception ex) {
                ex.printStackTrace();
            }

            return result.equals(rhyaSharedPreferences.DEFAULT_RETURN_STRING_VALUE);
        }catch (Exception ex) {
            ex.printStackTrace();
        }

        return true;
    }
}
