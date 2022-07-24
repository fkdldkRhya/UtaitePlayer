package kro.kr.rhya_network.utaiteplayer.fragment;

import android.graphics.Bitmap;
import android.graphics.Color;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.fragment.app.Fragment;
import androidx.palette.graphics.Palette;

import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.animation.AlphaAnimation;
import android.widget.ImageButton;
import android.widget.ScrollView;
import android.widget.SeekBar;
import android.widget.TextView;
import android.widget.Toast;

import com.bumptech.glide.Glide;
import com.bumptech.glide.request.target.BitmapImageViewTarget;
import com.bumptech.glide.request.transition.Transition;
import com.bumptech.glide.signature.ObjectKey;
import com.github.siyamed.shapeimageview.RoundedImageView;

import org.apache.commons.lang3.time.DateFormatUtils;

import java.io.File;
import java.util.Locale;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMarqueeTextView;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link PlayInfoFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class PlayInfoFragment extends Fragment {
    public boolean seekBarMoveChecker = false;

    private Toast toast;

    public ScrollView scrollView = null;
    private ConstraintLayout layoutTop = null;
    private ConstraintLayout layoutBottom = null;
    private RhyaMarqueeTextView songName = null;
    private ConstraintLayout multiPanelLyrics = null;
    private ConstraintLayout multiPanelNoLyrics = null;
    private TextView singerNameTextView = null;
    private ImageButton replaySettingButton = null;
    private ImageButton playListShowButton = null;
    private ImageButton lyricsShowButton = null;
    private ImageButton nextArrowLImageView = null;
    private ImageButton PlayAndPauseImageView = null;
    private ImageButton nextArrowRImageView = null;
    private RoundedImageView songImageView = null;
    private RoundedImageView songImageViewV2 = null;
    private TextView songNameV2 = null;
    private TextView lyricsTextView = null;
    private SeekBar seekBar = null;
    private TextView songTimeNow = null;
    private TextView songTimeEnd = null;
    private ImageButton shuffleSettingButton = null;


    public PlayInfoFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @return A new instance of fragment FragmentPlayInfo.
     */
    // TODO: Rename and change types and number of parameters
    public static PlayInfoFragment newInstance() {
        return new PlayInfoFragment();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_play_info, container, false);
    }




    public void setPlaySeekBarInfo(long nowTime, long endTime) {
        try {
            if (isUIActivity()) {
                if (nowTime >= 3600000) {
                    requireActivity().runOnUiThread(() -> songTimeNow.setText(DateFormatUtils.formatUTC(nowTime, "hh:mm:ss", Locale.KOREA)));
                }else {
                    requireActivity().runOnUiThread(() -> songTimeNow.setText(DateFormatUtils.formatUTC(nowTime, "mm:ss")));
                }

                if (endTime >= 3600000) {
                    requireActivity().runOnUiThread(() -> songTimeEnd.setText(DateFormatUtils.formatUTC(endTime, "hh:mm:ss",  Locale.KOREA)));
                }else {
                    requireActivity().runOnUiThread(() -> songTimeEnd.setText(DateFormatUtils.formatUTC(endTime, "mm:ss")));
                }

                requireActivity().runOnUiThread(() -> seekBar.setMax(Long.valueOf(endTime).intValue()));
                requireActivity().runOnUiThread(() -> seekBar.setProgress(Long.valueOf(nowTime).intValue()));
            }
        }catch (Exception ex) {
            ex.printStackTrace();
        }
    }
    public void checkTextViewSetting() {
        try {
            if (isUIActivity()) {
                if (isAdded()) {
                    if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                        if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null) {
                            if (((ActivityMain) requireActivity()).playerService.mMediaPlayer.isPlaying()) {
                                requireActivity().runOnUiThread(() -> songName.setEllipsize(TextUtils.TruncateAt.END));
                            }else {
                                requireActivity().runOnUiThread(() -> songName.setEllipsize(TextUtils.TruncateAt.MARQUEE));
                            }
                        }
                    }
                }
            }
        }catch (Exception ex) {
            ex.printStackTrace();
        }
    }




    public boolean isUIActivity() {
        return layoutTop != null || layoutBottom != null || playListShowButton != null || lyricsShowButton != null || PlayAndPauseImageView != null || nextArrowLImageView != null || nextArrowRImageView != null || songName != null || singerNameTextView != null || songImageView != null || seekBar != null || songTimeNow != null || songTimeEnd != null;
    }


    public void settingUIData() {
        if (isAdded()) {
            if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null && ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID != null) {
                    if (isUIActivity()) {
                        String uuid = ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID;
                        RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);
                        assert rhyaMusicInfoVO != null;

                        songName.setText(rhyaMusicInfoVO.getName());
                        songNameV2.setText(rhyaMusicInfoVO.getName());
                        singerNameTextView.setText(rhyaMusicInfoVO.getSinger());

                        if (!((ActivityMain) requireActivity()).playerService.mMediaPlayer.isPlaying()) {
                            PlayAndPauseImageView.setImageResource(R.drawable.ic_baseline_play_arrow_24);
                        }else {
                            PlayAndPauseImageView.setImageResource(R.drawable.ic_baseline_pause_24);
                        }

                        Glide.with(requireContext())
                                .load(rhyaMusicInfoVO.getImage())
                                .placeholder(R.drawable.img_load_error)
                                .error(R.drawable.img_load_error)
                                .fallback(R.drawable.img_load_error)
                                .signature(new ObjectKey(rhyaMusicInfoVO.getVersion()))
                                .into(songImageViewV2);

                        Glide.with(requireContext())
                                .asBitmap()
                                .load(rhyaMusicInfoVO.getImage())
                                .signature(new ObjectKey(rhyaMusicInfoVO.getVersion()))
                                .placeholder(R.drawable.img_load_error)
                                .error(R.drawable.img_load_error)
                                .fallback(R.drawable.img_load_error)
                                .into(new BitmapImageViewTarget(songImageView) {
                                    @Override
                                    public void onResourceReady(@NonNull Bitmap resource, @Nullable Transition<? super Bitmap> transition) {
                                        super.onResourceReady(resource, transition);

                                        Palette palette = Palette.from(resource).generate();
                                        Palette.Swatch vibrantSwatchTop = palette.getMutedSwatch();

                                        if (vibrantSwatchTop != null) {
                                            layoutTop.setBackgroundColor(vibrantSwatchTop.getRgb());
                                        }else {
                                            vibrantSwatchTop = palette.getDarkMutedSwatch();

                                            if (vibrantSwatchTop != null) {
                                                layoutTop.setBackgroundColor(vibrantSwatchTop.getRgb());
                                            }else {
                                                vibrantSwatchTop = palette.getLightVibrantSwatch();
                                                if (vibrantSwatchTop != null) {
                                                    layoutTop.setBackgroundColor(vibrantSwatchTop.getRgb());
                                                }
                                            }
                                        }
                                    }
                                });
                        try {
                            // 가사 읽기
                            StringBuilder sb = new StringBuilder();
                            sb.append(((ActivityMain) requireActivity()).rhyaCore.getMusicInfoLyricsDirectory(requireActivity()));
                            sb.append(rhyaMusicInfoVO.getUuid());
                            sb.append(".lyric");
                            File file = new File(sb.toString());

                            if (file.exists()) {
                                String lyrics = ((ActivityMain) requireActivity()).rhyaAESManager.aesDecode(((ActivityMain) requireActivity()).rhyaCore.readFile(sb.toString()));
                                String[] lines = lyrics.split(System.lineSeparator());
                                StringBuilder readLyrics = new StringBuilder();

                                for (String s : lines) {
                                    if (s.contains("[null]")) continue;

                                    readLyrics.append(s.trim());
                                    readLyrics.append(System.lineSeparator());
                                }

                                lyricsTextView.setText(readLyrics.toString());
                            }else {
                                lyricsTextView.setText("표시할 가사가 없습니다.");
                            }
                        }catch (Exception ex) {
                            ex.printStackTrace();
                        }
                    }
                }
            }
        }
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        ConstraintLayout constraintLayoutInfoV2 = view.findViewById(R.id.constraintLayoutInfoV2);
        scrollView = view.findViewById(R.id.scrollView);
        layoutTop = view.findViewById(R.id.layoutTop);
        layoutBottom = view.findViewById(R.id.layoutBottom);
        songName = view.findViewById(R.id.songName);
        singerNameTextView = view.findViewById(R.id.singerNameTextView);
        playListShowButton = view.findViewById(R.id.playListShowButton);
        replaySettingButton = view.findViewById(R.id.replaySettingButton);
        lyricsShowButton = view.findViewById(R.id.lyricsShowButton);
        songImageViewV2 = view.findViewById(R.id.imageView);
        songNameV2 = view.findViewById(R.id.name);
        multiPanelNoLyrics = view.findViewById(R.id.multiPanelNoLyrics);
        multiPanelLyrics = view.findViewById(R.id.multiPanelLyrics);
        nextArrowLImageView = view.findViewById(R.id.nextArrowLImageView);
        PlayAndPauseImageView = view.findViewById(R.id.PlayAndPauseImageView);
        nextArrowRImageView = view.findViewById(R.id.nextArrowRImageView);
        multiPanelLyrics = view.findViewById(R.id.multiPanelLyrics);
        songImageView = view.findViewById(R.id.songImageView);
        lyricsTextView = view.findViewById(R.id.lyricsTextView);
        seekBar = view.findViewById(R.id.seekBar);
        songTimeNow = view.findViewById(R.id.songTimeNow);
        songTimeEnd = view.findViewById(R.id.songTimeEnd);
        shuffleSettingButton = view.findViewById(R.id.shuffleSettingButton);

        if (isAdded()) {
            toast = Toast.makeText(getContext(), null, Toast.LENGTH_SHORT);
        }

        AlphaAnimation alphaAnimation1 = new AlphaAnimation(0, 1);
        alphaAnimation1.setDuration(100);
        alphaAnimation1.setRepeatCount(-1);

        AlphaAnimation alphaAnimation2 = new AlphaAnimation(1, 0);
        alphaAnimation2.setDuration(100);
        alphaAnimation2.setRepeatCount(-1);

        songTimeNow.setText(R.string.songDefTime);
        songTimeEnd.setText(R.string.songDefTime);

        playListShowButton.setOnClickListener(v -> {
            if (isAdded()) {
                if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                    if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null && ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID != null) {
                        if (isUIActivity()) {
                            ((ActivityMain) requireActivity()).showPlayListFragment();
                        }
                    }
                }
            }
        });

        nextArrowLImageView.setOnClickListener(v ->  {
            if (isAdded()) {
                if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                    if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null && ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID != null) {
                        if (isUIActivity()) {
                            ((ActivityMain) requireActivity()).playerService.onTrackPrevious();
                        }
                    }
                }
            }
        });
        PlayAndPauseImageView.setOnClickListener(v ->  {
            if (isAdded()) {
                if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                    if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null && ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID != null) {
                        if (isUIActivity()) {
                            ((ActivityMain) requireActivity()).playButtonOnClickListener();
                        }
                    }
                }
            }
        });
        nextArrowRImageView.setOnClickListener(v ->  {
            if (isAdded()) {
                if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                    if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null && ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID != null) {
                        if (isUIActivity()) {
                            ((ActivityMain) requireActivity()).playerService.onTrackNext(null, true);
                        }
                    }
                }
            }
        });
        seekBar.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                if (fromUser) {
                    seekBarMoveChecker = true;
                    if (isAdded()) {
                        if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                            if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null && ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID != null) {
                                if (isUIActivity()) {
                                    if (seekBar.getProgress() >= 3600000) {
                                        requireActivity().runOnUiThread(() -> songTimeNow.setText(DateFormatUtils.formatUTC(seekBar.getProgress(), "hh:mm:ss", Locale.KOREA)));
                                    } else {
                                        requireActivity().runOnUiThread(() -> songTimeNow.setText(DateFormatUtils.formatUTC(seekBar.getProgress(), "mm:ss")));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) {
                seekBarMoveChecker = true;
            }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) {
                seekBarMoveChecker = false;

                if (isAdded()) {
                    if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                        if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null && ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID != null) {
                            if (isUIActivity()) {
                                if (songTimeNow.getText().equals("00:00") || songTimeEnd.getText().equals("00:00")) {
                                    seekBar.setProgress(0);
                                    if (((ActivityMain) requireActivity()).playerService.mMediaPlayer.isPlaying()) {
                                        ((ActivityMain) requireActivity()).playerService.mMediaPlayer.seekTo(0);
                                        ((ActivityMain) requireActivity()).playerService.mMediaPlayer.setOnSeekCompleteListener(mp -> {
                                            seekBar.setProgress(mp.getCurrentPosition());
                                            ((ActivityMain) requireActivity()).playerService.mediaSessionUpdateFragment(0);
                                        });
                                    }
                                    return;
                                }


                                int progress = seekBar.getProgress();

                                try {
                                    // 버퍼링 확인
                                    int progressCheck = (int)((double) progress / (double) seekBar.getMax() * 100);
                                    int getNowBuffering = ((ActivityMain) requireActivity()).playerService.nowBufferingValue;

                                    if (((ActivityMain) requireActivity()).playerService.isMp3FilePlay) {
                                        ((ActivityMain) requireActivity()).playerService.mMediaPlayer.seekTo(progress);
                                        ((ActivityMain) requireActivity()).playerService.mMediaPlayer.setOnSeekCompleteListener(mp -> {
                                            seekBar.setProgress(mp.getCurrentPosition());
                                            ((ActivityMain) requireActivity()).playerService.mediaSessionUpdateFragment(progress);
                                        });

                                        long nowTime = ((ActivityMain) requireActivity()).playerService.mMediaPlayer.getCurrentPosition();
                                        long endTime = ((ActivityMain) requireActivity()).playerService.mMediaPlayer.getDuration();

                                        if (nowTime >= 3600000) {
                                            songTimeNow.setText(DateFormatUtils.formatUTC(nowTime, "hh:mm:ss", Locale.KOREA));
                                        }else {
                                            songTimeNow.setText(DateFormatUtils.formatUTC(nowTime, "mm:ss"));
                                        }

                                        if (endTime >= 3600000) {
                                            songTimeEnd.setText(DateFormatUtils.formatUTC(endTime, "hh:mm:ss", Locale.KOREA));
                                        }else {
                                            songTimeEnd.setText(DateFormatUtils.formatUTC(endTime, "mm:ss"));
                                        }

                                        seekBar.setMax(Long.valueOf(endTime).intValue());
                                        seekBar.setProgress(Long.valueOf(nowTime).intValue());

                                        return;
                                    }

                                    if (progressCheck <= getNowBuffering) {
                                        ((ActivityMain) requireActivity()).playerService.mMediaPlayer.seekTo(progress);
                                        ((ActivityMain) requireActivity()).playerService.mMediaPlayer.setOnSeekCompleteListener(mp -> {
                                            seekBar.setProgress(mp.getCurrentPosition());
                                            ((ActivityMain) requireActivity()).playerService.mediaSessionUpdateFragment(progress);
                                        });

                                        long nowTime = ((ActivityMain) requireActivity()).playerService.mMediaPlayer.getCurrentPosition();
                                        long endTime = ((ActivityMain) requireActivity()).playerService.mMediaPlayer.getDuration();

                                        if (nowTime >= 3600000) {
                                            songTimeNow.setText(DateFormatUtils.formatUTC(nowTime, "hh:mm:ss", Locale.KOREA));
                                        }else {
                                            songTimeNow.setText(DateFormatUtils.formatUTC(nowTime, "mm:ss"));
                                        }

                                        if (endTime >= 3600000) {
                                            songTimeEnd.setText(DateFormatUtils.formatUTC(endTime, "hh:mm:ss", Locale.KOREA));
                                        }else {
                                            songTimeEnd.setText(DateFormatUtils.formatUTC(endTime, "mm:ss"));
                                        }

                                        seekBar.setMax(Long.valueOf(endTime).intValue());
                                        seekBar.setProgress(Long.valueOf(nowTime).intValue());
                                    }else {
                                        ((ActivityMain) requireActivity()).playerService.noBufferedTimePlay(progress);
                                    }
                                }catch (Exception ex) {
                                    ex.printStackTrace();
                                }
                            }
                        }
                    }
                }
            }
        });
        lyricsShowButton.setOnClickListener(v -> visibilityChangeVer2());
        replaySettingButton.setOnClickListener(v -> {
            if (isAdded()) {
                if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                    if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null && ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID != null) {
                        checkLoop();
                    }
                }
            }
        });
        songImageView.setOnClickListener(v -> visibilityChangeVer2());
        songImageViewV2.setOnClickListener(v -> visibilityChangeVer2());
        songNameV2.setOnClickListener(v -> visibilityChangeVer2());
        constraintLayoutInfoV2.setOnClickListener(v -> visibilityChangeVer2());

        if (!((ActivityMain) requireActivity()).playerService.isLoopCheck && !((ActivityMain) requireActivity()).playerService.isAllLoopCheck) {
            replaySettingButton.setImageResource(R.drawable.ic_music_replay);
            replaySettingButton.setColorFilter(Color.parseColor("#000000"));
        } else if (!((ActivityMain) requireActivity()).playerService.isLoopCheck && ((ActivityMain) requireActivity()).playerService.isAllLoopCheck) {
            replaySettingButton.setImageResource(R.drawable.ic_music_replay);
            replaySettingButton.setColorFilter(Color.parseColor("#ff5e00"));
        } else if (((ActivityMain) requireActivity()).playerService.isLoopCheck && !((ActivityMain) requireActivity()).playerService.isAllLoopCheck) {
            replaySettingButton.setImageResource(R.drawable.ic_music_replay_this);
            replaySettingButton.setColorFilter(Color.parseColor("#ff5e00"));
        }

        if (((ActivityMain) requireActivity()).playerService.isShuffleCheck) {
            shuffleSettingButton.setColorFilter(Color.parseColor("#ff5e00"));
        }else {
            shuffleSettingButton.setColorFilter(Color.parseColor("#000000"));
        }
        shuffleSettingButton.setOnClickListener(v -> {
            if (isAdded()) {
                if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                    if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null && ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID != null) {
                        if (((ActivityMain) requireActivity()).playerService.isShuffleCheck) {
                            if (toast != null) {
                                toast.cancel();
                                toast.setText("셔플을 사용하지 않습니다.");
                                toast.show();
                            }

                            shuffleSettingButton.setColorFilter(Color.parseColor("#000000"));
                            ((ActivityMain) requireActivity()).playerService.isShuffleCheck = false;
                        }else {
                            if (toast != null) {
                                toast.cancel();
                                toast.setText("셔플을 사용합니다.");
                                toast.show();
                            }

                            shuffleSettingButton.setColorFilter(Color.parseColor("#ff5e00"));
                            ((ActivityMain) requireActivity()).playerService.isShuffleCheck = true;
                        }
                    }
                }
            }
        });

        settingUIData();

        if (isAdded()) {
            if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null && ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID != null) {
                    setPlaySeekBarInfo(((ActivityMain) requireActivity()).playerService.mMediaPlayer.getCurrentPosition(), ((ActivityMain) requireActivity()).playerService.mMediaPlayer.getDuration());
                }
            }
        }
    }


    private void checkLoop() {
        if (!((ActivityMain) requireActivity()).playerService.isLoopCheck && !((ActivityMain) requireActivity()).playerService.isAllLoopCheck) {
            if (toast != null) {
                toast.cancel();
                toast.setText("전체 음악을 반복합니다.");
                toast.show();
            }

            replaySettingButton.setImageResource(R.drawable.ic_music_replay);
            replaySettingButton.setColorFilter(Color.parseColor("#ff5e00"));

            ((ActivityMain) requireActivity()).playerService.isLoopCheck = false;
            ((ActivityMain) requireActivity()).playerService.isAllLoopCheck = true;
        } else if (!((ActivityMain) requireActivity()).playerService.isLoopCheck && ((ActivityMain) requireActivity()).playerService.isAllLoopCheck) {
            if (toast != null) {
                toast.cancel();
                toast.setText("현재 음악을 반복합니다.");
                toast.show();
            }

            replaySettingButton.setImageResource(R.drawable.ic_music_replay_this);

            ((ActivityMain) requireActivity()).playerService.isLoopCheck = true;
            ((ActivityMain) requireActivity()).playerService.isAllLoopCheck = false;
        } else if (((ActivityMain) requireActivity()).playerService.isLoopCheck && !((ActivityMain) requireActivity()).playerService.isAllLoopCheck) {
            if (toast != null) {
                toast.cancel();
                toast.setText("반복을 사용하지 않습니다.");
                toast.show();
            }

            replaySettingButton.setImageResource(R.drawable.ic_music_replay);
            replaySettingButton.setColorFilter(Color.parseColor("#000000"));

            ((ActivityMain) requireActivity()).playerService.isLoopCheck = false;
            ((ActivityMain) requireActivity()).playerService.isAllLoopCheck = false;
        }
    }


    private void visibilityChangeVer2() {
        if (isAdded()) {
            if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null && ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID != null) {
                    if (isUIActivity()) {
                        if (multiPanelNoLyrics.getVisibility() == View.VISIBLE) {
                            multiPanelNoLyrics.setAlpha(1);
                            multiPanelLyrics.setAlpha(0);

                            multiPanelNoLyrics.animate()
                                    .alpha(0f)
                                    .scaleX(0.5f)
                                    .scaleY(0.5f)
                                    .setDuration(100)
                                    .withEndAction(() -> multiPanelNoLyrics.setVisibility(View.INVISIBLE)).start();

                            multiPanelLyrics.animate()
                                    .alpha(1f)
                                    .scaleX(1f)
                                    .scaleY(1f)
                                    .setDuration(100)
                                    .withEndAction(() -> multiPanelLyrics.setVisibility(View.VISIBLE)).start();
                        }else {
                            multiPanelNoLyrics.setAlpha(0);
                            multiPanelLyrics.setAlpha(1);

                            multiPanelNoLyrics.animate()
                                    .alpha(1f)
                                    .scaleX(1f)
                                    .scaleY(1f)
                                    .setDuration(100)
                                    .withEndAction(() -> multiPanelNoLyrics.setVisibility(View.VISIBLE)).start();

                            multiPanelLyrics.animate()
                                    .alpha(0f)
                                    .scaleX(0.5f)
                                    .scaleY(0.5f)
                                    .setDuration(100)
                                    .withEndAction(() -> multiPanelLyrics.setVisibility(View.INVISIBLE)).start();
                        }
                    }
                }
            }
        }
    }


    public boolean serviceUIActivityCheck() {
        if (isAdded()) {
            if (((ActivityMain) requireActivity()).playerServiceBindTOF) {
                if (((ActivityMain) requireActivity()).playerService.mMediaPlayer != null && ((ActivityMain) requireActivity()).playerService.nowPlayMusicUUID != null) {
                    return isUIActivity();
                }
            }
        }

        return false;
    }
}