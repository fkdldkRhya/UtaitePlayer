package kro.kr.rhya_network.utaiteplayer.adapter;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.signature.ObjectKey;
import com.github.siyamed.shapeimageview.RoundedImageView;

import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;

public class RhyaNewSongAdapter extends RecyclerView.Adapter<RhyaNewSongAdapter.ViewHolder> {
    private ArrayList<RhyaMusicInfoVO> musicList;
    private final Activity activity;
    private Context context;

    public RhyaNewSongAdapter(Activity activity) {
        this.activity = activity;
    }


    @NonNull
    @Override
    public RhyaNewSongAdapter.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_horizontal_big, parent, false);
        context = view.getContext();

        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull RhyaNewSongAdapter.ViewHolder holder, int position) {
        holder.onBind(musicList.get(position));
    }

    @SuppressLint("NotifyDataSetChanged")
    public void setList(ArrayList<RhyaMusicInfoVO> list) {
        this.musicList = list;

        notifyDataSetChanged();
    }

    public ArrayList<RhyaMusicInfoVO> getList() {
        return musicList;
    }

    @Override
    public int getItemCount() {
        return musicList.size();
    }

    @Override
    public int getItemViewType(int position) {
        return position;
    }

    class ViewHolder extends RecyclerView.ViewHolder {
        private final ConstraintLayout layout;
        private final TextView songType;
        private final TextView songName;
        private final TextView songWriter;
        private final RoundedImageView songImage;

        public ViewHolder(@NonNull View itemView) {
            super(itemView);

            layout = itemView.findViewById(R.id.layout);
            songType = itemView.findViewById(R.id.songType);
            songName = itemView.findViewById(R.id.songName);
            songWriter = itemView.findViewById(R.id.songWriterName);
            songImage = itemView.findViewById(R.id.songImageView);
        }

        public void onBind(RhyaMusicInfoVO item){
            songType.setText(item.getType());
            songName.setText(item.getName());
            songWriter.setText(item.getSinger());

            Glide.with(context)
                    .load(item.getImage())
                    .placeholder(R.drawable.img_load_error)
                    .error(R.drawable.img_load_error)
                    .fallback(R.drawable.img_load_error)
                    .signature(new ObjectKey(item.getVersion()))
                    .into(songImage);

            songType.setOnClickListener(v -> runCommand(item));
            songName.setOnClickListener(v -> runCommand(item));
            songWriter.setOnClickListener(v -> runCommand(item));
            songImage.setOnClickListener(v -> runCommand(item));
            layout.setOnClickListener(v -> runCommand(item));
        }

        private void runCommand(RhyaMusicInfoVO item) {
            try {
                ((ActivityMain) activity).putMusicUUID(item.getUuid());
            }catch (Exception ex) {
                ex.printStackTrace();

                Toast.makeText(activity.getApplicationContext(), "노래를 추가하는 도중 오류 발생! (00053)", Toast.LENGTH_SHORT).show();
            }
        }
    }
}
