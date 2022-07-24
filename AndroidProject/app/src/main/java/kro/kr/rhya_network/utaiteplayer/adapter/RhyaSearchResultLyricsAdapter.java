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

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;

import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;

public class RhyaSearchResultLyricsAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    private ArrayList<RhyaMusicInfoVO> searchList;
    private final Activity activity;
    private Context context;


    public RhyaSearchResultLyricsAdapter(Activity activity) {
        this.activity = activity;
    }

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_lyrics, parent, false);
        context = view.getContext();

        return new RhyaSearchResultLyricsAdapter.ViewHolder(view);
    }

    @SuppressLint("NotifyDataSetChanged")
    public void setList(ArrayList<RhyaMusicInfoVO> list, boolean isCut) {
        if (isCut) {
            ArrayList<RhyaMusicInfoVO> arrayList = new ArrayList<>();
            if (list.size() > 5) {
                for (int i = 0; i < list.size(); i++) {
                    if (i >= 5) break;

                    arrayList.add(list.get(i));
                }

                this.searchList = arrayList;
            } else {
                this.searchList = list;
            }
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
        ((RhyaSearchResultLyricsAdapter.ViewHolder) holder).onBind(searchList.get(position));
    }


    private class ViewHolder extends RecyclerView.ViewHolder {
        private final ImageButton playBtn;
        private final ImageButton moreBtn;
        private final TextView songName;
        private final TextView songSinger;
        private final TextView lyrics;
        private final RoundedImageView songImage;


        public ViewHolder(@NonNull View itemView) {
            super(itemView);

            songName = itemView.findViewById(R.id.name);
            songSinger = itemView.findViewById(R.id.singer);
            songImage = itemView.findViewById(R.id.imageView);
            lyrics = itemView.findViewById(R.id.lyrics);
            playBtn = itemView.findViewById(R.id.playButton);
            moreBtn = itemView.findViewById(R.id.moreButton);
        }

        public void onBind(RhyaMusicInfoVO item) {
            songName.setText(item.getName());
            songSinger.setText(item.getSinger());

            if (item.getLyrics() != null && RhyaApplication.searchText != null) {
                if (item.getLyrics().contains(RhyaApplication.searchText)) {
                    SpannableString spannableString = new SpannableString(item.getLyrics());
                    int start = item.getLyrics().indexOf(RhyaApplication.searchText);
                    int end = start + RhyaApplication.searchText.length();
                    spannableString.setSpan(new ForegroundColorSpan(ContextCompat.getColor(context, R.color.app_main_base_6)), start, end, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

                    lyrics.setText(spannableString);
                } else {
                    lyrics.setText(item.getLyrics());
                }
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
