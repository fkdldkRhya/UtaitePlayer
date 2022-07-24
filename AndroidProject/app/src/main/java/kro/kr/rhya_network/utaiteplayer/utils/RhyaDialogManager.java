package kro.kr.rhya_network.utaiteplayer.utils;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.Dialog;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.graphics.Insets;
import android.graphics.Point;
import android.graphics.PorterDuff;
import android.graphics.Rect;
import android.graphics.drawable.ColorDrawable;
import android.graphics.drawable.Drawable;
import android.util.DisplayMetrics;
import android.util.Size;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowInsets;
import android.view.WindowManager;
import android.view.WindowMetrics;
import android.webkit.CookieManager;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.load.DataSource;
import com.bumptech.glide.load.engine.DiskCacheStrategy;
import com.bumptech.glide.load.engine.GlideException;
import com.bumptech.glide.request.RequestListener;
import com.bumptech.glide.request.target.Target;
import com.pnikosis.materialishprogress.ProgressWheel;

import org.json.JSONObject;

import java.net.URLDecoder;
import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaSongTypeDialogAdapter;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.core.RhyaCore;

public class RhyaDialogManager {
    private final Activity activity;
    public RhyaDialogManager(Activity activity) {
        this.activity = activity;
    }


    // Callback 인터페이스
    // ----------------------------------------------
    public interface DialogListener_Confirm {
        void onClickListenerButtonConfirm(Dialog dialog);
    }
    public interface DialogListener_Task {
        void taskListener(Dialog dialog);
    }
    public interface DialogListener_YesOrNo {
        void onClickListenerButtonYes(Dialog dialog);
        void onClickListenerButtonNo(Dialog dialog);
    }
    public interface DialogListener_WebView {
        void webViewCloseListener();
    }
    // ----------------------------------------------

    // Dialog info
    // ----------------------------------------------
    public final Drawable background = new ColorDrawable(Color.TRANSPARENT);
    public final Point size = new Point();
    // ----------------------------------------------


    /**
     * Dialog 생성 함수 / Confirm
     * @param context context
     * @param title 제목
     * @param message 내용
     * @param button 버튼 내용
     * @param isCancelable 취소 가능 여부
     * @param dialogListener 이벤트 리스너
     */
    public void createDialog_Confirm(Context context,
                                String title,
                                String message,
                                String button,
                                boolean isCancelable,
                                DialogListener_Confirm dialogListener) {
        // Dialog 기본 설정
        Dialog dialog = new Dialog(context);
        dialog.getWindow().setBackgroundDrawable(background);
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialog.setContentView(R.layout.dialog_confirm);
        dialog.setCancelable(isCancelable);
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

        params.height = WindowManager.LayoutParams.WRAP_CONTENT;
        dialog.getWindow().setAttributes(params);
        // Dialog 내용 설정
        TextView titleTextView = dialog.findViewById(R.id.title);
        TextView messageTextView = dialog.findViewById(R.id.message);
        Button confirmButton = dialog.findViewById(R.id.button);

        titleTextView.setText(title);
        messageTextView.setText(message);
        confirmButton.setText(button);
        confirmButton.setOnClickListener(view1 -> dialogListener.onClickListenerButtonConfirm(dialog));

        dialog.show();
    }


    /**
     * Dialog 생성 함수 / Yes or No
     * @param context context
     * @param message 내용
     * @param isCancelable 취소 가능 여부
     * @param dialogListener 이벤트 리스너
     */
    public void createDialog_YesOrNo(Context context,
                                     String title,
                                     String message,
                                     String buttonYes,
                                     String buttonNo,
                                     boolean isCancelable,
                                     DialogListener_YesOrNo dialogListener) {
        // Dialog 기본 설정
        Dialog dialog = new Dialog(context);
        dialog.getWindow().setBackgroundDrawable(background);
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialog.setContentView(R.layout.dialog_yes_no);
        dialog.setCancelable(isCancelable);
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
        params.height = WindowManager.LayoutParams.WRAP_CONTENT;
        dialog.getWindow().setAttributes(params);
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

        dialog.show();
    }


    /**
     * Dialog 생성 함수 / Task
     * @param context context
     * @param message 내용
     * @param isCancelable 취소 가능 여부
     * @param dialogListener 이벤트 리스너
     */
    public void createDialog_Task(Context context,
                                  String message,
                                  boolean isCancelable,
                                  DialogListener_Task dialogListener) {
        // Dialog 기본 설정
        Dialog dialog = new Dialog(context);
        dialog.getWindow().setBackgroundDrawable(background);
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialog.setContentView(R.layout.dialog_task);
        dialog.setCancelable(isCancelable);
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
        params.height = WindowManager.LayoutParams.WRAP_CONTENT;
        dialog.getWindow().setAttributes(params);
        // Dialog 내용 설정
        TextView messageTextView = dialog.findViewById(R.id.message);

        messageTextView.setText(message);

        dialog.show();

        dialogListener.taskListener(dialog);
    }


    /**
     * Dialog 생성 함수 / WebView
     * @param context context
     * @param urli URL
     * @param title 제못
     * @param isUrlRhyaOnly RHYA.Network 도메인 확인 여부
     * @param rhyaCore RhyaCore class
     * @param dialogListener_webView listener
     */
    @SuppressLint("SetJavaScriptEnabled")
    public void createDialog_WebView(Context context,
                                     String urli,
                                     String title,
                                     boolean isUrlRhyaOnly,
                                     RhyaCore rhyaCore,
                                     DialogListener_WebView dialogListener_webView) {
        // Dialog 기본 설정
        Dialog dialog = new Dialog(context);
        dialog.getWindow().setBackgroundDrawable(background);
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialog.setContentView(R.layout.dialog_webview);
        dialog.setCancelable(true);
        // Dialog 사이즈 설정
        WindowManager.LayoutParams params = dialog.getWindow().getAttributes();

        params.width = WindowManager.LayoutParams.MATCH_PARENT;
        params.height = WindowManager.LayoutParams.MATCH_PARENT;
        dialog.getWindow().setAttributes(params);
        // Dialog 내용 설정
        WebView webView = dialog.findViewById(R.id.webView);
        ImageButton button = dialog.findViewById(R.id.button);
        ProgressWheel progressWheel = dialog.findViewById(R.id.progressWheel);
        TextView textView = dialog.findViewById(R.id.title);

        button.setOnClickListener(v -> {
            if (dialogListener_webView != null) dialogListener_webView.webViewCloseListener();
            dialog.dismiss();
        });
        textView.setText(title);

        CookieManager cookieManager = CookieManager.getInstance();
        cookieManager.removeSessionCookies(null);
        cookieManager.removeAllCookies(null);
        WebSettings webSettings = webView.getSettings();
        webSettings.setJavaScriptEnabled(true);
        webSettings.setLoadWithOverviewMode(true);
        webSettings.setDefaultTextEncodingName("UTF-8");
        webSettings.setDomStorageEnabled(true);
        webSettings.setTextZoom(100);
        webView.setWebViewClient(new WebViewClient() {
            @Override
            public void onPageStarted(WebView view, String url, Bitmap favicon) {
                super.onPageStarted(view, url, favicon);

                // UI 설정
                view.setVisibility(View.INVISIBLE);
                progressWheel.setVisibility(View.VISIBLE);

                // URL 확인
                if (!rhyaCore.isAccessURL(url) && isUrlRhyaOnly) {
                    view.stopLoading();
                    view.loadUrl(urli);
                }
            }

            @Override
            public void onPageFinished(WebView view, String url) {
                super.onPageFinished(view, url);

                // UI 설정
                view.setVisibility(View.VISIBLE);
                progressWheel.setVisibility(View.INVISIBLE);
            }

            @Override
            public boolean shouldOverrideUrlLoading(WebView view, String url) {
                view.loadUrl(url);
                return true;
            }
        });

        webView.loadUrl(urli);

        dialog.show();
    }


    /**
     * Dialog 생성 함수 / Update text
     * @param context context
     * @param version version
     * @param button 버튼 내용
     * @param isCancelable 취소 가능 여부
     * @param rhyaSharedPreferences rhyaSharedPreferences
     * @param dialogListener 이벤트 리스너
     */
    public void createDialog_UpdateTextConfirm(Context context,
                                               String version,
                                               String button,
                                               boolean isCancelable,
                                               RhyaSharedPreferences rhyaSharedPreferences,
                                               DialogListener_Confirm dialogListener) {
        // Dialog 기본 설정
        Dialog dialog = new Dialog(context);
        dialog.getWindow().setBackgroundDrawable(background);
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialog.setContentView(R.layout.dialog_update_text);
        dialog.setCancelable(isCancelable);
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

        params.height = WindowManager.LayoutParams.WRAP_CONTENT;
        dialog.getWindow().setAttributes(params);
        // Dialog 내용 설정
        TextView titleTextView = dialog.findViewById(R.id.title);
        TextView messageTextView = dialog.findViewById(R.id.message);
        Button confirmButton = dialog.findViewById(R.id.button);

        titleTextView.setText(new StringBuilder().append("Ver ").append(version).append(" 패치내역").toString());

        // 서버 통신 비동기 작업
        new RhyaAsyncTask<String, String>() {
            @Override
            protected void onPreExecute() {
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    RhyaHttpConnection rhyaHttpConnection = new RhyaHttpConnection();

                    return new JSONObject(URLDecoder.decode(rhyaHttpConnection.request("https://rhya-network.kro.kr/RhyaNetwork/utaite_player_manager?mode=6", null), "UTF-8")).getString("update_description");
                }catch (Exception ex) {
                    ex.printStackTrace();

                    rhyaSharedPreferences.removeString("_UPDATE_TEXT_CHECKER_", context);

                    return null;
                }
            }

            @Override
            protected void onPostExecute(String result) {
                if (result == null) return;

                messageTextView.setText(result);
                confirmButton.setText(button);
                confirmButton.setOnClickListener(view1 -> dialogListener.onClickListenerButtonConfirm(dialog));

                dialog.show();
            }
        }.execute(null);
    }


    /**
     * Dialog 생성 함수 / Song type list
     * @param context context
     * @param isCancelable 취소 가능 여부
     */
    public void createDialog_SongTypeList(Context context,
                                          Fragment fragment,
                                          String type,
                                          boolean isCancelable) {
        // Dialog 기본 설정
        Dialog dialog = new Dialog(context);
        dialog.getWindow().setBackgroundDrawable(background);
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialog.setContentView(R.layout.dialog_song_type);
        dialog.setCancelable(isCancelable);
        // Dialog 사이즈 설정
        WindowManager.LayoutParams params = dialog.getWindow().getAttributes();
        params.width = WindowManager.LayoutParams.MATCH_PARENT;
        params.height = WindowManager.LayoutParams.MATCH_PARENT;
        dialog.getWindow().setAttributes(params);
        // Dialog 내용 설정
        RecyclerView recyclerView = dialog.findViewById(R.id.recyclerView);
        ProgressWheel progressWheel = dialog.findViewById(R.id.progressWheel);

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

        final ImageView allPlayButton = dialog.findViewById(R.id.allPlayButton);
        final ImageButton backButton = dialog.findViewById(R.id.backButton);
        final ImageView imageView = dialog.findViewById(R.id.imageView);
        final TextView songSizeText = dialog.findViewById(R.id.songSize);
        final TextView typeTitleText = dialog.findViewById(R.id.typeTitleText);


        ViewGroup.LayoutParams layoutParams = imageView.getLayoutParams();
        layoutParams.height = (int) (realDeviceHeight / 3.5);
        imageView.setLayoutParams(layoutParams);
        backButton.setOnClickListener(v -> dialog.dismiss());
        typeTitleText.setText(type);


        final RhyaSongTypeDialogAdapter rhyaSongTypeDialogAdapter = new RhyaSongTypeDialogAdapter(fragment);
        recyclerView.setAdapter(rhyaSongTypeDialogAdapter);
        recyclerView.setLayoutManager(new LinearLayoutManager(context));
        progressWheel.setVisibility(View.VISIBLE);
        dialog.show();

        Glide.with(context)
                .load("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/utils/anim_image_random.jsp?type=1")
                .diskCacheStrategy(DiskCacheStrategy.NONE)
                .skipMemoryCache(true)
                .placeholder(R.drawable.img_load_error)
                .error(R.drawable.img_load_error)
                .fallback(R.drawable.img_load_error)
                .addListener(new RequestListener<Drawable>() {
                    @Override
                    public boolean onLoadFailed(@Nullable GlideException e, Object model, Target<Drawable> target, boolean isFirstResource) {
                        songTypeDialogListenerEvent(type, context, progressWheel, allPlayButton, songSizeText, rhyaSongTypeDialogAdapter);

                        return false;
                    }

                    @Override
                    public boolean onResourceReady(Drawable resource, Object model, Target<Drawable> target, DataSource dataSource, boolean isFirstResource) {
                        songTypeDialogListenerEvent(type, context, progressWheel, allPlayButton, songSizeText, rhyaSongTypeDialogAdapter);

                        return false;
                    }
                })
                .into(imageView);

        imageView.setColorFilter(Color.parseColor("#BDBDBD"), PorterDuff.Mode.MULTIPLY);
    }
    private void songTypeDialogListenerEvent(String type, Context context, ProgressWheel progressWheel, ImageView allPlayButton, TextView songSizeText, RhyaSongTypeDialogAdapter rhyaSongTypeDialogAdapter) {
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
                    for (String key : RhyaApplication.rhyaMusicInfoVOHashMap.keySet()) {
                        try {
                            RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(key);
                            assert rhyaMusicInfoVO != null;
                            if (rhyaMusicInfoVO.getType().contains(type))
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

                    rhyaSongTypeDialogAdapter.setList(arrayList);
                }
            }
        }.execute(null);
    }
}
