package kro.kr.rhya_network.utaiteplayer.adapter;

import android.annotation.SuppressLint;

import android.content.Context;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.signature.ObjectKey;
import com.github.siyamed.shapeimageview.RoundedImageView;

import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVOv2;

public class RhyaNowPlayListAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {
    public ArrayList<RhyaMusicInfoVOv2> musicList = new ArrayList<>();

    private final Fragment fragment;

    private Context context;

    public boolean isLoading;

    public int playPos = -1;

    @NonNull
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_song, parent, false);
        context = view.getContext();

        return new ViewHolder(view);
    }


    public RhyaNowPlayListAdapter(Fragment fragment) {
        this.fragment = fragment;

        isLoading = false;
    }

    @SuppressLint("NotifyDataSetChanged")
    public void setList(ArrayList<RhyaMusicInfoVOv2> list) {
        this.musicList = list;

        notifyDataSetChanged();
    }


    @Override
    public int getItemCount() {
        return musicList == null ? 0 : musicList.size();
    }

    @Override
    public void onBindViewHolder(@NonNull RecyclerView.ViewHolder holder, int position) {
        ((ViewHolder) holder).onBind(musicList.get(position));
    }


    private class ViewHolder extends RecyclerView.ViewHolder {
        private final TextView songName;
        private final TextView songSinger;
        private final RoundedImageView songImage;
        private final ImageButton playBtn;
        private final ImageButton moreBtn;
        private final View itemView;


        public ViewHolder(@NonNull View itemView) {
            super(itemView);

            songName = itemView.findViewById(R.id.name);
            songSinger = itemView.findViewById(R.id.singer);
            songImage = itemView.findViewById(R.id.imageView);
            playBtn = itemView.findViewById(R.id.playButton);
            moreBtn = itemView.findViewById(R.id.moreButton);

            this.itemView = itemView;
        }

        public void onBind(RhyaMusicInfoVOv2 item) {
            boolean isPlay = false;

            songName.setText(item.getName());
            songSinger.setText(item.getSinger());

            if (item.getIsPlay()) {
                songName.setTextColor(ContextCompat.getColor(context, R.color.app_main_base_5));
                songSinger.setTextColor(ContextCompat.getColor(context, R.color.app_main_base_5));
                itemView.setBackgroundColor(ContextCompat.getColor(context, R.color.custom_gray_7));

                if (((ActivityMain) fragment.requireActivity()).playerService.mMediaPlayer != null) {
                    isPlay = ((ActivityMain) fragment.requireActivity()).playerService.mMediaPlayer.isPlaying();
                }

                if (isPlay) {
                    playBtn.setImageResource(R.drawable.ic_baseline_pause_24);
                }else {
                    playBtn.setImageResource(R.drawable.ic_baseline_play_arrow_24);
                }

                playPos = getLayoutPosition();
            }else {
                songName.setTextColor(ContextCompat.getColor(context, R.color.black));
                songSinger.setTextColor(Color.parseColor("#5A5A5A"));
                itemView.setBackgroundColor(ContextCompat.getColor(context, R.color.white));

                playBtn.setImageResource(R.drawable.ic_baseline_play_arrow_24);
            }

            Glide.with(context)
                    .load(item.getImage())
                    .placeholder(R.drawable.img_load_error)
                    .error(R.drawable.img_load_error)
                    .fallback(R.drawable.img_load_error)
                    .signature(new ObjectKey(item.getVersion()))
                    .into(songImage);

            moreBtn.setOnClickListener(v -> {
                int pos = getLayoutPosition();
                if (pos != RecyclerView.NO_POSITION) {
                    ((ActivityMain) fragment.requireActivity()).showMorePopupMenu(moreBtn, item.getUuid(), item.getItemID(), pos, 2);
                }
            });

            playBtn.setOnClickListener(v -> {
                try {
                    int pos = getLayoutPosition();
                    if (pos != RecyclerView.NO_POSITION) {
                        if (item.getIsPlay()) {
                            if (((ActivityMain) fragment.requireActivity()).playerServiceBindTOF) {
                                if (((ActivityMain) fragment.requireActivity()).playerService.mMediaPlayer != null
                                        && ((ActivityMain) fragment.requireActivity()).playerService.nowPlayMusicUUID != null) {
                                    if (item.getUuid().equals(((ActivityMain) fragment.requireActivity()).playerService.nowPlayMusicUUID)) {
                                        ((ActivityMain) fragment.requireActivity()).playButtonOnClickListener();
                                    }else {
                                        ((ActivityMain) fragment.requireActivity()).putMusicNoAddUUID(item.getUuid(), item.getIndex());
                                    }
                                }else {
                                    ((ActivityMain) fragment.requireActivity()).putMusicNoAddUUID(item.getUuid(), item.getIndex());
                                }
                            }
                        }else {
                            if (((ActivityMain) fragment.requireActivity()).playerServiceBindTOF) {
                                ((ActivityMain) fragment.requireActivity()).putMusicNoAddUUID(item.getUuid(), item.getIndex());
                            }
                        }
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    Toast.makeText(fragment.requireContext(), "알 수 없는 오류가 발생하였습니다. (00052)", Toast.LENGTH_SHORT).show();
                }
            });
        }
    }
}
