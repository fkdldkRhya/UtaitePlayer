package kro.kr.rhya_network.utaiteplayer.adapter;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.signature.ObjectKey;
import com.github.siyamed.shapeimageview.RoundedImageView;

import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;

public class RhyaSearchResultSingerSongAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    private final Activity activity;
    private ArrayList<RhyaMusicInfoVO> musicList;
    private Context context;

    public RhyaSearchResultSingerSongAdapter(Activity activity) {
        this.activity = activity;
    }


    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_singer_song, parent, false);
        context = view.getContext();

        return new RhyaSearchResultSingerSongAdapter.ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull  RecyclerView.ViewHolder holder, int position) {
        ((RhyaSearchResultSingerSongAdapter.ViewHolder) holder).onBind(musicList.get(position));
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

    @Override
    public int getItemViewType(int position) {
        return position;
    }

    private class ViewHolder extends RecyclerView.ViewHolder {
        private final TextView songName;
        private final RoundedImageView songImage;

        public ViewHolder(@NonNull View itemView) {
            super(itemView);

            songName = itemView.findViewById(R.id.songName);
            songImage = itemView.findViewById(R.id.songImageView);

            itemView.setOnClickListener(v -> {
                int pos = getBindingAdapterPosition();
                if (pos != RecyclerView.NO_POSITION) {
                    RhyaMusicInfoVO item = musicList.get(pos);
                    ((ActivityMain) activity).putMusicUUID(item.getUuid());
                }
            });

            itemView.setOnLongClickListener(view -> {
                int pos = getBindingAdapterPosition();
                if (pos != RecyclerView.NO_POSITION) {
                    RhyaMusicInfoVO item = musicList.get(pos);
                    ((ActivityMain) activity).showSongInfoDialog( item.getUuid());

                }

                return true;
            });
        }

        public void onBind(RhyaMusicInfoVO item){
            songName.setText(item.getName());

            Glide.with(context)
                    .load(item.getImage())
                    .placeholder(R.drawable.img_load_error)
                    .error(R.drawable.img_load_error)
                    .fallback(R.drawable.img_load_error)
                    .signature(new ObjectKey(item.getVersion()))
                    .into(songImage);

        }
    }
}