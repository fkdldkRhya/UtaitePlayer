package kro.kr.rhya_network.utaiteplayer.adapter;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.text.Spannable;
import android.text.SpannableString;
import android.text.style.ForegroundColorSpan;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.github.siyamed.shapeimageview.RoundedImageView;

import java.util.ArrayList;
import java.util.HashMap;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSingerDataVO;

public class RhyaSearchResultSingerAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    private ArrayList<String> searchList;
    private HashMap<String, ArrayList<RhyaMusicInfoVO>> stringArrayListHashMap;
    private final Activity activity;
    private Context context;


    public RhyaSearchResultSingerAdapter(Activity activity) {
        this.activity = activity;
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_singer, parent, false);
        context = view.getContext();

        return new RhyaSearchResultSingerAdapter.ViewHolder(view);
    }

    @SuppressLint("NotifyDataSetChanged")
    public void setList(ArrayList<String> list, HashMap<String, ArrayList<RhyaMusicInfoVO>> stringArrayListHashMap, boolean isCut) {
        if (isCut) {
            if (list.size() > 3) {
                ArrayList<String> arrayList = new ArrayList<>();
                for (int i = 0; i < list.size(); i++) {
                    if (i >= 3) break;

                    arrayList.add(list.get(i));
                }

                this.searchList = arrayList;
            } else {
                this.searchList = list;
            }
        }else {
            this.searchList = list;
        }

        this.stringArrayListHashMap = stringArrayListHashMap;

        notifyDataSetChanged();
    }

    @Override
    public int getItemCount() {
        return searchList == null ? 0 : searchList.size();
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        ((RhyaSearchResultSingerAdapter.ViewHolder) holder).onBind(searchList.get(position));
    }


    private class ViewHolder extends RecyclerView.ViewHolder {
        private final Button button;
        private final TextView singerName;
        private final RoundedImageView singerImage;
        private final RecyclerView recyclerView;


        public ViewHolder(@NonNull View itemView) {
            super(itemView);

            button = itemView.findViewById(R.id.button);
            singerName = itemView.findViewById(R.id.name);
            singerImage = itemView.findViewById(R.id.imageView);
            recyclerView = itemView.findViewById(R.id.recyclerView);
        }

        public void onBind(String uuid) {
            RhyaSingerDataVO item = RhyaApplication.stringRhyaSingerDataVOHashMap.get(uuid);
            assert item != null;


            String value1 = item.getName();
            if (value1.contains(RhyaApplication.searchText)) {
                SpannableString spannableString = new SpannableString(value1);
                int start = value1.indexOf(RhyaApplication.searchText);
                int end = start + RhyaApplication.searchText.length();
                spannableString.setSpan(new ForegroundColorSpan(ContextCompat.getColor(context, R.color.app_main_base_6)), start, end, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

                singerName.setText(spannableString);
            } else {
                singerName.setText(value1);
            }


            RhyaSearchResultSingerSongAdapter rhyaSearchResultSingerSongAdapter = new RhyaSearchResultSingerSongAdapter(activity);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(activity);
            linearLayoutManager.setOrientation(RecyclerView.HORIZONTAL);
            recyclerView.setAdapter(rhyaSearchResultSingerSongAdapter);
            recyclerView.setLayoutManager(linearLayoutManager);
            rhyaSearchResultSingerSongAdapter.setList(stringArrayListHashMap.get(uuid));


            boolean isSubscribe = false;
            for (RhyaSingerDataVO rhyaSingerDataVO : RhyaApplication.rhyaMusicDataVO.getSubscribeList()) {
                String uuidSinger = rhyaSingerDataVO.getUuid();
                if (uuidSinger.equals(item.getUuid())) {
                    isSubscribe = true;

                    break;
                }
            }


            if (isSubscribe) {
                // 구독 중
                button.setText("구독 중");
                button.setEnabled(false);
            }else {
                button.setText("구독");
                button.setEnabled(true);
            }


            button.setOnClickListener(v -> {
                if (button.getText().equals("구독")) {
                    // 구독 작업
                    try {
                        button.setText("구독 중");
                        button.setEnabled(false);

                        ((ActivityMain) activity).subscribeTask(item);
                    }catch (Exception ex) {
                        ex.printStackTrace();
                    }
                }
            });

            Glide.with(context)
                    .load(item.getImage())
                    .placeholder(R.drawable.img_load_error)
                    .error(R.drawable.img_load_error)
                    .fallback(R.drawable.img_load_error)
                    .into(singerImage);
        }
    }
}