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
import android.widget.ImageButton;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.signature.ObjectKey;
import com.github.siyamed.shapeimageview.RoundedImageView;

import java.util.ArrayList;
import java.util.Arrays;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;


public class RhyaSearchResultSongAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    private ArrayList<String> searchList;
    private final Activity activity;
    private Context context;


    public RhyaSearchResultSongAdapter(Activity activity) {
        this.activity = activity;
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_song, parent, false);
        context = view.getContext();

        return new RhyaSearchResultSongAdapter.ViewHolder(view);
    }

    @SuppressLint("NotifyDataSetChanged")
    public void setList(ArrayList<String> list) {
        ArrayList<String> arrayList = new ArrayList<>();
        if (list.size() > 5) {
            for (int i = 0; i < list.size(); i ++) {
                if (i >= 5) break;

                arrayList.add(list.get(i));
            }

            this.searchList = arrayList;
        }else {
            this.searchList = list;
        }

        notifyDataSetChanged();
    }

    @Override
    public int getItemCount() {
        return searchList == null ? 0 : searchList.size();
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        ((RhyaSearchResultSongAdapter.ViewHolder) holder).onBind(searchList.get(position));
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

        public void onBind(String uuid) {
            RhyaMusicInfoVO item = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);
            assert item != null;


            String value1 = item.getName();
            if (value1.contains(RhyaApplication.searchText)) {
                SpannableString spannableString = new SpannableString(value1);
                int start = value1.indexOf(RhyaApplication.searchText);
                int end = start + RhyaApplication.searchText.length();
                spannableString.setSpan(new ForegroundColorSpan(ContextCompat.getColor(context, R.color.app_main_base_6)), start, end, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

                songName.setText(spannableString);
            } else {
                songName.setText(value1);
            }

            String value2 = item.getSinger();
            if (value2.contains(RhyaApplication.searchText)) {
                SpannableString spannableString = new SpannableString(value2);
                int start = value2.indexOf(RhyaApplication.searchText);
                int end = start + RhyaApplication.searchText.length();
                spannableString.setSpan(new ForegroundColorSpan(ContextCompat.getColor(context, R.color.app_main_base_6)), start, end, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

                songSinger.setText(spannableString);
            }else {
                songSinger.setText(value2);
            }


            Glide.with(context)
                    .load(item.getImage())
                    .placeholder(R.drawable.img_load_error)
                    .error(R.drawable.img_load_error)
                    .fallback(R.drawable.img_load_error)
                    .signature(new ObjectKey(item.getVersion()))
                    .into(songImage);

            moreBtn.setOnClickListener(v -> ((ActivityMain) activity).showMorePopupMenu(moreBtn, item.getUuid(),null, -1, 1));

            playBtn.setOnClickListener(v -> {
                try {
                    ((ActivityMain) activity).putMusicUUID(item.getUuid());
                }catch (Exception ex) {
                    ex.printStackTrace();
                }
            });
        }
    }
}
