package kro.kr.rhya_network.utaiteplayer.utils;

import android.app.Activity;
import android.app.Dialog;
import android.content.Context;
import android.graphics.Color;
import android.graphics.Insets;
import android.graphics.Point;
import android.graphics.PorterDuff;
import android.graphics.Rect;
import android.graphics.drawable.ColorDrawable;
import android.graphics.drawable.Drawable;
import android.text.Editable;
import android.text.InputFilter;
import android.text.TextWatcher;
import android.util.DisplayMetrics;
import android.util.Size;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowInsets;
import android.view.WindowManager;
import android.view.WindowMetrics;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.load.DataSource;
import com.bumptech.glide.load.engine.DiskCacheStrategy;
import com.bumptech.glide.load.engine.GlideException;
import com.bumptech.glide.request.RequestListener;
import com.bumptech.glide.request.target.Target;
import com.pnikosis.materialishprogress.ProgressWheel;

import java.util.ArrayList;
import java.util.HashMap;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaMyPlayListAddSongAdapter;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaMyPlayListInfoAdapter;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaMyPlaylistImageSelectAdapter;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaSearchResultLyricsAdapter;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaSearchResultSingerAdapter;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaSearchSongListAdapter;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;

public class RhyaDialogManagerNew {
    private final Activity activity;
    // Task dialog
    private final Dialog dialog;
    // Background
    public final Drawable background = new ColorDrawable(Color.TRANSPARENT);
    // Toast
    private final Toast toast;
    // Value
    public int imageSelected = -1;



    /**
     * Dialog 관리자 초기화
     *
     * @param activity activity
     * @param isCancelable Cancelable 가능 불가능 여부
     * @param type Dialog 타입
     */
    public RhyaDialogManagerNew(Activity activity, boolean isCancelable, int type) {
        // Set activity
        // Activity
        this.activity = activity;

        Point size = new Point();
        // Dialog 기본 설정
        dialog = new Dialog(activity);
        dialog.getWindow().setBackgroundDrawable(background);
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialog.setCancelable(isCancelable);

        toast = Toast.makeText(activity, null, Toast.LENGTH_SHORT);

        switch (type) {
            default:
            case 0:
                dialog.setContentView(R.layout.dialog_task);
                break;

            case 1:
            case 3:
                dialog.setContentView(R.layout.dialog_all_song_list);

                return;

            case 2:
                dialog.setContentView(R.layout.dialog_all_singer_list);

                return;


            case 4:
                dialog.setContentView(R.layout.dialog_yes_no);

                break;

            case 5: {
                dialog.setContentView(R.layout.dialog_my_playlist);
                WindowManager.LayoutParams params = dialog.getWindow().getAttributes();
                params.width = WindowManager.LayoutParams.MATCH_PARENT;
                params.height = WindowManager.LayoutParams.MATCH_PARENT;
                dialog.getWindow().setAttributes(params);

                return;
            }

            case 6: {
                dialog.getWindow().setGravity(Gravity.BOTTOM);
                dialog.setContentView(R.layout.dialog_my_playlist_song_menu);
                WindowManager.LayoutParams params = dialog.getWindow().getAttributes();
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
                    params.width = Math.round((legacySize.getWidth()));
                } else {
                    activity.getWindowManager().getDefaultDisplay().getRealSize(size);
                    params.width = Math.round((size.x));
                }
                params.windowAnimations = R.style.AnimationPopupStyle;

                dialog.getWindow().setAttributes(params);

                return;
            }

            case 7: {
                dialog.setContentView(R.layout.dialog_my_playlist_add_song);
                WindowManager.LayoutParams params = dialog.getWindow().getAttributes();
                params.width = WindowManager.LayoutParams.MATCH_PARENT;
                params.height = WindowManager.LayoutParams.MATCH_PARENT;
                dialog.getWindow().setAttributes(params);

                return;
            }
        }


        // Dialog 사이즈 설정
        WindowManager.LayoutParams params = dialog.getWindow().getAttributes();
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

        dialog.getWindow().setAttributes(params);
    }



    // =========================================================================
    // =========================================================================
    public void setDialogSettingType_0(String message) {
        ((TextView) dialog.findViewById(R.id.message)).setText(message);
    }
    // =========================================================================
    // =========================================================================



    // =========================================================================
    // =========================================================================
    public void setAndShowDialogSettingType_1(ArrayList<String> songUUIDList) {
        WindowManager.LayoutParams params = dialog.getWindow().getAttributes();
        params.width = WindowManager.LayoutParams.MATCH_PARENT;
        params.height = WindowManager.LayoutParams.MATCH_PARENT;
        dialog.getWindow().setAttributes(params);

        final int realDeviceHeight;
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

            realDeviceHeight = legacySize.getHeight();
        }else {
            DisplayMetrics displayMetrics = new DisplayMetrics();
            activity.getWindowManager().getDefaultDisplay().getRealMetrics(displayMetrics);
            realDeviceHeight = displayMetrics.heightPixels;
        }

        // Dialog 내용 설정
        final TextView noItemTitle = dialog.findViewById(R.id.noItemTitle);
        final RecyclerView recyclerView = dialog.findViewById(R.id.recyclerView);
        final ProgressWheel progressWheel = dialog.findViewById(R.id.progressWheel);
        final ImageView allPlayButton = dialog.findViewById(R.id.allPlayButton);
        final ImageButton backButton = dialog.findViewById(R.id.backButton);
        final ImageView imageView = dialog.findViewById(R.id.imageView);
        final TextView songSizeText = dialog.findViewById(R.id.songSize);

        final RhyaSearchSongListAdapter rhyaSearchSongListAdapter = new RhyaSearchSongListAdapter(activity);

        recyclerView.setAdapter(rhyaSearchSongListAdapter);
        recyclerView.setLayoutManager(new LinearLayoutManager(activity.getApplicationContext()));
        progressWheel.setVisibility(View.VISIBLE);
        dialog.show();

        ViewGroup.LayoutParams layoutParams = imageView.getLayoutParams();
        layoutParams.height = (int) (realDeviceHeight / 3.5);
        imageView.setLayoutParams(layoutParams);
        backButton.setOnClickListener(v -> dialog.dismiss());

        dialog.show();

        Glide.with(activity)
                .load("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/utils/anim_image_random.jsp?type=1")
                .diskCacheStrategy(DiskCacheStrategy.NONE)
                .skipMemoryCache(true)
                .placeholder(R.drawable.img_load_error)
                .error(R.drawable.img_load_error)
                .fallback(R.drawable.img_load_error)
                .addListener(new RequestListener<Drawable>() {
                    @Override
                    public boolean onLoadFailed(@Nullable GlideException e, Object model, Target<Drawable> target, boolean isFirstResource) {
                        eventDialogSettingType_1(songUUIDList, activity.getApplicationContext(), noItemTitle, progressWheel, allPlayButton, songSizeText, rhyaSearchSongListAdapter);

                        return false;
                    }

                    @Override
                    public boolean onResourceReady(Drawable resource, Object model, Target<Drawable> target, DataSource dataSource, boolean isFirstResource) {
                        eventDialogSettingType_1(songUUIDList, activity.getApplicationContext(), noItemTitle, progressWheel, allPlayButton, songSizeText, rhyaSearchSongListAdapter);

                        return false;
                    }
                })
                .into(imageView);

        imageView.setColorFilter(Color.parseColor("#BDBDBD"), PorterDuff.Mode.MULTIPLY);
    }
    private void eventDialogSettingType_1(ArrayList<String> songUUIDList, Context context, TextView noItemTitle, ProgressWheel progressWheel, ImageView allPlayButton, TextView songSizeText, RhyaSearchSongListAdapter rhyaSearchSongListAdapter) {
        new RhyaAsyncTask<String, String>() {
            private ArrayList<RhyaMusicInfoVO> arrayList;

            @Override
            protected void onPreExecute() {
                // ArrayList 초기화
                arrayList = new ArrayList<>();
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    for (String key : songUUIDList) {
                        try {
                            RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(key);
                            assert rhyaMusicInfoVO != null;

                            arrayList.add(rhyaMusicInfoVO);
                        }catch (Exception ex) {
                            ex.printStackTrace();
                        }
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    Toast.makeText(context, ex.toString(), Toast.LENGTH_SHORT).show();

                    return null;
                }

                return "success";
            }

            @Override
            protected void onPostExecute(String result) {
                if (result != null) {
                    progressWheel.setVisibility(View.INVISIBLE);

                    StringBuilder stringBuilder;
                    stringBuilder = new StringBuilder();
                    stringBuilder.append(arrayList.size());
                    stringBuilder.append("곡");

                    allPlayButton.setOnClickListener(v -> ((ActivityMain) activity).putMusicArrayListRhyaMusicVO(arrayList));

                    songSizeText.setText(stringBuilder.toString());

                    if (arrayList.size() > 0) {
                        noItemTitle.setVisibility(View.GONE);
                    }else {
                        noItemTitle.setVisibility(View.VISIBLE);
                    }

                    rhyaSearchSongListAdapter.setList(arrayList);
                }
            }
        }.execute(null);
    }
    // =========================================================================
    // =========================================================================



    // =========================================================================
    // =========================================================================
    public void setAndShowDialogSettingType_2(ArrayList<String> singerUUIDList, HashMap<String, ArrayList<RhyaMusicInfoVO>> stringArrayListHashMap) {
        WindowManager.LayoutParams params = dialog.getWindow().getAttributes();
        params.width = WindowManager.LayoutParams.MATCH_PARENT;
        params.height = WindowManager.LayoutParams.MATCH_PARENT;
        dialog.getWindow().setAttributes(params);

        final int realDeviceHeight;
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

            realDeviceHeight = legacySize.getHeight();
        }else {
            DisplayMetrics displayMetrics = new DisplayMetrics();
            activity.getWindowManager().getDefaultDisplay().getRealMetrics(displayMetrics);
            realDeviceHeight = displayMetrics.heightPixels;
        }


        // Dialog 내용 설정
        final ImageView imageView = dialog.findViewById(R.id.imageView);
        final TextView noItemTitle = dialog.findViewById(R.id.noItemTitle);
        final RecyclerView recyclerView = dialog.findViewById(R.id.recyclerView);
        final ProgressWheel progressWheel = dialog.findViewById(R.id.progressWheel);
        final ImageButton backButton = dialog.findViewById(R.id.backButton);

        final RhyaSearchResultSingerAdapter rhyaSearchResultSingerAdapter = new RhyaSearchResultSingerAdapter(activity);

        ViewGroup.LayoutParams layoutParams = imageView.getLayoutParams();
        layoutParams.height = (int) (realDeviceHeight / 3.5);
        imageView.setLayoutParams(layoutParams);

        recyclerView.setAdapter(rhyaSearchResultSingerAdapter);
        recyclerView.setLayoutManager(new LinearLayoutManager(activity.getApplicationContext()));
        progressWheel.setVisibility(View.VISIBLE);
        dialog.show();

        Glide.with(activity)
                .load("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/utils/anim_image_random.jsp?type=1")
                .diskCacheStrategy(DiskCacheStrategy.NONE)
                .skipMemoryCache(true)
                .placeholder(R.drawable.img_load_error)
                .error(R.drawable.img_load_error)
                .fallback(R.drawable.img_load_error)
                .addListener(new RequestListener<Drawable>() {
                    @Override
                    public boolean onLoadFailed(@Nullable GlideException e, Object model, Target<Drawable> target, boolean isFirstResource) {
                        imageView.setColorFilter(Color.parseColor("#BDBDBD"), PorterDuff.Mode.MULTIPLY);

                        backButton.setOnClickListener(v -> dialog.dismiss());

                        progressWheel.setVisibility(View.INVISIBLE);

                        if (singerUUIDList.size() > 0) {
                            noItemTitle.setVisibility(View.GONE);
                        }else {
                            noItemTitle.setVisibility(View.VISIBLE);
                        }

                        rhyaSearchResultSingerAdapter.setList(singerUUIDList, stringArrayListHashMap, false);

                        return false;
                    }

                    @Override
                    public boolean onResourceReady(Drawable resource, Object model, Target<Drawable> target, DataSource dataSource, boolean isFirstResource) {
                        imageView.setColorFilter(Color.parseColor("#BDBDBD"), PorterDuff.Mode.MULTIPLY);

                        backButton.setOnClickListener(v -> dialog.dismiss());

                        progressWheel.setVisibility(View.INVISIBLE);

                        if (singerUUIDList.size() > 0) {
                            noItemTitle.setVisibility(View.GONE);
                        }else {
                            noItemTitle.setVisibility(View.VISIBLE);
                        }

                        rhyaSearchResultSingerAdapter.setList(singerUUIDList, stringArrayListHashMap, false);
                        return false;
                    }
                })
                .into(imageView);
    }
    // =========================================================================
    // =========================================================================



    // =========================================================================
    // =========================================================================
    public void setAndShowDialogSettingType_3(ArrayList<RhyaMusicInfoVO> rhyaMusicInfoVOArrayList) {
        WindowManager.LayoutParams params = dialog.getWindow().getAttributes();
        params.width = WindowManager.LayoutParams.MATCH_PARENT;
        params.height = WindowManager.LayoutParams.MATCH_PARENT;
        dialog.getWindow().setAttributes(params);

        final int realDeviceHeight;
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

            realDeviceHeight = legacySize.getHeight();
        }else {
            DisplayMetrics displayMetrics = new DisplayMetrics();
            activity.getWindowManager().getDefaultDisplay().getRealMetrics(displayMetrics);
            realDeviceHeight = displayMetrics.heightPixels;
        }

        // Dialog 내용 설정
        final TextView noItemTitle = dialog.findViewById(R.id.noItemTitle);
        final RecyclerView recyclerView = dialog.findViewById(R.id.recyclerView);
        final ProgressWheel progressWheel = dialog.findViewById(R.id.progressWheel);
        final ImageView allPlayButton = dialog.findViewById(R.id.allPlayButton);
        final ImageButton backButton = dialog.findViewById(R.id.backButton);
        final ImageView imageView = dialog.findViewById(R.id.imageView);
        final TextView songSizeText = dialog.findViewById(R.id.songSize);

        final RhyaSearchResultLyricsAdapter rhyaSearchResultLyricsAdapter = new RhyaSearchResultLyricsAdapter(activity);

        recyclerView.setAdapter(rhyaSearchResultLyricsAdapter);
        recyclerView.setLayoutManager(new LinearLayoutManager(activity.getApplicationContext()));
        progressWheel.setVisibility(View.VISIBLE);
        dialog.show();

        ViewGroup.LayoutParams layoutParams = imageView.getLayoutParams();
        layoutParams.height = (int) (realDeviceHeight / 3.5);
        imageView.setLayoutParams(layoutParams);
        backButton.setOnClickListener(v -> dialog.dismiss());

        dialog.show();

        Glide.with(activity)
                .load("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/utils/anim_image_random.jsp?type=1")
                .diskCacheStrategy(DiskCacheStrategy.NONE)
                .skipMemoryCache(true)
                .placeholder(R.drawable.img_load_error)
                .error(R.drawable.img_load_error)
                .fallback(R.drawable.img_load_error)
                .addListener(new RequestListener<Drawable>() {
                    @Override
                    public boolean onLoadFailed(@Nullable GlideException e, Object model, Target<Drawable> target, boolean isFirstResource) {
                        eventDialogSettingType_3(rhyaMusicInfoVOArrayList, noItemTitle, progressWheel, allPlayButton, songSizeText, rhyaSearchResultLyricsAdapter);

                        return false;
                    }

                    @Override
                    public boolean onResourceReady(Drawable resource, Object model, Target<Drawable> target, DataSource dataSource, boolean isFirstResource) {
                        eventDialogSettingType_3(rhyaMusicInfoVOArrayList, noItemTitle, progressWheel, allPlayButton, songSizeText, rhyaSearchResultLyricsAdapter);

                        return false;
                    }
                })
                .into(imageView);

        imageView.setColorFilter(Color.parseColor("#BDBDBD"), PorterDuff.Mode.MULTIPLY);
    }
    private void eventDialogSettingType_3(ArrayList<RhyaMusicInfoVO> rhyaMusicInfoVOArrayList, TextView noItemTitle, ProgressWheel progressWheel, ImageView allPlayButton, TextView songSizeText, RhyaSearchResultLyricsAdapter rhyaSearchResultLyricsAdapter) {
        progressWheel.setVisibility(View.INVISIBLE);

        StringBuilder stringBuilder;
        stringBuilder = new StringBuilder();
        stringBuilder.append(rhyaMusicInfoVOArrayList.size());
        stringBuilder.append("곡");

        allPlayButton.setOnClickListener(v -> ((ActivityMain) activity).putMusicArrayListRhyaMusicVO(rhyaMusicInfoVOArrayList));

        songSizeText.setText(stringBuilder.toString());

        if (rhyaMusicInfoVOArrayList.size() > 0) {
            noItemTitle.setVisibility(View.GONE);
        }else {
            noItemTitle.setVisibility(View.VISIBLE);
        }

        rhyaSearchResultLyricsAdapter.setList(rhyaMusicInfoVOArrayList, false);
    }
    // =========================================================================
    // =========================================================================



    // =========================================================================
    // =========================================================================
    public void setDialogSettingType_4(String title, String message, String buttonYes, String buttonNo, DialogListener_YesOrNo dialogListener) {
        // Dialog 내용 설정
        TextView titleTextView = dialog.findViewById(R.id.title);
        TextView messageTextView = dialog.findViewById(R.id.message);
        Button yesButton = dialog.findViewById(R.id.buttonYes);
        Button noButton = dialog.findViewById(R.id.buttonNo);

        titleTextView.setText(title);
        messageTextView.setText(message);
        yesButton.setText(buttonYes);
        noButton.setText(buttonNo);
        yesButton.setOnClickListener(view1 -> dialogListener.onClickListenerButtonYes(dialog));
        noButton.setOnClickListener(view1 -> dialogListener.onClickListenerButtonNo(dialog));
    }
    public interface DialogListener_YesOrNo {
        void onClickListenerButtonYes(Dialog dialog);
        void onClickListenerButtonNo(Dialog dialog);
    }
    // =========================================================================
    // =========================================================================



    // =========================================================================
    // =========================================================================
    public void setAndShowDialogSettingType_5(String title, String initName, int initImage, DialogListener_PlayListOkButtonListener dialogListener) {
        // 이미지 선택 Dialog 설정
        Point size = new Point();
        // Dialog 기본 설정
        Dialog dialogForImageSelect = new Dialog(activity);
        dialogForImageSelect.getWindow().setBackgroundDrawable(background);
        dialogForImageSelect.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialogForImageSelect.setCancelable(true);
        dialogForImageSelect.getWindow().setGravity(Gravity.BOTTOM);
        dialogForImageSelect.setContentView(R.layout.dialog_playlist_image_select);
        WindowManager.LayoutParams params = dialogForImageSelect.getWindow().getAttributes();
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
            params.width = Math.round((legacySize.getWidth()));
        }else {
            activity.getWindowManager().getDefaultDisplay().getRealSize(size);
            params.width = Math.round((size.x));
        }
        params.windowAnimations = R.style.AnimationPopupStyle;

        dialogForImageSelect.getWindow().setAttributes(params);

        // Dialog 내용 설정
        TextView titleTextView = dialog.findViewById(R.id.title);
        TextView okTextViewButton = dialog.findViewById(R.id.okTextViewButton);
        EditText searchEditText = dialog.findViewById(R.id.searchEditText);
        TextView count = dialog.findViewById(R.id.count);
        ImageView playListImageView = dialog.findViewById(R.id.playListImageView);
        ImageButton backButton = dialog.findViewById(R.id.backButton);


        Button okButton = dialogForImageSelect.findViewById(R.id.okButton);
        RhyaMyPlaylistImageSelectAdapter rhyaMyPlaylistImageSelectAdapter = new RhyaMyPlaylistImageSelectAdapter();
        LinearLayoutManager linearLayoutManager = new LinearLayoutManager(activity);
        linearLayoutManager.setOrientation(LinearLayoutManager.HORIZONTAL);
        RecyclerView recyclerView = dialogForImageSelect.findViewById(R.id.recyclerView);
        recyclerView.setLayoutManager(linearLayoutManager);
        recyclerView.setAdapter(rhyaMyPlaylistImageSelectAdapter);
        rhyaMyPlaylistImageSelectAdapter.setList();

        titleTextView.setText(title);
        searchEditText.setFilters(new InputFilter[] {( source, start, end, dest, dstart, dend) -> source,new InputFilter.LengthFilter(40)});
        backButton.setOnClickListener(v -> dialog.dismiss());
        okTextViewButton.setOnClickListener(v -> {
            String inputValue = searchEditText.getText().toString().replace(System.lineSeparator(), "");

            // 입력 확인
            if (!(inputValue.length() > 0)) {
                // 메시지 박스 출력
                new RhyaDialogManager(activity).createDialog_Confirm(activity, "플레이리스트 생성 불가", "이름은 공백으로 설정 할 수 없습니다. 플레이리스트의 이름을 입력해주세요.", "확인", true, Dialog::dismiss);

                return;
            }

            // 길이 확인
            if (searchEditText.getText().toString().length() <= 40) {
                dialogListener.onClickListenerOkButton(dialog, inputValue, imageSelected);
            }
        });

        okButton.setOnClickListener(v -> {
            if (rhyaMyPlaylistImageSelectAdapter.selectedPos != -1) {
                RhyaImageData image = rhyaMyPlaylistImageSelectAdapter.listMain.get(rhyaMyPlaylistImageSelectAdapter.selectedPos);

                Glide.with(activity)
                        .load(image.getImage())
                        .placeholder(R.drawable.app_logo_for_black)
                        .error(R.drawable.app_logo_for_black)
                        .fallback(R.drawable.app_logo_for_black)
                        .into(playListImageView);

                imageSelected = image.getId();
            }else {
                Glide.with(activity)
                        .load(R.drawable.app_logo_for_black)
                        .placeholder(R.drawable.app_logo_for_black)
                        .error(R.drawable.app_logo_for_black)
                        .fallback(R.drawable.app_logo_for_black)
                        .into(playListImageView);

                imageSelected = R.drawable.app_logo_for_black;
            }

            dialogForImageSelect.dismiss();
        });

        final int maxLen = 40;
        searchEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

            }

            @Override
            public void afterTextChanged(Editable editable) {
                StringBuilder stringBuilder;
                stringBuilder = new StringBuilder();
                stringBuilder.append(searchEditText.getText().toString().length());
                stringBuilder.append("/");
                stringBuilder.append(maxLen);

                count.setText(stringBuilder.toString());
            }
        });
        searchEditText.setText(initName);

        RhyaPlayListImageManager rhyaPlayListImageManager = new RhyaPlayListImageManager();

        Glide.with(activity)
                .load(rhyaPlayListImageManager.getImageID(initImage))
                .placeholder(R.drawable.app_logo_for_black)
                .error(R.drawable.app_logo_for_black)
                .fallback(R.drawable.app_logo_for_black)
                .into(playListImageView);

        playListImageView.setOnClickListener(v -> dialogForImageSelect.show());

        dialog.show();
    }
    public interface DialogListener_PlayListOkButtonListener {
        void onClickListenerOkButton(Dialog dialog, String title, final int image);
    }
    // =========================================================================
    // =========================================================================



    // =========================================================================
    // =========================================================================
    public void setAndShowDialogSettingType_6(RhyaMusicInfoVO item, RhyaMyPlayListInfoAdapter rhyaMyPlayListInfoAdapter, int pos) {
        // Dialog 내용 설정
        final TextView songName = dialog.findViewById(R.id.title);
        final TextView singerName = dialog.findViewById(R.id.singer);
        final Button closeButton = dialog.findViewById(R.id.closeButton);
        final TextView songInfoTextViewButton = dialog.findViewById(R.id.songInfoTextViewButton);
        final TextView addPlayListTextViewButton = dialog.findViewById(R.id.addPlayListTextViewButton);
        final TextView deleteTextViewButton = dialog.findViewById(R.id.deleteTextViewButton);

        songName.setText(item.getName());
        singerName.setText(item.getSinger());

        closeButton.setOnClickListener(v -> dialog.dismiss());
        songInfoTextViewButton.setOnClickListener(v -> {
            dialog.dismiss();
            ((ActivityMain) activity).showSongInfoDialog(item.getUuid());
        });
        addPlayListTextViewButton.setOnClickListener(v -> {
            dialog.dismiss();
            ((ActivityMain) activity).putNowPlayListSong(item.getUuid());
        });
        deleteTextViewButton.setOnClickListener(v -> {
            dialog.dismiss();
            rhyaMyPlayListInfoAdapter.removeItem(pos);
        });

        dialog.show();
    }
    // =========================================================================
    // =========================================================================



    // =========================================================================
    // =========================================================================
    public void setAndShowDialogSettingType_7(ArrayList<RhyaMusicInfoVO> listMain, String HEADER_TYPE_SETTING_NAME, RhyaMyPlayListInfoAdapter rhyaMyPlayListInfoAdapter) {
        // Dialog 내용 설정
        final ImageButton xImageButton = dialog.findViewById(R.id.xImageButton);
        final ConstraintLayout noResultLayout = dialog.findViewById(R.id.noResultLayout);
        final TextView saveButton = dialog.findViewById(R.id.saveButton);
        final TextView songCount = dialog.findViewById(R.id.songCount);
        final EditText searchEditText = dialog.findViewById(R.id.searchEditText);
        final ImageButton backButton = dialog.findViewById(R.id.backButton);

        backButton.setOnClickListener(v -> dialog.dismiss());

        final RecyclerView recyclerView = dialog.findViewById(R.id.recyclerView);
        final RhyaMyPlayListAddSongAdapter rhyaMyPlayListAddSongAdapter = new RhyaMyPlayListAddSongAdapter(songCount, listMain.size());
        final LinearLayoutManager linearLayoutManager = new LinearLayoutManager(activity);
        recyclerView.setAdapter(rhyaMyPlayListAddSongAdapter);
        recyclerView.setLayoutManager(linearLayoutManager);

        songCount.setText("0개 선택됨");

        final ArrayList<RhyaMusicInfoVOv2> rhyaMusicInfoVOv2s = new ArrayList<>();

        new RhyaAsyncTask<String, String>() {
            @Override
            protected void onPreExecute() {
                dialog.show();

                ((ActivityMain) activity).showTaskDialog();
            }

            @Override
            protected String doInBackground(String arg) {
                for (String uuid : RhyaApplication.rhyaMusicInfoVOHashMap.keySet()) {
                    RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);

                    if (rhyaMusicInfoVO != null) {
                        RhyaMusicInfoVOv2 rhyaMusicInfoVOv2 = new RhyaMusicInfoVOv2(rhyaMusicInfoVO.getUuid(),
                                rhyaMusicInfoVO.getName(),
                                rhyaMusicInfoVO.getTime(),
                                rhyaMusicInfoVO.getLyrics(),
                                rhyaMusicInfoVO.getSinger(),
                                rhyaMusicInfoVO.getSingerUuid(),
                                rhyaMusicInfoVO.getSingerImage(),
                                rhyaMusicInfoVO.getSongWriter(),
                                rhyaMusicInfoVO.getImage(),
                                rhyaMusicInfoVO.getMp3(),
                                rhyaMusicInfoVO.getType(),
                                rhyaMusicInfoVO.getDate(),
                                rhyaMusicInfoVO.getVersion(),
                                false,
                                null,
                                0,
                                0);

                        boolean isNoExt = false;
                        for (RhyaMusicInfoVO checker : listMain) {
                            if (checker != null && !checker.getUuid().equals(HEADER_TYPE_SETTING_NAME) && checker.getUuid().equals(rhyaMusicInfoVOv2.getUuid())) {
                                isNoExt = true;

                                break;
                            }
                        }

                        if (!isNoExt) {
                            rhyaMusicInfoVOv2s.add(rhyaMusicInfoVOv2);
                        }
                    }
                }

                return null;
            }

            @Override
            protected void onPostExecute(String result) {
                rhyaMyPlayListAddSongAdapter.setList(rhyaMusicInfoVOv2s);

                noResultLayout.setVisibility(View.GONE);

                searchEditText.addTextChangedListener(new TextWatcher() {
                    @Override
                    public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {

                    }

                    @Override
                    public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {

                    }

                    @Override
                    public void afterTextChanged(Editable editable) {
                        if (searchEditText.getText().toString().length() >= 1) {
                            noResultLayout.setVisibility(View.GONE);
                            xImageButton.setVisibility(View.VISIBLE);

                            new RhyaAsyncTask<String, String>() {
                                private final ArrayList<RhyaMusicInfoVOv2> rhyaMusicInfoVOv2sForNew = new ArrayList<>();
                                @Override
                                protected void onPreExecute() {

                                }

                                @Override
                                protected String doInBackground(String arg) {
                                    for (RhyaMusicInfoVOv2 rhyaMusicInfoVOv2 : rhyaMusicInfoVOv2s) {
                                        if (rhyaMusicInfoVOv2.getName().contains(arg)) {
                                            rhyaMusicInfoVOv2sForNew.add(rhyaMusicInfoVOv2);
                                        }
                                    }

                                    return null;
                                }

                                @Override
                                protected void onPostExecute(String result) {
                                    if (rhyaMusicInfoVOv2sForNew.size() <= 0) {
                                        noResultLayout.setVisibility(View.VISIBLE);
                                    }else {
                                        noResultLayout.setVisibility(View.GONE);
                                    }

                                    rhyaMyPlayListAddSongAdapter.setList(rhyaMusicInfoVOv2sForNew);
                                }
                            }.execute(searchEditText.getText().toString());
                        }else {
                            noResultLayout.setVisibility(View.GONE);
                            xImageButton.setVisibility(View.GONE);

                            rhyaMyPlayListAddSongAdapter.setList(rhyaMusicInfoVOv2s);
                        }
                    }
                });

                saveButton.setOnClickListener(v -> new RhyaAsyncTask<String, String>() {
                    @Override
                    protected void onPreExecute() {
                        ((ActivityMain) activity).showTaskDialog();
                    }

                    @Override
                    protected String doInBackground(String arg) {
                        int inputCount = 100 - (listMain.size() - 2);
                        for (RhyaMusicInfoVOv2 rhyaMusicInfoVOv2 : rhyaMusicInfoVOv2s) {
                            if (rhyaMusicInfoVOv2.getIsPlay()) {
                                if (inputCount < 0) {
                                    break;
                                }

                                inputCount --;

                                RhyaMusicInfoVO rhyaMusicInfoVO = new RhyaMusicInfoVO(
                                        rhyaMusicInfoVOv2.getUuid(),
                                        rhyaMusicInfoVOv2.getName(),
                                        rhyaMusicInfoVOv2.getTime(),
                                        rhyaMusicInfoVOv2.getLyrics(),
                                        rhyaMusicInfoVOv2.getSinger(),
                                        rhyaMusicInfoVOv2.getSingerUuid(),
                                        rhyaMusicInfoVOv2.getSingerImage(),
                                        rhyaMusicInfoVOv2.getSongWriter(),
                                        rhyaMusicInfoVOv2.getImage(),
                                        rhyaMusicInfoVOv2.getMp3(),
                                        rhyaMusicInfoVOv2.getType(),
                                        rhyaMusicInfoVOv2.getDate(),
                                        rhyaMusicInfoVOv2.getVersion()
                                );

                                rhyaMyPlayListInfoAdapter.listMain.add(rhyaMusicInfoVO);
                                activity.runOnUiThread(() -> rhyaMyPlayListInfoAdapter.notifyItemInserted(rhyaMyPlayListInfoAdapter.listMain.size() - 1));
                            }
                        }

                        return null;
                    }

                    @Override
                    protected void onPostExecute(String result) {
                        ((ActivityMain) activity).dismissTaskDialog();

                        rhyaMyPlayListInfoAdapter.notifyItemChanged(1);
                        dialog.dismiss();

                    }
                }.execute(null));

                ((ActivityMain) activity).dismissTaskDialog();
            }
        }.execute(null);
    }
    // =========================================================================
    // =========================================================================



    // =========================================================================
    // =========================================================================
    public void showDialog() {
        if (!dialog.isShowing()) {
            dialog.show();
        }
    }
    public void dismissDialog() {
        if (dialog.isShowing()) {
            dialog.dismiss();
        }
    }
    // =========================================================================
    // =========================================================================
}
