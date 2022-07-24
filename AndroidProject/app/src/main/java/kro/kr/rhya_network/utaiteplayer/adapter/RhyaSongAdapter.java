package kro.kr.rhya_network.utaiteplayer.adapter;

import android.annotation.SuppressLint;

import android.app.Activity;
import android.app.Dialog;
import android.content.ContentValues;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.signature.ObjectKey;
import com.github.siyamed.shapeimageview.RoundedImageView;
import com.pnikosis.materialishprogress.ProgressWheel;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.text.MessageFormat;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.Iterator;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.fragment.HomeFragment;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHttpsConnection;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicDataVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaUserDataVO;

public class RhyaSongAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    private final int VIEW_TYPE_ITEM = 0;

    public ArrayList<RhyaMusicInfoVO> musicList;

    public boolean isLoading;

    // Loading data
    private ArrayList<RhyaMusicInfoVO> loadedDataNewSong = null;
    public boolean isUseLoadedData = false;


    private final Fragment fragment;
    private Context context;


    public RhyaSongAdapter(Fragment fragment) {
        this.fragment = fragment;
        isLoading = false;
    }


    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        if (viewType == 2) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_fragment_home, parent, false);

            context = view.getContext();

            return new ViewHolderHomeFragment(view);
        }

        if (viewType == VIEW_TYPE_ITEM) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_song, parent, false);

            context = view.getContext();

            return new ViewHolder(view);
        }else {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_progress, parent, false);

            context = view.getContext();

            return new ViewHolderLoading(view);
        }
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        if (holder instanceof ViewHolder) {
            ((ViewHolder) holder).onBind(musicList.get(position));
        }else if (holder instanceof ViewHolderLoading) {
            ((ViewHolderLoading) holder).onBind();
        }else if (holder instanceof  ViewHolderHomeFragment) {
            ((ViewHolderHomeFragment) holder).onBind();
        }
    }

    @Override
    public int getItemViewType(int position) {
        if (musicList.get(position) == null) {
            return 1;
        }else if (musicList.get(position).getUuid().equals("main")) {
            return 2;
        }else {
            return VIEW_TYPE_ITEM;
        }
    }


    @SuppressLint("NotifyDataSetChanged")
    public void setList(ArrayList<RhyaMusicInfoVO> list) {
        this.musicList = list;

        notifyDataSetChanged();
    }


    @Override
    public int getItemCount() {
        return musicList == null ? 0 : musicList.size();
    }


    private class ViewHolderHomeFragment extends RecyclerView.ViewHolder {
        // UI Object
        private final TextView newSongAllPlayerTitleTextView;
        private final TextView mySubscribeTitleTextView;
        private final TextView mySubscribeCountTextView;
        // Adapter
        private final RhyaNewSongAdapter rhyaNewSongAdapter;
        private final RhyaMySubscribeAdapter rhyaMySubscribeAdapter;
        // Auth token
        private String auth;


        public ViewHolderHomeFragment(@NonNull View view) {
            super(view);

            // UI Object
            newSongAllPlayerTitleTextView = view.findViewById(R.id.newSongAllPlayerTitleTextView);
            RecyclerView newMusicRecyclerView = view.findViewById(R.id.newSongRecyclerView);
            RecyclerView mySubscribeRecyclerView = view.findViewById(R.id.mySubscribeRecyclerView);
            mySubscribeTitleTextView = view.findViewById(R.id.mySubscribeTitleTextView);
            mySubscribeCountTextView = view.findViewById(R.id.mySubscribeCountTextView);
            // Adapter init
            rhyaNewSongAdapter = new RhyaNewSongAdapter(fragment.requireActivity());
            rhyaMySubscribeAdapter = new RhyaMySubscribeAdapter(fragment);
            // Adapter setting
            LinearLayoutManager newMusicLinearLayoutManager = new LinearLayoutManager(fragment.requireActivity());
            newMusicLinearLayoutManager.setOrientation(LinearLayoutManager.HORIZONTAL);
            LinearLayoutManager mySubscribeLinearLayoutManager = new LinearLayoutManager(fragment.requireActivity());
            mySubscribeLinearLayoutManager.setOrientation(LinearLayoutManager.HORIZONTAL);


            newMusicRecyclerView.setAdapter(rhyaNewSongAdapter);
            newMusicRecyclerView.setLayoutManager(newMusicLinearLayoutManager);
            rhyaNewSongAdapter.setList(new ArrayList<>());

            mySubscribeRecyclerView.setAdapter(rhyaMySubscribeAdapter);
            mySubscribeRecyclerView.setLayoutManager(mySubscribeLinearLayoutManager);
            rhyaMySubscribeAdapter.setList(RhyaApplication.rhyaMusicDataVO.getSubscribeList());

            mySubscribeTitleTextView.setText(String.format("%s님이 구독한 사람들", RhyaApplication.rhyaUserDataVO.getUserName()));
        }

        public void onBind() {
            if (isUseLoadedData) {
                newSongLoad(null, null);

                setMySubscribeCountTextView(RhyaApplication.rhyaMusicDataVO.getSubscribeList().size());

                rhyaMySubscribeAdapter.setList(RhyaApplication.rhyaMusicDataVO.getSubscribeList());

                mySubscribeTitleTextView.setText(String.format("%s님이 구독한 사람들", RhyaApplication.rhyaUserDataVO.getUserName()));

                return;
            }

            newSongAllPlayerTitleTextView.setOnClickListener(v -> {
                try {
                    ArrayList<RhyaMusicInfoVO> rhyaMusicInfoVOArrayList = rhyaNewSongAdapter.getList();
                    if (rhyaMusicInfoVOArrayList != null) {
                        ((ActivityMain) fragment.requireActivity()).putMusicArrayListRhyaMusicVO(rhyaMusicInfoVOArrayList);
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();
                }
            });

            ((ActivityMain) fragment.requireActivity()).rhyaSharedPreferences.removeString("LOAD_DATA_DATE", fragment.requireActivity());
            ((ActivityMain) fragment.requireActivity()).rhyaDialogManager.createDialog_Task(fragment.requireContext(),
                    "작업 처리 중...",
                    false,
                    dialogSub -> new RhyaAsyncTask<String, String>() {
                        @Override
                        protected void onPreExecute() {
                            ((ActivityMain) fragment.requireActivity()).rhyaSharedPreferences.removeString((((ActivityMain) fragment.requireActivity())).rhyaSharedPreferences.SHARED_PREFERENCES_USER_METADATA_CHANGE, fragment.requireActivity());
                            ((ActivityMain) fragment.requireActivity()).rhyaSharedPreferences.removeString((((ActivityMain) fragment.requireActivity())).rhyaSharedPreferences.SHARED_PREFERENCES_USER_DATA_CHANGE, fragment.requireActivity());
                        }

                        @Override
                        protected String doInBackground(String arg) {
                            try {
                                try {
                                    auth = ((ActivityMain) fragment.requireActivity()).rhyaCore.getAutoLogin(((ActivityMain) fragment.requireActivity()).rhyaSharedPreferences, fragment.requireActivity());

                                    return "success";
                                } catch (NoSuchPaddingException | InvalidKeyException | IOException |
                                        IllegalBlockSizeException | BadPaddingException | NoSuchAlgorithmException |
                                        InvalidAlgorithmParameterException e) {

                                    e.printStackTrace();

                                    // 예외 처리
                                    return "00008";
                                }
                            } catch (Exception ex) {
                                ex.printStackTrace();

                                return "00017";
                            }
                        }

                        @Override
                        protected void onPostExecute(String result) {
                            if (result != null) {
                                if (result.equals("00017")) {
                                    ((ActivityMain) fragment.requireActivity()).rhyaDialogManager.createDialog_Confirm(fragment.requireActivity(),
                                            "Unknown Error",
                                            "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00017)",
                                            "닫기",
                                            false,
                                            dialog -> {
                                                dialog.dismiss();
                                                dialogSub.dismiss();
                                            });

                                    return;
                                }

                                if (result.equals("00008")) {
                                    ((ActivityMain) fragment.requireActivity()).rhyaDialogManager.createDialog_Confirm(fragment.requireActivity(),
                                            "Data Loading Error",
                                            "데이터 로딩 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00008)",
                                            "닫기",
                                            false,
                                            dialog -> {
                                                dialog.dismiss();
                                                dialogSub.dismiss();
                                            });

                                    return;
                                }

                                // 비동기 처리
                                new RhyaAsyncTask<String, String>() {

                                    @Override
                                    protected void onPreExecute() {

                                    }

                                    @Override
                                    protected String doInBackground(String arg) {
                                        try {
                                            RhyaUserDataVO rhyaUserDataVO = ((ActivityMain) fragment.requireActivity()).rhyaCore.getUserInfo(auth, (((ActivityMain) fragment.requireActivity())).rhyaSharedPreferences, ((ActivityMain) fragment.requireActivity()).rhyaAESManager, fragment.requireActivity(), fragment.requireActivity());

                                            RhyaMusicDataVO rhyaMusicDataVO = ((ActivityMain) fragment.requireActivity()).rhyaCore.getMetaDataInfo(auth, ((ActivityMain) fragment.requireActivity()).rhyaSharedPreferences, ((ActivityMain) fragment.requireActivity()).rhyaAESManager, fragment.requireActivity(), fragment.requireActivity());

                                            if (rhyaMusicDataVO != null && rhyaUserDataVO != null) {
                                                RhyaApplication.rhyaMusicDataVO = rhyaMusicDataVO;
                                                RhyaApplication.rhyaUserDataVO = rhyaUserDataVO;

                                                if (((ActivityMain) fragment.requireActivity()).rhyaCore.musicInfoReload(((ActivityMain) fragment.requireActivity()).rhyaAESManager,
                                                        ((ActivityMain) fragment.requireActivity()).rhyaSharedPreferences,
                                                        fragment.requireActivity(),
                                                        fragment.requireActivity(),
                                                        auth)) {

                                                    ((ActivityMain) fragment.requireActivity()).rhyaCore.InitSingerInfo();
                                                }
                                            }
                                        }catch (Exception ignored) {}

                                        return null;
                                    }

                                    @Override
                                    protected void onPostExecute(String result) {

                                    }
                                }.execute(null);

                                newSongLoad(auth, dialogSub);

                                rhyaMySubscribeAdapter.setList(RhyaApplication.rhyaMusicDataVO.getSubscribeList());

                                setMySubscribeCountTextView(RhyaApplication.rhyaMusicDataVO.getSubscribeList().size());

                                mySubscribeTitleTextView.setText(String.format("%s님이 구독한 사람들", RhyaApplication.rhyaUserDataVO.getUserName()));

                                dialogSub.dismiss();
                            }else {
                                ((ActivityMain) fragment.requireActivity()).rhyaDialogManager.createDialog_Confirm(fragment.requireActivity(),
                                        "Data Loading Error",
                                        "데이터 로딩 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00007)",
                                        "닫기",
                                        false,
                                        dialog -> {
                                            dialog.dismiss();
                                            dialogSub.dismiss();
                                        });
                            }
                        }
                    }.execute(null));
        }

        // 최신 노래 로딩
        public void newSongLoad(String authToken, Dialog dialog) {
            if (loadedDataNewSong != null && isUseLoadedData) {
                rhyaNewSongAdapter.setList(loadedDataNewSong);

                return;
            }

            ArrayList<RhyaMusicInfoVO> rhyaMusicInfoVOArrayList = new ArrayList<>();

            String url = ((ActivityMain) fragment.requireActivity()).rhyaCore.MAIN_URL;

            // 서버 접속
            new RhyaAsyncTask<String, String>() {
                @Override
                protected void onPreExecute() {
                }

                @Override
                protected String doInBackground(String arg) {
                    try{
                        try {
                            RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                            ContentValues urlParm = new ContentValues();

                            urlParm.put("auth", authToken);
                            urlParm.put("mode", "1");
                            urlParm.put("new", "1");

                            JSONObject jsonObjectRoot = new JSONObject(rhyaHttpsConnection.request(url, urlParm));

                            for (Iterator<String> iter = jsonObjectRoot.keys(); iter.hasNext(); ) {
                                String key = iter.next();
                                JSONObject jsonObject = new JSONObject(jsonObjectRoot.getString(key));

                                if (!RhyaApplication.rhyaMusicInfoVOHashMap.containsKey(jsonObject.getString("uuid"))) {
                                    continue;
                                }

                                RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(jsonObject.getString("uuid"));
                                rhyaMusicInfoVOArrayList.add(Integer.parseInt(key) - 1, rhyaMusicInfoVO);
                            }
                        } catch (JSONException e) {
                            e.printStackTrace();

                            return "error";
                        }
                    }catch (Exception ex) {
                        ex.printStackTrace();

                        return "error";
                    }

                    return "success";
                }

                @Override
                protected void onPostExecute(String result) {
                    if (dialog != null) {
                        dialog.dismiss();
                    }

                    if (result.equals("success")) {
                        Activity activity = fragment.requireActivity();
                        if (!activity.isFinishing()) {
                            loadedDataNewSong = rhyaMusicInfoVOArrayList;
                            rhyaNewSongAdapter.setList(rhyaMusicInfoVOArrayList);

                            isUseLoadedData = true;
                        }
                    }else {
                        ((ActivityMain) fragment.requireActivity()).rhyaDialogManager.createDialog_Confirm(fragment.requireContext(),
                                "Data Loading Error",
                                "데이터 로딩 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00009)",
                                "닫기",
                                false,
                                Dialog::dismiss);
                    }
                }
            }.execute(null);
        }

        public void setMySubscribeCountTextView(int count) {
            mySubscribeCountTextView.setText(MessageFormat.format("{0}명", count));
        }
    }

    private class ViewHolder extends RecyclerView.ViewHolder {
        private final ImageButton playBtn;
        private final ImageButton moreBtn;
        private final TextView songName;
        private final TextView songSinger;
        private final RoundedImageView songImage;


        public ViewHolder(@NonNull View itemView) {
            super(itemView);

            songName = itemView.findViewById(R.id.name);
            songSinger = itemView.findViewById(R.id.singer);
            songImage = itemView.findViewById(R.id.imageView);
            playBtn = itemView.findViewById(R.id.playButton);
            moreBtn = itemView.findViewById(R.id.moreButton);
        }

        public void onBind(RhyaMusicInfoVO item) {
            songName.setText(item.getName());
            songSinger.setText(item.getSinger());

            Glide.with(context)
                    .load(item.getImage())
                    .placeholder(R.drawable.img_load_error)
                    .error(R.drawable.img_load_error)
                    .fallback(R.drawable.img_load_error)
                    .signature(new ObjectKey(item.getVersion()))
                    .into(songImage);

            moreBtn.setOnClickListener(v -> ((ActivityMain) fragment.requireActivity()).showMorePopupMenu(moreBtn, item.getUuid(),null, -1, 1));

            playBtn.setOnClickListener(v -> {
                try {
                    ((ActivityMain) fragment.requireActivity()).putMusicUUID(item.getUuid());
                }catch (Exception ex) {
                    ex.printStackTrace();
                }
            });
        }
    }

    private class ViewHolderLoading extends RecyclerView.ViewHolder {
        private final ProgressWheel progressWheel;
        private final TextView textView;

        public ViewHolderLoading(@NonNull View itemView) {
            super(itemView);

            progressWheel = itemView.findViewById(R.id.progressWheel);
            textView = itemView.findViewById(R.id.viewMore);
        }

        public void onBind() {
            textView.setVisibility(View.VISIBLE);
            progressWheel.setVisibility(View.GONE);

            textView.setOnClickListener(v -> {
                if (!isLoading && !((HomeFragment) fragment).isNoSongMoreView) {
                    int pos = getBindingAdapterPosition();

                    if (pos != RecyclerView.NO_POSITION) {
                        textView.setVisibility(View.GONE);
                        progressWheel.setVisibility(View.VISIBLE);

                        new RhyaAsyncTask<String, String>() {
                            @Override
                            protected void onPreExecute() {
                                isLoading = true;
                            }

                            @Override
                            protected String doInBackground(String arg) {
                                try {
                                    ((HomeFragment) fragment).songListLoadingIndex = ((HomeFragment) fragment).songListLoadingIndex + 20;
                                    musicList = ((HomeFragment) fragment).getArrayListToMusicInfo();

                                    if (musicList == null) {
                                        return "listNull";
                                    }

                                    return "success";
                                }catch (Exception ex) {
                                    ex.printStackTrace();
                                }

                                return null;
                            }

                            @Override
                            protected void onPostExecute(String result) {
                                textView.setVisibility(View.GONE);
                                progressWheel.setVisibility(View.VISIBLE);

                                try {
                                    if (result != null) {
                                        if (result.equals("success")) {
                                            musicList.remove(null);

                                            textView.setVisibility(View.VISIBLE);
                                            progressWheel.setVisibility(View.GONE);

                                            notifyItemRemoved(pos);

                                            notifyItemInserted(pos);

                                            musicList.add(null);

                                            isLoading = false;

                                            if (((HomeFragment) fragment).isNoSongMoreView) {
                                                textView.setVisibility(View.GONE);
                                                progressWheel.setVisibility(View.GONE);
                                            }
                                        }
                                    }else {
                                        ((ActivityMain) fragment.requireActivity()).rhyaDialogManager.createDialog_Confirm(context,
                                                "Unknown Error",
                                                "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00018)",
                                                "닫기",
                                                true,
                                                Dialog::dismiss);
                                    }
                                }catch (Exception ex) {
                                    ex.printStackTrace();

                                    Toast.makeText(fragment.requireActivity().getApplicationContext(), "알 수 없는 오류가 발생하였습니다. (00054)", Toast.LENGTH_SHORT).show();
                                }
                            }
                        }.execute(null);
                    }
                }
            });
        }
    }
}
