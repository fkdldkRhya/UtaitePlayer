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
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.github.siyamed.shapeimageview.RoundedImageView;

import org.json.JSONObject;

import java.net.URLEncoder;
import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.fragment.MyPlayListFragment;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaDialogManagerNew;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHttpsConnection;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaPlayList;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaPlayListImageManager;

public class RhyaMyPlayListAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    private ArrayList<RhyaPlayList> listMain;
    private final Activity activity;
    private final Fragment fragment;
    private Context context;


    public RhyaMyPlayListAdapter(Activity activity, Fragment fragment) {
        this.activity = activity;
        this.fragment = fragment;
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        if (viewType == 1) {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_fragment_my_playlist, parent, false);
            context = view.getContext();

            return new RhyaMyPlayListAdapter.ViewHolder1(view);
        }else {
            View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_my_playlist, parent, false);
            context = view.getContext();

            return new RhyaMyPlayListAdapter.ViewHolder2(view);
        }
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        if (holder instanceof ViewHolder1) {
            ((RhyaMyPlayListAdapter.ViewHolder1) holder).onBind();
        }else {
            ((RhyaMyPlayListAdapter.ViewHolder2) holder).onBind(listMain.get(position));
        }
    }

    @SuppressLint("NotifyDataSetChanged")
    public void setList(ArrayList<RhyaPlayList> list) {
        this.listMain = list;

        notifyDataSetChanged();
    }

    @Override
    public int getItemCount() {
        return listMain == null ? 0 : listMain.size();
    }

    @Override
    public int getItemViewType(int position) {
        if (listMain.get(position) == null) {
            return 1;
        }else {
            return 2;
        }
    }


    class ViewHolder1 extends RecyclerView.ViewHolder {
        private final ImageView imageViewButton;
        private final TextView playListCount;

        public ViewHolder1(@NonNull View itemView) {
            super(itemView);

            imageViewButton = itemView.findViewById(R.id.createPlayList);
            playListCount = itemView.findViewById(R.id.playListCount);
        }

        public void onBind() {
            imageViewButton.setOnClickListener(v -> new RhyaDialogManagerNew(activity, true, 5).setAndShowDialogSettingType_5("플레이리스트 생성", "", -1, playListOkButtonListener));

            StringBuilder stringBuilder;
            stringBuilder = new StringBuilder();
            stringBuilder.append(listMain.size() - 1);
            stringBuilder.append("개");

            playListCount.setText(stringBuilder.toString());
        }

        private final RhyaDialogManagerNew.DialogListener_PlayListOkButtonListener playListOkButtonListener = (dialog, title, image) -> {
            dialog.dismiss();

            // 서버 접속
            new RhyaAsyncTask<String, String>() {

                @Override
                protected void onPreExecute() {
                    ((ActivityMain) activity).showTaskDialog();
                }

                @Override
                protected String doInBackground(String arg) {
                    try {
                        String authToken = ((ActivityMain) activity).rhyaCore.getAutoLogin(((ActivityMain) activity).rhyaSharedPreferences, activity);
                        RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                        ContentValues urlParam = new ContentValues();

                        urlParam.put("mode", "14");
                        urlParam.put("index", "1");
                        urlParam.put("auth", authToken);
                        urlParam.put("value1", URLEncoder.encode(URLEncoder.encode(title, "UTF-8"), "UTF-8"));
                        urlParam.put("value2", URLEncoder.encode("_IMAGE_TYPE_".concat(String.valueOf(image)), "UTF-8"));
                        urlParam.put("value3", "0");

                        try {
                            JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(((ActivityMain) activity).rhyaCore.MAIN_URL, urlParam));
                            String result = jsonObject.getString("result");
                            if (result.equals("success")) {
                                return "success";
                            }else {
                                return "fail";
                            }
                        }catch (Exception ex) {
                            ex.printStackTrace();

                            return "error2";
                        }
                    }catch (Exception ex) {
                        ex.printStackTrace();

                        return "error1";
                    }
                }

                @Override
                protected void onPostExecute(String result) {
                    switch (result) {

                        case "error1": {
                            Toast.makeText(activity, "서버 접속 중에 오류가 발생하였습니다. (00082)", Toast.LENGTH_SHORT).show();

                            break;
                        }

                        case "error2": {
                            Toast.makeText(activity, "JSON 구문분석 중 오류가 발생하였습니다. (00083)", Toast.LENGTH_SHORT).show();

                            break;
                        }

                        case "fail": {
                            Toast.makeText(activity, "서버가 접근을 명령을 거부하였습니다. (00084)", Toast.LENGTH_SHORT).show();

                            break;
                        }

                        case "success": {
                            ((MyPlayListFragment) fragment).changeDataValue();

                            break;
                        }
                    }

                    ((ActivityMain) activity).dismissTaskDialog();
                }
            }.execute(null);
        };
    }


    class ViewHolder2 extends RecyclerView.ViewHolder {
        private final TextView name;
        private final TextView count;
        private final RoundedImageView image;
        private final View view;

        public ViewHolder2(@NonNull View itemView) {
            super(itemView);

            name = itemView.findViewById(R.id.name);
            count = itemView.findViewById(R.id.count);
            ImageButton deleteButton = itemView.findViewById(R.id.deleteButton);
            image = itemView.findViewById(R.id.imageView);
            this.view = itemView.findViewById(R.id.view);

            deleteButton.setOnClickListener(v -> {
                int pos = getAbsoluteAdapterPosition();
                if (pos != RecyclerView.NO_POSITION) {
                    try {
                        RhyaPlayList rhyaPlayList =  listMain.get(pos);
                        // 메시지 박스 생성
                        RhyaDialogManagerNew rhyaDialogManagerNew = new RhyaDialogManagerNew(activity, true, 4);
                        rhyaDialogManagerNew.setDialogSettingType_4(
                                "플레이리스트 삭제",
                                "정말로 해당 플레이리스트를 삭제하시겠습니까?",
                                "삭제",
                                "취소",
                                new RhyaDialogManagerNew.DialogListener_YesOrNo() {
                                    @Override
                                    public void onClickListenerButtonYes(Dialog dialog) {
                                        // 서버 접속 비동기 작업
                                        new RhyaAsyncTask<String, String>() {
                                            @Override
                                            protected void onPreExecute() {
                                                ((ActivityMain) activity).showTaskDialog();

                                                dialog.dismiss();
                                            }

                                            @Override
                                            protected String doInBackground(String arg) {
                                                try {
                                                    RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                                                    ContentValues urlParam = new ContentValues();
                                                    urlParam.put("auth", ((ActivityMain) activity).rhyaCore.getAutoLogin(((ActivityMain) activity).rhyaSharedPreferences, activity.getApplicationContext()));
                                                    urlParam.put("mode", 14);
                                                    urlParam.put("index", 0);
                                                    urlParam.put("value1", rhyaPlayList.getUuid());
                                                    urlParam.put("value2", "0");
                                                    urlParam.put("value3", "0");

                                                    JSONObject result = new JSONObject(rhyaHttpsConnection.request(((ActivityMain) activity).rhyaCore.MAIN_URL, urlParam));
                                                    if (result.getString("result").equals("success")) {
                                                        return "success";
                                                    }else {
                                                        return "fail";
                                                    }
                                                }catch (Exception ex) {
                                                    ex.printStackTrace();
                                                }

                                                return null;
                                            }

                                            @Override
                                            protected void onPostExecute(String result) {
                                                ((ActivityMain) activity).dismissTaskDialog();

                                                if (result == null) {
                                                    Toast.makeText(context, "플레이리스트 삭제 중 오류가 발생했습니다.", Toast.LENGTH_SHORT).show();
                                                    return;
                                                }

                                                if (result.equals("success")) {
                                                    Toast.makeText(context, "해당 플레이리스트를 삭제하였습니다.", Toast.LENGTH_SHORT).show();
                                                }else {
                                                    Toast.makeText(context, "서버가 작업을 거부하였습니다.", Toast.LENGTH_SHORT).show();
                                                }

                                                ((MyPlayListFragment) fragment).changeDataValue();
                                            }
                                        }.execute(null);
                                    }

                                    @Override
                                    public void onClickListenerButtonNo(Dialog dialog) {
                                        dialog.dismiss();
                                    }
                                });
                        rhyaDialogManagerNew.showDialog();
                    }catch (Exception ex) {
                        ex.printStackTrace();
                    }
                }
            });

            itemView.setOnClickListener(v -> {
                int pos = getAbsoluteAdapterPosition();
                if (pos != RecyclerView.NO_POSITION) {
                    try {
                        RhyaPlayList rhyaPlayList = listMain.get(pos);
                        ((ActivityMain) activity).showMyPlayListInfoFragment(rhyaPlayList);
                    }catch (Exception ex) {
                        ex.printStackTrace();
                    }
                }
            });
        }

        public void onBind(RhyaPlayList item){
            name.setText(item.getName());

            int pos = getAbsoluteAdapterPosition();
            if (pos != RecyclerView.NO_POSITION) {
                if (pos + 1 == listMain.size()) {
                    view.setVisibility(View.INVISIBLE);
                }else {
                    view.setVisibility(View.VISIBLE);
                }
            }

            StringBuilder stringBuilder;
            stringBuilder = new StringBuilder();
            stringBuilder.append(item.getMusicList().size());
            stringBuilder.append("곡");
            count.setText(stringBuilder.toString());

            RhyaPlayListImageManager rhyaPlayListImageManager = new RhyaPlayListImageManager();

            Glide.with(context)
                    .load(rhyaPlayListImageManager.getImageID(item.getImageType()))
                    .placeholder(R.drawable.app_logo_for_black)
                    .error(R.drawable.app_logo_for_black)
                    .fallback(R.drawable.app_logo_for_black)
                    .into(this.image);
        }
    }
}
