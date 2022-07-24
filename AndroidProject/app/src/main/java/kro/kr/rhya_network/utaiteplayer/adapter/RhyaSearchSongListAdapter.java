package kro.kr.rhya_network.utaiteplayer.adapter;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.signature.ObjectKey;
import com.github.siyamed.shapeimageview.RoundedImageView;

import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;

public class RhyaSearchSongListAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    public ArrayList<RhyaMusicInfoVO> musicList;

    private final Activity activity;
    private Context context;




    public RhyaSearchSongListAdapter(Activity activity) {
        this.activity = activity;
    }



    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_song, parent, false);

        context = view.getContext();

        return new RhyaSearchSongListAdapter.ViewHolder(view);
    }



    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        ((RhyaSearchSongListAdapter.ViewHolder) holder).onBind(musicList.get(position));
    }



    @Override
    public int getItemViewType(int position) {
        return 0;
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





    private class ViewHolder extends RecyclerView.ViewHolder {
        // UI Object
        private final ImageButton playBtn;
        private final ImageButton moreBtn;
        private final TextView songName;
        private final TextView songSinger;
        private final RoundedImageView songImage;




        public ViewHolder(@NonNull View view) {
            super(view);

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
