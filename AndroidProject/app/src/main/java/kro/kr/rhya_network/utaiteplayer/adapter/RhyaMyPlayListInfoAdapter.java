package kro.kr.rhya_network.utaiteplayer.adapter;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.ContentValues;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.signature.ObjectKey;
import com.github.siyamed.shapeimageview.RoundedImageView;

import org.json.JSONArray;
import org.json.JSONObject;

import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Iterator;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.fragment.MyPlayListFragment;
import kro.kr.rhya_network.utaiteplayer.fragment.MyPlayListInfoFragment;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaDialogManagerNew;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHttpsConnection;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaItemTouchHelperCallBack;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaPlayList;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaPlayListImageManager;

public class RhyaMyPlayListInfoAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> implements RhyaItemTouchHelperCallBack.OnItemMoveListener {
    public ArrayList<RhyaMusicInfoVO> listMain;
    private final Activity activity;
    private final Fragment fragment;
    private final String HEADER_TYPE_SETTING_NAME = "_BUTTON_HEADER_";
    private final RhyaMyPlayListInfoAdapter rhyaMyPlayListInfoAdapter;
    private Context context;



    public RhyaMyPlayListInfoAdapter(Activity activity, Fragment fragment) {
        this.activity = activity;
        this.fragment = fragment;

        rhyaMyPlayListInfoAdapter = this;

        ((ActivityMain) activity).setActionPlayListSongsRemoveButton(() -> new RhyaAsyncTask<String, String>() {
            private final ArrayList<Integer> deleteIndex = new ArrayList<>();

            @Override
            protected void onPreExecute() {
                ((ActivityMain) activity).showTaskDialog();
            }

            @Override
            protected String doInBackground(String arg) {
                if (listMain == null) return null;

                ArrayList<RhyaMusicInfoVO> target = new ArrayList<>();
                for (int index = 2; index < listMain.size(); index++) {
                    if (listMain.get(index).getVersion() == -10) {
                        deleteIndex.add(index);
                        target.add(listMain.get(index));
                    }
                }

                for (RhyaMusicInfoVO rhyaMusicInfoVO : target) {
                    listMain.remove(rhyaMusicInfoVO);
                }

                return null;
            }

            @Override
            protected void onPostExecute(String result) {
                notifyItemChanged(0);
                notifyItemChanged(1);

                for (int index = deleteIndex.size() - 1; index >= 0; index--) {
                    notifyItemRemoved(deleteIndex.get(index));
                }

                ((ActivityMain) activity).setStatePlayListSongsRemoveButton(false);

                ((ActivityMain) activity).dismissTaskDialog();
            }
        }.execute(null));
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        if (viewType == 1) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_my_playlist_1, parent, false);
            context = view.getContext();

            return new RhyaMyPlayListInfoAdapter.ViewHolder1(view);
        }else if (viewType == 2) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_my_playlist_2, parent, false);
            context = view.getContext();

            return new RhyaMyPlayListInfoAdapter.ViewHolder2(view);
        }else {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_song, parent, false);
            context = view.getContext();

            return new RhyaMyPlayListInfoAdapter.ViewHolder3(view);
        }
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        if (holder instanceof RhyaMyPlayListInfoAdapter.ViewHolder1) {
            ((RhyaMyPlayListInfoAdapter.ViewHolder1) holder).onBind();
        }else if (holder instanceof RhyaMyPlayListInfoAdapter.ViewHolder2) {
            ((RhyaMyPlayListInfoAdapter.ViewHolder2) holder).onBind();
        }else {
            ((RhyaMyPlayListInfoAdapter.ViewHolder3) holder).onBind(listMain.get(position));
        }
    }

    @SuppressLint("NotifyDataSetChanged")
    public void setList(ArrayList<RhyaMusicInfoVO> list) {
        this.listMain = list;

        new RhyaAsyncTask<String, String>() {
            private boolean isChecked = false;
            @Override
            protected void onPreExecute() {
                ((ActivityMain) activity).showTaskDialog();
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    for (RhyaMusicInfoVO rhyaMusicInfoVO : listMain) {
                        if (rhyaMusicInfoVO != null && !rhyaMusicInfoVO.getUuid().equals(HEADER_TYPE_SETTING_NAME) && rhyaMusicInfoVO.getVersion() == -10) {
                            isChecked = true;
                            break;
                        }
                    }

                    return "success";
                }catch (Exception ex) {
                    ex.printStackTrace();
                }

                return null;
            }

            @Override
            protected void onPostExecute(String result) {
                try {
                    if (result == null) {
                        Toast.makeText(context, "알 수 없는 오류가 발생하였습니다. (00089)", Toast.LENGTH_SHORT).show();
                        return;
                    }

                    if (!isChecked) {
                        ((ActivityMain) activity).setStatePlayListSongsRemoveButton(false);

                        notifyDataSetChanged();

                        ((ActivityMain) activity).dismissTaskDialog();
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    Toast.makeText(context, "알 수 없는 오류가 발생하였습니다. (00086)", Toast.LENGTH_SHORT).show();
                }
            }
        }.execute(null);
    }
    public void removeItem(int pos) {
        listMain.remove(pos);

        notifyItemRemoved(pos);
        notifyItemChanged(0);
        notifyItemChanged(1);
    }

    @Override
    public int getItemCount() {
        return listMain == null ? 0 : listMain.size();
    }

    @Override
    public int getItemViewType(int position) {
        if (listMain.get(position) == null) {
            return 1;
        }else if (listMain.get(position).getUuid().equals(HEADER_TYPE_SETTING_NAME)) {
            return 2;
        }else {
            return 3;
        }
    }



    // =========================================================================== //
    // Sticky Header function
    // =========================================================================== //
    public boolean isHeader(int pos) {
        if (listMain.get(pos) != null) {
            return listMain.get(pos).getUuid().equals(HEADER_TYPE_SETTING_NAME);
        }

        return false;
    }
    public View getHeaderLayoutView(RecyclerView recyclerView, int pos) {
        if (listMain.get(pos) != null) {
            View view = LayoutInflater.from(context).inflate(R.layout.item_my_playlist_2, recyclerView, false);

            TextView textView = view.findViewById(R.id.count);
            StringBuilder stringBuilder;
            stringBuilder = new StringBuilder();
            stringBuilder.append(listMain.size() - 2);
            stringBuilder.append("/100 곡");
            textView.setText(stringBuilder.toString());

            return view;
        }

        return null;
    }
    // =========================================================================== //
    // =========================================================================== //


    @Override
    public void onItemMove(int fromPosition, int toPosition) {
        Collections.swap(listMain, fromPosition, toPosition);
        notifyItemMoved(fromPosition, toPosition);
    }




    class ViewHolder1 extends RecyclerView.ViewHolder {
        private final RoundedImageView playListImageView;
        private final TextView playListTitle;
        private final TextView userName;
        private final TextView userID;
        private final Button addSongButton;
        private final Button allPlayButton;
        private final TextView saveTextViewButton;


        public ViewHolder1(@NonNull View itemView) {
            super(itemView);

            playListImageView = itemView.findViewById(R.id.playListImageView);
            playListTitle = itemView.findViewById(R.id.title);
            userName = itemView.findViewById(R.id.userName);
            userID = itemView.findViewById(R.id.userID);
            addSongButton = itemView.findViewById(R.id.button1);
            allPlayButton = itemView.findViewById(R.id.button2);
            saveTextViewButton = itemView.findViewById(R.id.saveTextViewButton);
        }

        public void onBind() {
            try {
                RhyaPlayList rhyaPlayList = ((MyPlayListInfoFragment) fragment).rhyaPlayList;

                playListTitle.setText(rhyaPlayList.getName());
                userName.setText(RhyaApplication.rhyaUserDataVO.getUserName());
                userID.setText(RhyaApplication.rhyaUserDataVO.getUserID());

                RhyaPlayListImageManager rhyaPlayListImageManager = new RhyaPlayListImageManager();

                Glide.with(context)
                        .load(rhyaPlayListImageManager.getImageID(rhyaPlayList.getImageType()))
                        .placeholder(R.drawable.app_logo_for_black)
                        .error(R.drawable.app_logo_for_black)
                        .fallback(R.drawable.app_logo_for_black)
                        .into(playListImageView);

                allPlayButton.setOnClickListener(v ->
                    new RhyaAsyncTask<String, String>() {
                        private final ArrayList<RhyaMusicInfoVO> rhyaMusicInfoVOArrayList = new ArrayList<>();

                        @Override
                        protected void onPreExecute() {
                            ((ActivityMain) activity).showTaskDialog();
                        }

                        @Override
                        protected String doInBackground(String arg) {
                            try {
                                for (RhyaMusicInfoVO rhyaMusicInfoVO : listMain) {
                                    if (rhyaMusicInfoVO != null && !rhyaMusicInfoVO.getUuid().equals(HEADER_TYPE_SETTING_NAME)) {
                                        rhyaMusicInfoVOArrayList.add(rhyaMusicInfoVO);
                                    }
                                }

                                return "success";
                            }catch (Exception ex) {
                                ex.printStackTrace();
                            }

                            return null;
                        }

                        @Override
                        protected void onPostExecute(String result) {
                            if (result == null) {
                                Toast.makeText(context, "알 수 없는 오류가 발생하였습니다. (00090)", Toast.LENGTH_SHORT).show();
                                return;
                            }

                            ((ActivityMain) activity).putMusicArrayListRhyaMusicVO(rhyaMusicInfoVOArrayList);
                            ((ActivityMain) activity).dismissTaskDialog();
                        }
                    }.execute(null)
                );

                addSongButton.setOnClickListener(v -> new RhyaDialogManagerNew(activity, true, 7).setAndShowDialogSettingType_7(listMain, HEADER_TYPE_SETTING_NAME, rhyaMyPlayListInfoAdapter));

                saveTextViewButton.setOnClickListener(v -> new RhyaAsyncTask<String, String>() {
                    @Override
                    protected void onPreExecute() {
                        ((ActivityMain) activity).showTaskDialog();
                    }

                    @Override
                    protected String doInBackground(String arg) {
                        try {
                            JSONArray jsonArray = new JSONArray();

                            // JSON 데이터 생성
                            for (RhyaMusicInfoVO rhyaMusicInfoVO : listMain) {
                                if (rhyaMusicInfoVO != null && !rhyaMusicInfoVO.getUuid().equals(HEADER_TYPE_SETTING_NAME)) {
                                    jsonArray.put(rhyaMusicInfoVO.getUuid());
                                }
                            }

                            final RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                            final String authToken = ((ActivityMain) activity).rhyaCore.getAutoLogin(((ActivityMain) activity).rhyaSharedPreferences, activity);
                            final String url = "https://rhya-network.kro.kr/RhyaNetwork/data_buffer_user_manager";
                            String jsonArrayStr = jsonArray.toString();

                            final int divDataSize = 1500;
                            if (jsonArrayStr.length() > divDataSize) { // 분할 전종
                                String requestKey = null;

                                for (int i = 0; i < (jsonArrayStr.length() / divDataSize) + 1; i ++) {
                                    String splitStr;
                                    int subStringLenForStart = divDataSize * i;
                                    int subStringLenForEnd = subStringLenForStart + divDataSize;

                                    if (jsonArrayStr.length() >= subStringLenForEnd) {
                                        splitStr = jsonArrayStr.substring(subStringLenForStart, subStringLenForEnd);
                                    }else {
                                        splitStr = jsonArrayStr.substring(subStringLenForStart);
                                    }

                                    splitStr = URLEncoder.encode(splitStr, "UTF-8");
                                    splitStr = URLEncoder.encode(splitStr, "UTF-8");

                                    // 데이터 전송 [Data-Buffer]
                                    ContentValues urlParam = new ContentValues();

                                    if (i == 0) {
                                        urlParam.put("mode", "0");
                                        urlParam.put("default", splitStr);
                                        JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(url, urlParam));

                                        String result = jsonObject.getString("result");
                                        if (result.equals("success")) {
                                            requestKey = jsonObject.getString("message");
                                        }else {
                                            return null;
                                        }
                                    }else {
                                        int index = i - 1;
                                        urlParam.put("mode", "1");
                                        urlParam.put("index", index);
                                        urlParam.put("request", requestKey);
                                        urlParam.put("input", splitStr);
                                        JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(url, urlParam));

                                        String result = jsonObject.getString("result");
                                        if (!result.equals("success")) {
                                            return null;
                                        }
                                    }
                                }

                                ContentValues urlParam = new ContentValues();
                                urlParam.put("mode", "14");
                                urlParam.put("auth", authToken);
                                urlParam.put("value1", requestKey);
                                urlParam.put("value2", ((MyPlayListInfoFragment) fragment).rhyaPlayList.getUuid());
                                urlParam.put("value3", "0");
                                urlParam.put("index", "4");

                                JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(((ActivityMain) activity).rhyaCore.MAIN_URL, urlParam));

                                String result = jsonObject.getString("result");
                                if (!result.equals("success")) {
                                    return null;
                                }
                            }else { // 바로 전송
                                jsonArrayStr = URLEncoder.encode(jsonArrayStr, "UTF-8");
                                jsonArrayStr = URLEncoder.encode(jsonArrayStr, "UTF-8");

                                ContentValues urlParam = new ContentValues();
                                urlParam.put("mode", "14");
                                urlParam.put("auth", authToken);
                                urlParam.put("value1", ((MyPlayListInfoFragment) fragment).rhyaPlayList.getUuid());
                                urlParam.put("value2", jsonArrayStr);
                                urlParam.put("value3", "0");
                                urlParam.put("index", "3");

                                JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(((ActivityMain) activity).rhyaCore.MAIN_URL, urlParam));

                                String result = jsonObject.getString("result");
                                if (!result.equals("success")) {
                                    return null;
                                }
                            }

                            // 데이터 적용
                            ((MyPlayListFragment) ((ActivityMain) activity).myPlayListFragment).isReload = true;

                            return "success";
                        }catch (Exception ex) {
                            ex.printStackTrace();
                        }

                        return null;
                    }

                    @Override
                    protected void onPostExecute(String result) {
                        if (result != null) {
                            Toast.makeText(context, "수정 사항을 적용하였습니다.", Toast.LENGTH_SHORT).show();
                        }else {
                            Toast.makeText(context, "저장 중 알 수 없는 오류가 발생하였습니다. (00084)", Toast.LENGTH_SHORT).show();
                        }

                        ((ActivityMain) activity).dismissTaskDialog();
                    }
                }.execute(null));
            }catch (Exception ignored) {}
        }
    }

    class ViewHolder2 extends RecyclerView.ViewHolder {
        private final TextView textView;

        public ViewHolder2(@NonNull View itemView) {
            super(itemView);

            textView = itemView.findViewById(R.id.count);
        }

        public void onBind() {
            StringBuilder stringBuilder;
            stringBuilder = new StringBuilder();
            stringBuilder.append(listMain.size() - 2);
            stringBuilder.append("/100 곡");

            textView.setText(stringBuilder.toString());
        }
    }

    class ViewHolder3 extends RecyclerView.ViewHolder {
        private final ImageButton playBtn;
        private final ImageButton moreBtn;
        private final TextView songName;
        private final TextView songSinger;
        private final RoundedImageView songImage;
        private final ImageView clickEvent;


        public ViewHolder3(@NonNull View itemView) {
            super(itemView);

            songName = itemView.findViewById(R.id.name);
            songSinger = itemView.findViewById(R.id.singer);
            songImage = itemView.findViewById(R.id.imageView);
            playBtn = itemView.findViewById(R.id.playButton);
            moreBtn = itemView.findViewById(R.id.moreButton);
            clickEvent = itemView.findViewById(R.id.clickEvent);
        }

        public void onBind(RhyaMusicInfoVO item) {
            try {
                if (item.getVersion() == -10) {
                    itemView.setBackgroundColor(ContextCompat.getColor(context, R.color.custom_gray_4));
                }else {
                    itemView.setBackgroundColor(ContextCompat.getColor(context, R.color.white));
                }

                songName.setText(item.getName());
                songSinger.setText(item.getSinger());

                Glide.with(context)
                        .load(item.getImage())
                        .placeholder(R.drawable.img_load_error)
                        .error(R.drawable.img_load_error)
                        .fallback(R.drawable.img_load_error)
                        .signature(new ObjectKey(item.getVersion()))
                        .into(songImage);

                moreBtn.setOnClickListener(v -> {
                    try {
                        if (item.getVersion() == -10) {
                            Toast.makeText(context, "선택을 해제해 주세요.", Toast.LENGTH_SHORT).show();

                            return;
                        }

                        new RhyaDialogManagerNew(activity, true, 6).setAndShowDialogSettingType_6(item, ((MyPlayListInfoFragment) fragment).rhyaMyPlayListInfoAdapter, getAbsoluteAdapterPosition());
                    }catch (Exception ex) {
                        ex.printStackTrace();
                        Toast.makeText(context, "알 수 없는 오류가 발생하였습니다. (00087)", Toast.LENGTH_SHORT).show();
                    }
                });
                playBtn.setOnClickListener(v -> {
                    try {
                        ((ActivityMain) activity).putMusicUUID(item.getUuid());
                    }catch (Exception ex) {
                        ex.printStackTrace();
                    }
                });

                clickEvent.setOnClickListener(v -> {
                    int pos = getAbsoluteAdapterPosition();
                    if (pos != RecyclerView.NO_POSITION) {
                        if (item.getVersion() == -10) {
                            item.setVersion(-20);
                            itemView.setBackgroundColor(ContextCompat.getColor(context, R.color.white));

                            new RhyaAsyncTask<String, String>() {
                                private boolean isChecked = false;
                                @Override
                                protected void onPreExecute() {

                                }

                                @Override
                                protected String doInBackground(String arg) {
                                    try {
                                        for (RhyaMusicInfoVO rhyaMusicInfoVO : listMain) {
                                            if (rhyaMusicInfoVO != null && !rhyaMusicInfoVO.getUuid().equals(HEADER_TYPE_SETTING_NAME) && rhyaMusicInfoVO.getVersion() == -10) {
                                                isChecked = true;
                                                break;
                                            }
                                        }
                                    }catch (Exception ex) {
                                        ex.printStackTrace();
                                        Toast.makeText(context, "알 수 없는 오류가 발생하였습니다. (00088)", Toast.LENGTH_SHORT).show();
                                    }

                                    return null;
                                }

                                @Override
                                protected void onPostExecute(String result) {
                                    if (!isChecked) {
                                        ((ActivityMain) activity).setStatePlayListSongsRemoveButton(false);
                                    }
                                }
                            }.execute(null);
                        }else {
                            item.setVersion(-10);
                            itemView.setBackgroundColor(ContextCompat.getColor(context, R.color.custom_gray_4));

                            ((ActivityMain) activity).setStatePlayListSongsRemoveButton(true);
                        }
                    }
                });
            }catch (Exception ex) {
                ex.printStackTrace();
            }
        }
    }
}
